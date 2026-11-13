using Traders.Assets;
using Traders.Assets.Loading;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Presentation.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.App;

/// <summary>
/// The program as the host runs it, without a window: the core's program machine (main 1777:000C with all its call steps and
/// the week loop with the ported rule steps) and the menu scene on the graphics layers. Per display frame the host calls
/// <see cref="Frame"/> with the held keys; the core processes the frame exactly once (<see cref="ProgramMachine.Advance"/>),
/// then the scene plays the menu events of that frame. The scene and the layers keep their own data segment image; they read
/// the core's state and never write it.
/// </summary>
// [APPROX] Host parts of the program start: the heap addresses (text buffer, page buffer) and the segment value of DS are
// fixed host choices; the display part of 0DFD:000D is reduced to what the menu scene needs (pages 0B67:0002, fonts from
// FONTS.PP as the original's driver table, masks). The loading picture and the code request are not shown (F1 = B); the
// screen steps of the week (R5 ff.) are pending, so the week machine stops at the weekly balance.
public sealed class ProgramHost
{
    /// <summary>
    /// Segment value of DS: the image segment of DGROUP, as if T.EXE were loaded at segment 0 (the far pointers in the
    /// initialised data are not relocated by <see cref="Dgroup.FromExe"/>).
    /// </summary>
    public const ushort DataSegmentValue = DgroupLayout.Segment;

    /// <summary>Far address of the text buffer DS:4884 (host heap).</summary>
    public const ushort TextBufferSegment = 0x6000, TextBufferOffset = 0x0000;

    /// <summary>Far address of the page buffer of 0B67:0002 (host heap).</summary>
    public const uint PageBuffer = 0x7000_0000;

    private readonly string _dir;

    public ProgramHost(string dosDirectory, IClockInput clock, IMenuHost? menuHost = null)
    {
        _dir = dosDirectory;
        var exe = Load("T.EXE");
        Core = ProgramSteps.InitialState(exe);
        Start = new ProgramStart(Lengths, new MenuTextPlaque(), new TextFile(Load(ResourceTable.EnglishText), TextBufferOffset, TextBufferSegment));
        Menu = new MainMenuProgramStep(menuHost ?? new SaveFileHost(dosDirectory), Lengths);
        Machine = new ProgramMachine(Core, new OriginalRandom(clock), ProgramSteps.All(ProgramInitialisation.AdLib, Start, Menu),
            WeekRuleSteps.Handlers());

        Driver = new ModeXDriver();
        var fonts = PowerPacker.Decrunch(Load(ResourceTable.Fonts));
        // [APPROX] Font table of the driver as 0DFD:0158 leaves it (small 5/6 at 5A0h, large 12/14 at 0, digits 5/5 from "/" at
        // 7F8h; checked against the original's table in MenuSceneHarness and by the menu picture); 0158 itself is not ported.
        Driver.Fonts =
        [
            new FontDescriptor(fonts, 0x5A0, 5, 6, 0x20), new FontDescriptor(fonts, 0, 12, 14, 0x20),
            new FontDescriptor(fonts, 0x7F8, 5, 5, 0x2F),
        ];
        Driver.Masks = PowerPacker.Decrunch(Load(ResourceTable.Masks));
        var ds = new byte[0x10000];
        ProgramSteps.InitialState(exe).Bytes.CopyTo(ds, 0);
        ds[GraphicsLayers.CardMode] = (byte)ProgramInitialisation.Vga; // word as the initialisation 1777:04E4 leaves it (VGA only)
        ds[GraphicsLayers.CardMode + 1] = 0;
        Layers = new GraphicsLayers(Driver, ds, new VgaDac(), Retraces) { DataSegment = DataSegmentValue };
        Layers.InitializePages(PageBuffer);
        var texts = new TextMemory(Start);
        Layers.OtherMemory = texts;
        Scene = new MenuScene(Layers, Core, texts, new Pictures(dosDirectory));
    }

