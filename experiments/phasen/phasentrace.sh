#!/usr/bin/env bash
# Lane "phasen": phase trace of the original T.EXE in the reference emulator (taken over from dev/ap5-phasen, 8ed0d34).
#
#   bash experiments/phasen/phasentrace.sh <absolute output dir> [emulated seconds per run, default 1500]
#
# Needs the original files (gamedata/ from tools/claude/fetch-reference.sh original), ndisasm and dotnet (source the env
# file of tools/claude/setup-workspace.sh first). Generates the call graph with tools/callgraph.py into <out>/callgraph
# (local only, it contains original texts - never commit it), builds a small console program outside the repository
# against src/Traders.RefEmu, boots the game, starts a game with four computer players and records every entry into the
# phase functions (from callgraph.json), call stacks sampled twice per second, the phase markers 21FD:0977(n), the keys
# it had to press, and frames shortly after each phase start.
# Output: <out>/<run>/{trace.txt,samples.txt,counts.txt,frames/*.png,events.log} - frames are derived from the original
# and stay local as well.
set -euo pipefail
out="$(realpath "$1")"
seconds="${2:-1500}"
here="$(cd "$(dirname "$0")" && pwd)"
repo="$(cd "$here/../.." && pwd)"
dos="$(realpath "$repo/gamedata/dos")"
[ -f "$dos/T.EXE" ] || { echo "Originaldaten fehlen ($repo/gamedata/dos/T.EXE)" >&2; exit 2; }
mkdir -p "$out"
python3 -I "$repo/tools/callgraph.py" "$dos/T.EXE" "$out/callgraph"
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT
mkdir -p "$work/PhaseTrace"
cat > "$work/PhaseTrace/PhaseTrace.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="$repo/src/Traders.RefEmu/Traders.RefEmu.csproj" />
  </ItemGroup>
</Project>
EOF
cat > "$work/PhaseTrace/Program.cs" <<'EOF'
using System.Globalization;
using System.Text.Json;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;

// args: dosDir callgraphJson outDir runName seconds [pokes...]   poke = dsOffset(hex)=value(hex)[:b|w]
var dosDir = args[0]; var callgraph = args[1]; var outDir = args[2]; var run = args[3];
double seconds = double.Parse(args[4], CultureInfo.InvariantCulture);
var pokes = args.Skip(5).ToArray();
var dir = Path.Combine(outDir, run);
Directory.CreateDirectory(Path.Combine(dir, "frames"));
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

// ---- functions from the call graph ----
var funcs = new List<(int Lin, int Size, string Id, string? Name, string[] Calls)>();
using (var doc = JsonDocument.Parse(File.ReadAllText(callgraph)))
    foreach (var f in doc.RootElement.EnumerateArray())
    {
        string id = f.GetProperty("id").GetString()!;
        int lin = Convert.ToInt32(id[..4], 16) * 16 + Convert.ToInt32(id[5..], 16);
        string? nm = f.GetProperty("name").ValueKind == JsonValueKind.String ? f.GetProperty("name").GetString() : null;
        var calls = f.GetProperty("calls").EnumerateArray().Select(c => c.GetProperty("target").GetString()!).ToArray();
        funcs.Add((lin, f.GetProperty("size").GetInt32(), id, nm, calls));
    }
funcs.Sort((a, b) => a.Lin.CompareTo(b.Lin));
var byId = funcs.ToDictionary(f => f.Id);
string[] helperSegs = ["0000", "0A96", "0A69", "23CD", "21FD", "186C", "21A7", "218B", "21AF", "27C6", "0D73", "2295", "2821", "2850"];
bool IsHelper(string id) => helperSegs.Contains(id[..4]) && id != "21FD:0977" && id != "21FD:021F" && id != "21FD:0687";

