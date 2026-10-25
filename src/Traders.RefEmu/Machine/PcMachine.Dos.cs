using System.Text;
using Traders.RefEmu.Cpu;

namespace Traders.RefEmu.Machine;

public sealed partial class PcMachine
{
    private sealed class OpenFile(string name, byte[] data, bool writable)
    {
        public string Name { get; } = name;
        public byte[] Data { get; set; } = data;
        public int Length { get; set; } = data.Length;
        public int Position { get; set; }
        public bool Writable { get; } = writable;
        public bool Dirty { get; set; }
    }

    private sealed record MemBlock(ushort Segment, ushort Paragraphs);

    private readonly Dictionary<int, OpenFile> _files = [];
    private readonly List<MemBlock> _blocks = [];
    private ushort _psp;
    private ushort _dtaSeg, _dtaOff;
    private ushort _lastError;
    private readonly List<string> _findResults = [];
    private readonly StringBuilder _consoleLine = new();

    /// <summary>PSP segment of the loaded program.</summary>
    public ushort PspSegment => _psp;

    /// <summary>Segment where the load module starts (PSP + 10h); image segment 0000 of the analysis maps here.</summary>
    public ushort LoadSegment => (ushort)(_psp + 0x10);

    private const ushort EnvSegment = 0x0FD0;

    private void InitDos()
    {
        _psp = Options.PspSegment;
        _dtaSeg = _psp;
        _dtaOff = 0x80;
    }

    /// <summary>Loads an MZ program like DOS EXEC would and prepares the CPU at its entry point (does not run it).</summary>
    public MzHeader LoadProgram(string exeName, string commandTail = "")
    {
        byte[] exe = Files.Load(exeName) ?? throw new FileNotFoundException(exeName);
        ushort psp = _psp;

        // environment block
        string env = "COMSPEC=C:\\COMMAND.COM\0PATH=C:\\\0PROMPT=$P$G\0\0";
        var envBytes = new List<byte>(Encoding.ASCII.GetBytes(env)) { 1, 0 };
        envBytes.AddRange(Encoding.ASCII.GetBytes("C:\\" + DosFileSystem.Normalize(exeName) + "\0"));
        Mem.WriteBlock(Memory.Linear(EnvSegment, 0), envBytes.ToArray());
        _blocks.Clear();
        _blocks.Add(new MemBlock(EnvSegment, (ushort)(psp - EnvSegment)));
        _blocks.Add(new MemBlock(psp, (ushort)(Options.MemoryTop - psp)));

        // PSP
        Mem.Write16(psp, 0x00, 0x20CD);
        Mem.Write16(psp, 0x02, Options.MemoryTop);
        Mem.Write16(psp, 0x2C, EnvSegment);
        for (int i = 0; i < 20; i++) Mem.Write8(psp, (ushort)(0x18 + i), (byte)(i < 5 ? i : 0xFF));
        Mem.Write16(psp, 0x32, 20);
        Mem.Write16(psp, 0x34, 0x18);
        Mem.Write16(psp, 0x36, psp);
        Mem.Write8(psp, 0x5C, 0);
        Mem.Write8(psp, 0x6C, 0);
        string tail = commandTail.Length > 0 && commandTail[0] != ' ' ? " " + commandTail : commandTail;
        Mem.Write8(psp, 0x80, (byte)tail.Length);
        Mem.WriteBlock(Memory.Linear(psp, 0x81), Encoding.ASCII.GetBytes(tail + "\r"));

        var h = MzLoader.Load(Mem, exe, LoadSegment);
        var c = Cpu;
        c.Cs = (ushort)(LoadSegment + h.InitialCs);
        c.IP = h.InitialIp;
        c.Ss = (ushort)(LoadSegment + h.InitialSs);
        c.Sp = h.InitialSp;
        c.Ds = c.Es = psp;
        c.Ax = 0; c.Bx = 0; c.Cx = 0x00FF; c.Dx = psp; c.Si = c.IP; c.Di = c.Sp; c.Bp = 0x091C;
        c.Flags = Cpu286.IF;
        LogEvent(EventKind.Dos, $"load {exeName} PSP={psp:X4} load={LoadSegment:X4} entry={c.Cs:X4}:{c.IP:X4} stack={c.Ss:X4}:{c.Sp:X4}");
        return h;
    }

