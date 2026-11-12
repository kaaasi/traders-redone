using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Traders.App.Input;
using Traders.App.Rendering;
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
/// the page the scene drew. Without one (no original files) it shows a fixed mode-X page.
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

    public HostGame(HostOptions options, ProgramHost program) : this(options, program.Driver.Vram, program.Layers.Dac.Registers) =>
        _program = program;

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
        _batch = new SpriteBatch(GraphicsDevice);
        _texture = new Texture2D(GraphicsDevice, FramePresenter.Width, FramePresenter.Height, false, SurfaceFormat.Color);
    }

    protected override void Update(GameTime gameTime)
    {
        var step = gameTime.ElapsedGameTime < MaxStep ? gameTime.ElapsedGameTime : MaxStep;
        var (ticks, frames) = _clock.Advance(step);
        for (int i = 0; i < ticks; i++) Timer.Tick();

        _scancodes.Clear();
        if (IsActive) Edges.Update(Keyboard.GetState().GetPressedKeys(), _scancodes);
        else Edges.ReleaseAll(_scancodes);
        Keys.Receive(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_scancodes));
        if (_program is not null && frames > 0)
        {
            // every frame passed is processed once with the keys held now (the core catches up after a short stall)
            _held.Clear();
            for (int sc = 0; sc < KeyStateTable.Size; sc++)
                if (Keys.IsDown((byte)sc)) _held.Add((byte)sc);
            _program.Frame(_clock.Frames - 1, _held);
            if (_program.Machine.Status == Traders.Core.Phases.ProgramStatus.Ended) Exit();
            DisplayStart = _program.Driver.DisplayStart;
        }

        Window.Title = _program is null
            ? $"Traders – frame {_clock.Frames}, tick {_clock.Ticks}, last make {Keys.LastMake:X2}"
            : $"Traders – frame {_clock.Frames}, step {_program.Machine.NextStep} {_program.Machine.Status}";
        if (_options.MaxFrames is long max && _clock.Frames >= max) Exit();
        base.Update(gameTime);
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
