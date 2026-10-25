using System.Security.Cryptography;
using Traders.Assets;

return args switch
{
    ["export", var src, var dst] => Export(src, dst),
    ["decrunch", var src, var dst] => Decrunch(src, dst),
    _ => Usage(),
};

static int Usage()
{
    Console.Error.WriteLine("traders-tools export <original-dos-dir> <out-dir>");
    Console.Error.WriteLine("traders-tools decrunch <file.pp> <out.bin>");
    return 1;
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

    var text = TextTable.Load(Path.Combine(srcDir, ResourceTable.EnglishText));
    using (var w = new StreamWriter(Path.Combine(dstDir, "ENGLISCH.lines.txt")))
        for (int i = 0; i < text.Count; i++) w.WriteLine($"{i,4}: {text[i]}");
    Console.WriteLine($"ENGLISCH.TXT: {text.Count} lines");
    return 0;
}
