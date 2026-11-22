using Traders.Assets;
using Traders.Audio.Opl;

namespace Traders.Audio.Music;

/// <summary>
/// Port of the resident music driver FXDRIVER.EXE V1.06 (finding docs/analyse/FXDRIVER.md): the int 64h functions the
/// game uses, the module loader and the timer tick, down to the OPL2 register writes. The driver's data segment is kept
/// as a byte image with the driver's own offsets, initialised from <see cref="FxDriverImage"/>; all tables (F-numbers,
/// operator and voice tables, default parameters, percussion masks) are read from there, none is copied into the code.
/// Deterministic: no clock, no files of its own (the module bytes come from <c>openFile</c>), output only through
/// <see cref="Written"/>. Offsets in the markers are image offsets of FXDRIVER (CS:x = 2920 + x, DS:x = 4810 + x).
/// </summary>
public sealed partial class FxMusicDriver
{
    /// <summary>Size of one pattern buffer handed over with function 10h (64 rows × 30h bytes + length words).</summary>
    public const int BufferSize = 0x0C04;

    public const int BufferCount = 24;

    private readonly byte[] _ds = new byte[0x10000];
    private readonly byte[][] _buffers = new byte[BufferCount][];
    private readonly Func<string, byte[]?> _openFile;

    /// <summary>OPL2 register writes (register, value) in the order of the original.</summary>
    public event Action<byte, byte>? Written;

    /// <summary>The same writes with the tick they belong to (<see cref="TickCount"/> at the write; common interface of the
    /// sound spurs, <see cref="OplWrite"/>).</summary>
    public event Action<OplWrite>? WrittenAt;

    /// <summary>Calls of <see cref="Tick"/> so far: the tick of writes made by a call is the number of that call
    /// (1, 2, …); writes of int 64h functions carry the number of the last call before them (0 before the first).</summary>
    public long TickCount { get; private set; }

    /// <param name="image">Unpacked driver; its data segment is the initial state.</param>
    /// <param name="openFile">Returns the bytes of a module file by its DOS name (null: open fails).</param>
    public FxMusicDriver(FxDriverImage image, Func<string, byte[]?> openFile)
    {
        _openFile = openFile;
        int n = Math.Min(_ds.Length, image.Length - image.DataBase);
        image.Image.Slice(image.DataBase, n).CopyTo(_ds);
        for (int i = 0; i < BufferCount; i++) _buffers[i] = new byte[BufferSize];
    }

    /// <summary>True while the driver's timer handler is hooked into int 8 (function 01h until stop or end).</summary>
    public bool TimerActive { get; private set; }

    /// <summary>Byte of the driver's data segment (driver offset DS:x), for comparisons with the original.</summary>
    public byte DataByte(int offset) => _ds[offset & 0xFFFF];

    /// <summary>Pattern buffer <paramref name="index"/> as handed over with function 10h.</summary>
    public ReadOnlySpan<byte> Buffer(int index) => _buffers[index];

    // ------------------------------------------------------------------ data segment

    private byte B(int o) => _ds[o & 0xFFFF];
    private void SetB(int o, int v) => _ds[o & 0xFFFF] = (byte)v;
    private ushort W(int o) => (ushort)(_ds[o & 0xFFFF] | _ds[(o + 1) & 0xFFFF] << 8);
    private void SetW(int o, int v)
    {
        _ds[o & 0xFFFF] = (byte)v;
        _ds[(o + 1) & 0xFFFF] = (byte)(v >> 8);
    }

