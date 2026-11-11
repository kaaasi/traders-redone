using Traders.Assets;
using Traders.RefEmu;
using Traders.Tests.RefEmu;
using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Runs the original 23CD VGA driver in the reference emulator next to <see cref="ModeXDriver"/>.
/// Both sides get identical VRAM, atlas, mask and font contents; every operation is applied to
/// both and the complete VRAM (and atlas) can be compared afterwards.
/// </summary>
internal sealed class DriverHarness
{
    // Emulator placement (all free memory above the loaded image, which ends at 3E6E:0000).
    public const ushort AtlasSeg = 0x5000, MaskSeg = 0x7000, FontTableSeg = 0x7200, GlyphSeg = 0x7300, ScratchSeg = 0x7400;
    private const ushort Driver = 0x23CD;

    public OriginalProgram Original { get; }
    public ModeXDriver Port { get; } = new();
    public Random Rng { get; }

    public DriverHarness(int seed, bool realFonts = false)
    {
        Rng = new Random(seed);
        Original = OriginalProgram.LoadOnly(TestGameData.DosDir);
        Original.Call(Driver, 0x04F6, 1); // select VGA driver: mode 13h -> Mode X, vectors to CS:0004

        // atlas, masks: random contents
        var atlas = new byte[PlanarAtlas.Length];
        Rng.NextBytes(atlas);
        Port.Atlas = new PlanarAtlas((byte[])atlas.Clone());
        Original.Machine.Mem.WriteBlock((uint)AtlasSeg << 4, atlas);
        var masks = new byte[8000];
        Rng.NextBytes(masks);
        Port.Masks = (byte[])masks.Clone();
        Original.Machine.Mem.WriteBlock((uint)MaskSeg << 4, masks);

        // fonts: FONTS.PP as the game loads it (large 16x12 from 0, small 16x5 from 1440)
        var fontData = PowerPacker.Decrunch(File.ReadAllBytes(RefEmuPaths.Dos("FONTS.PP")));
        Original.Machine.Mem.WriteBlock((uint)GlyphSeg << 4, fontData);
        var fonts = new[]
        {
            new FontDescriptor(fontData, 0, 12, 13, 32),
            new FontDescriptor(fontData, 1440, 5, 6, 32),
        };
        Port.Fonts = fonts;
        var table = new byte[fonts.Length * 10];
        for (int i = 0; i < fonts.Length; i++)
        {
            var f = fonts[i];
            Put(table, i * 10 + 0, (ushort)f.Offset);
            Put(table, i * 10 + 2, GlyphSeg);
            Put(table, i * 10 + 4, f.Height);
            Put(table, i * 10 + 6, f.Advance);
            Put(table, i * 10 + 8, f.FirstChar);
        }
        Original.Machine.Mem.WriteBlock((uint)FontTableSeg << 4, table);
        Original.Call(Driver, 0x058A, 0, FontTableSeg, 0, AtlasSeg, 0, 0, 0, MaskSeg);

        // VRAM: random contents on both sides
        for (int p = 0; p < 4; p++)
        {
            Rng.NextBytes(Port.Vram.Planes[p]);
            Port.Vram.Planes[p].CopyTo(Original.Machine.Vga.Planes[p], 0);
        }
        SetPage(0xA000);
        SetColor(0x0F);
    }

    private static void Put(byte[] b, int at, ushort v)
    {
        b[at] = (byte)v;
        b[at + 1] = (byte)(v >> 8);
    }

    public (ushort Ax, ushort Dx) CallOriginal(ushort offset, params ushort[] args) => Original.Call(Driver, offset, args);

    public void SetPage(ushort segment)
    {
        CallOriginal(0x0570, 0, segment);           // jmp far [cs:44] -> 311A
        Port.SetDrawPage(0, segment);
    }

    public void SetColor(ushort color)
    {
        CallOriginal(0x05C3, color);
        Port.Color = color;
    }

    public void SetLinePattern(ushort pattern)
    {
        CallOriginal(0x05D0, pattern);
        Port.LinePattern = pattern;
    }

    public void SetFont(ushort font)
    {
        CallOriginal(0x05B6, font);
        Port.Font = font;
    }

    /// <summary>Word of the driver's code-segment state (CS:offset of 23CD in the emulator).</summary>
    public ushort DriverWord(ushort offset) => Original.Machine.Mem.Read16(Original.Segment(Driver), offset);