    public Dgroup Core { get; }

    public ProgramMachine Machine { get; }

    public ProgramStart Start { get; }

    public MainMenuProgramStep Menu { get; }

    public ModeXDriver Driver { get; }

    public GraphicsLayers Layers { get; }

    public MenuScene Scene { get; }

    /// <summary>Retrace waits of the layers (the scene steps its fades and flips frame by frame instead).</summary>
    public RetraceCounter Retraces { get; } = new();

    /// <summary>Last frame processed (−1 before the first).</summary>
    public long LastFrame => Machine.LastFrame;

    /// <summary>
    /// Display frames up to <paramref name="frame"/>: every frame after <see cref="LastFrame"/> is processed on its own, the
    /// core first (with the keys held now), then the scene steps its fade or flip and draws the menu events of that frame.
    /// A host that skipped frames (short stall) thus gets the same core state and picture as one that presented each frame.
    /// </summary>
    public void Frame(long frame, IReadOnlyCollection<byte> held)
    {
        while (Machine.LastFrame < frame && Machine.Status != ProgramStatus.Ended)
        {
            Machine.Advance(new PhaseTick(Machine.LastFrame + 1, held));
            Machine.DrainEvents();
            Machine.DrainWeekEvents();
            Scene.NextFrame();
            foreach (var e in Menu.DrainEvents()) Scene.Apply(e);
        }
    }

    private byte[] Load(string name) => File.ReadAllBytes(Path.Combine(_dir, name));

    private ImageFileLengths Lengths(int number)
    {
        var (file, unpacked) = ImageFiles.Lengths(number, Load);
        return new ImageFileLengths(file, unpacked);
    }

    /// <summary>The text buffer of the program start behind its far address (the scene reads the menu texts there).</summary>
    private sealed class TextMemory(ProgramStart start) : IFarMemory
    {
        private int Index(ushort seg, ushort off)
        {
            int i = (seg - TextBufferSegment) * 16 + off - TextBufferOffset;
            var buffer = start.TextBuffer ?? throw new InvalidOperationException("Text buffer read before the program start.");
            if (i < 0 || i >= buffer.Length) throw new InvalidOperationException($"Far read {seg:X4}:{off:X4} outside the text buffer.");
            return i;
        }

        public byte Read(ushort seg, ushort off) => start.TextBuffer![Index(seg, off)];

        public void Write(ushort seg, ushort off, byte value) => throw new InvalidOperationException("The scene does not write texts.");
    }

    private sealed class Pictures(string dir) : IPictureSource
    {
        private readonly Dictionary<int, byte[]> _cache = [];

        public byte[] Decrunched(int resource)
        {
            if (!_cache.TryGetValue(resource, out var data))
                _cache[resource] = data = PowerPacker.Decrunch(File.ReadAllBytes(Path.Combine(dir, ResourceTable.Pictures[resource].Vga)));
            return data;
        }
    }
}

/// <summary>
/// Save file of the disk menu (decision F2 = A): SAVEGAME.DAT in the given directory through <see cref="SaveGame"/>
/// (2336:000A/0539); the joystick calibration always passes, as the host offers only the keyboard (F3 = A).
/// </summary>
// [APPROX] Disk changes and DOS errors of the original's disk menu are not modelled; a missing or wrong-sized file fails the load.
public sealed class SaveFileHost(string directory) : IMenuHost
{
    private string PathName => System.IO.Path.Combine(directory, Core.State.SaveGame.FileName);

    public bool LoadGame(Dgroup state)
    {
        try
        {
            return File.Exists(PathName) && Core.State.SaveGame.Read(File.ReadAllBytes(PathName), state);
        }
        catch (IOException)
        {
            return false;
        }
    }

    public bool SaveGame(Dgroup state)
    {
        try
        {
            File.WriteAllBytes(PathName, Core.State.SaveGame.Write(state));
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public bool CalibrationFailed(int stick, int step) => false;
}
