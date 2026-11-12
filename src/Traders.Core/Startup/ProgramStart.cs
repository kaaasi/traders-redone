using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Core.Startup;

/// <summary>
/// Program step 2, the program start 0DFD:000D, reduced to the state the core keeps (finding
/// docs/routines/0DFD_000D_programmstart.md): the picture table of the resource loader for the six preloaded pictures
/// (0DFD:0239), the text table 192D:18B6 of lane menue (DS:37CC, from the text file the host loads into its buffer) and the
/// code request 21FD:0687 with the new seed and the text plaque of lane menue (<see cref="ITextPlaque"/>, normally
/// <see cref="MenuTextPlaque"/>). Fonts, masks, page and object tables, palettes,
/// the drawing list, the loading picture and the music are the output layers' and the host's (table "Zustand" of the
/// finding); they write nothing the rules read.
/// </summary>
// [ORIGINAL] 0DFD:000D – 0074 (buffers, pages), 0132 (page flip), 21AF:000D, 0158 (fonts), 21FD:05DB(0), 192D:18B6 (texts),
// 186C:0AB1/01A9 (file at DS:0678), 0239 (loading picture, six pictures 186C:0002), 21FD:0687 (code request)
// [APPROX] The display parts of 0DFD:0074..0239 (heap buffers DS:45D6/4BC6/4884/3CAA, pages DS:082E/0830/4516/40DE, objects
// DS:330C, font records DS:2BF0, colours DS:0142/34A4/3CA8, palettes, drawing list DS:15BA) and the music 218B:0094(0, 0) are
// not run by the core (Traders.Vga keeps them in its own state).
// [APPROX] The far address of the text buffer DS:4884 comes from the host's heap (0DFD:0074) and is an input
// (<see cref="TextFile"/>); 186C:0B0A loads the file there. Behind a file shorter than the scan the buffer is taken as zeros
// (the original scans whatever the heap holds; ENGLISCH.TXT ends the scan at its last line, entry 130h).
public sealed class ProgramStart(Func<int, ImageFileLengths> files, ITextPlaque plaque, TextFile texts) : IProgramStepHandler
{
    /// <summary>The code shown by the last run (null before).</summary>
    public CodeRequest? Code { get; private set; }

    /// <summary>Contents of the text buffer after 192D:18B6 (lines as zero-terminated strings; null before the first run).</summary>
    public byte[]? TextBuffer { get; private set; }

    public StepOutcome Begin(ProgramStepContext context)
    {
        var d = context.State;
        d.Set(MenuLayout.TextBuffer, texts.BufferOffset);
        d.Set(MenuLayout.TextBuffer + 2, texts.BufferSegment);
        var buffer = new byte[Math.Max(texts.Contents.Length, MenuTexts.ScanLimit)];
        texts.Contents.Span.CopyTo(buffer);
        MenuTexts.Build(d, buffer); // [ORIGINAL] 0DFD:000D – call of 192D:18B6
        TextBuffer = buffer;
        foreach (int word in ImageTable.PreloadOrder) ImageTable.RecordLoad(d, d.S16(word), files); // [ORIGINAL] 0DFD:0239 – loads at offsets 0305..033B
        Code = CodeQuery.Run(d, context.Random, files, plaque);
        return StepOutcome.Finished(0);
    }

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("the program start finishes at once");
}

/// <summary>The text file (ENGLISCH.TXT) and the far address of the heap buffer the host loads it to (DS:4884).</summary>
public sealed record TextFile(ReadOnlyMemory<byte> Contents, ushort BufferOffset, ushort BufferSegment);
