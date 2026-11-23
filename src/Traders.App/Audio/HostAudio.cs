using Microsoft.Xna.Framework.Audio;
using Traders.Audio.Opl;

namespace Traders.App.Audio;

/// <summary>A sound device that plays queued 16-bit mono PCM buffers.</summary>
public interface IPcmOutput : IDisposable
{
    int SampleRate { get; }

    /// <summary>Buffers submitted but not yet played.</summary>
    int PendingBuffers { get; }

    void Submit(ReadOnlySpan<short> samples);
}

/// <summary>MonoGame <see cref="DynamicSoundEffectInstance"/> as <see cref="IPcmOutput"/>.</summary>
public sealed class MonoGamePcmOutput : IPcmOutput
{
    private readonly DynamicSoundEffectInstance _instance;
    private byte[] _bytes = [];

    public MonoGamePcmOutput(int sampleRate)
    {
        SampleRate = sampleRate;
        _instance = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Mono);
        _instance.Play();
    }

    public int SampleRate { get; }

    public int PendingBuffers => _instance.PendingBufferCount;

    public void Submit(ReadOnlySpan<short> samples)
    {
        int n = samples.Length * 2;
        if (_bytes.Length != n) _bytes = new byte[n];
        for (int i = 0; i < samples.Length; i++)
        {
            _bytes[2 * i] = (byte)samples[i];
            _bytes[2 * i + 1] = (byte)(samples[i] >> 8);
        }
        _instance.SubmitBuffer(_bytes);
        if (_instance.State != SoundState.Playing) _instance.Play();
    }

    public void Dispose() => _instance.Dispose();
}

/// <summary>
/// Sound output of the host: OPL2 synthesis fed by <see cref="Write"/> (writes stamped with host timer ticks) and pumped
/// into the sound device by <see cref="Update"/>, once per host update. Keeps <see cref="QueuedBuffers"/> buffers of
/// <see cref="BufferSamples"/> samples queued, so the device never runs dry while the game updates. Without a sound device
/// (<see cref="Create"/> fails to open one) the host runs on silently: writes are dropped, nothing is synthesised.
/// </summary>
public sealed class HostAudio : IDisposable
{
    public const int DefaultSampleRate = 44_100;

    /// <summary>About 1/60 s per buffer.</summary>
    public const int BufferSamples = DefaultSampleRate / 60;

    public const int QueuedBuffers = 3;

    private readonly IPcmOutput? _output;
    private readonly OplScheduler? _scheduler;
    private readonly short[] _buffer;

    public HostAudio(IPcmOutput? output)
    {
        _output = output;
        int rate = output?.SampleRate ?? DefaultSampleRate;
        int bufferSamples = rate / 60;
        _buffer = new short[bufferSamples];
        if (output is not null) _scheduler = new OplScheduler(rate, latencySamples: QueuedBuffers * bufferSamples);
    }

    /// <summary>Opens the default sound device; on any failure (no device, no OpenAL) a silent instance.</summary>
    public static HostAudio Create(int sampleRate = DefaultSampleRate, TextWriter? log = null)
    {
        try
        {
            return new HostAudio(new MonoGamePcmOutput(sampleRate));
        }
        catch (Exception e) when (e is NoAudioHardwareException or InstancePlayLimitException or DllNotFoundException
                                      or TypeInitializationException or InvalidOperationException or PlatformNotSupportedException)
        {
            log?.WriteLine($"Ton: kein Tongerät ({e.GetType().Name}: {e.Message}) – weiter ohne Ton");
            return new HostAudio(null);
        }
    }

    /// <summary>True when no sound device is open.</summary>
    public bool IsSilent => _output is null;

    /// <summary>The scheduler (null when silent).</summary>
    public OplScheduler? Scheduler => _scheduler;

    /// <summary>OPL2 register write at host timer tick <paramref name="tick"/>.</summary>
    public void Write(long tick, byte register, byte value) => _scheduler?.Enqueue(new OplWrite(tick, register, value));

    /// <summary>Tops up the device queue; call once per host update.</summary>
    public void Update()
    {
        if (_output is null || _scheduler is null) return;
        while (_output.PendingBuffers < QueuedBuffers)
        {
            _scheduler.Render(_buffer);
            _output.Submit(_buffer);
        }
    }

    public void Dispose() => _output?.Dispose();
}
