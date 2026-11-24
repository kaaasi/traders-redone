using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Traders.App.Audio;
using Traders.App.Input;
using Traders.App.Rendering;
using Traders.App.Sound;
using Traders.App.Timing;
using Traders.Vga;

namespace Traders.App;

/// <summary>Start options of the host window.</summary>
public sealed record HostOptions
{
    /// <summary>Integer scale of the 320 × 200 picture in the initial window.</summary>
    public int Scale { get; init; } = 3;

    /// <summary>Stretch to 4:3 like a CRT (pixels 1.2 times as high as wide) instead of square pixels.</summary>
    public bool AspectCorrection { get; init; }

    /// <summary>Exit after this many frames (manual smoke test); null = run until the window is closed.</summary>
    public long? MaxFrames { get; init; }
}

/// <summary>
/// Desktop host: counts timer ticks and frames from host time, turns key presses into scancode bytes for the key-state table
/// and, with a <see cref="ProgramHost"/>, hands every frame with the held keys to the core's program machine (R4) and shows
/// the page the scene drew. Without one (no original files) it shows a fixed mode-X page. The program's sound (lane partie
/// part 2) is ticked with the timer ticks (effects every tick, music on the ticks passed on, <see cref="HostSound.Tick"/>) and
/// its OPL writes go to the sound device (<see cref="HostAudio"/>, silent without one).
/// </summary>
public sealed class HostGame : Game
{
    /// <summary>Longest host interval taken over in one update; longer stalls are dropped.</summary>
    // [APPROX] Like lost timer interrupts on a busy PC: after a stall (window moved, debugger) the host does not
    // catch up on more than a quarter second of ticks and frames.
    public static readonly TimeSpan MaxStep = TimeSpan.FromSeconds(0.25);

    private readonly GraphicsDeviceManager _graphics;
    private readonly HostOptions _options;
    private readonly HostClock _clock = new();
    private readonly List<byte> _scancodes = [];
    private readonly uint[] _pixels = new uint[FramePresenter.Width * FramePresenter.Height];
    private SpriteBatch _batch = null!;
    private Texture2D _texture = null!;

    public ModeXMemory Vram { get; }
    public byte[] Dac { get; }
    public ushort DisplayStart { get; set; }
    public KeyStateTable Keys { get; } = new();
    public KeyboardEdges Edges { get; } = new();
    public TimerTicks Timer { get; } = new();

    private readonly ProgramHost? _program;
    private readonly List<byte> _held = [];
    private bool _haltReported;
    private HostAudio? _audio;
    private long _tick;

    public HostGame(HostOptions options, ProgramHost program) : this(options, program.Driver.Vram, program.Layers.Dac.Registers) =>
        _program = program;

    /// <summary>The program's sound host when it is the host's own (<see cref="HostSound"/>), else null.</summary>
    private HostSound? Sound => _program?.SoundHost as HostSound;

