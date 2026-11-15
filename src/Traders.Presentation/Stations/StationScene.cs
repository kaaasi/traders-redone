using Traders.Core.State;
using Traders.Core.Stations;
using Traders.Presentation.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Presentation.Stations;

/// <summary>
/// Drawing side of the stations phase 1B03:0009 (finding docs/routines/1B03_0009_stationen.md, section "Szene"): plays every
/// <see cref="StationEvent"/> of the core's <see cref="StationPhase"/> through the graphics layers – command list 0A96,
/// shadowed and typed texts 0A69, numbers in texts 2295, objects 21AF/23CD, pages 0B67, palettes, fades, flips and retrace
/// waits 21FD, the picture load 186C. The events come in the order and with the arguments of the original's calls; the scene
/// receives each one when the core reports it (<see cref="StationPhase.Output"/>), so what it reads from the core's data
/// segment (object records, key table) is the state at that call. The core's data segment is never written; the graphics
/// state lives in the layers' own data segment image, the number texts 2295:0001 in the text buffer behind
/// <see cref="GraphicsLayers.OtherMemory"/>. Retrace waits go to the layers' <see cref="GraphicsLayers.Retrace"/> (one per
/// frame of the original).
/// </summary>
public sealed class StationScene
{
    private const int ObjectCoreFields = 5;

    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly IPictureSource _pictures;

    /// <param name="layers">
    /// Graphics layers with their own data segment image (DataSegment = segment value of DS) and the text buffer DS:4884 as
    /// <see cref="GraphicsLayers.OtherMemory"/> (typed texts, names, number texts).
    /// </param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public StationScene(GraphicsLayers layers, Dgroup state, IPictureSource pictures)
    {
        _gfx = layers;
        _state = state;
        _pictures = pictures;
    }

    public GraphicsLayers Layers => _gfx;

    /// <summary>Sounds 21A7:0054(channel, sound) of the phase, for the sound layer (optional).</summary>
    public Action<short, short>? Sound { get; set; }

    private static ushort W(short v) => unchecked((ushort)v);

    /// <summary>A segment argument: the core's placeholder for DS becomes the layers' DS segment.</summary>
    private ushort Seg(short v) => v == StationEvent.DataSegment ? _gfx.DataSegment : W(v);

    private static uint Long(short low, short high) => (uint)(W(low) | W(high) << 16);

