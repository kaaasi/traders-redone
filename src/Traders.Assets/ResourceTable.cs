namespace Traders.Assets;

/// <summary>
/// Picture resources in the order of the original resource list at DS:09CE in T.EXE
/// (10 entries of 19 bytes, each with far pointers to the VGA and EGA file names).
/// Verified against T.EXE SHA-256 b73a2170...af0185.
/// </summary>
public enum PictureId
{
    Anbau = 0,
    Rakete = 1,
    Berge = 2,
    FatMike = 3,
    AHandel = 4,
    BHandel = 5,
    Menu = 6,
    Ende = 7,
    Kampf = 8,
    Abend = 9,
}

public static class ResourceTable
{
    public static readonly IReadOnlyList<(PictureId Id, string Vga, string Ega)> Pictures =
    [
        (PictureId.Anbau, "V-ANBAU.PP", "E-ANBAU.PP"),
        (PictureId.Rakete, "V-RAKETE.PP", "E-RAKETE.PP"),
        (PictureId.Berge, "V-BERGE.PP", "E-BERGE.PP"),
        (PictureId.FatMike, "V-FATMIK.PP", "E-FATMIK.PP"),
        (PictureId.AHandel, "V-AHANDE.PP", "E-AHANDE.PP"),
        (PictureId.BHandel, "V-BHANDE.PP", "E-BHANDE.PP"),
        (PictureId.Menu, "V-MENU.PP", "E-MENU.PP"),
        (PictureId.Ende, "V-ENDE.PP", "E-ENDE.PP"),
        (PictureId.Kampf, "V-KAMPF.PP", "E-KAMPF.PP"),
        (PictureId.Abend, "V-ABEND.PP", "E-ABEND.PP"),
    ];

    public static readonly IReadOnlyList<string> Music = ["TRADINTR.CMF", "TRADMENU.CMF", "TRAD_END.CMF"];

    public const string Fonts = "FONTS.PP";
    public const string Masks = "MASKS.PP";
    public const string EnglishText = "ENGLISCH.TXT";
    public const string Executable = "T.EXE";
    public const string ExecutableSha256 = "b73a2170922b2c797b3478e5b010bf890b3b9360452bce28968752b989af0185";
}