    public HostGame(HostOptions options, ModeXMemory vram, byte[] dac)
    {
        _options = options;
        Vram = vram;
        Dac = dac;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = FramePresenter.Width * options.Scale,
            PreferredBackBufferHeight = (int)(FramePresenter.Height * options.Scale * (options.AspectCorrection ? 1.2 : 1.0)),
            SynchronizeWithVerticalRetrace = true,
        };
        IsFixedTimeStep = false;
        Window.AllowUserResizing = true;
        Window.Title = "Traders";
    }

    protected override void LoadContent()
    {
        if (Sound is { } sound)
        {
            _audio = HostAudio.Create(log: Console.Out);
            sound.Written += w => _audio.Write(w.Tick, w.Register, w.Value);
        }
        _batch = new SpriteBatch(GraphicsDevice);
        _texture = new Texture2D(GraphicsDevice, FramePresenter.Width, FramePresenter.Height, false, SurfaceFormat.Color);
    }

    protected override void Update(GameTime gameTime)
    {
        var step = gameTime.ElapsedGameTime < MaxStep ? gameTime.ElapsedGameTime : MaxStep;
        var (ticks, frames) = _clock.Advance(step);
        for (int i = 0; i < ticks; i++)
        {
            bool chained = Timer.Tick();
            Sound?.Tick(++_tick, chained);
        }

        _scancodes.Clear();
        if (IsActive) Edges.Update(Keyboard.GetState().GetPressedKeys(), _scancodes);
        else Edges.ReleaseAll(_scancodes);
        Keys.Receive(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_scancodes));
        if (_program is not null && frames > 0)
        {
            // every frame passed is processed once with the keys held now: whenever an update covers more than one frame –
            // regularly, as the window updates at the monitor's rate (often 60 Hz) and the game runs at 70 Hz, and after a
            // stall up to MaxStep (17–18 frames) – ProgramHost.Frame catches up frame by frame, core and scene in step,
            // before the last one is presented
            _held.Clear();
            for (int sc = 0; sc < KeyStateTable.Size; sc++)
                if (Keys.IsDown((byte)sc)) _held.Add((byte)sc);
            _program.Frame(TargetFrame(_clock), _held);
            if (_program.Machine.Status == Traders.Core.Phases.ProgramStatus.Ended) Exit();
            DisplayStart = _program.Driver.DisplayStart;
        }

        if (_program?.Halt is { } notice && !_haltReported)
        {
            _haltReported = true;
            Console.WriteLine($"Traders: {notice}");
        }
        Window.Title = _program is null
            ? $"Traders – frame {_clock.Frames}, tick {_clock.Ticks}, last make {Keys.LastMake:X2}"
            : _program.Halt is { } halt
                ? $"Traders – {halt}"
                : $"Traders – frame {_clock.Frames}, step {_program.Machine.NextStep} {_program.Machine.Status}"
                  + (_program.Deviation is { } deviation ? $" – {deviation}" : "");
        if (_options.MaxFrames is long max && _clock.Frames >= max) Exit();
        _audio?.Update();
        base.Update(gameTime);
    }

    protected override void UnloadContent()
    {
        _audio?.Dispose();
        base.UnloadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        FramePresenter.Present(Vram, DisplayStart, Dac, _pixels);
        _texture.SetData(_pixels);
        GraphicsDevice.Clear(Color.Black);
        _batch.Begin(samplerState: SamplerState.PointClamp);
        _batch.Draw(_texture, Viewport(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, _options.AspectCorrection),
            Color.White);
        _batch.End();
        base.Draw(gameTime);
    }

    /// <summary>
    /// Last frame the program has to have processed when the host clock stands at <paramref name="clock"/>: frames are
    /// numbered from 0, so after <see cref="HostClock.Frames"/> completed frames it is the one before (−1 before the first).
    /// <see cref="Update"/> hands it to <see cref="ProgramHost.Frame"/>, which catches up frame by frame (issue #158).
    /// </summary>
    public static long TargetFrame(HostClock clock) => clock.Frames - 1;

    /// <summary>
    /// Largest rectangle for the picture inside a window of the given size, centred: integer multiples of 320 × 200
    /// when the window allows at least 1×, with aspect correction the largest 4:3 rectangle.
    /// </summary>
    public static Rectangle Viewport(int windowWidth, int windowHeight, bool aspectCorrection)
    {
        int w, h;
        if (aspectCorrection)
        {
            w = Math.Min(windowWidth, windowHeight * 4 / 3);
            h = w * 3 / 4;
        }
        else
        {
            int s = Math.Max(1, Math.Min(windowWidth / FramePresenter.Width, windowHeight / FramePresenter.Height));
            w = FramePresenter.Width * s;
            h = FramePresenter.Height * s;
        }
        return new Rectangle((windowWidth - w) / 2, (windowHeight - h) / 2, w, h);
    }
}
