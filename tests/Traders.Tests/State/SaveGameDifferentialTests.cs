using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.RefEmu;
using Xunit.Abstractions;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.State;

/// <summary>
/// Differential tests of 2336:000A (save) and 2336:0539 (load): the original routines run in the
/// reference emulator after booting to the main menu (the save buffer DS:45D6 is the picture
/// decrunch buffer and must be allocated); the port (<see cref="SaveGame"/>) must produce the same
/// file bytes and the same data segment.
/// </summary>
public class SaveGameDifferentialTests(ITestOutputHelper output)
{
    private const ushort Seg = 0x2336, SaveOff = 0x000A, LoadOff = 0x0539;

    private static OriginalProgram Boot() =>
        OriginalProgram.BootToMainMenu(Path.Combine(RefEmuPaths.RepoRoot, "original", "dos"));

    private static Dgroup ReadDs(OriginalProgram p) =>
        new(p.Machine.Mem.ReadBlock(p.Dgroup, 0, L.Size));

    private static void WriteDs(OriginalProgram p, Dgroup ds) =>
        p.Machine.Mem.WriteBlock(((uint)p.Dgroup) << 4, ds.Bytes);

    /// <summary>Scrambled state written over the live data segment (only modelled fields change).</summary>
    private static Dgroup Prepare(OriginalProgram p, int seed)
    {
        var ds = ReadDs(p);
        GameStateMapper.Store(StateTests.Scrambled(seed), ds);
        WriteDs(p, ds);
        return ds;
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    public void Save_writes_the_same_file_as_the_original(int seed)
    {
        var p = Boot();
        var before = Prepare(p, seed);
        var expectedFile = SaveGame.Write(new Dgroup((byte[])before.Bytes.Clone()));

        var (ax, _) = p.Call(Seg, SaveOff);
        Assert.Equal(1, ax & 0xFF);
        Assert.True(p.Machine.Files.WrittenFiles.TryGetValue("SAVEGAME.DAT", out var file));
        Assert.Equal(SaveGame.FileSize, file!.Length);

        int used = SaveGame.Slots.Count * 4;
        for (int i = 0; i < used; i++)
            Assert.True(expectedFile[i] == file[i],
                $"byte {i:X3} (slot {i / 4}, DS:{SaveGame.Slots[i / 4].Offset:X4}): original {file[i]:X2}, port {expectedFile[i]:X2}");

        var after = ReadDs(p);
        Assert.Equal(4, after.S8(L.ActivePlayer));
        Assert.Equal(4, after.S16(SaveGame.LoopWare));
        Assert.Equal(9, after.S8(SaveGame.LoopFieldX));
        Assert.Equal(6, after.S8(SaveGame.LoopFieldY));
        // the save itself changes no modelled field
        var expectedState = GameStateMapper.Load(before);
        expectedState.ActivePlayer = 4;
        Assert.Equal(Json(expectedState), Json(GameStateMapper.Load(after)));
    }

    [Theory]
    [InlineData(21, 22)]
    [InlineData(23, 24)]
    public void Load_sets_the_same_data_segment_as_the_original(int fileSeed, int previousSeed)
    {
        var p = Boot();
        var file = SaveGame.Write(StateTests.Scrambled(fileSeed));
        var before = Prepare(p, previousSeed);
        p.Machine.Files.Store("SAVEGAME.DAT", file);

        var expected = new Dgroup((byte[])before.Bytes.Clone());
        Assert.True(SaveGame.Read(file, expected));

        var (ax, _) = p.Call(Seg, LoadOff);
        Assert.Equal(1, ax & 0xFF);
        var after = ReadDs(p);

        // every saved byte and the loop variables exactly as predicted
        foreach (var slot in SaveGame.Slots)
            for (int i = 0; i < slot.Width; i++)
                Assert.True(expected.Bytes[slot.Offset + i] == after.Bytes[slot.Offset + i],
                    $"DS:{slot.Offset + i:X4}: original {after.Bytes[slot.Offset + i]:X2}, port {expected.Bytes[slot.Offset + i]:X2}");
        Assert.Equal(Json(GameStateMapper.Load(expected)), Json(GameStateMapper.Load(after)));
        foreach (int o in new[] { (int)L.ActivePlayer, SaveGame.LoopWare, SaveGame.LoopWare + 1, SaveGame.LoopFieldX, SaveGame.LoopFieldY })
            Assert.Equal(expected.Bytes[o], after.Bytes[o]);

        // everything else the original changed (field origin, buffer pointer, screen reload) – for the record
        var other = Enumerable.Range(0, L.Size).Where(i => expected.Bytes[i] != after.Bytes[i]).ToList();
        output.WriteLine($"other changed DS bytes: {string.Join(" ", other.Select(i => i.ToString("X4")))}");
    }

    [Fact]
    public void Load_of_a_short_file_fails_and_changes_no_saved_field()
    {
        var p = Boot();
        var before = Prepare(p, 31);
        p.Machine.Files.Store("SAVEGAME.DAT", new byte[0x8FF]);
        var (ax, _) = p.Call(Seg, LoadOff);
        Assert.Equal(0, ax & 0xFF);
        Assert.False(SaveGame.Read(new byte[0x8FF], before));
        var after = ReadDs(p);
        foreach (var slot in SaveGame.Slots)
            for (int i = 0; i < slot.Width; i++)
                Assert.Equal(before.Bytes[slot.Offset + i], after.Bytes[slot.Offset + i]);
    }

    private static string Json(GameState s) => System.Text.Json.JsonSerializer.Serialize(s);
}