    // Driver variables (DS offsets, FXDRIVER.md §2–§5).
    private const int Port = 0x01D6, Card = 0x132C, Speaker = 0x42, Percussion = 0x1530, VoiceCount = 0x168;
    private const int PatternCount = 0x40, Exponent = 0x48, PercussionFlag = 0x44, SubTick = 0x4A, Running = 0x50;
    private const int Row = 0x6A, OrderPos = 0x6C, Playing = 0x70, Repeat = 0x7C, Channel = 0x86, Orders = 0x8A;
    private const int Pattern = 0x166, FadeDirection = 0x17A, Fade = 0x19E, FadeStep = 0x1A0, ChannelState = 0x1A6;
    private const int Instruments = 0x1EA, InstrumentSize = 0x45, Pointers = 0x1430;
    private const int KeyOn = 0x151, Volume = 0x15A, Note = 0x170, BlockShadow = 0x194, Bend = 0x1D8;
    private const int Rhythm = 0x132E, AmDepth = 0x132A, VibDepth = 0x132F, NoteSelect = 0x150, PitchRange = 0x1330;
    private const int WaveSelect = 0x16E, SlotParams = 0x1332, SlotSize = 14;
    private const int MelodicSlots = 0x0000, PercussionSlots = 0x0012, FNumbers = 0x0028, PercussionMask = 0x1531;
    private const int DefaultModulator = 0x1536, DefaultCarrier = 0x1544, SlotOffset = 0x15A6, Carrier = 0x15B8;
    private const int MelodicVoice = 0x15CA, PercussionVoice = 0x15DC;

    private bool Perc => B(Percussion) != 0;

    private byte P(int slot, int i) => B(SlotParams + slot * SlotSize + i);

    // ------------------------------------------------------------------ install and int 64h

    /// <summary>
    /// What the driver's start-up sets in its data segment before it stays resident (main CS:1D10): int 64h number,
    /// fade off with step 1, card search at 388h, 288h, 318h, 218h, percussion mode.
    /// </summary>
    /// <remarks>[ORIGINAL-FX] 4630 – [APPROX] the card test reads the OPL status port; the port assumes an AdLib at 388h
    /// (DS:0910 = 1, Nutzer 11.11.2026), so the search ends at the first port. Vector bookkeeping (DS:142E, 016A) is not
    /// kept: the host calls <see cref="Tick"/> while <see cref="TimerActive"/>.</remarks>
    public void Install()
    {
        SetW(0x142E, 0x64);
        SetW(0x1A4, 0);
        SetW(0x1A2, 0x0FFF);
        SetW(FadeDirection, 0);
        SetW(Fade, 0);
        SetW(FadeStep, 1);
        SetW(Card, Detect(0x388));
        if (W(Card) == 0) SetW(Speaker, 2);
        SetMode(1);
        SetW(Running, 0);
    }

    /// <summary>
    /// int 64h with AX = function; returns the registers as the driver leaves them. Function 00h takes the module name in
    /// <paramref name="fileName"/> instead of the far pointer BX:CX (the pointer is still stored like the original does
    /// not: the driver only reads the string).
    /// </summary>
    /// <remarks>[ORIGINAL-FX] 427D – dispatcher; functions 00h, 01h, 03h, 05h, 06h, 10h, 12h, 15h, 18h–1Bh as used by the
    /// game (FXDRIVER.md §2); other functions are not ported and return the registers unchanged except AX.</remarks>
    public (ushort Ax, ushort Bx, ushort Cx, ushort Dx) Call(ushort ax, ushort bx = 0, ushort cx = 0, ushort dx = 0, string? fileName = null)
    {
        SetW(0x7E, ax);
        SetW(0x80, bx);
        SetW(0x82, cx);
        SetW(0x84, dx);
        SetW(0x17C, ax);
        switch (ax)
        {
            case 0x00: Load(fileName ?? string.Empty); break;
            case 0x01: Start(); break;
            case 0x03:
                SetW(FadeDirection, 0);
                SetW(Fade, 0);
                Stop();
                break;
            case 0x05:
                SetW(FadeDirection, 0);
                SetW(Fade, 0);
                WarmInit();
                break;
            case 0x06: FindCard(); break;
            case 0x10:
                SetW(Pointers + 4 * bx + 2, cx);
                SetW(Pointers + 4 * bx, dx);
                break;
            case 0x11:
                SetW(0x80, 1);
                SetW(0x82, 0);
                break;
            case 0x12: SetW(0x80, W(Running)); break;
            case 0x15: SetW(0x80, BufferSize); break;
            case 0x18: SetW(Repeat, W(0x80)); break;
            case 0x19:
                SetW(Fade, 1);
                SetW(FadeDirection, 0);
                break;
            case 0x1A:
                SetW(Fade, 0x7F);
                SetW(FadeDirection, 1);
                break;
            case 0x1B: SetW(FadeStep, W(0x80)); break;
            default:
                if (ax > 0x1C) return (ax, bx, cx, dx); // 42D1: registers come back as they were pushed
                break;
        }
        return (W(0x7E), W(0x80), W(0x82), W(0x84));
    }

