using System.IO.Compression;
using System.Security.Cryptography;
using Traders.Assets;
using Traders.Audio.Opl;

return args switch
{
    ["export", var src, var dst] => Export(src, dst),
    ["decrunch", var src, var dst] => Decrunch(src, dst),
    ["opl-wav", var src, var dst] => OplWav(src, dst, 44_100),
    ["opl-wav", var src, var dst, var rate] => OplWav(src, dst, int.Parse(rate, System.Globalization.CultureInfo.InvariantCulture)),
    _ => Usage(),
};

static int Usage()
{
    Console.Error.WriteLine("traders-tools export <original-dos-dir> <out-dir>");
    Console.Error.WriteLine("traders-tools decrunch <file.pp> <out.bin>");
    Console.Error.WriteLine("traders-tools opl-wav <opl-trace.txt[.gz]> <out.wav> [sample-rate]");
    return 1;
}

// Renders an OplTrace text file (RefEmu, OplTrace.WriteText) to a WAV file for listening (lane klang, M14).
static int OplWav(string src, string dst, int rate)
{
    using var file = File.OpenRead(src);
    using Stream input = src.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) ? new GZipStream(file, CompressionMode.Decompress) : file;
    using var reader = new StreamReader(input);
    var writes = OplRenderer.ReadTrace(reader);
    var pcm = OplRenderer.Render(writes, OplClock.Pit, rate);
    using (var output = File.Create(dst)) OplRenderer.WriteWav(output, pcm, rate);
    Console.WriteLine($"{src}: {writes.Count} writes -> {dst}: {pcm.Length} samples, {pcm.Length / (double)rate:F1} s at {rate} Hz");
    return 0;
}

static int Decrunch(string src, string dst)
{
    var data = PowerPacker.Decrunch(File.ReadAllBytes(src));
    File.WriteAllBytes(dst, data);
    Console.WriteLine($"{src}: {data.Length} bytes");
    return 0;
}

static int Export(string srcDir, string dstDir)
{
    Directory.CreateDirectory(dstDir);
    var exe = Path.Combine(srcDir, ResourceTable.Executable);
    var hash = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(exe)));
    Console.WriteLine($"T.EXE sha256 {hash} {(hash == ResourceTable.ExecutableSha256 ? "OK" : "UNEXPECTED VERSION")}");

    foreach (var file in Directory.GetFiles(srcDir, "*.PP").Order(StringComparer.Ordinal))
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var data = PowerPacker.Decrunch(File.ReadAllBytes(file));
        File.WriteAllBytes(Path.Combine(dstDir, name + ".bin"), data);
        if (name.StartsWith("V-", StringComparison.Ordinal))
        {
            var img = VgaImage.FromDecrunched(data);
            PngWriter.WriteIndexed(Path.Combine(dstDir, name + ".png"), VgaImage.Width, VgaImage.Height, img.Pixels, img.Palette);
        }
        Console.WriteLine($"{name}: {data.Length} bytes");
    }

    var mono = Palette.Monochrome();
    var fonts = FontSet.Load(Path.Combine(srcDir, ResourceTable.Fonts));
    foreach (var (name, font) in new[] { ("FONTS-large", fonts.Large), ("FONTS-small", fonts.Small) })
    {
        var sheet = SheetRenderer.Font(font);
        PngWriter.WriteIndexed(Path.Combine(dstDir, name + ".png"), sheet.Width, sheet.Height, sheet.Pixels, mono);
    }
    var masks = SheetRenderer.Mask(MaskImage.Load(Path.Combine(srcDir, ResourceTable.Masks)));
    PngWriter.WriteIndexed(Path.Combine(dstDir, "MASKS.png"), masks.Width, masks.Height, masks.Pixels, mono);

    foreach (var m in ResourceTable.Music)
    {
        var music = MusicModule.Load(Path.Combine(srcDir, m));
        Console.WriteLine($"{m}: {music.Orders.Count} orders, {music.PatternCount} patterns");
    }

    var text = TextTable.Load(Path.Combine(srcDir, ResourceTable.EnglishText));
    using (var w = new StreamWriter(Path.Combine(dstDir, "ENGLISCH.lines.txt")))
        for (int i = 0; i < text.Count; i++) w.WriteLine($"{i,4}: {text[i]}");
    Console.WriteLine($"ENGLISCH.TXT: {text.Count} lines");
    return 0;
}
