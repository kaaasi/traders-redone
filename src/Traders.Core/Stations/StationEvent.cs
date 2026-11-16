namespace Traders.Core.Stations;

/// <summary>
/// Output calls of the stations phase, one per original call into the output layers (graphics list 0A96, text 0A69, objects
/// 21AF/23CD, pages 0B67, palette and frames 21FD, sound 21A7, picture loader 186C, number text 2295). The core reports them in
/// the original order with the original arguments; the scene (part 3) plays them back. The routine address is given with each
/// kind.
/// </summary>
public enum StationCall
{
    /// <summary>0A96:001B – empty the graphics list.</summary>
    ClearList,
    /// <summary>0A96:06A8 – execute the graphics list.</summary>
    RunList,
    /// <summary>0A96:0030(x, y, w, h, sx, sy) – blit from the RAM atlas.</summary>
    Blit,
    /// <summary>0A96:0311(x, y, w, h, colour) – 8-pixel blit.</summary>
    Blit8,
    /// <summary>0A96:03FB(x, y, colour) – pixel.</summary>
    Plot,
    /// <summary>0A96:0525(x, y, text, DS, colour, shadow) – text from DS.</summary>
    Text,
    /// <summary>0A96:0C3F(page) – copy the hidden page.</summary>
    CopyPage,
    /// <summary>0A69:004D(x, y, text, segment, colour, shadow, ox, oy) – shadowed text.</summary>
    ShadowText,
    /// <summary>0A69:011B(x, y, low, high, digits, colour, shadow, ox, oy) – shadowed number.</summary>
    ShadowNumber,
    /// <summary>0A69:0243(x, y, w, text, segment, colour, wait, final wait) – centred typed text.</summary>
    TypedText,
    /// <summary>0A69:0288(x, y, w, lines, segment, count, colour, wait) – typed lines.</summary>
    TypedLines,
    /// <summary>21A7:0054(channel, sound) – sound effect.</summary>
    Sound,
    /// <summary>23CD:06E9(page) – draw the objects.</summary>
    DrawObjects,
    /// <summary>23CD:071C(page) – restore the backgrounds of the objects.</summary>
    RestoreObjects,
    /// <summary>186C:0002(picture) – load a picture.</summary>
    LoadPicture,
    /// <summary>21AF:00CA(object, frame, x, y, mx, my) – image and mask of an object frame.</summary>
    ObjectImage,
    /// <summary>21AF:022B(object, x1, y1, x2, y2) – save buffers of an object.</summary>
    ObjectBuffers,
    /// <summary>21AF:0412(object, frame) – show an object (core part: record +0, +18h).</summary>
    ShowObject,
    /// <summary>21AF:045E(object) – hide an object (core part: record +0).</summary>
    HideObject,
    /// <summary>21FD:0185(low, high) – wait the given number of retraces.</summary>
    Delay,
    /// <summary>21FD:060D – execute the list, flip, execute again (list on both pages).</summary>
    ListOnBothPages,
    /// <summary>21FD:051F – flip the pages.</summary>
    Flip,
    /// <summary>21FD:0245 – pause check (P).</summary>
    Pause,
    /// <summary>21FD:0665 – last frame: flip, restore, list, objects.</summary>
    LastFrame,
    /// <summary>21FD:037D(rgb12) – fade out to a colour.</summary>
    FadeOut,
    /// <summary>21FD:026C(offset, DS) – load the base palette.</summary>
    BasePalette,
    /// <summary>21FD:0329 – fade in.</summary>
    FadeIn,
    /// <summary>21FD:03EB(rgb12, steps) – one palette step towards a colour.</summary>
    PaletteStep,
    /// <summary>0B67:01D4 – draw on the shown page.</summary>
    DrawOnShownPage,
    /// <summary>0B67:0206 – draw on the hidden page.</summary>
    DrawOnHiddenPage,
    /// <summary>2295:0001(low, high, width, text, segment) – a number written into a text.</summary>
    NumberText,
}

/// <summary>One output call with its word arguments (first argument first). <see cref="StationEvent.DataSegment"/> stands for DS.</summary>
public sealed record StationEvent(StationCall Call, short[] Args)
{
    /// <summary>Placeholder for the data segment pushed as the segment of a far pointer.</summary>
    public const short DataSegment = unchecked((short)0xD5D5);

    public override string ToString() =>
        $"{Call}({string.Join(", ", Args.Select(a => a == DataSegment ? "DS" : a.ToString()))})";
}
