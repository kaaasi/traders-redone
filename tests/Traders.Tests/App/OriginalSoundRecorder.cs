using Traders.Core.Sound;
using Traders.RefEmu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.App;

/// <summary>Records int 64h (from the RefEmu's log) and the calls of the effect machine and of 27C6:041B, in order.</summary>
internal sealed class OriginalSoundRecorder
{
    private readonly OriginalProgram _p;
    private readonly List<(int LogIndex, SoundCommand Command)> _hooked = [];

    public OriginalSoundRecorder(OriginalProgram p)
    {
        _p = p;
        var m = p.Machine;
        m.Log.Capacity = int.MaxValue; // the int 64h calls are read from the log: nothing may be dropped on long runs
        void Hook(ushort seg, ushort off, Func<PcMachine, SoundCommand> record) =>
            m.AddCodeHook(p.Segment(seg), off, mm => _hooked.Add((mm.Log.Events.Count, record(mm))));
        Hook(SoundControl.EffectsSegment, 0x06FE, _ => new SoundCommand(SoundCommandKind.EffectsReset));
        Hook(SoundControl.EffectsSegment, 0x0724, _ => new SoundCommand(SoundCommandKind.EffectsStop));
        Hook(SoundControl.EffectsSegment, 0x0750, mm => new SoundCommand(SoundCommandKind.Effect, Bx: Arg(mm, 0)));
        Hook(SoundControl.SpeakerSegment, 0x00CC, mm => new SoundCommand(SoundCommandKind.SpeakerEffect, Bx: Arg(mm, 0)));
        Hook(0x27C6, 0x041B, mm => new SoundCommand(SoundCommandKind.TickCallback,
            Bx: (ushort)(Arg(mm, 1) - mm.LoadSegment), Cx: Arg(mm, 0)));
    }

    private static ushort Arg(PcMachine m, int index) => m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 4 + 2 * index));

    private readonly List<SoundCommand> _commands = [];
    private int _logDone, _hookDone;

    /// <summary>
    /// The commands so far, int 64h with the registers the game sets on purpose (the same list, extended at every call: the
    /// log is read only from where the last call stopped).
    /// </summary>
    public List<SoundCommand> Commands()
    {
        var log = _p.Machine.Log.Events;
        if (_p.Machine.Log.Dropped > 0) throw new InvalidOperationException("RefEmu log dropped events");
        for (; _logDone < log.Count; _logDone++)
        {
            for (; _hookDone < _hooked.Count && _hooked[_hookDone].LogIndex <= _logDone; _hookDone++) _commands.Add(_hooked[_hookDone].Command);
            if (log[_logDone].Kind != EventKind.Sound) continue;
            var f = log[_logDone].Text.Split(' ').Skip(2).Select(t => Convert.ToUInt16(t[3..], 16)).ToArray(); // AX= BX= CX= DX=
            _commands.Add(SoundCommand.Driver(f[0], f[1], f[2], f[3]));
        }
        for (; _hookDone < _hooked.Count; _hookDone++) _commands.Add(_hooked[_hookDone].Command);
        return _commands;
    }

    /// <summary>Host values out of the comparison: buffer addresses of function 10h, the name's segment of function 00h.</summary>
    public static SoundCommand Comparable(SoundCommand c, ushort nameSegment) => c switch
    {
        { Kind: SoundCommandKind.Driver, Function: SoundCommand.Buffer } => c with { Cx = 0, Dx = 0 },
        { Kind: SoundCommandKind.Driver, Function: SoundCommand.Load } => c with { Bx = (ushort)(c.Bx - nameSegment) },
        _ => c,
    };
}
