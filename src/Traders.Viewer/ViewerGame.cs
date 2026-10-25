using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Traders.Assets;

namespace Traders.Viewer;

/// <summary>
/// Asset viewer for the original resources. Keys: Left/Right = previous/next page,
/// +/- = zoom, L = toggle line at row 200 (end of visible screen), Esc = quit.
/// Mouse position, palette index and RGB are shown in the window title.
/// With a screenshot directory, every page is rendered once through the GPU pipeline,
/// saved as PNG and the program exits (used by CI under a virtual display).
/// </summary>
public sealed class ViewerGame : Game
{
    private sealed record Page(string Name, int Width, int Height, byte[] Pixels, Palette Palette,
        Func<int, int, string> Describe, bool IsAtlas);

    private readonly GraphicsDeviceManager _graphics;
    private readonly List<Page> _pages = [];
    private SpriteBatch _batch = null!;
    private Texture2D _texture = null!;
    private Texture2D _white = null!;
    private int _page;
    private int _zoom = 3;
    private bool _showLine = true;
    private KeyboardState _prevKeys;
    private readonly string? _screenshotDir;

    /// <summary>Number of screenshots written in screenshot mode (-1 = failed).</summary>
    public int ScreenshotsWritten { get; private set; }

    public ViewerGame(AssetLibrary assets, string? screenshotDir = null)
    {
        _screenshotDir = screenshotDir;
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;

        foreach (var (id, file, _) in ResourceTable.Pictures)
        {
            var img = assets.Pictures[id];
            _pages.Add(new Page($"{file} ({id})", VgaImage.Width, VgaImage.Height, img.Pixels, img.Palette,
                (x, y) =>
                {
                    int i = img[x, y];
                    var (r, g, b) = img.Palette.Rgb(i);
                    return $"x={x} y={y} index={i} rgb=({r},{g},{b}){(y >= VgaImage.VisibleHeight ? " [off-screen]" : "")}";
                }, true));
        }

        var mono = Palette.Monochrome();
        var mask = SheetRenderer.Mask(assets.Masks);
        _pages.Add(new Page("MASKS.PP", mask.Width, mask.Height, mask.Pixels, mono,
            (x, y) => $"x={x} y={y} mask={(assets.Masks[x, y] ? 1 : 0)}", false));

        AddFont("FONTS.PP large 16x12", assets.Fonts.Large);
        AddFont("FONTS.PP small 16x5", assets.Fonts.Small);
    }

    private void AddFont(string name, BitmapFont font)
    {
        var sheet = SheetRenderer.Font(font);
        _pages.Add(new Page(name, sheet.Width, sheet.Height, sheet.Pixels, Palette.Monochrome(), (x, y) =>
        {
            int g = SheetRenderer.GlyphAt(font, x, y);
            if (g < 0) return "";
            int code = BitmapFont.FirstChar + g;
            return $"glyph {g} code {code} '{(char)code}'";
        }, false));
    }

    protected override void LoadContent()
    {
        _batch = new SpriteBatch(GraphicsDevice);
        _white = new Texture2D(GraphicsDevice, 1, 1);
        _white.SetData(new[] { Color.White });
        ShowPage(0);
    }

    private void ShowPage(int index, bool resizeWindow = true)
    {
        _page = (index % _pages.Count + _pages.Count) % _pages.Count;
        var p = _pages[_page];
        _texture?.Dispose();
        _texture = new Texture2D(GraphicsDevice, p.Width, p.Height);
        var colors = new Color[p.Width * p.Height];
        for (int i = 0; i < colors.Length; i++)
        {
            var (r, g, b) = p.Palette.Rgb(p.Pixels[i]);
            colors[i] = new Color(r, g, b);
        }
        _texture.SetData(colors);
        if (!resizeWindow) return;
        _graphics.PreferredBackBufferWidth = p.Width * _zoom;
        _graphics.PreferredBackBufferHeight = p.Height * _zoom;
        _graphics.ApplyChanges();
    }

    private bool Pressed(KeyboardState k, Keys key) => k.IsKeyDown(key) && !_prevKeys.IsKeyDown(key);

    protected override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();
        if (k.IsKeyDown(Keys.Escape)) Exit();
        if (Pressed(k, Keys.Right) || Pressed(k, Keys.PageDown)) ShowPage(_page + 1);
        if (Pressed(k, Keys.Left) || Pressed(k, Keys.PageUp)) ShowPage(_page - 1);
        if (Pressed(k, Keys.OemPlus) || Pressed(k, Keys.Add)) { _zoom = Math.Min(8, _zoom + 1); ShowPage(_page); }
        if (Pressed(k, Keys.OemMinus) || Pressed(k, Keys.Subtract)) { _zoom = Math.Max(1, _zoom - 1); ShowPage(_page); }
        if (Pressed(k, Keys.L)) _showLine = !_showLine;
        _prevKeys = k;

        var p = _pages[_page];
        var m = Mouse.GetState();
        int px = m.X / _zoom, py = m.Y / _zoom;
        string info = px >= 0 && py >= 0 && px < p.Width && py < p.Height ? p.Describe(px, py) : "";
        Window.Title = $"Traders asset viewer – [{_page + 1}/{_pages.Count}] {p.Name} – zoom {_zoom}x – {info}";
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (_screenshotDir is not null)
        {
            SaveScreenshots(_screenshotDir);
            Exit();
            return;
        }
        GraphicsDevice.Clear(Color.Black);
        DrawPage(_pages[_page], _zoom);
        base.Draw(gameTime);
    }

    private void DrawPage(Page p, int zoom)
    {
        _batch.Begin(samplerState: SamplerState.PointClamp);
        _batch.Draw(_texture, new Rectangle(0, 0, p.Width * zoom, p.Height * zoom), Color.White);
        if (_showLine && p.IsAtlas)
            _batch.Draw(_white, new Rectangle(0, VgaImage.VisibleHeight * zoom, p.Width * zoom, 1), Color.Magenta);
        _batch.End();
    }

    private void SaveScreenshots(string dir)
    {
        try
        {
            Directory.CreateDirectory(dir);
            const int zoom = 2;
            for (int i = 0; i < _pages.Count; i++)
            {
                ShowPage(i, resizeWindow: false);
                var p = _pages[i];
                using var target = new RenderTarget2D(GraphicsDevice, p.Width * zoom, p.Height * zoom);
                GraphicsDevice.SetRenderTarget(target);
                GraphicsDevice.Clear(Color.Black);
                DrawPage(p, zoom);
                GraphicsDevice.SetRenderTarget(null);
                var name = new string(p.Name.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
                using var fs = File.Create(Path.Combine(dir, $"{i:00}-{name}.png"));
                target.SaveAsPng(fs, target.Width, target.Height);
                ScreenshotsWritten++;
            }
            Console.WriteLine($"{ScreenshotsWritten} screenshots written to {dir}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            ScreenshotsWritten = -1;
        }
    }
}
