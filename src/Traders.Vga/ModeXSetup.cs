namespace Traders.Vga;

/// <summary>Card selection and set-up of the VGA driver part.</summary>
public sealed partial class ModeXDriver
{
    /// <summary>
    /// [ORIGINAL] 23CD:04F6: select the driver part for display mode <paramref name="mode"/> (0 EGA, 1 VGA, 2 and 63h
    /// Tandy variants; any other value does nothing). Only VGA is ported (decision "VGA only", BESTAND.md §3.3).
    /// </summary>
    public void SelectCard(ushort mode)
    {
        switch (mode)
        {
            case 1:
                InitializeVga();
                break;
            case 0 or 2 or 0x63:
                throw new NotSupportedException($"Display mode {mode}: only the VGA part (mode 1) of 23CD is ported.");
        }
    }

    /// <summary>
    /// [ORIGINAL] 23CD:3042: BIOS mode 13h, then chain-4 off and CRTC byte mode (Mode X), then 30D1. The vector copy
    /// (23CD:053C) has no counterpart: the routines are methods of this class.
    /// </summary>
    public void InitializeVga()
    {
        // [APPROX] Effect of INT 10h mode 13h as modelled by the RefEmu BIOS stub: all four planes cleared, map mask 0Fh,
        // start address 0. A real BIOS clears the memory in chain-4 addressing; the difference is not visible after the
        // game has drawn its first screen.
        foreach (var plane in Vram.Planes) Array.Clear(plane);
        Vram.MapMask = 0x0F;
        DisplayStart = 0;
        ResetGraphicsController();
    }
}