// logged = game functions up to depth 3 below main; everything else is only counted
var depth = new Dictionary<string, int> { ["1777:000C"] = 0 };
var queue = new Queue<string>(); queue.Enqueue("1777:000C");
while (queue.Count > 0)
{
    var id = queue.Dequeue();
    if (depth[id] >= 3) continue;
    foreach (var c in byId[id].Calls)
        if (!IsHelper(c) && !depth.ContainsKey(c)) { depth[c] = depth[id] + 1; queue.Enqueue(c); }
}
const int CodeEnd = 0x28DC0;
var kindAt = new byte[CodeEnd]; // 0 none, 1 counted, 2 logged
var idAt = new string?[CodeEnd];
foreach (var f in funcs)
{
    if (f.Id.StartsWith("0000")) continue;
    kindAt[f.Lin] = depth.ContainsKey(f.Id) ? (byte)2 : (byte)1;
    idAt[f.Lin] = f.Id;
}
var topLevel = new HashSet<string>(byId["1777:01F8"].Calls.Concat(byId["1777:000C"].Calls).Concat(byId["20B3:0005"].Calls));
string Label(string id) => byId[id].Name is { } n ? $"{id} {n}" : id;
string FuncAt(int lin)
{
    int lo = 0, hi = funcs.Count - 1, best = -1;
    while (lo <= hi) { int mid = (lo + hi) / 2; if (funcs[mid].Lin <= lin) { best = mid; lo = mid + 1; } else hi = mid - 1; }
    return best >= 0 && lin < funcs[best].Lin + funcs[best].Size ? funcs[best].Id : $"?{lin:X5}";
}

// ---- boot ----
var prog = OriginalProgram.BootToMainMenu(dosDir);
var m = prog.Machine;
ushort load = m.LoadSegment;
foreach (var p in pokes)
{
    var parts = p.Split('=', ':');
    ushort off = Convert.ToUInt16(parts[0], 16); ushort val = Convert.ToUInt16(parts[1], 16);
    if (parts.Length > 2 && parts[2] == "b") prog.WriteByte(off, (byte)val); else prog.WriteWord(off, val);
}

var trace = File.CreateText(Path.Combine(dir, "trace.txt"));
var samples = File.CreateText(Path.Combine(dir, "samples.txt"));
var counts = new Dictionary<(int Week, string Id), int>();
var pendingFrames = new List<(double Time, string Tag)>();
double lastLogged = m.Seconds;
int frameNo = 0;
string State()
{
    int week = (short)prog.ReadWord(0x4B9A);
    int ap = (sbyte)prog.ReadByte(0x37AA);
    uint seed = prog.ReadDword(0x10D2);
    var cash = string.Join(",", Enumerable.Range(0, 4).Select(i => (int)prog.ReadDword((ushort)(0x3CD2 + 4 * i))));
    return $"week={week} player={ap} seed={seed:X8} cash={cash}";
}
trace.WriteLine($"# run {run}, pokes: {string.Join(' ', pokes)}");
trace.WriteLine($"{m.Seconds,9:F2}s boot finished (main menu) {State()}");

bool Hook(Traders.RefEmu.Machine.PcMachine mm)
{
    int lin = ((mm.Cpu.Cs - load) << 4) + mm.Cpu.IP;
    if ((uint)lin >= CodeEnd) return false;
    byte k = kindAt[lin];
    if (k == 0) return false;
    string id = idAt[lin]!;
    int week = (short)prog.ReadWord(0x4B9A);
    counts[(week, id)] = counts.GetValueOrDefault((week, id)) + 1;
    if (k == 2)
    {
        string extra = "";
        if (id == "21FD:0977") extra = $" n={mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 4))}";
        string indent = new(' ', 2 * Math.Max(0, depth[id] - 1));
        trace.WriteLine($"{mm.Seconds,9:F2}s {indent}{Label(id)}{extra}  [{State()}]");
        lastLogged = mm.Seconds;
        if (topLevel.Contains(id) && id != "21FD:0977")
        {
            pendingFrames.Add((mm.Seconds + 1.5, id.Replace(':', '_') + "_a"));
            pendingFrames.Add((mm.Seconds + 5.0, id.Replace(':', '_') + "_b"));
        }
    }
    return false;
}

void PressKey(byte sc, string why)
{
    m.Keyboard.Enqueue(sc);
    double t = m.Seconds;
    trace.WriteLine($"{t,9:F2}s KEY {sc:X2} ({why})");
    m.RunFor(0.25, Hook);
    m.Keyboard.Enqueue((byte)(sc | 0x80));
}