    // ------------------------------------------------------------------ int 21h

    private void DosOk() => SetCarry(false);

    private void DosError(ushort code)
    {
        _lastError = code;
        Cpu.Ax = code;
        SetCarry(true);
    }

    private void Int21(ushort trapIp)
    {
        var c = Cpu;
        byte ah = (byte)(c.Ax >> 8), al = (byte)c.Ax;
        switch (ah)
        {
            case 0x00: Exit(0); return;
            case 0x01:
            case 0x07:
            case 0x08:
                if (!BufferPeek(out _)) { RetryLater(trapIp); return; }
                c.Ax = (ushort)(c.Ax & 0xFF00 | (byte)BufferGet());
                return;
            case 0x02: ConsoleOut((byte)c.Dx); return;
            case 0x06:
                if ((byte)c.Dx != 0xFF) { ConsoleOut((byte)c.Dx); return; }
                if (BufferPeek(out _)) { c.Ax = (ushort)(c.Ax & 0xFF00 | (byte)BufferGet()); c.SetFlag(Cpu286.ZF, false); }
                else { c.Ax &= 0xFF00; c.SetFlag(Cpu286.ZF, true); }
                return;
            case 0x09:
                for (ushort o = c.Dx; ; o++)
                {
                    byte ch = Mem.Read8(c.Ds, o);
                    if (ch == '$') break;
                    ConsoleOut(ch);
                }
                return;
            case 0x0B: c.Ax = (ushort)(c.Ax & 0xFF00 | (BufferPeek(out _) ? 0xFF : 0)); return;
            case 0x0E: c.Ax = (ushort)(c.Ax & 0xFF00 | 5); return;
            case 0x19: c.Ax = (ushort)(c.Ax & 0xFF00 | 2); return;
            case 0x1A: _dtaSeg = c.Ds; _dtaOff = c.Dx; return;
            case 0x25:
                Mem.Write16(0, (ushort)(al * 4), c.Dx);
                Mem.Write16(0, (ushort)(al * 4 + 2), c.Ds);
                LogEvent(EventKind.Dos, $"set vector {al:X2} = {c.Ds:X4}:{c.Dx:X4}");
                return;
            case 0x2A:
            {
                var t = CurrentTime;
                c.Cx = (ushort)t.Year;
                c.Dx = (ushort)((t.Month << 8) | t.Day);
                c.Ax = (ushort)(c.Ax & 0xFF00 | (int)t.DayOfWeek);
                LogEvent(EventKind.Clock, $"get date {t:yyyy-MM-dd}");
                return;
            }
            case 0x2B:
            case 0x2D:
                LogEvent(EventKind.Clock, $"set date/time AH={ah:X2} ignored");
                c.Ax &= 0xFF00;
                return;
            case 0x2C:
            {
                var t = CurrentTime;
                c.Cx = (ushort)((t.Hour << 8) | t.Minute);
                c.Dx = (ushort)((t.Second << 8) | (t.Millisecond / 10));
                LogEvent(EventKind.Clock, $"get time {t:HH:mm:ss.ff}");
                return;
            }
            case 0x2F: c.Es = _dtaSeg; c.Bx = _dtaOff; return;
            case 0x30: c.Ax = 0x0005; c.Bx = 0; c.Cx = 0; return;
            case 0x33:
                if (al == 0x00) c.Dx &= 0xFF00;
                else if (al == 0x05) c.Dx = (ushort)(c.Dx & 0xFF00 | 3);
                else if (al == 0x06) { c.Bx = 0x0005; c.Dx = 0; }
                return;
            case 0x35:
                c.Bx = Mem.Read16(0, (ushort)(al * 4));
                c.Es = Mem.Read16(0, (ushort)(al * 4 + 2));
                return;
            case 0x3C: CreateFile(); return;
            case 0x3D: OpenFileCall(al); return;
            case 0x3E: CloseFile(c.Bx); return;
            case 0x3F: ReadFile(); return;
            case 0x40: WriteFile(); return;
            case 0x41:
            {
                string name = Mem.ReadAsciiz(c.Ds, c.Dx);
                LogEvent(EventKind.File, $"delete {name}");
                if (Files.Delete(name)) DosOk(); else DosError(2);
                return;
            }
            case 0x42: Seek(al); return;
            case 0x43:
            {
                string name = Mem.ReadAsciiz(c.Ds, c.Dx);
                if (!Files.Exists(name)) { DosError(2); return; }
                if (al == 0) c.Cx = 0x20;
                DosOk();
                return;
            }
            case 0x44: Ioctl(al); return;
            case 0x47:
                Mem.Write8(c.Ds, c.Si, 0);
                c.Ax = 0x0100;
                DosOk();
                return;
            case 0x48: Allocate(c.Bx); return;
            case 0x49: Free(c.Es); return;
            case 0x4A: Resize(c.Es, c.Bx); return;
            case 0x4C: Exit(al); return;
            case 0x4E: FindFirst(); return;
            case 0x4F: FindNext(); return;
            case 0x50: _psp = c.Bx; return;
            case 0x51:
            case 0x62: c.Bx = _psp; return;
            case 0x54: c.Ax &= 0xFF00; return;
            case 0x58:
                if (al == 0) c.Ax = 0;
                DosOk();
                return;
            case 0x59: c.Ax = _lastError; c.Bx = 0x0D01; c.Cx = 0; return;
            default:
                throw new EmulationException($"DOS function {c.Ax:X4} not implemented (BX={c.Bx:X4} CX={c.Cx:X4} DX={c.Dx:X4}) at {Mem.Read16(c.Ss, (ushort)(c.Sp + 2)):X4}:{Mem.Read16(c.Ss, c.Sp):X4}");
        }
    }

