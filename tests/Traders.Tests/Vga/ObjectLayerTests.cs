using Traders.Tests.RefEmu;
using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the moving objects 21AF: the records at DS:330C and their pointer tables are set up by the
/// original routines (000D, 00CA, 022B, 0412, 045E, 0472) and by the port; then the driver loops 23CD:06E9/071C run
/// on these records. Compared: whole data segment, DAC, retrace waits, VRAM, draw page and the RAM atlas (save buffers).
/// </summary>
public class ObjectLayerTests
{
    private const ushort Objects = 0x21AF, Driver = 0x23CD;

    private static LayerHarness Start(int seed)
    {
        var h = new LayerHarness(seed);
        h.WriteDwordBoth(GraphicsLayers.AtlasPointer, (uint)DriverHarness.AtlasSeg << 16);
        h.WriteDwordBoth(GraphicsLayers.MaskPointer, (uint)DriverHarness.MaskSeg << 16);
        h.Randomize(GraphicsLayers.ObjectTable, GraphicsLayers.ObjectCount * GraphicsLayers.ObjectSize);
        h.Randomize(GraphicsLayers.ObjectRestoreRecords, 0x40);
        h.Call(Objects, 0x000D);
        h.Port.InitializeObjects();
        h.AssertStateEqual("21AF:000D");
        return h;
    }

    private static void Both(LayerHarness h, ushort segment, ushort offset, ushort[] args, Action<GraphicsLayers> port, string what)
    {
        h.Call(segment, offset, args);
        port(h.Port);
        h.AssertStateEqual(what);
    }

    /// <summary>Images for frames 0…3, save buffers, height and position of all eight objects.</summary>
    private static void SetUpObjects(LayerHarness h)
    {
        for (ushort i = 0; i < 8; i++)
        {
            ushort height = h.Next(1, 24);
            for (ushort f = 0; f < 4; f++)
            {
                ushort x = h.Next(0, 300), y = h.Next(0, 256 - height), mx = h.Next(0, 300), my = h.Next(0, 200 - height);
                Both(h, Objects, 0x00CA, [i, f, x, y, mx, my], l => l.SetObjectImage(i, f, x, y, mx, my), $"00CA({i},{f},{x},{y},{mx},{my})");
            }
            ushort x1 = h.Next(0, 300), y1 = h.Next(0, 256 - height), x2 = h.Next(0, 300), y2 = h.Next(0, 256 - height);
            Both(h, Objects, 0x022B, [i, x1, y1, x2, y2], l => l.SetObjectSaveBuffers(i, x1, y1, x2, y2), $"022B({i},{x1},{y1},{x2},{y2})");
            h.WriteWordBoth(GraphicsLayers.ObjectTable + i * GraphicsLayers.ObjectSize + 2, height);
        }
    }

    private static void Move(LayerHarness h)
    {
        for (ushort i = 0; i < 8; i++)
        {
            int at = GraphicsLayers.ObjectTable + i * GraphicsLayers.ObjectSize;
            h.WriteWordBoth(at + 0x14, h.Next(0, 304));
            h.WriteWordBoth(at + 0x16, h.Next(0, 200 - h.Port.Ds[at + 2]));
            switch (h.Rng.Next(5))
            {
                case 0:
                    Both(h, Objects, 0x045E, [i], l => l.HideObject(i), $"045E({i})");
                    break;
                case 1 or 2:
                {
                    ushort f = h.Next(0, 3);
                    Both(h, Objects, 0x0412, [i, f], l => l.ShowObject(i, f), $"0412({i},{f})");
                    break;
                }
                default:
                    h.WriteWordBoth(at + 0x18, h.Next(0, 3));   // frame changed directly, as the game does
                    break;
            }
        }
    }

    // [DIFFERENTIAL] 21AF:000D
    // [DIFFERENTIAL] 21AF:00CA
    // [DIFFERENTIAL] 21AF:022B
    // [DIFFERENTIAL] 21AF:0412
    // [DIFFERENTIAL] 21AF:045E
    // [DIFFERENTIAL] 21AF:0472
    // [DIFFERENTIAL] 23CD:06E9
    // [DIFFERENTIAL] 23CD:071C
    [GameDataFact]
    public void Objects_drawn_and_restored_from_DS_match_original()
    {
        var h = Start(400);
        SetUpObjects(h);
        for (int round = 0; round < 24; round++)
        {
            ushort page = (ushort)(round & 1);
            h.Driver.SetPage(page == 0 ? (ushort)0xA000 : (ushort)0xA400);
            if (round % 2 == 0 || round > 2)
            {
                Both(h, Driver, 0x071C, [page], l => l.RestoreObjects(page), $"071C({page}) round {round}");
                h.AssertScreenEqual($"071C({page}) round {round}");
                h.Driver.AssertAtlasEqual($"071C round {round}");
            }
            Move(h);
            Both(h, Driver, 0x06E9, [page], l => l.DrawObjects(page), $"06E9({page}) round {round}");
            h.AssertScreenEqual($"06E9({page}) round {round}");
            h.Driver.AssertAtlasEqual($"06E9 round {round}");
            if (round == 12)
                Both(h, Objects, 0x0472, [], l => l.HideAllObjects(), "0472");
        }
    }

    /// <summary>Image and save buffer pointers on the other cards (offset arithmetic only, without normalisation).</summary>
    [GameDataFact]
    public void Object_pointers_for_EGA_and_Tandy_match_original()
    {
        foreach (ushort mode in new ushort[] { 0, 2, 0x63 })
        {
            var h = Start(410 + mode);
            h.WriteWordBoth(GraphicsLayers.CardMode, mode);
            for (int k = 0; k < 30; k++)
            {
                ushort i = h.Next(0, 7), f = h.Next(0, 20), x = h.Next(0, 319), y = h.Next(0, 255), mx = h.Next(0, 319), my = h.Next(0, 199);
                Both(h, Objects, 0x00CA, [i, f, x, y, mx, my], l => l.SetObjectImage(i, f, x, y, mx, my), $"mode {mode} 00CA #{k}");
                ushort x2 = h.Next(0, 319), y2 = h.Next(0, 255);
                Both(h, Objects, 0x022B, [i, x, y, x2, y2], l => l.SetObjectSaveBuffers(i, x, y, x2, y2), $"mode {mode} 022B #{k}");
            }
        }
    }

    /// <summary>VGA pointers with all frames 0…20 and the atlas pointer not at offset 0 (normalisation with carry).</summary>
    // [DIFFERENTIAL] 0000:5101
    [GameDataFact]
    public void Object_pointer_normalisation_matches_original()
    {
        var h = Start(420);
        foreach (uint atlas in new uint[] { 0x5000_0000, 0x4FFF_0010, 0x4F00_FFF8, 0x4000_8000 })
        {
            h.WriteDwordBoth(GraphicsLayers.AtlasPointer, atlas);
            for (int k = 0; k < 25; k++)
            {
                ushort i = h.Next(0, 7), f = (ushort)(k % 21), x = h.Next(0, 319), y = h.Next(0, 255), mx = h.Next(0, 319), my = h.Next(0, 199);
                Both(h, Objects, 0x00CA, [i, f, x, y, mx, my], l => l.SetObjectImage(i, f, x, y, mx, my), $"{atlas:X8} 00CA #{k}");
                ushort x2 = h.Next(0, 319), y2 = h.Next(0, 255);
                Both(h, Objects, 0x022B, [i, x, y, x2, y2], l => l.SetObjectSaveBuffers(i, x, y, x2, y2), $"{atlas:X8} 022B #{k}");
            }
        }
    }
}
