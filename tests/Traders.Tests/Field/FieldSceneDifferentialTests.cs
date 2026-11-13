using Traders.Core.Field;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.RefEmu;
using H = Traders.Tests.Field.FieldSceneHarness;

namespace Traders.Tests.Field;

/// <summary>
/// Pixel tests of the field layer's drawing routines 0609:1ED9, 1256, 1F04, 1879, 1854/1A61 and of the cursor step's drawing
/// events (0609:1B72 time bar, 0EDD rank icon) against the RefEmu on a week state (finding docs/routines/0609_1256_rahmen.md):
/// same arguments, list executed, all four planes equal. The scene must not change the core's data segment.
/// </summary>
public class FieldSceneDifferentialTests
{
    private const ushort Seg = 0x0609;

    private static ushort W(int v) => unchecked((ushort)v);

    private static (int, byte)[] Player(int p) => [(FieldGrid.CurrentPlayer, (byte)p)];

    private static void AssertReadOnly(H.Case c) => H.AssertReadOnly(c);

    // [DIFFERENTIAL] 0609:1ED9
    [GameDataFact]
    public void Background_matches_the_original()
    {
        var c = new H.Case("1ED9", [], p => p.Call(Seg, 0x1ED9), (_, s) => s.Background());
        Assert.True(H.Draws(c), "1ED9 draws nothing on the week state");
        AssertReadOnly(c);
    }

    // [DIFFERENTIAL] 0609:1256
    // [DIFFERENTIAL] 0609:1F04
    [GameDataFact]
    public void Frame_and_interior_match_the_original()
    {
        // the callers' frames (0825:0072, 08CF:2078, 111D:0066, 111D:0773), the smallest frame, then random frames
        var frames = new List<(short X, short Y, short W, short H)>
            { (0, 0, 0x140, 0xC8), (0, 0, 0x140, 0x4C), (0x8C, 0x88, 0xA0, 0x2C), (0x28, 0x4B, 0xF0, 0x32), (0, 0, 0x20, 0x2C),
              (0x130 - 0x20, 0xC8 - 0x2C, 0x20, 0x2C), (3, 5, 0x21, 0x2D), (1, 2, 0x13F, 0xC6) };
        var r = new Random(0x1256);
        for (int i = 0; i < 24; i++)
        {
            short w = (short)r.Next(0x20, 0x141), h = (short)r.Next(0x2C, 0xC9);
            frames.Add(((short)r.Next(0, 0x141 - w), (short)r.Next(0, 0xC9 - h), w, h));
        }
        foreach (var (x, y, w, h) in frames)
        {
            var c = new H.Case($"1256({x},{y},{w},{h})", [],
                p => p.Call(Seg, 0x1256, W(x), W(y), W(w), W(h)), (_, s) => s.Frame(x, y, w, h));
            AssertReadOnly(c);
        }
        Assert.True(H.Draws(new H.Case("1256 draws", [], p => p.Call(Seg, 0x1256, 0x28, 0x4B, 0xF0, 0x32), (_, _) => { })));
        for (int i = 0; i < 8; i++)
        {
            short w = (short)r.Next(0, 0x141), h = (short)r.Next(0, 0xC9);
            short x = (short)r.Next(0, 0x141 - w), y = (short)r.Next(0, 0xC9 - h);
            AssertReadOnly(new H.Case($"1F04({x},{y},{w},{h})", [],
                p => p.Call(Seg, 0x1F04, W(x), W(y), W(w), W(h)), (_, s) => s.Interior(x, y, w, h)));
        }
    }

    // [DIFFERENTIAL] 0609:1879
    [GameDataFact]
    public void Emblem_matches_the_original_for_every_player_and_rank()
    {
        var r = new Random(0x1879);
        bool drawn = false;
        for (int p = 0; p < 4; p++)
            foreach (short rank in new short[] { -1, 0, 1, 2, 3, 4, 5, 6, -32768 })
            {
                short x = (short)r.Next(0, 0x110), y = (short)r.Next(0, 0x9C);
                if (rank == 0 && p == 0) (x, y) = (0x100, 0x0E); // 0825:0072, player 0
                var c = new H.Case($"1879({x},{y},{rank}) p {p}", Player(p),
                    o => o.Call(Seg, 0x1879, W(x), W(y), W(rank)), (_, s) => s.Emblem(x, y, rank));
                drawn |= rank == 3 && H.Draws(c);
                AssertReadOnly(c);
            }
        Assert.True(drawn, "1879 draws nothing");
    }

    // [DIFFERENTIAL] 0609:1854
    // [DIFFERENTIAL] 0609:1A61
    [GameDataFact]
    public void Rank_icon_with_marker_matches_the_original()
    {
        var r = new Random(0x1854);
        for (int p = 0; p < 4; p++)
            foreach (byte rank in new byte[] { 0, 1, 3, 5, 0x80, 0xFF })
                foreach (byte marker in new byte[] { 0, 1, 2 })
                {
                    short x = (short)((p << 6) + 0x20), y = 6; // 0609:0EDD
                    if (r.Next(2) == 0) (x, y) = ((short)r.Next(0, 0x110), (short)r.Next(0, 0x9C));
                    var c = new H.Case($"1854({x},{y}) p {p} rank {rank} marker {marker}",
                        [(FieldGrid.CurrentPlayer, (byte)p), (0x3D42 + p, rank), (0x43FA + p, marker)],
                        o => o.Call(Seg, 0x1854, W(x), W(y)), (_, s) => s.Rank(x, y));
                    AssertReadOnly(c);
                }
    }

    /// <summary>
    /// The cursor step's events end to end: the original's 0609:1B72 and 0609:0EDD against the core's
    /// <see cref="FieldCursor.TimeBar"/>/<see cref="FieldCursor.OutOfTime"/> with the scene playing their events.
    /// </summary>
    [GameDataFact]
    public void Time_bar_and_out_of_time_events_draw_like_the_original()
    {
        var r = new Random(0x1B72);
        for (int n = 0; n < 40; n++)
        {
            int p = n % 4;
            short left = (short)r.Next(0, 0x30);
            sbyte step = r.Next(2) == 0 ? (sbyte)1 : (sbyte)-1;
            var c = new H.Case($"1B72({step}) p {p} left {left}",
                [(FieldGrid.CurrentPlayer, (byte)p), (FieldCursor.TimeLeft + 2 * p, (byte)left), (FieldCursor.TimeLeft + 2 * p + 1, 0)],
                o => o.Call(Seg, 0x1B72, W(step)),
                (d, s) => FieldCursor.TimeBar(d, step, new H.SceneOutput(s)));
            var (ods, core) = H.AssertSame(c);
            Assert.Equal(ods[FieldCursor.TimeLeft + 2 * p], core[FieldCursor.TimeLeft + 2 * p]);
        }
        for (int p = 0; p < 4; p++)
            foreach (byte rank in new byte[] { 0, 2, 4 })
            {
                var c = new H.Case($"0EDD p {p} rank {rank}",
                    [(FieldGrid.CurrentPlayer, (byte)p), (0x3D42 + p, rank), (0x43FA + p, (byte)(rank == 2 ? 1 : 0))],
                    o => o.Call(Seg, 0x0EDD), (d, s) => FieldCursor.OutOfTime(d, new H.SceneOutput(s)));
                var (ods, core) = H.AssertSame(c);
                Assert.Equal(ods[FieldCursor.Mode + p], core[FieldCursor.Mode + p]);
            }
    }
}
