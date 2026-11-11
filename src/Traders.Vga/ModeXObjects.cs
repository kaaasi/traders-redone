namespace Traders.Vga;

/// <summary>
/// A far pointer into a memory block: <see cref="Base"/> is the linear start of the segment inside <see cref="Data"/>,
/// <see cref="Offset"/> the 16-bit offset. Indexing adds to the offset with 16-bit wrap-around, as the driver does.
/// </summary>
public readonly record struct FarBuffer(byte[] Data, int Base, ushort Offset)
{
    public byte this[ushort delta]
    {
        get => Data[Base + (ushort)(Offset + delta)];
        set => Data[Base + (ushort)(Offset + delta)] = value;
    }
}

/// <summary>Restore record of one moving object for one page (4 bytes in the original: page offset, x&amp;3, valid).</summary>
public struct RestoreRecord
{
    public ushort PageOffset;
    public byte Plane;
    public byte Valid;
}

/// <summary>
/// One of the eight moving objects at DS:330C (0x1A bytes each, set up by 21AF): active flag (+0), height (+2),
/// frame images (+4), frame masks (+8), save buffer per page (+0C), restore record per page (+10), x (+14),
/// y (+16), frame (+18). Width is always 16 pixels. Images and save buffers use the RAM atlas layout (rows of
/// 320 bytes, split by plane); masks use the mask atlas layout (rows of 40 bytes, one big-endian word per row).
/// </summary>
public sealed class MovingObject
{
    public byte Active;
    public ushort Height;
    public FarBuffer[] Images = [];
    public FarBuffer[] Masks = [];
    public FarBuffer[] SaveBuffers = [];
    public RestoreRecord[] Restore = [];
    public ushort X, Y, Frame;
}

/// <summary>Moving objects of the VGA driver part (vectors 24 and 28 with the loops 23CD:06E9 and 23CD:071C).</summary>
public sealed partial class ModeXDriver
{
    /// <summary>[ORIGINAL] 23CD:06E9: save the background of and draw every active object 0…7 on page <paramref name="page"/>.</summary>
    public void DrawObjects(ushort page, IReadOnlyList<MovingObject> objects)
    {
        for (int i = 0; i < 8; i++)
            if (objects[i].Active != 0) DrawObject(page, objects[i]);
        ResetGraphicsController();
    }

    /// <summary>[ORIGINAL] 23CD:071C: write back the saved background of objects 7…0 (active or not) on page <paramref name="page"/>.</summary>
    public void RestoreObjects(ushort page, IReadOnlyList<MovingObject> objects)
    {
        for (int i = 7; i >= 0; i--) RestoreObject(page, objects[i]);
        ResetGraphicsController();
    }

    /// <summary>
    /// [ORIGINAL] 23CD:3AAB (vector 24): store the restore record, copy the 16-pixel-wide background (4 bytes per plane
    /// and row) into the page's save buffer, then copy the image where the mask bit is set.
    /// </summary>
    public void DrawObject(ushort page, MovingObject obj)
    {
        ushort height = obj.Height;
        ushort di = PageAddr(obj.X, obj.Y);
        byte firstPlane = (byte)(obj.X & 3);
        obj.Restore[page] = new RestoreRecord { PageOffset = di, Plane = firstPlane, Valid = 1 };

        // background: read plane starts at x&3, save plane at 0
        var save = obj.SaveBuffers[page];
        ushort saveOff = 0, pageOff = di;
        byte savePlane = 0, readPlane = firstPlane;
        for (int n = 0; n < 4; n++)
        {
            ushort d = (ushort)(saveOff + savePlane * 80), s = pageOff;
            Vram.ReadMap = readPlane;
            ushort rows = height;
            do
            {
                for (int i = 0; i < 4; i++) save[d++] = Vram.Read(Page(s++));
                s = (ushort)(s + 76);
                d = (ushort)(d + 316);
            } while (--rows != 0);
            savePlane = (byte)((savePlane + 1) & 3);
            if (savePlane == 0) saveOff++;
            readPlane = (byte)((readPlane + 1) & 3);
            if (readPlane == 0) pageOff++;
        }

        // image through the mask: destination plane starts at x&3, source plane at 0; mask word shifted by the pass
        var image = obj.Images[obj.Frame];
        var mask = obj.Masks[obj.Frame];
        ushort imgOff = 0;
        byte srcPlane = 0, dstPlane = firstPlane;
        for (int n = 0; n < 4; n++)
        {
            ushort s = (ushort)(imgOff + srcPlane * 80), d = di, b = 0;
            Vram.MapMask = (byte)(1 << dstPlane);
            ushort rows = height;
            do
            {
                ushort ax = (ushort)((mask[b] << 8 | mask[(ushort)(b + 1)]) << n);
                if ((ax & 0x8000) != 0) Vram.Write(Page(d), image[s]);
                s++; d++;
                if ((ax & 0x0800) != 0) Vram.Write(Page(d), image[s]);
                s++; d++;
                if ((ax & 0x0080) != 0) Vram.Write(Page(d), image[s]);
                s++; d++;
                if ((ax & 0x0008) != 0) Vram.Write(Page(d), image[s]);
                s = (ushort)(s + 0x13D);
                d = (ushort)(d + 0x4D);
                b = (ushort)(b + 40);
            } while (--rows != 0);
            dstPlane = (byte)((dstPlane + 1) & 3);
            if (dstPlane == 0) di++;
            srcPlane = (byte)((srcPlane + 1) & 3);
            if (srcPlane == 0) imgOff++;
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>
    /// [ORIGINAL] 23CD:3C42 (vector 28): clear the restore record and, unless plane and valid flag were both 0, copy the
    /// save buffer back to the current draw page at the recorded offset.
    /// </summary>
    public void RestoreObject(ushort page, MovingObject obj)
    {
        ushort height = obj.Height;
        var rec = obj.Restore[page];
        ushort di = rec.PageOffset;
        byte dstPlane = rec.Plane;
        bool skip = rec.Plane == 0 && rec.Valid == 0;   // the word at +2 is tested, not only the flag
        obj.Restore[page] = rec with { Plane = 0, Valid = 0 };
        if (skip) return;

        var save = obj.SaveBuffers[page];
        ushort saveOff = 0;
        byte srcPlane = 0;
        for (int n = 0; n < 4; n++)
        {
            ushort s = (ushort)(saveOff + srcPlane * 80), d = di;
            Vram.MapMask = (byte)(1 << dstPlane);
            ushort rows = height;
            do
            {
                for (int i = 0; i < 4; i++) Vram.Write(Page(d++), save[s++]);
                s = (ushort)(s + 0x13C);
                d = (ushort)(d + 0x4C);
            } while (--rows != 0);
            dstPlane = (byte)((dstPlane + 1) & 3);
            if (dstPlane == 0) di++;
            srcPlane = (byte)((srcPlane + 1) & 3);
            if (srcPlane == 0) saveOff++;
        }
    }
}