    /// <summary>Function 06h: card search (BX = 0: four ports, else BX), then percussion mode.</summary>
    /// <remarks>[ORIGINAL-FX] 4324</remarks>
    private void FindCard()
    {
        SetW(FadeDirection, 0);
        SetW(Fade, 0);
        if (W(0x80) == 0)
        {
            foreach (int port in new[] { 0x388, 0x288, 0x318, 0x218 })
            {
                SetW(Card, Detect(port));
                if (W(Card) != 0) break;
            }
            SetMode(1);
            SetW(Running, 0);
        }
        else SetW(Card, Detect(W(0x80)));
        if (W(Card) == 0) SetW(Speaker, 2);
    }

    /// <summary>Card test at one port, then the warm initialisation.</summary>
    /// <remarks>[ORIGINAL-FX] 296B, 3403 – [APPROX] the status reads before and after the timer run are not modelled;
    /// the test counts as passed (AdLib assumed, DS:0910 = 1).</remarks>
    private ushort Detect(int port)
    {
        SetW(Port, port);
        Out(4, 0x60);
        Out(4, 0x80);
        Out(2, 0xFF);
        Out(4, 0x21);
        Out(4, 0x60);
        Out(4, 0x80);
        WarmInit();
        return 1;
    }

    /// <summary>Function 05h body: all registers 0, timer mask, voice state, melodic mode, global parameters.</summary>
    /// <remarks>[ORIGINAL-FX] 2993</remarks>
    private void WarmInit()
    {
        for (int r = 1; r <= 0xF5; r++) Out(r, 0);
        Out(4, 6);
        for (int v = 0; v < 9; v++)
        {
            SetW(Bend + 2 * v, 0x2000);
            SetB(KeyOn + v, 0);
            SetB(Note + v, 0);
        }
        for (int v = 0; v < 11; v++) SetB(Volume + v, 0x7F);
        SetMode(0);
        SetGlobalParameters(0, 0, 0);
        SetPitchRange(1);
        SetWaveSelect(1);
    }

    // ------------------------------------------------------------------ module and playback

