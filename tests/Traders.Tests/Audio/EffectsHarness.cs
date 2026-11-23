using Traders.Audio.Effects;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Audio;

/// <summary>
/// Runs the effect machine of T.EXE (code segment 0000) on the RefEmu (LoadOnly, direct far calls of 06FE, 0724, 0750, 0732)
/// and its port <see cref="EffectMachine"/> on the image read from T.EXE side by side. After each call the OPL2 register writes
/// of that call (register and value, in order; the AdLib stub records every data port write) and the machine's variables and
/// voice blocks CS:03EC–06FD are compared; <see cref="AssertSameSegment"/> compares the whole 64 KiB window except the bytes that
/// differ only by the loader's relocation (fixed when the harness is built and checked to lie outside the effect machine).
/// </summary>
internal sealed class EffectsHarness
{
    /// <summary>Compared variable range of the effect machine (variables, ring, voice blocks).</summary>
    public const int VariablesStart = 0x03EC, VariablesEnd = 0x06FE;

    private readonly OriginalProgram _p = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly List<(byte Register, byte Value)> _original = [];
    private readonly bool[] _relocated = new bool[EffectImage.Size];
    private readonly ushort _segment;
    private readonly byte[] _initialOriginal;
    private readonly byte[] _image;

    public EffectsHarness()
    {
        _segment = _p.Segment(0x0000);
        var previous = _p.Machine.AdLib.RegisterWritten;
        _p.Machine.AdLib.RegisterWritten = (reg, value) =>
        {
            previous?.Invoke(reg, value);
            _original.Add(((byte)reg, value));
        };
        _image = EffectImage.FromExe(File.ReadAllBytes(TestGameData.Dos("T.EXE")));
        _initialOriginal = Window();
        for (int i = 0; i < EffectImage.Size; i++)
        {
            if (_image[i] == _initialOriginal[i]) continue;
            if (i is >= 0x0208 and < 0x2684)
                throw new InvalidOperationException($"loaded image differs from T.EXE inside the effect machine at CS:{i:X4}");
            _relocated[i] = true;
        }
        Port = new EffectMachine(_image);
    }

    public EffectMachine Port { get; private set; }

    public OriginalProgram Program => _p;

    /// <summary>Restores the loaded segment window on the RefEmu and starts a fresh port on the image from T.EXE.</summary>
    public void Restart()
    {
        _p.Machine.Mem.WriteBlock(Memory.Linear(_segment, 0), _initialOriginal);
        _p.Machine.Cpu.Ss = 0x9000;
        _p.Machine.Cpu.Sp = 0xFFF0;
        Port = new EffectMachine(_image);
    }

    private byte[] Window() => _p.Machine.Mem.ReadBlock(_segment, 0, EffectImage.Size);

    public byte OriginalByte(int offset) => _p.Machine.Mem.Read8(_segment, (ushort)offset);

    public void WriteBoth(int offset, byte value)
    {
        _p.Machine.Mem.Write8(_segment, (ushort)offset, value);
        Port.Memory[offset] = value;
    }

    public void Reset(string label) => Compare(label, 0x06FE, [], Port.Reset);

    public void Stop(string label) => Compare(label, 0x0724, [], Port.Stop);

    public void Enqueue(string label, ushort effect) => Compare(label, 0x0750, [effect], () => Port.Enqueue(effect));

    public void Tick(string label) => Compare(label, 0x0732, [], Port.Tick);

    /// <summary>True when no voice has a running counter [DI+5] (all sequences ended or halted).</summary>
    public bool Idle()
    {
        for (int v = 0; v < EffectMachine.Voices; v++)
        {
            int block = OriginalByte(EffectMachine.VoiceBlocks + 2 * v) | (OriginalByte(EffectMachine.VoiceBlocks + 2 * v + 1) << 8);
            if (OriginalByte(block + 5) != 0) return false;
        }
        return true;
    }

    /// <summary>Number of OPL writes the last compared call produced.</summary>
    public int LastWrites { get; private set; }

    private void Compare(string label, ushort offset, ushort[] args, Action port)
    {
        _original.Clear();
        Port.ClearWrites();
        _p.Call(0x0000, offset, args);
        port();
        var portWrites = Port.Writes.Select(w => (w.Register, w.Value)).ToList();
        if (!_original.SequenceEqual(portWrites))
            Assert.Fail($"{label}: OPL writes differ\n original: {Format(_original)}\n port:     {Format(portWrites)}");
        Assert.All(Port.Writes, w => Assert.Equal(Port.TickCount, w.Tick));
        LastWrites = portWrites.Count;
        var original = _p.Machine.Mem.ReadBlock(_segment, VariablesStart, VariablesEnd - VariablesStart);
        for (int i = 0; i < original.Length; i++)
        {
            if (original[i] != Port.Memory[VariablesStart + i])
                Assert.Fail($"{label}: CS:{VariablesStart + i:X4} original {original[i]:X2}, port {Port.Memory[VariablesStart + i]:X2}");
        }
    }

    /// <summary>Compares the whole segment window (except relocated bytes).</summary>
    public void AssertSameSegment(string label)
    {
        var original = Window();
        for (int i = 0; i < original.Length; i++)
        {
            if (_relocated[i] || original[i] == Port.Memory[i]) continue;
            Assert.Fail($"{label}: CS:{i:X4} original {original[i]:X2}, port {Port.Memory[i]:X2}");
        }
    }

    private static string Format(IEnumerable<(byte Register, byte Value)> writes) =>
        string.Join(" ", writes.Select(w => $"{w.Register:X2}={w.Value:X2}"));
}
