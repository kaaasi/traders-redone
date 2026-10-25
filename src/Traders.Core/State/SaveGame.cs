using System.Buffers.Binary;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.State;

/// <summary>One value location in the save record: DS offset, width in bytes, sign extension.</summary>
public readonly record struct SaveSlot(int Offset, int Width, bool Signed);

/// <summary>
/// SAVEGAME.DAT exactly as 2336:000A writes and 2336:0539 reads it: every value is stored as a
/// little-endian 32-bit dword (bytes/words sign- or zero-extended, see <see cref="SaveSlot"/>),
/// in the order of <see cref="Slots"/>; the file is always 0x900 bytes long. 522 slots fill the
/// first 0x828 bytes; the rest of the original buffer (allocated far memory) is not initialised
/// by the game. Loading truncates each dword to the field width.
/// Side effect kept from the original: the global loop variables DS:37AA, 45D0, 2EB0 and 2F00
/// hold their final loop values afterwards.
/// </summary>
public static class SaveGame
{
    public const int FileSize = 0x900;
    public const string FileName = "SAVEGAME.DAT";

    /// <summary>The 52 store sites of 2336:000A in program order (loops not unrolled).</summary>
    public static IReadOnlyList<SaveSlot> Sites { get; } =
    [
        // per player p = 0..3 (DS:37AA)
        new(L.HqX, 1, true), new(L.HqY, 1, true), new(L.Rockets, 2, true), new(L.Oil, 2, true),
        new(L.GemCredit, 2, true), new(L.Gems, 1, true), new(L.Cash, 4, true), new(L.Station, 2, true),
        new(L.FatMikeFlag, 1, false), new(L.Unknown40F6, 2, true), new(L.IsComputer, 1, false),
        new(L.ControlDevice, 1, true), new(L.Unknown4BBE, 1, false), new(L.FieldCount, 1, false),
        new(L.FieldsByKind, 1, true), new(L.Stock, 2, true),        // k = 0..3, interleaved
        new(L.WealthHistory, 4, true),                               // 40 weeks
        new(L.RobotParts, 1, false),                                 // 6 parts
        new(L.Robot376C, 2, false), new(L.Robot376E, 2, false), new(L.Robot2EF8, 2, false),
        new(L.Robot377C, 1, false), new(L.Robot40EC, 2, false), new(L.Robot33DC, 2, false),
        new(L.Robot3016, 2, false),
        // globals
        new(L.WarePrice, 2, true),                                   // k = 0..3
        new(L.FieldOwner, 1, true), new(L.FieldKind, 1, true), new(L.FieldQuality, 1, true), // y 0..5, x 0..8
        new(L.FieldColumns, 2, true), new(L.FieldRows, 2, true),
        new(L.StationVisited, 1, false), new(L.StationKind, 2, true), new(L.StationValue, 2, true), // 12
        new(L.PiratePresence, 2, true), new(L.Weather, 1, true), new(L.FieldTotal, 1, true),
        new(L.Week, 2, true), new(L.RobotBattle, 1, false), new(L.OilConsumption, 1, false),
        new(L.InitialCash, 2, true), new(L.Target, 4, true), new(L.LottoReturns, 1, true),
        new(L.TimeLimit, 1, false), new(L.Unknown4580, 1, true), new(L.Level, 2, true),
        new(L.ComputerIq, 2, true), new(L.Catastrophes, 1, false), new(L.GemTrading, 1, false),
        new(L.RobotCycle, 1, true), new(L.BaseUnit, 4, true), new(L.GemPriceFactor, 4, true),
    ];

    /// <summary>All 522 slots in file order (loops unrolled).</summary>
    public static IReadOnlyList<SaveSlot> Slots { get; } = Unroll();