    /// <summary>Plays one output call of the phase.</summary>
    public void Apply(StationEvent e)
    {
        var a = e.Args;
        MirrorKeys();
        switch (e.Call)
        {
            // ---------------------------------------------------------- command list 0A96
            case StationCall.ClearList: _gfx.ClearList(); break;
            case StationCall.RunList: _gfx.ExecuteList(); break;
            case StationCall.Blit: _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.Blit8: _gfx.AddBlit8(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case StationCall.Plot: _gfx.AddPixel(W(a[0]), W(a[1]), W(a[2])); break;
            case StationCall.Text: _gfx.AddText(W(a[0]), W(a[1]), W(a[2]), Seg(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.CopyPage: _gfx.CopyGamePage(W(a[0])); break;

            // ---------------------------------------------------------- texts 0A69, numbers 2295
            case StationCall.ShadowText:
                _gfx.AddShadowText(W(a[0]), W(a[1]), W(a[2]), Seg(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case StationCall.ShadowNumber:
                _gfx.AddShadowNumber(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]), W(a[8]));
                break;
            case StationCall.TypedText:
                _gfx.CenteredTypewriter(W(a[0]), W(a[1]), W(a[2]), W(a[3]), Seg(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case StationCall.TypedLines:
                _gfx.TypewriterLines(W(a[0]), W(a[1]), W(a[2]), W(a[3]), Seg(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case StationCall.NumberText: _gfx.FormatNumber(Long(a[0], a[1]), W(a[2]), W(a[3]), Seg(a[4])); break;

            // ---------------------------------------------------------- objects 21AF, 23CD
            case StationCall.ObjectImage: _gfx.SetObjectImage(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.ObjectBuffers: _gfx.SetObjectSaveBuffers(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case StationCall.ShowObject: _gfx.ShowObject(W(a[0]), W(a[1])); break;
            case StationCall.HideObject: _gfx.HideObject(W(a[0])); break;
            case StationCall.DrawObjects: DrawObjects(W(a[0])); break;
            case StationCall.RestoreObjects: RestoreObjects(W(a[0])); break;

            // ---------------------------------------------------------- pages 0B67, frames 21FD
            case StationCall.DrawOnShownPage: _gfx.DrawOnShownPage(); break;
            case StationCall.DrawOnHiddenPage: _gfx.DrawOnHiddenPage(); break;
            case StationCall.Flip: _gfx.FlipPages(); break;
            case StationCall.ListOnBothPages:
                // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
                _gfx.ExecuteList();
                _gfx.FlipPages();
                _gfx.ExecuteList();
                break;
            case StationCall.LastFrame:
                // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C([082E]), 0A96:06A8, 23CD:06E9([082E])
                _gfx.FlipPages();
                RestoreObjects(DrawPage);
                _gfx.ExecuteList();
                DrawObjects(DrawPage);
                break;
            case StationCall.Delay: Delay(a[0], a[1]); break;
            case StationCall.Pause: break; // 21FD:0245: the pause key is the core's; nothing drawn

            // ---------------------------------------------------------- palettes 21FD, picture 186C, sound 21A7
            case StationCall.FadeOut: _gfx.FadeOut(W(a[0])); break;
            // [ORIGINAL] 21FD:026C – the argument is not used: loaded palette DS:5017 → base palette DS:3DCB
            case StationCall.BasePalette: _gfx.CopyLoadedToBase(); break;
            case StationCall.FadeIn: _gfx.FadeIn(); break;
            case StationCall.PaletteStep: _gfx.Blend(W(a[0]), W(a[1])); break;
            case StationCall.LoadPicture: LoadPicture(a[0]); break;
            case StationCall.Sound: Sound?.Invoke(a[0], a[1]); break;
            default: throw new NotSupportedException($"Station call {e.Call} is not played.");
        }
    }

    /// <summary>DS:082E of the layers: index of the page being drawn (21FD:051F).</summary>
    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    /// <summary>[ORIGINAL] 21FD:0185(n): n retraces, n a long (low, high); none when the high word is negative.</summary>
    private void Delay(short low, short high)
    {
        if (high < 0) return;
        long n = ((long)high << 16) | W(low);
        for (long i = 0; i < n; i++) _gfx.WaitRetrace();
    }

    /// <summary>[ORIGINAL] 23CD:071C(page) on the layers' records with the core's fields mirrored.</summary>
    private void RestoreObjects(ushort page)
    {
        MirrorObjects();
        _gfx.RestoreObjects(page);
    }

    /// <summary>[ORIGINAL] 23CD:06E9(page) on the layers' records with the core's fields mirrored.</summary>
    private void DrawObjects(ushort page)
    {
        MirrorObjects();
        _gfx.DrawObjects(page);
    }

    /// <summary>
    /// The core's fields of the eight object records DS:330C (+0 active, +2 height, +14h/+16h position, +18h frame) copied
    /// into the layers' records; image and save pointers and the restore records stay the layers'.
    /// </summary>
    private void MirrorObjects()
    {
        ReadOnlySpan<int> fields = [0x00, 0x02, 0x14, 0x16, 0x18];
        for (int i = 0; i < GraphicsLayers.ObjectCount; i++)
        {
            int at = GraphicsLayers.ObjectTable + i * GraphicsLayers.ObjectSize;
            for (int k = 0; k < ObjectCoreFields; k++)
            {
                int f = at + fields[k];
                _gfx.Ds[f] = _state.U8(f);
                if (k > 0) _gfx.Ds[f + 1] = _state.U8(f + 1);
            }
        }
    }

    /// <summary>The key table DS:3D47 as the core holds it (21FD:051F reads DS:3D95/3D91).</summary>
    private void MirrorKeys()
    {
        for (int sc = 0; sc < Core.Menus.KeyTable.Length; sc++)
            _gfx.Ds[DgroupLayout.KeyState + sc] = _state.U8(DgroupLayout.KeyState + sc);
    }

    /// <summary>
    /// Picture load 186C:0002(n) on VGA as the menu and field scenes do it: DS:09C8 = n, the decrunched picture's first 300h
    /// bytes become the loaded palette DS:5017, the rest goes to the start of the atlas as plane rows, then 21AF:000D.
    /// </summary>
    // [APPROX] Effect only, from the resource source instead of 186C (lane programm), like FieldScene.LoadPicture; the loader's
    // cache (resource table DS:09CE, EMS/disk state) is not modelled.
    private void LoadPicture(int resource)
    {
        var data = _pictures.Decrunched(resource);
        _gfx.Ds[0x09C8] = (byte)resource;
        _gfx.Ds[0x09C9] = (byte)(resource >> 8);
        Array.Copy(data, 0, _gfx.Ds, GraphicsLayers.LoadedPalette, 0x300);
        var atlas = _gfx.Driver.Atlas.Data;
        int n = Math.Min(atlas.Length, data.Length - 0x300);
        Array.Copy(data, 0x300, atlas, 0, n);
        PlanarAtlas.ConvertToPlanarRows(atlas, PlanarAtlas.HeaderSize, PlanarAtlas.Width, PlanarAtlas.Height);
        _gfx.InitializeObjects();
    }
}