    private void ConsoleOut(byte ch)
    {
        if (ch == '\n')
        {
            LogEvent(EventKind.Console, _consoleLine.ToString());
            _consoleLine.Clear();
        }
        else if (ch != '\r') _consoleLine.Append((char)ch);
    }

    // ------------------------------------------------------------------ files

    private int FreeHandle()
    {
        for (int h = 5; h < 20; h++) if (!_files.ContainsKey(h)) return h;
        return -1;
    }

    private void CreateFile()
    {
        string name = Mem.ReadAsciiz(Cpu.Ds, Cpu.Dx);
        int h = FreeHandle();
        if (h < 0) { DosError(4); return; }
        _files[h] = new OpenFile(DosFileSystem.Normalize(name), [], true) { Dirty = true };
        Files.Store(name, []);
        LogEvent(EventKind.File, $"create {name} -> {h}");
        Cpu.Ax = (ushort)h;
        DosOk();
    }

    private void OpenFileCall(byte mode)
    {
        string name = Mem.ReadAsciiz(Cpu.Ds, Cpu.Dx);
        var data = Files.Load(name);
        if (data is null)
        {
            LogEvent(EventKind.File, $"open {name} mode {mode:X2}: not found");
            DosError(2);
            return;
        }
        int h = FreeHandle();
        if (h < 0) { DosError(4); return; }
        bool writable = (mode & 3) != 0;
        _files[h] = new OpenFile(DosFileSystem.Normalize(name), writable ? (byte[])data.Clone() : data, writable);
        LogEvent(EventKind.File, $"open {name} mode {mode:X2} -> {h} ({data.Length} bytes)");
        Cpu.Ax = (ushort)h;
        DosOk();
    }

