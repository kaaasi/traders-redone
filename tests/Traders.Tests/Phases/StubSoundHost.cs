using Traders.Core.Phases;
using Traders.Core.Sound;

namespace Traders.Tests.Phases;

/// <summary>
/// The host's end of the sound control as the RefEmu's default run has it (<c>RecordSoundDriver</c>): an int 64h vector is
/// there, so 218B:0002 starts the driver, and the driver answers function 12h with "nothing playing" (BL = 0, or
/// <see cref="PlaybackState"/>). Records the commands it is given, in order and in the batches the core passes them.
/// </summary>
internal sealed class StubSoundHost : ISoundHost
{
    /// <summary>Host heap address of the buffer block (any non-null value).</summary>
    public const uint Block = 0xB000_0000;

    public uint DriverVector { get; set; } = 0x0000_195D;

    /// <summary>The answer to function 12h; every read is noted in <see cref="ReadsAt"/>.</summary>
    public byte PlaybackState
    {
        get
        {
            ReadsAt.Add(Executed.Count);
            return Playing;
        }
    }

    /// <summary>BL of function 12h (0 = nothing playing, as the RefEmu's recording stub).</summary>
    public byte Playing { get; set; }

    /// <summary>Number of commands executed when the core read the answer, per read.</summary>
    public List<int> ReadsAt { get; } = [];

    public List<SoundCommand> Executed { get; } = [];

    public List<int> Batches { get; } = [];

    public uint Allocate(int size) => Block;

    public void Execute(IReadOnlyList<SoundCommand> commands)
    {
        Executed.AddRange(commands);
        Batches.Add(commands.Count);
    }
}
