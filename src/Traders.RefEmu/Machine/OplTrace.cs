using System.Globalization;
using Traders.RefEmu.Hardware;

namespace Traders.RefEmu.Machine;

/// <summary>
/// Records every OPL2 register write (from the game or from a resident FXDRIVER) with emulated
/// time, for comparing a music/effects player against the original.
/// </summary>
public sealed class OplTrace
{
    public readonly record struct Write(long Instruction, long Time, byte Register, byte Value)
    {
        public double Seconds => Time / (double)Pit.ClockHz;
    }

    private readonly List<Write> _writes = [];

    public IReadOnlyList<Write> Writes => _writes;

    public int Capacity { get; set; } = 5_000_000;

    public OplTrace(PcMachine machine)
    {
        var previous = machine.AdLib.RegisterWritten;
        machine.AdLib.RegisterWritten = (reg, value) =>
        {
            previous?.Invoke(reg, value);
            if (_writes.Count < Capacity) _writes.Add(new Write(machine.Cpu.InstructionCount, machine.Now, (byte)reg, value));
        };
    }

    /// <summary>One line per write: PIT clock time, seconds, register, value (hex).</summary>
    public void WriteText(TextWriter w)
    {
        w.WriteLine("# pit-clocks seconds register value  (PIT clock = 1/1193182 s)");
        foreach (var x in _writes)
            w.WriteLine($"{x.Time.ToString(CultureInfo.InvariantCulture)} {x.Seconds.ToString("F6", CultureInfo.InvariantCulture)} {x.Register:X2} {x.Value:X2}");
    }
}