    private void CloseFile(ushort h)
    {
        if (h < 5) { DosOk(); return; }
        if (!_files.Remove(h, out var f)) { DosError(6); return; }
        if (f.Dirty) Files.Store(f.Name, f.Data.AsSpan(0, f.Length).ToArray());
        LogEvent(EventKind.File, $"close {h} ({f.Name})");
        DosOk();
    }

    private void ReadFile()
    {
        var c = Cpu;
        if (c.Bx < 5) { c.Ax = 0; DosOk(); return; }
        if (!_files.TryGetValue(c.Bx, out var f)) { DosError(6); return; }
        int n = Math.Max(0, Math.Min(c.Cx, f.Length - f.Position));
        uint dst = Memory.Linear(c.Ds, c.Dx);
        Mem.WriteBlock(dst, f.Data.AsSpan(f.Position, n));
        LogEvent(EventKind.File, $"read {c.Bx} ({f.Name}) pos {f.Position} want {c.Cx} got {n} -> {c.Ds:X4}:{c.Dx:X4}");
        f.Position += n;
        c.Ax = (ushort)n;
        DosOk();
    }

    private void WriteFile()
    {
        var c = Cpu;
        byte[] data = Mem.ReadBlock(c.Ds, c.Dx, c.Cx);
        if (c.Bx is 1 or 2)
        {
            foreach (var b in data) ConsoleOut(b);
            c.Ax = c.Cx;
            DosOk();
            return;
        }
        if (c.Bx < 5) { c.Ax = c.Cx; DosOk(); return; }
        if (!_files.TryGetValue(c.Bx, out var f) || !f.Writable) { DosError(5); return; }
        if (c.Cx == 0) { f.Length = f.Position; f.Dirty = true; c.Ax = 0; DosOk(); return; }
        int end = f.Position + data.Length;
        if (end > f.Data.Length)
        {
            var nd = new byte[Math.Max(end, f.Data.Length * 2)];
            f.Data.AsSpan(0, f.Length).CopyTo(nd);
            f.Data = nd;
        }
        data.CopyTo(f.Data, f.Position);
        f.Position = end;
        f.Length = Math.Max(f.Length, end);
        f.Dirty = true;
        LogEvent(EventKind.File, $"write {c.Bx} ({f.Name}) {data.Length} bytes");
        c.Ax = (ushort)data.Length;
        DosOk();
    }

    private void Seek(byte origin)
    {
        var c = Cpu;
        if (!_files.TryGetValue(c.Bx, out var f)) { DosError(6); return; }
        int dist = (c.Cx << 16) | c.Dx;
        int basePos = origin switch { 0 => 0, 1 => f.Position, _ => f.Length };
        int pos = basePos + dist;
        if (pos < 0) { DosError(0x19); return; }
        f.Position = pos;
        c.Ax = (ushort)pos;
        c.Dx = (ushort)(pos >> 16);
        DosOk();
    }

    private void Ioctl(byte al)
    {
        var c = Cpu;
        switch (al)
        {
            case 0x00:
                if (c.Bx < 5) c.Dx = c.Bx switch { 0 => 0x80D3, 1 or 2 => 0x80D3, 3 => 0x80C0, _ => 0xA0C0 };
                else if (_files.ContainsKey(c.Bx)) c.Dx = 0x0002;
                else { DosError(6); return; }
                c.Ax = c.Dx;
                DosOk();
                return;
            case 0x01: DosOk(); return;
            case 0x06:
            case 0x07: c.Ax = (ushort)(c.Ax & 0xFF00 | 0xFF); DosOk(); return;
            case 0x08: c.Ax = 1; DosOk(); return;
            case 0x0E: c.Ax &= 0xFF00; DosOk(); return;
            default:
                LogEvent(EventKind.Dos, $"ioctl {al:X2} not supported");
                DosError(1);
                return;
        }
    }

    // ------------------------------------------------------------------ find first / next