    private static List<SaveSlot> Unroll()
    {
        var s = Sites;
        var r = new List<SaveSlot>(522);
        for (int p = 0; p < L.PlayerCount; p++)
        {
            for (int i = 0; i <= 13; i++) r.Add(Index(s[i], p * s[i].Width));
            for (int k = 0; k < 4; k++)
            {
                r.Add(Index(s[14], p * 4 + k));
                r.Add(Index(s[15], p * 8 + k * 2));
            }
            for (int w = 0; w < L.WealthHistoryLength; w++) r.Add(Index(s[16], p * 0xA0 + w * 4));
            for (int i = 0; i < L.RobotPartCount; i++) r.Add(Index(s[17], p * 6 + i));
            r.Add(Index(s[18], p * 4));
            r.Add(Index(s[19], p * 4));
            r.Add(Index(s[20], p * 2));
            r.Add(Index(s[21], p));
            r.Add(Index(s[22], p * 2));
            r.Add(Index(s[23], p * 2));
            r.Add(Index(s[24], p * 2));
        }
        for (int k = 0; k < 4; k++) r.Add(Index(s[25], k * 2));
        for (int y = 0; y < L.FieldRowsMax; y++)
            for (int x = 0; x < L.FieldColumnsMax; x++)
                for (int i = 26; i <= 28; i++) r.Add(Index(s[i], L.FieldIndex(x, y)));
        r.Add(s[29]);
        r.Add(s[30]);
        for (int i = 0; i < L.StationCount; i++)
        {
            r.Add(Index(s[31], i));
            r.Add(Index(s[32], i * 2));
            r.Add(Index(s[33], i * 2));
        }
        for (int i = 34; i < s.Count; i++) r.Add(s[i]);
        return r;
    }

    private static SaveSlot Index(SaveSlot site, int delta) => site with { Offset = site.Offset + delta };

    /// <summary>Builds the file content from a DGROUP image (2336:000A). Unused tail bytes are zero.</summary>
    public static byte[] Write(Dgroup ds)
    {
        var file = new byte[FileSize];
        int pos = 0;
        foreach (var slot in Slots)
        {
            int value = slot.Width switch
            {
                1 => slot.Signed ? ds.S8(slot.Offset) : ds.U8(slot.Offset),
                2 => slot.Signed ? ds.S16(slot.Offset) : ds.U16(slot.Offset),
                _ => ds.S32(slot.Offset),
            };
            BinaryPrimitives.WriteInt32LittleEndian(file.AsSpan(pos), value);
            pos += 4;
        }
        SetLoopVariables(ds);
        return file;
    }

    /// <summary>
    /// Applies a save file to a DGROUP image (2336:0539). Returns false like the original when the
    /// file is not exactly 0x900 bytes; the image is then unchanged. Fields not in the save
    /// (e.g. land type, sound, random seed) keep their value.
    /// </summary>
    public static bool Read(ReadOnlySpan<byte> file, Dgroup ds)
    {
        if (file.Length != FileSize) return false;
        int pos = 0;
        foreach (var slot in Slots)
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(file[pos..]);
            pos += 4;
            switch (slot.Width)
            {
                case 1: ds.Set(slot.Offset, unchecked((byte)value)); break;
                case 2: ds.Set(slot.Offset, unchecked((ushort)value)); break;
                default: ds.Set(slot.Offset, value); break;
            }
        }
        SetLoopVariables(ds);
        return true;
    }

    /// <summary>Convenience: save/load a <see cref="GameState"/> through a DGROUP image.</summary>
    public static byte[] Write(GameState state)
    {
        var ds = new Dgroup();
        GameStateMapper.Store(state, ds);
        return Write(ds);
    }

    /// <summary>
    /// The original uses global variables as loop counters; after save/load they hold:
    /// player 4 (37AA), ware/price index 4 (45D0), field x 9 (2EB0) and y 6 (2F00).
    /// </summary>
    private static void SetLoopVariables(Dgroup ds)
    {
        ds.Set(L.ActivePlayer, (sbyte)4);
        ds.Set(LoopWare, (short)4);
        ds.Set(LoopFieldX, (sbyte)9);
        ds.Set(LoopFieldY, (sbyte)6);
    }

    /// <summary>DS:45D0 word loop variable (wares, price list).</summary>
    public const int LoopWare = 0x45D0;
    /// <summary>DS:2EB0 byte loop variable (field column).</summary>
    public const int LoopFieldX = 0x2EB0;
    /// <summary>DS:2F00 byte loop variable (field row).</summary>
    public const int LoopFieldY = 0x2F00;
}
