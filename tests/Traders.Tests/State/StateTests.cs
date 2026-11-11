using System.Text.Json;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.State;

public class StateTests
{
    private static string RepoFile(params string[] parts) => RepositoryPaths.File(parts);

    private static Dgroup OriginalDgroup() => Dgroup.FromExe(File.ReadAllBytes(TestGameData.Dos("T.EXE")));

    private static string Json(GameState s) => JsonSerializer.Serialize(s);

    /// <summary>Fills every modelled field with distinct, width-exhausting values.</summary>
    internal static GameState Scrambled(int seed)
    {
        var r = new Random(seed);
        sbyte S8() => (sbyte)r.Next(-128, 128);
        byte U8() => (byte)r.Next(256);
        short S16() => (short)r.Next(short.MinValue, short.MaxValue + 1);
        ushort U16() => (ushort)r.Next(65536);
        int S32() => r.Next(int.MinValue, int.MaxValue);

        var s = new GameState();
        var o = s.Settings;
        o.Level = S16(); o.ComputerIq = S16(); o.OilConsumption = U8(); o.RobotBattle = U8();
        o.GemTrading = U8(); o.Catastrophes = U8(); o.PiratePresence = S16(); o.InitialCash = S16();
        o.Target = S32(); o.LottoReturns = S8(); o.TimeLimit = U8(); o.LandType = U8();
        o.RobotCycle = S8(); o.FieldColumns = S16(); o.FieldRows = S16(); o.Sound = S16();
        foreach (var p in s.Players)
        {
            p.IsComputer = U8(); p.ControlDevice = S8(); p.Cash = S32(); p.Rockets = S16(); p.Oil = S16();
            p.Gems = S8(); p.GemCredit = S16(); p.HqX = S8(); p.HqY = S8(); p.Station = S16();
            p.FatMikeFlag = U8(); p.Unknown40F6 = S16(); p.Unknown4BBE = U8(); p.FieldCount = U8();
            for (int k = 0; k < 4; k++) { p.FieldsByKind[k] = S8(); p.Stock[k] = S16(); }
            for (int w = 0; w < p.WealthHistory.Length; w++) p.WealthHistory[w] = S32();
            for (int i = 0; i < p.RobotParts.Length; i++) p.RobotParts[i] = U8();
            p.Robot376C = U16(); p.Robot376E = U16(); p.Robot2EF8 = U16(); p.Robot377C = U8();
            p.Robot40EC = U16(); p.Robot33DC = U16(); p.Robot3016 = U16();
        }
        foreach (var f in s.Fields) { f.Owner = S8(); f.Kind = S8(); f.Quality = S8(); }
        foreach (var st in s.Stations) { st.Visited = U8(); st.Kind = S16(); st.Value = S16(); }
        for (int k = 0; k < 4; k++) s.WarePrices[k] = S16();
        s.Weather = S8(); s.FieldTotal = S8(); s.Week = S16(); s.Unknown4580 = S8();
        s.BaseUnit = S32(); s.GemPriceFactor = S32(); s.ActivePlayer = S8(); s.RandomSeed = (uint)S32();
        return s;
    }