    private void FindFirst()
    {
        string pattern = Mem.ReadAsciiz(Cpu.Ds, Cpu.Dx);
        _findResults.Clear();
        _findResults.AddRange(Files.List().Where(n => DosFileSystem.Matches(pattern, n)));
        LogEvent(EventKind.File, $"find first {pattern}: {_findResults.Count} matches");
        if (_findResults.Count == 0) { DosError(2); return; }
        Mem.Write16(_dtaSeg, _dtaOff, 0); // our search index lives in the reserved area
        FindNext();
    }

    private void FindNext()
    {
        ushort idx = Mem.Read16(_dtaSeg, _dtaOff);
        if (idx >= _findResults.Count) { DosError(0x12); return; }
        string name = _findResults[idx];
        Mem.Write16(_dtaSeg, _dtaOff, (ushort)(idx + 1));
        var data = Files.Load(name) ?? [];
        Mem.Write8(_dtaSeg, (ushort)(_dtaOff + 0x15), 0x20);
        Mem.Write16(_dtaSeg, (ushort)(_dtaOff + 0x16), 0x6000);      // 12:00:00
        Mem.Write16(_dtaSeg, (ushort)(_dtaOff + 0x18), (ushort)(((1991 - 1980) << 9) | (6 << 5) | 1));
        Mem.Write16(_dtaSeg, (ushort)(_dtaOff + 0x1A), (ushort)data.Length);
        Mem.Write16(_dtaSeg, (ushort)(_dtaOff + 0x1C), (ushort)(data.Length >> 16));
        var nb = Encoding.ASCII.GetBytes(name);
        for (int i = 0; i < 13; i++) Mem.Write8(_dtaSeg, (ushort)(_dtaOff + 0x1E + i), i < nb.Length ? nb[i] : (byte)0);
        DosOk();
    }

    // ------------------------------------------------------------------ memory

    private int BlockIndex(ushort seg) => _blocks.FindIndex(b => b.Segment == seg);

    private int SpaceAfter(int index)
    {
        ushort start = _blocks[index].Segment;
        ushort next = Options.MemoryTop;
        foreach (var b in _blocks) if (b.Segment > start && b.Segment < next) next = b.Segment;
        return next - start;
    }

    private void Allocate(ushort paras)
    {
        // first fit in the gaps between blocks
        var sorted = _blocks.OrderBy(b => b.Segment).ToList();
        int largest = 0;
        for (int i = 0; i < sorted.Count; i++)
        {
            int gapStart = sorted[i].Segment + sorted[i].Paragraphs;
            int gapEnd = i + 1 < sorted.Count ? sorted[i + 1].Segment : Options.MemoryTop;
            int gap = gapEnd - gapStart;
            if (gap >= paras && paras > 0)
            {
                _blocks.Add(new MemBlock((ushort)gapStart, paras));
                Cpu.Ax = (ushort)gapStart;
                LogEvent(EventKind.Dos, $"alloc {paras:X4} paras -> {gapStart:X4}");
                DosOk();
                return;
            }
            largest = Math.Max(largest, gap);
        }
        LogEvent(EventKind.Dos, $"alloc {paras:X4} paras failed (largest {largest:X4})");
        Cpu.Bx = (ushort)largest;
        DosError(8);
    }

    private void Free(ushort seg)
    {
        int i = BlockIndex(seg);
        if (i < 0) { DosError(9); return; }
        _blocks.RemoveAt(i);
        LogEvent(EventKind.Dos, $"free {seg:X4}");
        DosOk();
    }

    private void Resize(ushort seg, ushort paras)
    {
        int i = BlockIndex(seg);
        if (i < 0) { DosError(9); return; }
        int max = SpaceAfter(i);
        if (paras > max)
        {
            LogEvent(EventKind.Dos, $"resize {seg:X4} to {paras:X4} failed (max {max:X4})");
            Cpu.Bx = (ushort)max;
            DosError(8);
            return;
        }
        _blocks[i] = new MemBlock(seg, paras);
        LogEvent(EventKind.Dos, $"resize {seg:X4} to {paras:X4}");
        DosOk();
    }
}
