using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;

namespace Traders.Tests.Ai;

/// <summary>
/// Differential tests of the computer turn 135D (finding docs/routines/135D_000E_computerzug.md). The same data segment goes
/// into the original (RefEmu, LoadOnly, direct Call of the entry points 000E, 0935, 0AB3) and the port; the whole data segment
/// (including the action DS:3308 and the random state DS:10D2, which must stay unchanged – 135D draws no random) must match.
/// </summary>
public class PlantingDifferentialTests
{
    private const ushort Seg = 0x135D;

    private const int Cur = 0x37AA, Cols = 0x08FC, Rows = 0x08FE, Owner = 0x4442, Kind = 0x2EB1, Quality = 0x45DA;
    private const int Mode = 0x43B2, MoveDelay = 0x3786, HqX = 0x40FE, HqY = 0x4102;
    private const int Rockets = 0x4BAC, TimeLeft = 0x4520, TimeLimit = 0x45BA, FreeFields = 0x2F05, IsComp = 0x0840;
    private const int WantKind = 0x40F6, CandCount = 0x48EC, CandIndex = 0x3C90, Marker43FA = 0x43FA, Sel = 0x2E94;
    private const int CandX = 0x4654, CandY = 0x472C, CandKind = 0x2F31, Stock = 0x3CF4, LimitHigh = 0x48A2;
    private const int Marker4580 = 0x4580, FatMike = 0x4BC2, T2EEA = 0x2EEA, ComputerIq = 0x2E8E, KindWare2 = 0x08E7;

    private static (OriginalProgram P, byte[] Initial) Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        return (p, p.DataSegment());
    }

    private static void Arrange(OriginalProgram p, byte[] initial, Random r)
    {
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void W(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));

        int cols = r.Next(4, 9), rows = r.Next(3, 7);
        W(Cols, cols);
        W(Rows, rows);
        B(Cur, r.Next(4));
        W(ComputerIq, r.Next(0, 4));
        B(LimitHigh, r.Next(0, 40));
        B(TimeLimit, r.Next(3) == 0 ? 0 : 1);
        B(FreeFields, r.Next(3) == 0 ? 0 : r.Next(1, 40));
        B(Marker4580, r.Next(5) == 0 ? r.Next(4) : 0xFF);

        for (int pl = 0; pl < 4; pl++)
        {
            bool hasHq = r.Next(5) != 0;
            B(HqX + pl, hasHq ? r.Next(cols) : 0xFF);
            B(HqY + pl, r.Next(rows));
            B(IsComp + pl, r.Next(2));
            W(Rockets + 2 * pl, r.Next(3) == 0 ? 0 : r.Next(1, 11));
            W(TimeLeft + 2 * pl, r.Next(4) == 0 ? r.Next(-4, 10) : r.Next(0, 48));
            W(WantKind + 2 * pl, r.Next(0, 3));
            W(CandIndex + 2 * pl, r.Next(0, 6));
            B(Marker43FA + pl, r.Next(2));
            B(Sel + pl, r.Next(-1, 5));
            B(FatMike + pl, r.Next(2));
            W(MoveDelay + 4 * pl, r.Next(3) == 0 ? r.Next(1, 5) : 0);
            W(MoveDelay + 4 * pl + 2, 0);
            B(T2EEA + 4 * pl, r.Next(0, 4));
            W(CandCount + 2 * pl, r.Next(0, 6));
            for (int w = 0; w < 4; w++) W(Stock + 8 * pl + 2 * w, r.Next(0, 999));
            for (int i = 0; i < 54; i++)
            {
                B(CandX + pl * 54 + i, r.Next(3) == 0 ? 0xFF : r.Next(cols));
                B(CandY + pl * 54 + i, r.Next(rows));
                B(CandKind + pl * 54 + i, r.Next(0, 4));
            }
        }
        for (int k = 0; k < 3; k++) B(KindWare2 + k, r.Next(0, 4));

        for (int c = 0; c < 9 * 6; c++)
        {
            B(Owner + c, r.Next(3) == 0 ? 0xFF : r.Next(4));
            B(Kind + c, new[] { -2, 0, 1, 2, 3, 4 }[r.Next(6)]);
            B(Quality + c, r.Next(0, 4));
        }
    }

    // [DIFFERENTIAL] 135D:000E
    // [DIFFERENTIAL] 135D:00AC
    // [DIFFERENTIAL] 135D:0240
    // [DIFFERENTIAL] 135D:0263
    // [DIFFERENTIAL] 135D:0284
    // [DIFFERENTIAL] 135D:04F5
    // [DIFFERENTIAL] 135D:0A59
    // [DIFFERENTIAL] 135D:02BD
    // [DIFFERENTIAL] 135D:0480
    // [DIFFERENTIAL] 135D:05AF
    // [DIFFERENTIAL] 135D:06A1
    // [DIFFERENTIAL] 135D:0851
    [GameDataFact]
    public void ComputerTurn_matches_the_original_for_all_modes()
    {
        var (p, initial) = Load();
        var r = new Random(0x000E);
        var modesSeen = new HashSet<int>();
        for (int n = 0; n < 4000; n++)
        {
            Arrange(p, initial, r);
            int cur = p.ReadByte(Cur);
            p.WriteByte((ushort)(Mode + cur), (byte)r.Next(0, 8));
            short x = (short)(r.Next(10) == 0 ? r.Next(-2, 11) : r.Next(0, p.ReadWord(Cols)));
            short y = (short)(r.Next(10) == 0 ? r.Next(-2, 9) : r.Next(0, p.ReadWord(Rows)));
            modesSeen.Add(p.ReadByte((ushort)(Mode + cur)));
            var ds = new Dgroup(p.DataSegment());
            PlantingAi.ComputerTurn(ds, x, y);
            p.Call(Seg, 0x000E, (ushort)x, (ushort)y);
            ControlHarness.AssertSameData($"000E case {n} mode {ds.U8(Mode + ds.S8(Cur))}", p.DataSegment(), ds.Bytes);
        }
        for (int m = 2; m <= 5; m++) Assert.Contains(m, modesSeen);
    }

    // [DIFFERENTIAL] 135D:0935
    [GameDataFact]
    public void CommitCandidate_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x0935);
        for (int n = 0; n < 3000; n++)
        {
            Arrange(p, initial, r);
            var ds = new Dgroup(p.DataSegment());
            PlantingAi.CommitCandidate(ds);
            p.Call(Seg, 0x0935);
            ControlHarness.AssertSameData($"0935 case {n}", p.DataSegment(), ds.Bytes);
        }
    }

    // [DIFFERENTIAL] 135D:0AB3
    [GameDataFact]
    public void ChooseWantKind_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x0AB3);
        for (int n = 0; n < 2000; n++)
        {
            Arrange(p, initial, r);
            int cur = p.ReadByte(Cur);
            p.WriteByte((ushort)(HqX + cur), (byte)r.Next(p.ReadWord(Cols)));
            p.WriteByte((ushort)(HqY + cur), (byte)r.Next(p.ReadWord(Rows)));
            var ds = new Dgroup(p.DataSegment());
            PlantingAi.ChooseWantKind(ds);
            p.Call(Seg, 0x0AB3);
            ControlHarness.AssertSameData($"0AB3 case {n}", p.DataSegment(), ds.Bytes);
        }
    }
}
