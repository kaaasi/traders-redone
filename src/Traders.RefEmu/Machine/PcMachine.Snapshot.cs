namespace Traders.RefEmu.Machine;

public sealed partial class PcMachine
{
    /// <summary>Snapshot of the DOS layer: open files, memory blocks, PSP, DTA, error and search state, console line.</summary>
    internal void SaveDosState(BinaryWriter w)
    {
        w.Write(_files.Count);
        foreach (var (handle, f) in _files.OrderBy(e => e.Key))
        {
            w.Write(handle);
            w.Write(f.Name);
            w.Write(f.Writable);
            w.Write(f.Data.Length);
            w.Write(f.Data);
            w.Write(f.Length);
            w.Write(f.Position);
            w.Write(f.Dirty);
        }
        WriteBlocks(w, _blocks);
        WriteBlocks(w, _residentBlocks);
        w.Write(_psp);
        w.Write(_dtaSeg);
        w.Write(_dtaOff);
        w.Write(_lastError);
        w.Write(_envSegment);
        w.Write(_freeStart);
        w.Write(_findResults.Count);
        foreach (var n in _findResults) w.Write(n);
        w.Write(_consoleLine.ToString());
    }

    internal void LoadDosState(BinaryReader r)
    {
        _files.Clear();
        int n = r.ReadInt32();
        for (int i = 0; i < n; i++)
        {
            int handle = r.ReadInt32();
            string name = r.ReadString();
            bool writable = r.ReadBoolean();
            byte[] data = r.ReadBytes(r.ReadInt32());
            _files[handle] = new OpenFile(name, data, writable)
            {
                Length = r.ReadInt32(),
                Position = r.ReadInt32(),
                Dirty = r.ReadBoolean(),
            };
        }
        ReadBlocks(r, _blocks);
        ReadBlocks(r, _residentBlocks);
        _psp = r.ReadUInt16();
        _dtaSeg = r.ReadUInt16();
        _dtaOff = r.ReadUInt16();
        _lastError = r.ReadUInt16();
        _envSegment = r.ReadUInt16();
        _freeStart = r.ReadUInt16();
        _findResults.Clear();
        n = r.ReadInt32();
        for (int i = 0; i < n; i++) _findResults.Add(r.ReadString());
        _consoleLine.Clear().Append(r.ReadString());
    }

    private static void WriteBlocks(BinaryWriter w, List<MemBlock> blocks)
    {
        w.Write(blocks.Count);
        foreach (var b in blocks)
        {
            w.Write(b.Segment);
            w.Write(b.Paragraphs);
        }
    }

    private static void ReadBlocks(BinaryReader r, List<MemBlock> blocks)
    {
        blocks.Clear();
        int n = r.ReadInt32();
        for (int i = 0; i < n; i++) blocks.Add(new MemBlock(r.ReadUInt16(), r.ReadUInt16()));
    }

    /// <summary>
    /// Snapshot of the machine's own clock and input script: idle time offset, next timer interrupt, pending host stop,
    /// scheduled scancodes (all of them, with the index of the next one) and the exit state.
    /// </summary>
    internal void SaveClockState(BinaryWriter w)
    {
        w.Write(_timeOffset);
        w.Write(_nextTimerIrq);
        w.Write(_stopHit);
        w.Write(_scheduledKeys.Count);
        foreach (var (time, sc) in _scheduledKeys)
        {
            w.Write(time);
            w.Write(sc);
        }
        w.Write(_nextScheduledKey);
        w.Write(Exited);
        w.Write(ExitCode);
    }

    internal void LoadClockState(BinaryReader r)
    {
        _timeOffset = r.ReadInt64();
        _nextTimerIrq = r.ReadInt64();
        _stopHit = r.ReadBoolean();
        _scheduledKeys.Clear();
        int n = r.ReadInt32();
        for (int i = 0; i < n; i++) _scheduledKeys.Add((r.ReadInt64(), r.ReadByte()));
        _nextScheduledKey = r.ReadInt32();
        Exited = r.ReadBoolean();
        ExitCode = r.ReadInt32();
    }
}