string Stack()
{
    var names = new List<string> { FuncAt(((m.Cpu.Cs - load) << 4) + m.Cpu.IP) };
    ushort bp = m.Cpu.Bp;
    for (int i = 0; i < 24 && bp != 0; i++)
    {
        ushort ip = m.Mem.Read16(m.Cpu.Ss, (ushort)(bp + 2));
        ushort cs = m.Mem.Read16(m.Cpu.Ss, (ushort)(bp + 4));
        int lin = ((cs - load) << 4) + ip;
        if (lin < 0 || lin >= CodeEnd) break;
        names.Add(FuncAt(lin));
        ushort next = m.Mem.Read16(m.Cpu.Ss, bp);
        if (next <= bp) break;
        bp = next;
    }
    return string.Join(" < ", names);
}

// start the game from the main menu: Enter
PressKey(0x1C, "main menu: start");
string lastHash = "";
double lastFrameChange = m.Seconds;
int stallKeys = 0;
try
{
    while (m.Seconds < seconds && !m.Exited)
    {
        m.RunFor(0.5, Hook);
        var f = FrameDump.Capture(m.Vga);
        string h = f.IsEmpty ? "" : f.Hash();
        if (h != lastHash) { lastHash = h; lastFrameChange = m.Seconds; }
        samples.WriteLine($"{m.Seconds,9:F2}s {h} {State()} | {Stack()}");
        foreach (var pf in pendingFrames.Where(x => x.Time <= m.Seconds).ToList())
        {
            pendingFrames.Remove(pf);
            if (!f.IsEmpty) f.WritePng(Path.Combine(dir, "frames", $"{++frameNo:D4}_{m.Seconds:0000.0}s_{pf.Tag}.png"));
        }
        // nothing new for a while: the original waits for input (computer-only games still do at some screens)
        if (m.Seconds - lastLogged > 20 && m.Seconds - lastFrameChange > 3)
        {
            if (!f.IsEmpty) f.WritePng(Path.Combine(dir, "frames", $"{++frameNo:D4}_{m.Seconds:0000.0}s_wait.png"));
            trace.WriteLine($"{m.Seconds,9:F2}s WAIT in {Stack()}");
            PressKey(0x1C, "stall");
            lastLogged = m.Seconds;
            if (++stallKeys > 200) break;
        }
    }
}
catch (Exception e)
{
    trace.WriteLine($"EXCEPTION {e}");
}
trace.WriteLine($"{m.Seconds,9:F2}s end: exited={m.Exited} code={m.ExitCode} {State()}");
trace.Dispose();
samples.Dispose();
using (var w = File.CreateText(Path.Combine(dir, "counts.txt")))
    foreach (var g in counts.GroupBy(c => c.Key.Week).OrderBy(g => g.Key))
    {
        w.WriteLine($"## week {g.Key}");
        foreach (var c in g.OrderBy(c => c.Key.Id)) w.WriteLine($"{Label(c.Key.Id)} {c.Value}");
    }
using (var w = File.CreateText(Path.Combine(dir, "events.log")))
    foreach (var e in m.Log.Events.Where(e => e.Kind is Traders.RefEmu.Machine.EventKind.Keyboard or Traders.RefEmu.Machine.EventKind.File or Traders.RefEmu.Machine.EventKind.Clock))
        w.WriteLine(e);
Console.WriteLine($"{run}: {m.Seconds:F1} s emulated, exited={m.Exited}");
EOF
cd "$work/PhaseTrace"
dotnet build -c Release -nologo -v q
dll="$work/PhaseTrace/bin/Release/net10.0/PhaseTrace.dll"
# A: default settings (level 2: oil, gems, catastrophes on; robot battle off), four computer players
dotnet "$dll" "$dos" "$out/callgraph/callgraph.json" "$out" A-standard "$seconds" > "$out/A.log" 2>&1 || echo "run A failed" >> "$out/A.log" &
# B: level-3 options poked into DGROUP before the game starts (robot battle on, pirates HIGH)
dotnet "$dll" "$dos" "$out/callgraph/callgraph.json" "$out" B-roboter "$seconds" 08EA=3 411A=1:b 45BA=1:b 4BF2=1:b 2F30=1:b 0900=3 2E8E=2 > "$out/B.log" 2>&1 || echo "run B failed" >> "$out/B.log" &
wait
du -sh "$out"/* || true
