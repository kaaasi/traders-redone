using System.IO.Compression;
using System.Text;

namespace Traders.RefEmu.Machine;

/// <summary>Parts of a <see cref="MachineSnapshot"/>; restoring a subset leaves the other parts as on a fresh machine.</summary>
[Flags]
public enum SnapshotParts
{
    None = 0,
    /// <summary>Registers, flags, halt state, instruction counter, interrupt shadow.</summary>
    Cpu = 1 << 0,
    /// <summary>RAM (1 MB + HMA) and A20; includes resident programs such as FXDRIVER and the BIOS data area.</summary>
    Memory = 1 << 1,
    Pic = 1 << 2,
    /// <summary>Timer channels (reload, load time, access state, latch).</summary>
    Pit = 1 << 3,
    /// <summary>Keyboard controller (queue, output buffer, spacing).</summary>
    Keyboard = 1 << 4,
    /// <summary>VGA planes, registers, latches, DAC.</summary>
    Vga = 1 << 5,
    AdLib = 1 << 6,
    /// <summary>DOS layer: open files, memory blocks, PSP, DTA, search state.</summary>
    Dos = 1 << 7,
    /// <summary>Files written or deleted by the program (in-memory overlay of drive C:).</summary>
    Files = 1 << 8,
    /// <summary>Machine clock (idle offset, next timer interrupt), input script, exit state.</summary>
    Clock = 1 << 9,
    All = (1 << 10) - 1,
}

/// <summary>
/// Complete state of a <see cref="PcMachine"/> between two instructions, to resume a reference run later (for example at a
/// phase marker) instead of starting from the beginning. Restoring all parts into a fresh machine with the same options
/// continues bit-identically. Not part of the state: event log, code hooks and tracers (observers; re-attach them after
/// <see cref="Restore"/>), and the host directory (an input of the run).
/// The snapshot contains memory derived from T.EXE: write it only to temporary folders, never into the repository.
/// </summary>
public sealed class MachineSnapshot
{
    private static readonly byte[] Magic = "TRSNAP01"u8.ToArray();

    private readonly Dictionary<SnapshotParts, byte[]> _parts;

    public long InstructionsPerSecond { get; }
    public DateTime StartTime { get; }
    public bool RecordSoundDriver { get; }
    public bool AdLib { get; }
    public ushort PspSegment { get; }
    public ushort MemoryTop { get; }

    /// <summary>Instruction counter at the time of the capture.</summary>
    public long InstructionCount { get; }

    private MachineSnapshot(MachineOptions o, long instructionCount, Dictionary<SnapshotParts, byte[]> parts)
        : this(o.InstructionsPerSecond, o.StartTime, o.RecordSoundDriver, o.AdLib, o.PspSegment, o.MemoryTop, instructionCount, parts)
    {
    }

    private MachineSnapshot(long ips, DateTime start, bool recordSound, bool adLib, ushort psp, ushort top, long instructionCount,
        Dictionary<SnapshotParts, byte[]> parts)
    {
        InstructionsPerSecond = ips;
        StartTime = start;
        RecordSoundDriver = recordSound;
        AdLib = adLib;
        PspSegment = psp;
        MemoryTop = top;
        InstructionCount = instructionCount;
        _parts = parts;
    }

    /// <summary>Single parts in a fixed order (file layout).</summary>
    public static IReadOnlyList<SnapshotParts> PartOrder { get; } =
        Enum.GetValues<SnapshotParts>().Where(p => p != SnapshotParts.None && p != SnapshotParts.All).ToArray();

    /// <summary>Captures the machine; call between instructions (not from inside a code hook that changes state).</summary>
    public static MachineSnapshot Capture(PcMachine m)
    {
        var parts = new Dictionary<SnapshotParts, byte[]>();
        foreach (var p in PartOrder) parts[p] = Serialize(w => Save(m, p, w));
        return new MachineSnapshot(m.Options, m.Cpu.InstructionCount, parts);
    }