    /// <summary>Function 00h: reads an "A.H." module into the instrument table and the pattern buffers.</summary>
    /// <remarks>[ORIGINAL-FX] 3657 – [APPROX] cells of a channel beyond the pattern buffer are dropped (the original
    /// writes behind its buffer; the three modules never do).</remarks>
    private void Load(string name)
    {
        if (!name.Contains('.')) name += ".CMF";
        var file = _openFile(name);
        if (file is null) return;
        int pos = 0;
        int Read(int n, Action<int, byte> store)
        {
            int k = 0;
            for (; k < n && pos < file.Length; k++) store(k, file[pos++]);
            return k;
        }

        var magic = new byte[5];
        Read(5, (k, b) => magic[k] = b);
        if (magic[0] != 'A' || magic[1] != '.' || magic[2] != 'H' || magic[3] != '.' || magic[4] != 0) return;
        Read(0xC6, (k, b) => SetB(Orders + k, b));
        Read(2, (k, b) => SetB(PatternCount + k, b));
        Read(2, (k, b) => SetB(Exponent + k, b));
        Read(2, (k, b) => SetB(PercussionFlag + k, b));
        if (W(Speaker) == 0) SetMode(W(PercussionFlag));
        int count = 0;
        Read(2, (k, b) => count |= b << 8 * k);
        for (int i = 0; i < 0x40; i++)
        {
            int at = Instruments + i * InstrumentSize;
            for (int j = 0; j < 13; j++)
            {
                SetW(at + 2 * j, 0);
                SetW(at + 0x1A + 2 * j, 0);
            }
            SetW(at + 0x34, 0);
            SetW(at + 0x36, 0);
            for (int j = 0; ; j++) // strcpy of the default name at DS:160B
            {
                byte c = B(0x160B + j);
                SetB(at + 0x38 + j, c);
                if (c == 0) break;
            }
        }
        for (int i = 0; i < count; i++)
        {
            int at = i < 0x40 ? Instruments + i * InstrumentSize : 0x12E5;
            Read(InstrumentSize, (k, b) => SetB(at + k, b));
        }
        int patterns = W(PatternCount);
        for (int p = 0; p < patterns && p < BufferCount; p++)
        {
            var buf = _buffers[p];
            for (int row = 0; row < 0x40; row++)
                for (int ch = 0; ch < 0x0B; ch++)
                {
                    int c = row * 0x30 + ch * 4;
                    (buf[c], buf[c + 1], buf[c + 2], buf[c + 3]) = (0, 0, 0x7F, 0x20);
                    SetBufferWord(buf, 0xC02, 0x40);
                    SetBufferWord(buf, 0xC00, 1);
                }
        }
        for (int p = 0; p < patterns;)
        {
            int row = 0;
            Read(1, (_, b) => row = b);
            if (row == 0xFF)
            {
                int len = 0;
                if (p < BufferCount)
                {
                    var buf = _buffers[p];
                    while (buf[len * 0x30] != 1 && len < 0x40) len++;
                    SetBufferWord(buf, 0xC02, len);
                    SetBufferWord(buf, 0xC00, 1);
                }
                p++;
                continue;
            }
            int ch = 0;
            Read(1, (_, b) => ch = b);
            var cell = new byte[4];
            int got = Read(4, (k, b) => cell[k] = b);
            int at = row * 0x30 + ch * 4;
            if (row < 0x40 && p < BufferCount && at + 4 <= BufferSize)
                for (int k = 0; k < got; k++) _buffers[p][at + k] = cell[k];
            if (got == 0) break; // end of file inside the pattern data: read() returns nothing any more
        }
    }

    private static void SetBufferWord(byte[] buf, int at, int v) => (buf[at], buf[at + 1]) = ((byte)v, (byte)(v >> 8));

    private static int BufferWord(byte[] buf, int at) => buf[at] | buf[at + 1] << 8;

    /// <summary>Function 01h: channel state cleared, default operators, timer hooked, all voices off, start of the order.</summary>
    /// <remarks>[ORIGINAL-FX] 4150</remarks>
    private void Start()
    {
        SetW(Running, 1);
        for (int k = 0; k < 0x0B; k++)
            for (int j = 0; j < 4; j++) SetB(ChannelState + 4 * k + j, 0);
        DefaultOperators();
        TimerActive = true;
        for (int k = 0; k < 0x0B; k++) NoteOff(k);
        SetW(Row, 0);
        SetW(OrderPos, 0);
    }

    /// <summary>Function 03h body: all voices off, timer unhooked.</summary>
    /// <remarks>[ORIGINAL-FX] 4236 – the PC speaker reset (in al,61h …) has no OPL effect.</remarks>
    private void Stop()
    {
        for (int k = 0; k < 0x0B; k++) NoteOff(k);
        TimerActive = false;
        SetW(Running, 0);
    }

