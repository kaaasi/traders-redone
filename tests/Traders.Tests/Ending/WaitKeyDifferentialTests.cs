using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.Ending;

/// <summary>
/// The any-key wait 27C6:00D2 (finding docs/routines/27C6_0043_eingabe_zeitgeber.md, 00D2) against the original: after the
/// boot (keyboard handler 27C6:0043 installed) the make counter DS:115D holds an arbitrary value, a key is pressed a while
/// after the call; the original returns once the make has been counted. The port's <see cref="EndScreens.WaitKey"/> clears
/// DS:115D, polls per frame and ends in the frame after the keyboard handler's increment (here done by the test as the host
/// does). Compared: DS:115D after the wait; the port waits at least one frame.
/// </summary>
public class WaitKeyDifferentialTests
{
    // [DIFFERENTIAL] 27C6:00D2
    [GameDataTheory]
    [InlineData(0x00)]
    [InlineData(0x37)]
    [InlineData(0xFF)]
    public void Wait_for_a_key_matches_the_original(byte before)
    {
        var p = OriginalProgram.BootToMainMenu(TestGameData.DosDir);
        p.WriteByte(EndScreens.MakeCount, before);
        p.Press(Scancodes.Enter, delay: 0.5);
        p.Call(0x27C6, 0x00D2);
        byte original = p.ReadByte(EndScreens.MakeCount);

        var d = new Dgroup();
        d.Set(EndScreens.MakeCount, before);
        var wait = EndScreens.WaitKey(d, new Host()).GetEnumerator();
        int polls = 0;
        while (wait.MoveNext())
        {
            Assert.Equal(FieldWait.Poll, wait.Current);
            polls++;
            if (polls == 3) d.Set(EndScreens.MakeCount, unchecked((byte)(d.U8(EndScreens.MakeCount) + 1))); // 27C6:0043, a make
        }
        Assert.Equal(1, original);
        Assert.Equal(original, d.U8(EndScreens.MakeCount));
        Assert.Equal(3, polls);
    }

    private sealed class Host : IEndingHost
    {
        public void ComputerTurn(Dgroup d, short x, short y) => throw new InvalidOperationException();
        public void Emit(FieldEvent e) { }
        public void Emit(EndingEvent e) => Assert.Equal(EndingEventKind.WaitKey, e.Kind);
        public IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random) => [];
    }
}
