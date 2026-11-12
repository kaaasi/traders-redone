using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Core.Startup;

/// <summary>
/// Program step 2, the program start 0DFD:000D, reduced to the state the core keeps (finding
/// docs/routines/0DFD_000D_programmstart.md): the picture table of the resource loader for the six preloaded pictures
/// (0DFD:0239) and the code request 21FD:0687 with the new seed; the text plaque of the request (random draws) comes from
/// lane menue (<see cref="ITextPlaque"/>). Fonts, masks, texts, page and object tables, palettes,
/// the drawing list, the loading picture and the music are the output layers' and the host's (table "Zustand" of the
/// finding); they write nothing the rules read.
/// </summary>
// [ORIGINAL] 0DFD:000D – 0074 (buffers, pages), 0132 (page flip), 21AF:000D, 0158 (fonts), 21FD:05DB(0), 192D:18B6 (texts),
// 186C:0AB1/01A9 (file at DS:0678), 0239 (loading picture, six pictures 186C:0002), 21FD:0687 (code request)
// [APPROX] The display parts of 0DFD:0074..0239 (heap buffers DS:45D6/4BC6/4884/3CAA, pages DS:082E/0830/4516/40DE, objects
// DS:330C, font records DS:2BF0, colours DS:0142/34A4/3CA8, palettes, drawing list DS:15BA) and the music 218B:0094(0, 0) are
// not run by the core; the text pointers DS:37CC belong to lane menue (192D:18B6).
public sealed class ProgramStart(Func<int, ImageFileLengths> files, ITextPlaque plaque) : IProgramStepHandler
{
    /// <summary>The code shown by the last run (null before).</summary>
    public CodeRequest? Code { get; private set; }

    public StepOutcome Begin(ProgramStepContext context)
    {
        var d = context.State;
        foreach (int word in ImageTable.PreloadOrder) ImageTable.RecordLoad(d, d.S16(word), files); // [ORIGINAL] 0DFD:0239 – loads at offsets 0305..033B
        Code = CodeQuery.Run(d, context.Random, files, plaque);
        return StepOutcome.Finished(0);
    }

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("the program start finishes at once");
}