    /// <summary>One call of the driver's timer handler (every fourth tick of the game's timer, analyse/TON.md §2).</summary>
    /// <remarks>[ORIGINAL-FX] 3B82, 3BED, 405C, 40B4</remarks>
    public void Tick()
    {
        TickCount++;
        if (W(Running) > 1)
        {
            SetW(Running, W(Running) - 1);
            return;
        }
        SetW(Pattern, W(Orders + 2 * W(OrderPos)));
        if (W(Pattern) >= 0x40 && W(Repeat) == 0)
        {
            End();
        }
        else
        {
            if (W(Pattern) >= 0x40)
            {
                SetW(OrderPos, 0);
                SetW(Pattern, W(Orders));
            }
            SetW(Playing, 1);
            for (SetW(Channel, 0); W(Channel) < 0x0B; SetW(Channel, W(Channel) + 1)) PlayChannel(W(Channel));
            SetW(SubTick, W(SubTick) + 1);
            if ((ushort)(1 << ((W(Exponent) - 1) & 0x1F)) <= W(SubTick))
            {
                SetW(SubTick, 0);
                SetW(Row, W(Row) + 1);
            }
            var buf = CurrentBuffer();
            if (buf is null || BufferWord(buf, 0xC02) <= W(Row))
            {
                SetW(Row, 0);
                SetW(OrderPos, W(OrderPos) + 1);
                if (W(OrderPos) >= 0x40) SetW(OrderPos, 0);
            }
            SetW(Running, 1);
        }
        // fade (CS:1794)
        if (W(Fade) == 0) return;
        if (W(FadeDirection) != 0) SetW(Fade, W(Fade) >= W(FadeStep) ? W(Fade) - W(FadeStep) : 0);
        else SetW(Fade, W(Fade) + W(FadeStep));
        if (W(Fade) >= 0x80) End();
    }

    private byte[]? CurrentBuffer() => W(Pattern) < BufferCount ? _buffers[W(Pattern)] : null;

    /// <summary>End of the order without repeat, or fade-out finished: voices off, timer unhooked.</summary>
    private void End()
    {
        for (SetW(Channel, 0); W(Channel) < 0x0B; SetW(Channel, W(Channel) + 1)) NoteOff(W(Channel));
        TimerActive = false;
        SetW(Playing, 0);
        SetW(Running, 0);
        SetW(OrderPos, 0x41);
    }

    /// <summary>One channel of the current row (CS:12CD); AdLib path only (DS:0042 = 0).</summary>
    private void PlayChannel(int ch)
    {
        var buf = CurrentBuffer();
        int at = W(Row) * 0x30 + ch * 4;
        Span<byte> cell = stackalloc byte[4];
        if (buf is not null && at + 4 <= BufferSize) buf.AsSpan(at, 4).CopyTo(cell);
        int s = ChannelState + 4 * ch;
        bool row0 = W(Row) == 0;
        if (cell[0] > 0x0B)
        {
            if (row0 || cell[1] != B(s + 1))
            {
                if (cell[1] != 0) SetVoiceTimbre(ch, 0x1A5 + cell[1] * InstrumentSize);
                SetB(s + 1, cell[1]);
            }
            if (row0 || (ushort)(cell[2] - W(Fade)) != B(s + 2))
            {
                SetVoiceVolume(ch, cell[2] >= W(Fade) ? cell[2] - W(Fade) : 0);
                SetB(s + 2, cell[2] - B(Fade));
            }
            if (row0 || cell[3] != B(s + 3))
            {
                SetVoicePitch(ch, cell[3] << 8 | 0xFF);
                SetB(s + 3, cell[3]);
            }
            if (W(SubTick) == 0)
            {
                NoteOff(ch);
                NoteOn(ch, cell[0]);
                SetB(s, cell[0]);
            }
        }
        if (cell[0] == 4)
        {
            SetB(s, 0);
            NoteOff(ch);
        }
    }
}