    /// <summary>Map mask (sequencer 2, low four bits) and read map (graphics controller 4) as left by the last call.</summary>
    public void AssertRegistersEqual(string what)
    {
        var vga = Original.Machine.Vga;
        Assert.True((vga.Seq[2] & 0x0F) == (Port.Vram.MapMask & 0x0F),
            $"{what}: map mask original {vga.Seq[2]:X2} port {Port.Vram.MapMask:X2}");
        Assert.True((vga.Gc[4] & 0x03) == (Port.Vram.ReadMap & 0x03),
            $"{what}: read map original {vga.Gc[4]:X2} port {Port.Vram.ReadMap:X2}");
    }

    public void AssertVramEqual(string what)
    {
        AssertRegistersEqual(what);
        for (int p = 0; p < 4; p++)
        {
            var a = Original.Machine.Vga.Planes[p];
            var b = Port.Vram.Planes[p];
            int i = FirstDifference(a, b);
            if (i >= 0)
                Assert.Fail($"{what}: plane {p} offset {i:X4} original {a[i]:X2} port {b[i]:X2}");
        }
    }

    public void AssertAtlasEqual(string what)
    {
        var a = Original.Machine.Mem.ReadBlock(AtlasSeg, 0, 0x10000)
            .Concat(Original.Machine.Mem.ReadBlock(AtlasSeg + 0x1000, 0, PlanarAtlas.Length - 0x10000)).ToArray();
        int i = FirstDifference(a, Port.Atlas.Data);
        if (i >= 0)
            Assert.Fail($"{what}: atlas byte {i:X5} original {a[i]:X2} port {Port.Atlas.Data[i]:X2}");
    }

    /// <summary>
    /// Memory behind the A000h window (B0000h–BFFFFh) equal on both sides: in the emulator plain RAM, in the port
    /// <see cref="ModeXMemory.ReadOutside"/> (offsets relative to A0000h).
    /// </summary>
    public void AssertOutsideWindowEqual(string what)
    {
        var mem = Original.Machine.Mem;
        for (int k = ModeXMemory.PlaneSize; k < 2 * ModeXMemory.PlaneSize; k++)
        {
            byte a = mem.Read8((uint)(0xA0000 + k)), b = Port.Vram.ReadOutside(k);
            if (a != b) Assert.Fail($"{what}: memory {0xA0000 + k:X5} behind the window original {a:X2} port {b:X2}");
        }
    }

    /// <summary>
    /// Gives the port the whole 64 KB mask segment of the emulator (MASKS.PP followed by the font table, glyphs and
    /// the other buffers placed there), so that mask runs past the 8000 bytes of MASKS.PP read the same bytes.
    /// </summary>
    public void SyncMaskSegment() => Port.Masks = Original.Machine.Mem.ReadBlock(MaskSeg, 0, 0x10000);

