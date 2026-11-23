using Traders.Audio.Effects;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Xunit.Abstractions;

namespace Traders.Tests.Audio;

/// <summary>
/// <see cref="EffectMachine"/> against the running game with the resident FXDRIVER (music plays): from the start of T.EXE every
/// call of the effect machine (0000:06FE, 0724, 0750 with its argument, and the timer tick 0732 from the game's timer chain over
/// the callback 27C6:041B) is replayed into a port built from T.EXE in the same order. The OPL writes made by code in segment
/// 0000 between two such calls must equal the port's writes for the call; at every call the machine's variables CS:03EC–06FD
/// must equal the port's. After the boot to the main menu, effect requests are put into the ring between run slices (the same
/// two memory writes 0750 makes, applied to both sides), so effects run while the menu music plays.
/// </summary>
public class EffectsGameDifferentialTests(ITestOutputHelper output)
{
    private enum Kind { Reset, Stop, Enqueue, Tick }

    private sealed record Call(Kind Kind, ushort Argument, int FirstWrite);

    // [DIFFERENTIAL] 0000:0732, 0000:0750, 0000:06FE
    [GameDataFact]
    public void Effects_in_the_running_game_match_the_original()
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, runFxDriver: true);
        var m = p.Machine;
        ushort cs = p.Segment(0x0000);
        var calls = new List<Call>();
        var writes = new List<(byte Register, byte Value)>();
        var states = new List<byte[]>();
        var previous = m.AdLib.RegisterWritten;
        m.AdLib.RegisterWritten = (reg, value) =>
        {
            previous?.Invoke(reg, value);
            if (m.Cpu.Cs == cs) writes.Add(((byte)reg, value));
        };
        void Record(Kind kind, ushort argument)
        {
            states.Add(m.Mem.ReadBlock(cs, EffectsHarness.VariablesStart, EffectsHarness.VariablesEnd - EffectsHarness.VariablesStart));
            calls.Add(new Call(kind, argument, writes.Count));
        }
        m.AddCodeHook(cs, 0x06FE, _ => Record(Kind.Reset, 0));
        m.AddCodeHook(cs, 0x0724, _ => Record(Kind.Stop, 0));
        m.AddCodeHook(cs, 0x0750, mm => Record(Kind.Enqueue, mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 4))));
        m.AddCodeHook(cs, 0x0732, _ => Record(Kind.Tick, 0));

        var port = new EffectMachine(EffectImage.FromExe(File.ReadAllBytes(TestGameData.Dos("T.EXE"))));
        int replayed = 0, ticks = 0;
        var trace = new OplTrace(m);

        // Replays the calls recorded so far; the state at a call's entry is compared with the port before it.
        void Replay(int upTo)
        {
            for (; replayed < upTo; replayed++)
            {
                var c = calls[replayed];
                var state = states[replayed];
                for (int i = 0; i < state.Length; i++)
                {
                    if (state[i] != port.Memory[EffectsHarness.VariablesStart + i])
                        Assert.Fail($"call {replayed} ({c.Kind}, tick {ticks}): CS:{EffectsHarness.VariablesStart + i:X4} original {state[i]:X2}, port {port.Memory[EffectsHarness.VariablesStart + i]:X2}");
                }
                port.ClearWrites();
                switch (c.Kind)
                {
                    case Kind.Reset: port.Reset(); break;
                    case Kind.Stop: port.Stop(); break;
                    case Kind.Enqueue: port.Enqueue(c.Argument); break;
                    default: port.Tick(); ticks++; break;
                }
                int end = replayed + 1 < calls.Count ? calls[replayed + 1].FirstWrite : writes.Count;
                var original = writes.GetRange(c.FirstWrite, end - c.FirstWrite);
                var own = port.Writes.Select(w => (w.Register, w.Value)).ToList();
                if (!original.SequenceEqual(own))
                    Assert.Fail($"call {replayed} ({c.Kind}, tick {ticks}): OPL writes differ\n original: {string.Join(" ", original.Select(w => $"{w.Register:X2}={w.Value:X2}"))}\n port:     {string.Join(" ", own.Select(w => $"{w.Register:X2}={w.Value:X2}"))}");
            }
        }

        // boot to the main menu (menu music) and start the default game: 1777 calls 21A7:000D (reset, callback 0732);
        // replay all but the last call (its writes may still be incomplete)
        m.RunFor(OriginalProgram.MainMenuTime);
        int menuMusic = trace.Writes.Count - writes.Count;
        p.Press(Traders.RefEmu.Hardware.Scancodes.Enter, delay: 0.5);
        m.RunFor(3.0);
        output.WriteLine($"start: {string.Join(", ", calls.GroupBy(c => c.Kind).Select(g => $"{g.Key} {g.Count()}"))}, menu music writes {menuMusic}");
        Assert.True(calls.Count(c => c.Kind == Kind.Tick) > 100, "the timer chain did not call the effect machine");
        Replay(calls.Count - 1);

        var r = new Random(0x0732);
        int[] effects = [0x01, 0x07, 0x0D, 0x12, 0x17, 0x24, 0x28, 0x2F, 0x3C, 0x3D, 0x41, 0x45, 0x4B, 0x4F, 0x52];
        for (int slice = 0; slice < 60; slice++)
        {
            // the two memory writes of 0750 (ring slot [0417], head + 1 mod 16), on both sides
            int requests = r.Next(0, 3);
            if (requests > 0)
            {
                // the pending call (if any) must be replayed first, so the port is at the same point
                m.Run(10_000_000, mm => mm.Cpu.Cs != cs);
                Replay(calls.Count);
                for (int i = 0; i < requests; i++)
                {
                    ushort effect = (ushort)effects[r.Next(effects.Length)];
                    ushort head = m.Mem.Read16(cs, EffectMachine.RingHead);
                    m.Mem.Write16(cs, (ushort)(EffectMachine.Ring + head), effect);
                    m.Mem.Write16(cs, EffectMachine.RingHead, (ushort)((head + 1) & 0x0F));
                    port.Enqueue(effect);
                }
            }
            m.RunFor(0.05 + r.NextDouble() * 0.4);
            Replay(calls.Count - 1);
        }
        m.Run(10_000_000, mm => mm.Cpu.Cs != cs);
        Replay(calls.Count);
        int other = trace.Writes.Count - writes.Count;
        output.WriteLine($"calls {calls.Count}, ticks {ticks}, effect writes {writes.Count}, other OPL writes (FXDRIVER) {other}, week {p.Week}");
        Assert.True(writes.Count > 500, "too few effect writes");
    }
}
