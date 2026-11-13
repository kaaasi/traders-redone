using Traders.Assets;
using Traders.Assets.Loading;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Presentation.Week;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.App;

/// <summary>
/// The program as the host runs it, without a window: the core's program machine (main 1777:000C with all its call steps and
/// the week loop with the ported steps of <see cref="WeekSteps"/>: rule steps, week report, guard of call 4, week planning,
/// planting phase) and the scenes on the graphics layers – main menu, week report, planting field (decision F1 = A). Per
/// display frame the host calls <see cref="Frame"/> with the held keys; the core processes the frame exactly once
/// (<see cref="ProgramMachine.Advance"/>), the scenes play the events of that frame. The machine halts at the first call
/// still pending (call 9, week events, in weeks 0–2; call 4 later with catastrophes on); <see cref="Halt"/> then names it.
/// The scenes and the layers keep their own data segment image; they read the core's state and never write it.
/// </summary>
// [APPROX] Host parts of the program start: the heap addresses (text buffer, page buffer) and the segment value of DS are
// fixed host choices; the display part of 0DFD:000D is reduced to what the menu scene needs (pages 0B67:0002, page numbers
// and first flip 0DFD:0074/0132, fonts from FONTS.PP as the original's driver table, masks). The loading picture and the code
// request are not shown; the week runs up to its first pending call step and halts there with a notice (F1 = A, R5).
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
        Week = new WeekSteps(e => Field!.Apply(e), Lengths);
        Machine = new ProgramMachine(Core, new OriginalRandom(clock), ProgramSteps.All(ProgramInitialisation.AdLib, Start, Menu),
            Week.Handlers);

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
        SetUpGamePages(Layers);
        var texts = new TextMemory(Start);
        Layers.OtherMemory = texts;
        var pictures = new Pictures(dosDirectory);
        Scene = new MenuScene(Layers, Core, texts, pictures);
        Report = new ReportScene(Layers, Core, texts, pictures);
        Field = new FieldScene(Layers, Core) { Pictures = pictures };
    }

    /// <summary>The ported call steps of the week loop.</summary>
    public WeekSteps Week { get; }

    /// <summary>Scene of the week report (call 3).</summary>
    public ReportScene Report { get; }

    /// <summary>Scene of the planting phase (call 8); the core's field events reach it while the frame is processed.</summary>
    public FieldScene Field { get; }

    /// <summary>
    /// Notice when the week loop halts at a call step that is not ported yet (null while it runs): the step, its routine and
    /// the week.
    /// </summary>
    public string? Halt => Machine.WeekMachine is { Status: PhaseStatus.Blocked } w
        ? $"week {w.Week}: call {WeekLoop.Step(w.NextStep)} is not ported yet - halted"
        : null;

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
            Field.NextFrame(held); // the planting scene steps first; the core's field events reach it during Advance
            Machine.Advance(new PhaseTick(Machine.LastFrame + 1, held));
            Machine.DrainEvents();
            Machine.DrainWeekEvents();
            Scene.NextFrame();
            foreach (var e in Menu.DrainEvents()) Scene.Apply(e);
            Report.NextFrame();
            if (Week.Report.Report is { } report)
                foreach (var e in report.DrainEvents()) Report.Apply(e);
        }
    }

    /// <summary>
    /// [ORIGINAL] 0DFD:0074 (page part) and 0DFD:0132 (flip part) on the layers' data segment: page numbers DS:40DE…40E5 =
    /// 1, 2 (longs), 0B67:0238(40DE[082E], 40DE[0830]), then the first flip 21FD:051F. Afterwards the game draws on the
    /// hidden page and every flip of the menus shows the other page (DS:082E = 1, 0830 = 0, 4516 = 2 at the menu entry).
    /// Without the page numbers 21FD:051F neither shows nor selects a page (finding R4W-1): the host kept showing page 0
    /// while the menus drew their changes on page 1.
    /// </summary>
    // [APPROX] The heap buffers of 0074 and the pointer copies of 0132 (DS:44DE, 45BC) are the host's; the retrace waits of
    // the first flip are counted but take no display frames (the loading picture is not shown, F1 = B).
    internal static void SetUpGamePages(GraphicsLayers layers)
    {
        var ds = layers.Ds;
        void Set32(int at, uint v)
        {
            for (int i = 0; i < 4; i++) ds[at + i] = (byte)(v >> (8 * i));
        }
        uint Page(int index) => BitConverter.ToUInt32(ds, GraphicsLayers.PageNumbers + 4 * index);
        int Index(int at) => (ushort)(ds[at] | ds[at + 1] << 8);

        Set32(GraphicsLayers.PageNumbers, 1);
        Set32(GraphicsLayers.PageNumbers + 4, 2);
        layers.SelectPages(Page(Index(GraphicsLayers.DrawPageIndex)), Page(Index(GraphicsLayers.ShownPageIndex)));
        layers.FlipPages(); // [ORIGINAL] 0DFD:0132 – first flip 21FD:051F
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
