using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// Program step 1, the initialisation 1777:04E4, reduced to what the game state needs: the graphics card DS:090E and the sound
/// device DS:0910. Everything else it does belongs to the host and the output layers (finding
/// docs/routines/1777_04E4_initialisierung.md, table "Zustand"): heap (186C:021E), resource table marks (186C:0544, lane part 2),
/// interrupt vectors and handler mask (0D73:00F8..049B), music driver (218B:0002) and display pages (0B67:0002).
/// </summary>
// [ORIGINAL] 1777:04E4 – DS:090E = -1 → detect the card; still -1 → result 2; DS:0910 = -1 → detect the sound device
// [APPROX] The detections 0D73:0712 (card) and 0D73:04CA (AdLib at port 388h) are replaced: the port is VGA only (card 1, as
// the RefEmu detects it) and the host passes the sound device as an explicit input. The error result 1 of 0B67:0002 (no
// memory for the page buffer) is the host's.
public sealed class ProgramInitialisation(short soundDevice) : IProgramStepHandler
{
    /// <summary>DS offset of the graphics card (1 VGA, 0 EGA, 2 Tandy, 63h Tandy emulation, -1 detect).</summary>
    public const int GraphicsCard = 0x090E;

    /// <summary>DS offset of the sound device (0 PC speaker, 1 AdLib, -1 detect).</summary>
    public const int SoundDevice = 0x0910;

    public const short Vga = 1;

    public const short AdLib = 1;

    public const short PcSpeaker = 0;

    /// <summary>Exit code of main when no graphics card was found (closing message DS:095A).</summary>
    public const byte NoGraphicsCard = 2;

    public StepOutcome Begin(ProgramStepContext context)
    {
        var d = context.State;
        if (d.S16(GraphicsCard) == -1) d.Set(GraphicsCard, Vga);
        if (d.S16(GraphicsCard) == -1) return StepOutcome.Finished(NoGraphicsCard); // unreachable with the fixed detection
        if (d.S16(SoundDevice) == -1) d.Set(SoundDevice, soundDevice);
        return StepOutcome.Finished(0);
    }

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("the initialisation finishes at once");
}

/// <summary>
/// Sound calls of main (steps 3–7: 218B:013A, 21A7:000D, 21A7:0054(0, 1), 218B:0094(1, 2)) until the sound lane (R10):
/// they return at once with AL = 0 and change nothing. That is what 218B:013A returns without a music driver (DS:10A3 = 0),
/// and 218B:0094 then does nothing either (218B:00A5); main takes the same path as in the reference run.
/// </summary>
// [APPROX] Sound pending (R10): 21A7:000D/0054 set the sound effect source and play an effect (DS:10A5, timer callback 27C6:041B);
// in the port they are no-ops. The bytes they write in the original are listed in docs/routines/1777_000C_main.md ("Ton").
public sealed class PendingSoundStep : IProgramStepHandler
{
    public static PendingSoundStep Instance { get; } = new();

    public StepOutcome Begin(ProgramStepContext context) => StepOutcome.Finished(0);

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("sound stubs finish at once");
}

/// <summary>Handlers of the program steps the core has so far.</summary>
public static class ProgramSteps
{
    /// <summary>Program steps with a sound call (handled by <see cref="PendingSoundStep"/> until R10).</summary>
    public static IReadOnlyList<int> SoundSteps { get; } = [3, 4, 5, 6, 7];

    /// <summary>
    /// Initialisation and the sound stubs. Program start (2), main menu (8) and new game (9) stay pending until their parts
    /// and lanes are merged (lane programm part 2, lanes menue and partie); the host adds them to this dictionary.
    /// </summary>
    public static Dictionary<int, IProgramStepHandler> Defaults(short soundDevice = ProgramInitialisation.AdLib)
    {
        var d = new Dictionary<int, IProgramStepHandler> { [1] = new ProgramInitialisation(soundDevice) };
        foreach (int n in SoundSteps) d[n] = PendingSoundStep.Instance;
        return d;
    }

    /// <summary>
    /// Initial data segment of the program as the host prepares it: the initialised data of T.EXE (<see cref="Dgroup.FromExe"/>).
    /// What the C runtime startup writes before main (heap, environment, file table) is the host's.
    /// </summary>
    // [APPROX] Turbo C startup 0000:00E3 (DS:005A..008E, 1286..12B7, 1452..1579 and others) not modelled;
// the bytes are listed in docs/routines/1777_04E4_initialisierung.md.
    public static Dgroup InitialState(ReadOnlySpan<byte> exe) => Dgroup.FromExe(exe);
}
