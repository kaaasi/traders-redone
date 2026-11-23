using Traders.Audio.Opl;

namespace Traders.App.Audio;

/// <summary>
/// Places OPL2 writes stamped with host timer ticks (<see cref="Timing.HostClock.Ticks"/>, PIT divisor 4000h) onto the
/// output sample stream. The output is rendered on demand of the sound device, so its position can drift against the host
/// clock (separate oscillators, stalls). Each write goes to <c>sample(tick) + offset</c>; the offset is (re)anchored to
/// "now + latency" when a write would land in the past or too far ahead, and targets never go backwards, so the order of
/// writes is kept and a stall costs at most one re-anchoring, never a gap in the output. Single-threaded.
/// </summary>
public sealed class OplScheduler
{
    private readonly OplStream _stream;
    private readonly Queue<(long Sample, byte Register, byte Value)> _pending = new();
    private readonly OplClock _clock;
    private long _offset;
    private bool _anchored;
    private long _lastTarget;

    /// <param name="sampleRate">Output rate in Hz.</param>
    /// <param name="latencySamples">Distance between the render position and a newly anchored write.</param>
    /// <param name="clock">Unit of the write ticks (default: the game's timer tick).</param>
    public OplScheduler(int sampleRate, int latencySamples, OplClock? clock = null)
    {
        if (latencySamples < 0) throw new ArgumentOutOfRangeException(nameof(latencySamples));
        _stream = new OplStream(sampleRate);
        _clock = clock ?? OplClock.TimerTick;
        LatencySamples = latencySamples;
    }

    public int SampleRate => _stream.SampleRate;

    public int LatencySamples { get; }

    /// <summary>Output samples rendered so far.</summary>
    public long Rendered => _stream.SamplesRendered;

    /// <summary>Number of times the tick-to-sample offset was set (first write, stall or drift).</summary>
    public int Anchorings { get; private set; }

    /// <summary>Writes waiting for their sample position.</summary>
    public int Pending => _pending.Count;

    /// <summary>Queues a register write for tick <paramref name="tick"/>.</summary>
    public void Enqueue(OplWrite write)
    {
        long baseSample = _clock.SampleAt(write.Tick, SampleRate);
        long target = baseSample + _offset;
        long now = Rendered;
        if (!_anchored || target < now || target > now + 4L * LatencySamples + SampleRate / 10)
        {
            _offset = now + LatencySamples - baseSample;
            _anchored = true;
            Anchorings++;
            target = baseSample + _offset;
        }
        target = Math.Max(target, _lastTarget);
        _lastTarget = target;
        _pending.Enqueue((target, write.Register, write.Value));
    }

    /// <summary>Renders the next <paramref name="output"/>.Length samples, applying due writes at their positions.</summary>
    public void Render(Span<short> output)
    {
        int done = 0;
        while (done < output.Length)
        {
            while (_pending.Count > 0 && _pending.Peek().Sample <= Rendered)
            {
                var w = _pending.Dequeue();
                _stream.Write(w.Register, w.Value);
            }
            long until = _pending.Count > 0 ? _pending.Peek().Sample : long.MaxValue;
            int n = (int)Math.Min(output.Length - done, until - Rendered);
            _stream.Render(output.Slice(done, n));
            done += n;
        }
    }
}