    private static int FirstDifference(byte[] a, byte[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        for (int i = 0; i < n; i++)
            if (a[i] != b[i]) return i;
        return a.Length == b.Length ? -1 : n;
    }

    public ushort Next(int minInclusive, int maxInclusive) => (ushort)Rng.Next(minInclusive, maxInclusive + 1);

    // ------------------------------------------------------------------ moving objects (DS:330C, 8 × 1Ah)

    private const ushort ObjectTable = 0x330C, ObjectSize = 0x1A, PointerSeg = 0x7600, SaveSeg = 0x7800;
    private const int Frames = 4, Pages = 2;

    private static ushort SaveSegment(int obj, int page) => (ushort)(SaveSeg + (obj * Pages + page) * 0x200);

    /// <summary>
    /// Eight objects on both sides: pointer tables at 7600h (per object 40h bytes: images, masks, save buffers,
    /// restore records), images in the RAM atlas, masks in the mask atlas, save buffers of 8 KB from 7800h, all with
    /// random contents; restore records start with random plane and valid flag.
    /// </summary>
    public MovingObject[] SetUpObjects()
    {
        var mem = Original.Machine.Mem;
        var objects = new MovingObject[8];
        for (int i = 0; i < 8; i++)
        {
            var o = new MovingObject
            {
                Height = Next(1, 24),
                Images = new FarBuffer[Frames],
                Masks = new FarBuffer[Frames],
                SaveBuffers = new FarBuffer[Pages],
                Restore = new RestoreRecord[Pages],
            };
            ushort table = (ushort)(i * 0x40);
            for (int f = 0; f < Frames; f++)
            {
                ushort ax = Next(0, 300), ay = Next(0, 256 - o.Height);
                ushort para = (ushort)(ay * 20), off = (ushort)((ax >> 2) + PlanarAtlas.HeaderSize);
                o.Images[f] = new FarBuffer(Port.Atlas.Data, para * 16, off);
                mem.Write16(PointerSeg, (ushort)(table + f * 4), off);
                mem.Write16(PointerSeg, (ushort)(table + f * 4 + 2), (ushort)(AtlasSeg + para));
                ushort moff = (ushort)(Next(0, 200 - o.Height) * 40 + Next(0, 38));
                o.Masks[f] = new FarBuffer(Port.Masks, 0, moff);
                mem.Write16(PointerSeg, (ushort)(table + 0x10 + f * 4), moff);
                mem.Write16(PointerSeg, (ushort)(table + 0x10 + f * 4 + 2), MaskSeg);
            }
            for (int p = 0; p < Pages; p++)
            {
                var buffer = new byte[0x2000];
                Rng.NextBytes(buffer);
                mem.WriteBlock((uint)SaveSegment(i, p) << 4, buffer);
                o.SaveBuffers[p] = new FarBuffer(buffer, 0, 0);
                mem.Write16(PointerSeg, (ushort)(table + 0x20 + p * 4), 0);
                mem.Write16(PointerSeg, (ushort)(table + 0x20 + p * 4 + 2), SaveSegment(i, p));
                o.Restore[p] = new RestoreRecord { PageOffset = Next(0, 0x7FFF), Plane = (byte)Next(0, 3), Valid = (byte)Next(0, 1) };
                if (Rng.Next(3) == 0) o.Restore[p].Plane = 0;
                ushort rec = (ushort)(table + 0x30 + p * 4);
                mem.Write16(PointerSeg, rec, o.Restore[p].PageOffset);
                mem.Write8(PointerSeg, (ushort)(rec + 2), o.Restore[p].Plane);
                mem.Write8(PointerSeg, (ushort)(rec + 3), o.Restore[p].Valid);
            }
            ushort at = (ushort)(ObjectTable + i * ObjectSize);
            Original.WriteWord((ushort)(at + 0x02), o.Height);
            void Far(int field, ushort off) { Original.WriteWord((ushort)(at + field), off); Original.WriteWord((ushort)(at + field + 2), PointerSeg); }
            Far(0x04, table);
            Far(0x08, (ushort)(table + 0x10));
            Far(0x0C, (ushort)(table + 0x20));
            Far(0x10, (ushort)(table + 0x30));
            objects[i] = o;
        }
        return objects;
    }

    /// <summary>New random active flag, position and frame for every object (written to DS:330C and the port).</summary>
    public void MoveObjects(MovingObject[] objects, int round)
    {
        for (int i = 0; i < 8; i++)
        {
            var o = objects[i];
            o.Active = (byte)(Rng.Next(4) == 0 ? 0 : Next(1, 255));
            o.X = Next(0, round % 4 == 0 ? 319 : 304);
            o.Y = Next(0, 200 - o.Height);
            o.Frame = Next(0, Frames - 1);
            ushort at = (ushort)(ObjectTable + i * ObjectSize);
            Original.WriteByte(at, o.Active);
            Original.WriteWord((ushort)(at + 0x14), o.X);
            Original.WriteWord((ushort)(at + 0x16), o.Y);
            Original.WriteWord((ushort)(at + 0x18), o.Frame);
        }
    }

    /// <summary>VRAM, registers, save buffers and restore records equal on both sides.</summary>
    public void AssertObjectsEqual(MovingObject[] objects, string what)
    {
        AssertVramEqual(what);
        var mem = Original.Machine.Mem;
        for (int i = 0; i < 8; i++)
            for (int p = 0; p < Pages; p++)
            {
                var a = mem.ReadBlock(SaveSegment(i, p), 0, 0x2000);
                int k = FirstDifference(a, objects[i].SaveBuffers[p].Data);
                if (k >= 0) Assert.Fail($"{what}: object {i} page {p} save buffer byte {k:X4} original {a[k]:X2} port {objects[i].SaveBuffers[p].Data[k]:X2}");
                ushort rec = (ushort)(i * 0x40 + 0x30 + p * 4);
                var r = objects[i].Restore[p];
                Assert.True(mem.Read16(PointerSeg, rec) == r.PageOffset && mem.Read8(PointerSeg, (ushort)(rec + 2)) == r.Plane
                    && mem.Read8(PointerSeg, (ushort)(rec + 3)) == r.Valid, $"{what}: object {i} page {p} restore record");
            }
    }

    /// <summary>Second page at A400h (offset 4000h) or first page.</summary>
    public void RandomPage() => SetPage(Rng.Next(2) == 0 ? (ushort)0xA000 : (ushort)0xA400);
}
