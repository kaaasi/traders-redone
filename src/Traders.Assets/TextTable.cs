namespace Traders.Assets;

/// <summary>
/// ENGLISCH.TXT: one CRLF-terminated line per entry. The original indexes these lines;
/// the raw bytes are kept (the file contains at least one non-ASCII byte, 0xA7).
/// </summary>
public sealed class TextTable
{
    public IReadOnlyList<byte[]> RawLines { get; }

    private TextTable(List<byte[]> lines) => RawLines = lines;

    public int Count => RawLines.Count;

    /// <summary>Line as text, mapping bytes 1:1 to chars (Latin-1) so no information is lost.</summary>
    public string this[int index] => string.Create(RawLines[index].Length, RawLines[index], static (span, src) =>
    {
        for (int i = 0; i < src.Length; i++) span[i] = (char)src[i];
    });

    public static TextTable Parse(ReadOnlySpan<byte> data)
    {
        var lines = new List<byte[]>();
        int start = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == (byte)'\n')
            {
                int end = i;
                if (end > start && data[end - 1] == (byte)'\r') end--;
                lines.Add(data[start..end].ToArray());
                start = i + 1;
            }
        }
        if (start < data.Length)
            lines.Add(data[start..].ToArray());
        return new TextTable(lines);
    }

    public static TextTable Load(string path) => Parse(File.ReadAllBytes(path));
}