    private static void Save(PcMachine m, SnapshotParts part, BinaryWriter w)
    {
        switch (part)
        {
            case SnapshotParts.Cpu: m.Cpu.SaveState(w); break;
            case SnapshotParts.Memory: m.Mem.SaveState(w); break;
            case SnapshotParts.Pic: m.Pic.SaveState(w); break;
            case SnapshotParts.Pit: m.Pit.SaveState(w); break;
            case SnapshotParts.Keyboard: m.Keyboard.SaveState(w); break;
            case SnapshotParts.Vga: m.Vga.SaveState(w); break;
            case SnapshotParts.AdLib: m.AdLib.SaveState(w); break;
            case SnapshotParts.Dos: m.SaveDosState(w); break;
            case SnapshotParts.Files: m.Files.SaveState(w); break;
            case SnapshotParts.Clock: m.SaveClockState(w); break;
            default: throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }

    private static void Load(PcMachine m, SnapshotParts part, BinaryReader r)
    {
        switch (part)
        {
            case SnapshotParts.Cpu: m.Cpu.LoadState(r); break;
            case SnapshotParts.Memory: m.Mem.LoadState(r); break;
            case SnapshotParts.Pic: m.Pic.LoadState(r); break;
            case SnapshotParts.Pit: m.Pit.LoadState(r); break;
            case SnapshotParts.Keyboard: m.Keyboard.LoadState(r); break;
            case SnapshotParts.Vga: m.Vga.LoadState(r); break;
            case SnapshotParts.AdLib: m.AdLib.LoadState(r); break;
            case SnapshotParts.Dos: m.LoadDosState(r); break;
            case SnapshotParts.Files: m.Files.LoadState(r); break;
            case SnapshotParts.Clock: m.LoadClockState(r); break;
            default: throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }

    private static byte[] Serialize(Action<BinaryWriter> save)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) save(w);
        return ms.ToArray();
    }

    /// <summary>Raw bytes of one part (for comparisons in tests).</summary>
    public ReadOnlySpan<byte> PartBytes(SnapshotParts part) => _parts[part];

    /// <summary>
    /// Creates a new machine with the captured options over <paramref name="hostDirectory"/> and loads the selected parts.
    /// Parts left out keep the state of a freshly constructed machine (used by the counter-test of the snapshot).
    /// </summary>
    public PcMachine Restore(string hostDirectory, SnapshotParts parts = SnapshotParts.All)
    {
        var m = new PcMachine(new MachineOptions
        {
            HostDirectory = hostDirectory,
            InstructionsPerSecond = InstructionsPerSecond,
            StartTime = StartTime,
            RecordSoundDriver = RecordSoundDriver,
            AdLib = AdLib,
            PspSegment = PspSegment,
            MemoryTop = MemoryTop,
        });
        foreach (var p in PartOrder)
        {
            if ((parts & p) == 0) continue;
            using var r = new BinaryReader(new MemoryStream(_parts[p], writable: false), Encoding.UTF8);
            Load(m, p, r);
            if (r.BaseStream.Position != r.BaseStream.Length) throw new InvalidDataException($"snapshot part {p} not fully read");
        }
        return m;
    }

    /// <summary>Writes the snapshot (gzip). Use a temporary folder only: the content is derived from T.EXE.</summary>
    public void WriteTo(string path)
    {
        using var fs = File.Create(path);
        using var gz = new GZipStream(fs, CompressionLevel.Fastest);
        using var w = new BinaryWriter(gz, Encoding.UTF8);
        w.Write(Magic);
        w.Write(InstructionsPerSecond);
        w.Write(StartTime.Ticks);
        w.Write(RecordSoundDriver);
        w.Write(AdLib);
        w.Write(PspSegment);
        w.Write(MemoryTop);
        w.Write(InstructionCount);
        w.Write(PartOrder.Count);
        foreach (var p in PartOrder)
        {
            w.Write((int)p);
            w.Write(_parts[p].Length);
            w.Write(_parts[p]);
        }
    }

    public static MachineSnapshot ReadFrom(string path)
    {
        using var fs = File.OpenRead(path);
        using var gz = new GZipStream(fs, CompressionMode.Decompress);
        using var r = new BinaryReader(gz, Encoding.UTF8);
        if (!r.ReadBytes(Magic.Length).AsSpan().SequenceEqual(Magic)) throw new InvalidDataException("not a machine snapshot");
        long ips = r.ReadInt64();
        var start = new DateTime(r.ReadInt64());
        bool recordSound = r.ReadBoolean();
        bool adLib = r.ReadBoolean();
        ushort psp = r.ReadUInt16();
        ushort top = r.ReadUInt16();
        long count = r.ReadInt64();
        int n = r.ReadInt32();
        var parts = new Dictionary<SnapshotParts, byte[]>();
        for (int i = 0; i < n; i++)
        {
            var p = (SnapshotParts)r.ReadInt32();
            if (!PartOrder.Contains(p)) throw new InvalidDataException($"unknown snapshot part {(int)p}");
            parts[p] = r.ReadBytes(r.ReadInt32());
        }
        foreach (var p in PartOrder)
            if (!parts.ContainsKey(p)) throw new InvalidDataException($"snapshot part {p} missing");
        return new MachineSnapshot(ips, start, recordSound, adLib, psp, top, count, parts);
    }
}
