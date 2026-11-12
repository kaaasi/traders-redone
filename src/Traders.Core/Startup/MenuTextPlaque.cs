using Traders.Core.Menus;
using Traders.Core.State;

namespace Traders.Core.Startup;

/// <summary>
/// The text plaque 192D:16AF of lane menue for the code request: plaque top DS:5334, then the random brick wall
/// (192D:131B) and the plaque (192D:14FB) with the random draws of the original, through <see cref="MenuBackdrop"/>. The draw
/// calls are kept for the scene (<see cref="Ops"/>); the texts, the fade-out and the rest of the screen are drawn by the
/// output layers.
/// </summary>
// [ORIGINAL] 192D:16AF – top = (200 − h) / 2 → DS:5334; 131B; 14FB(37h, top, D2h, h)
// [APPROX] F1 = B: the fade-out 21FD:03B4(420h) and the texts are not drawn in the port (the request is not shown).
public sealed class MenuTextPlaque : ITextPlaque
{
    /// <summary>Draw calls of the last plaque (wall, then plaque), empty before the first.</summary>
    public IReadOnlyList<BackdropOp> Ops { get; private set; } = [];

    public void Show(Dgroup d, OriginalRandom random, short height, short step, int texts, short extra)
    {
        short top = Int16Math.Div(Int16Math.S(0xC8 - height), 2);
        d.Set(MenuLayout.PlaqueTop, top);
        var ops = MenuBackdrop.Wall(random.Next);
        ops.AddRange(MenuBackdrop.Plaque(random.Next, 0x37, top, 0xD2, height));
        Ops = ops;
    }
}
