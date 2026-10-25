using System.IO.Compression;
using System.Text;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Runs a fixed subset (40 per instruction form) of the SingleStepTests 80286 real-mode suite
/// (MIT, Daniel Balsom; recorded on a real 80C286) against <see cref="Cpu286"/>.
/// Data built by src/Traders.RefEmu/Tools/make_cputests.py.
/// </summary>
public class Cpu286SingleStepTests
{
    private static readonly string[] RegNames = ["ax", "bx", "cx", "dx", "cs", "ss", "ds", "es", "sp", "bp", "si", "di", "ip", "flags"];

    private sealed record TestCase(string Name, byte[] Bytes, ushort[] Init, ushort[] Final, int Exception, uint FlagAddress,
        (uint Addr, byte Value)[] InitRam, (uint Addr, byte Value)[] FinalRam);

    private sealed record Group(string Name, ushort FlagsMask, List<TestCase> Tests);

    private static readonly Lazy<Dictionary<string, Group>> Groups = new(Load);

    public static TheoryData<string> GroupNames()
    {
        var data = new TheoryData<string>();
        foreach (var name in Groups.Value.Keys) data.Add(name);
        return data;
    }

    [Fact]
    public void Test_data_is_complete()
    {
        Assert.Equal(326, Groups.Value.Count);
        Assert.Equal(13040, Groups.Value.Values.Sum(g => g.Tests.Count));
    }

    [Theory]
    [MemberData(nameof(GroupNames))]
    public void Instruction_form_matches_real_286(string group)
    {
        var g = Groups.Value[group];
        var mem = new Memory { A20 = true };
        var failures = new StringBuilder();
        int failed = 0;
        foreach (var t in g.Tests)
        {
            string? err = Run(mem, t, g.FlagsMask);
            if (err is null) continue;
            failed++;
            if (failed <= 5) failures.AppendLine(err);
        }
        Assert.True(failed == 0, $"{failed}/{g.Tests.Count} failed in {group}:\n{failures}");
    }

    private static string? Run(Memory mem, TestCase t, ushort flagsMask)
    {
        Array.Clear(mem.Ram);
        foreach (var (a, v) in t.InitRam) mem.Ram[a] = v;
        var cpu = new Cpu286(mem);
        for (int i = 0; i < 8; i++) cpu.R[i] = 0;
        cpu.Ax = t.Init[0]; cpu.Bx = t.Init[1]; cpu.Cx = t.Init[2]; cpu.Dx = t.Init[3];
        cpu.Cs = t.Init[4]; cpu.Ss = t.Init[5]; cpu.Ds = t.Init[6]; cpu.Es = t.Init[7];
        cpu.Sp = t.Init[8]; cpu.Bp = t.Init[9]; cpu.Si = t.Init[10]; cpu.Di = t.Init[11];
        cpu.IP = t.Init[12]; cpu.Flags = t.Init[13];

        int steps = 0;
        try
        {
            while (!cpu.Halted && steps++ < 1000) cpu.Step();
        }
        catch (Exception e)
        {
            return $"{Describe(t)}: {e.GetType().Name} {e.Message}";
        }

        ushort[] got =
        [
            cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx, cpu.Cs, cpu.Ss, cpu.Ds, cpu.Es,
            cpu.Sp, cpu.Bp, cpu.Si, cpu.Di, cpu.IP, cpu.Flags,
        ];
        var diff = new StringBuilder();
        for (int i = 0; i < 14; i++)
        {
            ushort exp = t.Final[i], act = got[i];
            if (i == 13) { exp &= flagsMask; act &= flagsMask; }
            if (exp != act) diff.Append($" {RegNames[i]}={act:X4}(exp {exp:X4})");
        }
        foreach (var (a, v) in t.FinalRam)
        {
            byte exp = v, act = mem.Ram[a];
            if (t.Exception != 0xFF && (a == t.FlagAddress || a == t.FlagAddress + 1))
            {
                byte m = a == t.FlagAddress ? (byte)flagsMask : (byte)(flagsMask >> 8);
                exp &= m; act &= m;
            }
            if (exp != act) diff.Append($" [{a:X6}]={act:X2}(exp {exp:X2})");
        }
        return diff.Length == 0 ? null : $"{Describe(t)}:{diff}";
    }

    private static string Describe(TestCase t)
    {
        var init = string.Join(' ', RegNames.Select((n, i) => $"{n}={t.Init[i]:X4}"));
        string exc = t.Exception == 0xFF ? "" : $" exc={t.Exception}";
        return $"[{Convert.ToHexString(t.Bytes)}]{exc} {init}";
    }

    private static Dictionary<string, Group> Load()
    {
        var path = Path.Combine(RefEmuPaths.RepoRoot, "tests", "Traders.Tests", "RefEmu", "Data", "cputests-286.bin.gz");
        using var gz = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
        using var ms = new MemoryStream();
        gz.CopyTo(ms);
        var r = new BinaryReader(new MemoryStream(ms.ToArray()));
        if (Encoding.ASCII.GetString(r.ReadBytes(4)) != "CPT1") throw new InvalidDataException("Bad CPU test file.");
        uint groups = r.ReadUInt32();
        var result = new Dictionary<string, Group>();
        for (int gi = 0; gi < groups; gi++)
        {
            string name = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()));
            ushort mask = r.ReadUInt16();
            uint count = r.ReadUInt32();
            var tests = new List<TestCase>();
            for (int ti = 0; ti < count; ti++)
            {
                byte[] bytes = r.ReadBytes(r.ReadByte());
                var init = new ushort[14];
                var fin = new ushort[14];
                for (int i = 0; i < 14; i++) init[i] = r.ReadUInt16();
                for (int i = 0; i < 14; i++) fin[i] = r.ReadUInt16();
                int exc = r.ReadByte();
                uint fa = r.ReadUInt32();
                var ram = new (uint, byte)[2][];
                for (int k = 0; k < 2; k++)
                {
                    int n = r.ReadUInt16();
                    ram[k] = new (uint, byte)[n];
                    for (int i = 0; i < n; i++) ram[k][i] = (r.ReadUInt32(), r.ReadByte());
                }
                tests.Add(new TestCase(name, bytes, init, fin, exc, fa, ram[0], ram[1]));
            }
            result[name] = new Group(name, mask, tests);
        }
        return result;
    }
}