    [Fact]
    public void Save_sites_match_the_original_save_routine()
    {
        // Reference extracted independently from the disassembly of 2336:000A by
        // tools/datenlayout/savegame.py.
        var expected = File.ReadAllLines(RepoFile("tests", "Traders.Tests", "State", "SaveGameSites.txt"))
            .Where(l => !l.StartsWith('#') && l.Length > 0)
            .Select(l => l.Split(' '))
            .Select(f => $"{f[0]} {f[1]} {f[2]}")
            .ToList();
        var actual = SaveGame.Sites.Select(s => $"{s.Offset:X4} {s.Width} {(s.Signed ? "signed" : "unsigned")}").ToList();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Save_record_has_522_dwords_and_fits_the_file()
    {
        Assert.Equal(522, SaveGame.Slots.Count);
        Assert.True(SaveGame.Slots.Count * 4 <= SaveGame.FileSize);
        // every DS byte is written at most once
        var bytes = SaveGame.Slots.SelectMany(s => Enumerable.Range(s.Offset, s.Width)).ToList();
        Assert.Equal(bytes.Count, bytes.Distinct().Count());
    }

    [Fact]
    public void Every_save_slot_is_covered_by_the_state_model()
    {
        // Storing a scrambled state must touch every byte the save routine reads.
        var a = new Dgroup();
        var b = new Dgroup();
        Array.Fill(b.Bytes, (byte)0x5A);
        var s = Scrambled(1);
        GameStateMapper.Store(s, a);
        GameStateMapper.Store(s, b);
        foreach (var slot in SaveGame.Slots)
            for (int i = 0; i < slot.Width; i++)
                Assert.Equal(a.Bytes[slot.Offset + i], b.Bytes[slot.Offset + i]);
    }

    [Fact]
    public void Dgroup_mapping_round_trips()
    {
        var s = Scrambled(2);
        var ds = new Dgroup();
        GameStateMapper.Store(s, ds);
        Assert.Equal(Json(s), Json(GameStateMapper.Load(ds)));
    }

    [Fact]
    public void Save_and_load_round_trip_keeps_unsaved_fields()
    {
        var s = Scrambled(3);
        var file = SaveGame.Write(s);
        Assert.Equal(SaveGame.FileSize, file.Length);

        var other = Scrambled(4);
        var ds = new Dgroup();
        GameStateMapper.Store(other, ds);
        Assert.True(SaveGame.Read(file, ds));
        var loaded = GameStateMapper.Load(ds);

        // not part of the save: keep the previous game's values
        s.Settings.LandType = other.Settings.LandType;
        s.Settings.Sound = other.Settings.Sound;
        s.RandomSeed = other.RandomSeed;
        s.ActivePlayer = 4; // loop variable after 2336:0539
        Assert.Equal(Json(s), Json(loaded));
    }

    [Fact]
    public void Save_extends_bytes_and_words_like_the_original()
    {
        var s = new GameState();
        s.Players[0].HqX = -1;             // signed byte  -> 0xFFFFFFFF
        s.Players[0].FatMikeFlag = 0xFF;    // unsigned byte -> 0x000000FF
        s.Players[0].Rockets = -2;          // signed word  -> 0xFFFFFFFE
        s.Players[0].Robot376C = 0xFFFF;    // unsigned word -> 0x0000FFFF
        s.Players[0].Cash = 123456;
        var file = SaveGame.Write(s);
        int Dword(int slot) => BitConverter.ToInt32(file, slot * 4);
        Assert.Equal(-1, Dword(0));
        Assert.Equal(-2, Dword(2));
        Assert.Equal(123456, Dword(6));
        Assert.Equal(0xFF, Dword(8));
        int robot = SaveGame.Slots.ToList().FindIndex(x => x.Offset == L.Robot376C);
        Assert.Equal(0xFFFF, Dword(robot));
    }

    [Fact]
    public void Load_rejects_files_of_wrong_size()
    {
        var ds = new Dgroup();
        Assert.False(SaveGame.Read(new byte[0x8FF], ds));
        Assert.All(ds.Bytes, b => Assert.Equal(0, b));
    }

    [GameDataFact]
    public void Initial_state_matches_initialised_data_in_TEXE()
    {
        var fromExe = GameStateMapper.Load(OriginalDgroup());
        Assert.Equal(Json(GameState.OriginalInitial()), Json(fromExe));
    }

    [GameDataFact]
    public void Level_presets_in_TEXE_are_as_documented()
    {
        // 192D:0592 copies these tables (DS:0B90..0BAF) when a level 0..3 is chosen.
        var ds = OriginalDgroup();
        short[] iq = [0, 1, 2, 2], pirates = [0, 1, 2, 3];
        byte[] oil = [0, 1, 1, 1], robot = [0, 0, 0, 1], gems = [0, 0, 1, 1], cat = [0, 1, 1, 1];
        for (int lv = 0; lv < 4; lv++)
        {
            Assert.Equal(iq[lv], ds.S16(0x0B90 + lv * 2));
            Assert.Equal(oil[lv], ds.U8(0x0B98 + lv));
            Assert.Equal(robot[lv], ds.U8(0x0B9C + lv));
            Assert.Equal(gems[lv], ds.U8(0x0BA0 + lv));
            Assert.Equal(cat[lv], ds.U8(0x0BA4 + lv));
            Assert.Equal(pirates[lv], ds.S16(0x0BA8 + lv * 2));
        }
    }

    [GameDataFact]
    public void Text_pointer_table_and_station_names_are_where_documented()
    {
        var ds = OriginalDgroup();
        // station names: 12 zero-terminated strings from DS:0DE7
        var names = new List<string>();
        int o = 0x0DE7;
        for (int i = 0; i < 12; i++)
        {
            int e = Array.IndexOf(ds.Bytes, (byte)0, o);
            names.Add(System.Text.Encoding.ASCII.GetString(ds.Bytes, o, e - o));
            o = e + 1;
        }
        string[] expected = ["VULCAN", "NECRONOM", "MATT", "MELMAC", "IRATA", "PSION 1998", "ACHERON", "SUB7",
            "CANSIZYAN", "SUM-SAGRO", "TAO", "LAVE"];
        Assert.Equal(expected, names);
        Assert.Equal(1u, ds.U32(L.RandomSeed));
    }

    [Fact]
    public void State_serialises_to_json_and_back()
    {
        var s = Scrambled(5);
        var back = JsonSerializer.Deserialize<GameState>(Json(s));
        Assert.NotNull(back);
        Assert.Equal(Json(s), Json(back!));
    }
}
