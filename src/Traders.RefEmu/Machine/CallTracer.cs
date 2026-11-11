using System.Globalization;

namespace Traders.RefEmu.Machine;

/// <summary>
/// Records calls of far functions of the loaded program (e.g. the graphics driver 0A96/0A69):
/// return address, target, the first argument words and the return value. Addresses are reported relative
/// to the load image, as in the analysis. Observation only: the run is not changed.
/// </summary>
public sealed class CallTracer
{
    public sealed class Call
    {
        public long Index { get; init; }
        public long Instruction { get; init; }
        public long Frame { get; init; }
        public double Seconds { get; init; }
        public int Depth { get; init; }
        public ushort ReturnSeg { get; init; }
        public ushort ReturnOff { get; init; }
        public ushort TargetSeg { get; init; }
        public ushort TargetOff { get; init; }
        public ushort[] Args { get; init; } = [];
        public bool Returned { get; set; }
        public ushort Ax { get; set; }
        public ushort Dx { get; set; }
        public long ReturnInstruction { get; set; }
    }

    private sealed record Pending(Call Call, ushort Ss, ushort Sp);

    private readonly PcMachine _m;
    private readonly ushort _base;
    private readonly int _argWords;
    private readonly List<Call> _calls = [];
    private readonly Dictionary<uint, List<Pending>> _pending = [];
    private int _depth;

    /// <summary>Maximum number of recorded calls (later calls are counted but not stored).</summary>
    public int Capacity { get; set; } = 2_000_000;

    public long TotalCalls { get; private set; }

    public IReadOnlyList<Call> Calls => _calls;

    /// <summary>Optional filter: only calls for which it returns true are recorded.</summary>
    public Func<PcMachine, bool>? Filter { get; set; }

    public CallTracer(PcMachine machine, IEnumerable<(ushort Seg, ushort Off)> imageTargets, int argWords = 6)
    {
        _m = machine;
        _base = machine.LoadSegment;
        _argWords = argWords;
        foreach (var (seg, off) in imageTargets)
        {
            ushort s = seg, o = off;
            machine.AddCodeHook((ushort)(_base + s), o, m => OnEntry(s, o));
        }
    }

    private void OnEntry(ushort seg, ushort off)
    {
        TotalCalls++;
        if (_calls.Count >= Capacity || (Filter is not null && !Filter(_m))) return;
        var c = _m.Cpu;
        var mem = _m.Mem;
        ushort retIp = mem.Read16(c.Ss, c.Sp);
        ushort retCs = mem.Read16(c.Ss, (ushort)(c.Sp + 2));
        var args = new ushort[_argWords];
        for (int i = 0; i < _argWords; i++) args[i] = mem.Read16(c.Ss, (ushort)(c.Sp + 4 + 2 * i));
        var call = new Call
        {
            Index = _calls.Count,
            Instruction = c.InstructionCount,
            Frame = _m.FrameNumber,
            Seconds = _m.Seconds,
            Depth = _depth,
            ReturnSeg = (ushort)(retCs - _base),
            ReturnOff = retIp,
            TargetSeg = seg,
            TargetOff = off,
            Args = args,
        };
        _calls.Add(call);
        _depth++;

        uint lin = (((uint)retCs << 4) + retIp) & 0xFFFFF;
        if (!_pending.TryGetValue(lin, out var list))
        {
            _pending[lin] = list = [];
            _m.AddCodeHook(retCs, retIp, _ => OnReturn(lin));
        }
        list.Add(new Pending(call, c.Ss, (ushort)(c.Sp + 4)));
    }

    private void OnReturn(uint lin)
    {
        var list = _pending[lin];
        var c = _m.Cpu;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var p = list[i];
            if (p.Ss != c.Ss || p.Sp != c.Sp) continue;
            p.Call.Returned = true;
            p.Call.Ax = c.Ax;
            p.Call.Dx = c.Dx;
            p.Call.ReturnInstruction = c.InstructionCount;
            list.RemoveAt(i);
            _depth = Math.Max(0, _depth - 1);
            return;
        }
    }

    /// <summary>One line per call: index, frame, seconds, depth, target, caller, args, return value.</summary>
    public void WriteText(TextWriter w)
    {
        w.WriteLine("# index frame seconds depth target return-address args... -> ax dx (image-relative addresses)");
        foreach (var c in _calls)
        {
            w.Write(c.Index.ToString(CultureInfo.InvariantCulture));
            w.Write(' ');
            w.Write(c.Frame.ToString(CultureInfo.InvariantCulture));
            w.Write(' ');
            w.Write(c.Seconds.ToString("F4", CultureInfo.InvariantCulture));
            w.Write($" {c.Depth} {c.TargetSeg:X4}:{c.TargetOff:X4} {c.ReturnSeg:X4}:{c.ReturnOff:X4}");
            foreach (var a in c.Args) w.Write($" {a:X4}");
            w.WriteLine(c.Returned ? $" -> {c.Ax:X4} {c.Dx:X4}" : " -> ?");
        }
        if (TotalCalls > _calls.Count) w.WriteLine($"# {TotalCalls - _calls.Count} further calls not stored");
    }

    /// <summary>Number of calls per target (for an overview).</summary>
    public IEnumerable<(string Target, int Count)> Histogram() =>
        _calls.GroupBy(c => $"{c.TargetSeg:X4}:{c.TargetOff:X4}").Select(g => (g.Key, g.Count())).OrderBy(t => t.Key);
}
