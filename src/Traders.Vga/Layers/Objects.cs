namespace Traders.Vga.Layers;

/// <summary>
/// Moving objects 21AF: eight records of 1Ah bytes at DS:330C (layout in <see cref="MovingObject"/>), their pointer
/// tables in DS, and the bridge to the driver loops 23CD:06E9/071C.
/// </summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:330C – eight object records of 1Ah bytes.</summary>
    public const int ObjectTable = 0x330C, ObjectSize = 0x1A, ObjectCount = 8;

    /// <summary>DS:34A6 – image pointers, 21 far pointers (54h bytes) per object.</summary>
    public const int ObjectImages = 0x34A6;

    /// <summary>DS:3068 – mask pointers, 21 far pointers per object.</summary>
    public const int ObjectMasks = 0x3068;

    /// <summary>Bytes per object in the image and mask pointer tables (21 frames).</summary>
    public const int FrameTableSize = 0x54;

    /// <summary>DS:3438 – save buffer pointers, two far pointers (one per page) per object.</summary>
    public const int ObjectSaveBuffers = 0x3438;

    /// <summary>DS:45D6 – far pointer to the RAM atlas buffer; DS:4BC6 – far pointer to the mask atlas.</summary>
    public const int AtlasPointer = 0x45D6, MaskPointer = 0x4BC6;

    private int ObjectAt(ushort i) => (ObjectTable + (ushort)(i * ObjectSize)) & 0xFFFF;

    /// <summary>
    /// [ORIGINAL] 21AF:000D: for objects 0…7 the four table pointers (+04 images DS:34A6 + 54h·i, +08 masks DS:3068 +
    /// 54h·i, +0C save buffers DS:3438 + 8·i, +10 restore records DS:33E4 + 8·i, segment DS), restore records
    /// cleared, inactive.
    /// </summary>
    public void InitializeObjects()
    {
        for (ushort i = 0; i < ObjectCount; i++)
        {
            int at = ObjectAt(i);
            SetFarPointer(at + 0x04, (ushort)(ObjectImages + i * FrameTableSize), DataSegment);
            SetFarPointer(at + 0x08, (ushort)(ObjectMasks + i * FrameTableSize), DataSegment);
            SetFarPointer(at + 0x0C, (ushort)(ObjectSaveBuffers + (i << 3)), DataSegment);
            SetFarPointer(at + 0x10, (ushort)(ObjectRestoreRecords + (i << 3)), DataSegment);
            InvalidateRestoreRecords(i);
            Ds[at] = 0;
        }
    }

    private void SetFarPointer(int at, ushort offset, ushort segment)
    {
        Set16(at, offset);
        Set16(at + 2, segment);
    }

    /// <summary>
    /// [ORIGINAL] 21AF:00CA(object, frame, x, y, mx, my): image pointer of the frame = atlas pointer DS:45D6 + offset,
    /// VGA: y·320 + x/4 + 4 as a long added to the far pointer with normalisation (0000:5101, offset 0…15 afterwards);
    /// EGA: y·160 + x/8 + 4, Tandy: y·160 + x/2 + 4, both added to the offset only. Mask pointer = DS:4BC6 +
    /// 2·(my·20 + mx/16). Divisions signed; the table index 54h·object + 4·frame is formed in 16 bits.
    /// </summary>
    public void SetObjectImage(ushort obj, ushort frame, ushort x, ushort y, ushort mx, ushort my)
    {
        ushort mode = U16(CardMode);
        int offset = mode switch
        {
            0 => (short)(ushort)((short)y * 160 + (short)x / 8 + 4),
            1 => (short)y * 320 + (short)x / 4 + 4,
            2 or 0x63 => (short)(ushort)((short)y * 160 + (short)x / 2 + 4),
            _ => throw new NotSupportedException($"Display mode {mode}: 21AF:00CA uses an uninitialised offset."),
        };
        ushort index = (ushort)(obj * FrameTableSize + (frame << 2));
        var (off, seg) = mode == 1 ? HugeAdd(U16(AtlasPointer), U16(AtlasPointer + 2), offset)
            : ((ushort)(U16(AtlasPointer) + offset), U16(AtlasPointer + 2));
        SetFarPointer(ObjectImages + index, off, seg);
        ushort maskOffset = (ushort)(((short)my * 20 + (short)mx / 16) << 1);
        SetFarPointer(ObjectMasks + index, (ushort)(U16(MaskPointer) + maskOffset), U16(MaskPointer + 2));
    }

    /// <summary>
    /// [ORIGINAL] 21AF:022B(object, x1, y1, x2, y2): save buffers for page 0 at (x1, y1) and page 1 at (x2, y2) in the
    /// RAM atlas, addressed like 00CA; on VGA x is first rounded up to a multiple of 4 ((x + 3)/4·4, signed).
    /// </summary>
    public void SetObjectSaveBuffers(ushort obj, ushort x1, ushort y1, ushort x2, ushort y2)
    {
        ushort mode = U16(CardMode);
        int Offset(ushort x, ushort y) => mode switch
        {
            0 => (short)(ushort)((short)y * 160 + (short)x / 8 + 4),
            1 => (short)y * 320 + (short)(ushort)(((short)(ushort)(x + 3) / 4) << 2) / 4 + 4,
            2 or 0x63 => (short)(ushort)((short)y * 160 + (short)x / 2 + 4),
            _ => throw new NotSupportedException($"Display mode {mode}: 21AF:022B uses uninitialised offsets."),
        };
        int first = Offset(x1, y1), second = Offset(x2, y2);
        ushort at = (ushort)(obj << 3);
        foreach (var (slot, offset) in new[] { (0, first), (4, second) })
        {
            var (off, seg) = mode == 1 ? HugeAdd(U16(AtlasPointer), U16(AtlasPointer + 2), offset)
                : ((ushort)(U16(AtlasPointer) + offset), U16(AtlasPointer + 2));
            SetFarPointer(ObjectSaveBuffers + at + slot, off, seg);
        }
    }

    /// <summary>[ORIGINAL] 21AF:0412(object, frame): frame (+18h), restore records cleared, active (+0 = 1).</summary>
    public void ShowObject(ushort obj, ushort frame)
    {
        int at = ObjectAt(obj);
        Set16(at + 0x18, frame);
        InvalidateRestoreRecords(obj);
        Ds[at] = 1;
    }

    /// <summary>[ORIGINAL] 21AF:045E(object): inactive (+0 = 0).</summary>
    public void HideObject(ushort obj) => Ds[ObjectAt(obj)] = 0;

    /// <summary>[ORIGINAL] 21AF:0472: 045E for objects 0…7.</summary>
    public void HideAllObjects()
    {
        for (ushort i = 0; i < ObjectCount; i++) HideObject(i);
    }

    /// <summary>
    /// [ORIGINAL] 0000:5101 (C runtime, huge pointer plus long, non-negative case): offset + low word with carry into
    /// the segment (1000h), the low nibble of the high word · 1000h onto the segment, then normalised to offset 0…15.
    /// </summary>
    private static (ushort Offset, ushort Segment) HugeAdd(ushort offset, ushort segment, int value)
    {
        if (value < 0) throw new NotSupportedException("Negative offsets (0000:5101 subtracts) are not ported.");
        uint sum = (uint)offset + (ushort)value;
        ushort seg = (ushort)(segment + (sum > 0xFFFF ? 0x1000 : 0) + (((value >> 16) & 0x0F) << 12));
        ushort ax = (ushort)sum;
        return ((ushort)(ax & 0x0F), (ushort)(seg + (ax >> 4)));
    }

    // ------------------------------------------------------------------ driver loops on the DS records

    /// <summary>
    /// [ORIGINAL] 23CD:06E9(page) on the records in DS: every active object is drawn by the driver; the restore
    /// records (far pointers +10h) are written back to their place.
    /// </summary>
    public void DrawObjects(ushort page)
    {
        var objects = LoadObjects(page);
        Driver.DrawObjects(page, objects);
        StoreRestoreRecords(page, objects);
    }

    /// <summary>[ORIGINAL] 23CD:071C(page) on the records in DS: backgrounds of objects 7…0 written back.</summary>
    public void RestoreObjects(ushort page)
    {
        var objects = LoadObjects(page);
        Driver.RestoreObjects(page, objects);
        StoreRestoreRecords(page, objects);
    }

    private MovingObject[] LoadObjects(ushort page)
    {
        var objects = new MovingObject[ObjectCount];
        for (ushort i = 0; i < ObjectCount; i++)
        {
            int at = ObjectAt(i);
            ushort frame = U16(at + 0x18);
            var o = new MovingObject
            {
                Active = Ds[at],
                Height = U16(at + 0x02),
                X = U16(at + 0x14),
                Y = U16(at + 0x16),
                Frame = frame,
                Images = new FarBuffer[frame + 1],
                Masks = new FarBuffer[frame + 1],
                SaveBuffers = new FarBuffer[page + 1],
                Restore = new RestoreRecord[page + 1],
            };
            if (o.Active != 0)
            {
                o.Images[frame] = Resolve(TableEntry(at + 0x04, frame));
                o.Masks[frame] = Resolve(TableEntry(at + 0x08, frame));
            }
            var (recOff, recSeg) = RecordAddress(at, page);
            var record = new RestoreRecord
            {
                PageOffset = ReadFar16(recSeg, recOff),
                Plane = ReadFar(recSeg, (ushort)(recOff + 2)),
                Valid = ReadFar(recSeg, (ushort)(recOff + 3)),
            };
            o.Restore[page] = record;
            // the save buffer is used by drawing (active objects) and by restoring (record word +2 ≠ 0) only
            if (o.Active != 0 || record.Plane != 0 || record.Valid != 0) o.SaveBuffers[page] = Resolve(TableEntry(at + 0x0C, page));
            objects[i] = o;
        }
        return objects;
    }

    private void StoreRestoreRecords(ushort page, MovingObject[] objects)
    {
        for (ushort i = 0; i < ObjectCount; i++)
        {
            var (recOff, recSeg) = RecordAddress(ObjectAt(i), page);
            var r = objects[i].Restore[page];
            WriteFar16(recSeg, recOff, r.PageOffset);
            WriteFar(recSeg, (ushort)(recOff + 2), r.Plane);
            WriteFar(recSeg, (ushort)(recOff + 3), r.Valid);
        }
    }

    /// <summary>Restore record of the page: the far pointer at +10h addresses the records themselves (4 bytes per page).</summary>
    private (ushort Offset, ushort Segment) RecordAddress(int at, ushort page) =>
        ((ushort)(U16(at + 0x10) + (page << 2)), U16(at + 0x12));

    /// <summary>Far pointer number <paramref name="index"/> of the table whose far address is stored at DS:<paramref name="field"/>.</summary>
    private (ushort Offset, ushort Segment) TableEntry(int field, ushort index)
    {
        ushort tableOff = U16(field), tableSeg = U16(field + 2);
        ushort at = (ushort)(tableOff + (index << 2));
        return (ReadFar16(tableSeg, at), ReadFar16(tableSeg, (ushort)(at + 2)));
    }

    /// <summary>
    /// A far pointer as a port buffer: inside the RAM atlas (from DS:45D6), the mask atlas (from DS:4BC6) or the data
    /// segment. The buffer index is linear address − buffer start with the 16-bit offset arithmetic of the driver.
    /// </summary>
    private FarBuffer Resolve((ushort Offset, ushort Segment) p)
    {
        int linear = (p.Segment << 4) + p.Offset;
        int atlas = (U16(AtlasPointer + 2) << 4) + U16(AtlasPointer);
        int masks = (U16(MaskPointer + 2) << 4) + U16(MaskPointer);
        if (linear >= atlas && linear < atlas + Driver.Atlas.Data.Length)
            return new FarBuffer(Driver.Atlas.Data, (p.Segment << 4) - atlas, p.Offset);
        if (linear >= masks && linear < masks + Driver.Masks.Length)
            return new FarBuffer(Driver.Masks, (p.Segment << 4) - masks, p.Offset);
        if (p.Segment == DataSegment) return new FarBuffer(Ds, 0, p.Offset);
        throw new NotSupportedException($"Far pointer {p.Segment:X4}:{p.Offset:X4} is neither in the atlas, the masks nor DS.");
    }
}
