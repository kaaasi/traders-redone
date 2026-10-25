namespace Traders.RefEmu.Cpu;

/// <summary>CPU exception raised during an instruction (vector number). The instruction is restarted from its first byte.</summary>
public sealed class CpuFault(int vector) : Exception($"CPU exception {vector}")
{
    public int Vector { get; } = vector;
}

/// <summary>Raised for situations the emulator deliberately does not support (instead of guessing).</summary>
public sealed class EmulationException(string message) : Exception(message);

/// <summary>
/// Interpreter for the Intel 80286 in real mode (8086 instruction set plus the 186/286 additions).
/// Behaviour follows the 286 where it differs from the 8086 (PUSH SP, shift count masking, faulting
/// IP of divide errors, #6 for invalid opcodes, #13 for word accesses at offset FFFF), because the
/// game ran on 286+ machines. Validated against the SingleStepTests 80286 real-mode suite.
/// </summary>
public sealed class Cpu286
{
    // Flag bits
    public const ushort CF = 0x0001, PF = 0x0004, AF = 0x0010, ZF = 0x0040, SF = 0x0080,
        TF = 0x0100, IF = 0x0200, DF = 0x0400, OF = 0x0800;

    // Register indices (8086 encoding)
    public const int AX = 0, CX = 1, DX = 2, BX = 3, SP = 4, BP = 5, SI = 6, DI = 7;
    public const int ES = 0, CS = 1, SS = 2, DS = 3;

    public readonly ushort[] R = new ushort[8];
    public readonly ushort[] S = new ushort[4];
    public ushort IP;
    private ushort _flags = 0x0002;

    public Memory Mem { get; }
    public IIoBus Io { get; set; }

    /// <summary>Host trap: opcode F1 nn calls this with nn (only when set; otherwise F1 is #6).</summary>
    public Action<Cpu286, byte>? HostTrap { get; set; }

    public bool Halted { get; set; }
    public long InstructionCount { get; private set; }

    /// <summary>One-instruction interrupt shadow after MOV SS / POP SS / STI.</summary>
    public bool InterruptShadow { get; private set; }

    public Cpu286(Memory mem, IIoBus? io = null)
    {
        Mem = mem;
        Io = io ?? new OpenIoBus();
    }

    public ushort Flags
    {
        get => _flags;
        set => _flags = (ushort)((value & 0x0FD5) | 0x0002); // 286 real mode: bits 12-15 read as 0
    }

    public bool GetFlag(ushort f) => (_flags & f) != 0;
    public void SetFlag(ushort f, bool on) { if (on) _flags |= f; else _flags &= (ushort)~f; }

    public ushort Ax { get => R[AX]; set => R[AX] = value; }
    public ushort Bx { get => R[BX]; set => R[BX] = value; }
    public ushort Cx { get => R[CX]; set => R[CX] = value; }
    public ushort Dx { get => R[DX]; set => R[DX] = value; }
    public ushort Sp { get => R[SP]; set => R[SP] = value; }
    public ushort Bp { get => R[BP]; set => R[BP] = value; }
    public ushort Si { get => R[SI]; set => R[SI] = value; }
    public ushort Di { get => R[DI]; set => R[DI] = value; }
    public ushort Es { get => S[ES]; set => S[ES] = value; }
    public ushort Cs { get => S[CS]; set => S[CS] = value; }
    public ushort Ss { get => S[SS]; set => S[SS] = value; }
    public ushort Ds { get => S[DS]; set => S[DS] = value; }

    public byte GetReg8(int i) => i < 4 ? (byte)R[i] : (byte)(R[i - 4] >> 8);

    public void SetReg8(int i, byte v)
    {
        if (i < 4) R[i] = (ushort)((R[i] & 0xFF00) | v);
        else R[i - 4] = (ushort)((R[i - 4] & 0x00FF) | (v << 8));
    }

    // ---------------------------------------------------------------- snapshot for fault restart

    private readonly ushort[] _snapR = new ushort[8];
    private readonly ushort[] _snapS = new ushort[4];
    private ushort _snapIp, _snapFlags;

    private void Snapshot()
    {
        Array.Copy(R, _snapR, 8);
        Array.Copy(S, _snapS, 4);
        _snapIp = IP;
        _snapFlags = _flags;
    }

    private void Restore()
    {
        Array.Copy(_snapR, R, 8);
        Array.Copy(_snapS, S, 4);
        IP = _snapIp;
        _flags = _snapFlags;
    }

    // ---------------------------------------------------------------- memory access

    public byte ReadByte(int seg, ushort off) => Mem.Read8(S[seg], off);
    public void WriteByte(int seg, ushort off, byte v) => Mem.Write8(S[seg], off, v);

    public ushort ReadWord(int seg, ushort off)
    {
        if (off == 0xFFFF) throw new CpuFault(13);
        uint lin = Memory.Linear(S[seg], off);
        return (ushort)(Mem.Read8(lin) | (Mem.Read8(lin + 1) << 8));
    }

    public void WriteWord(int seg, ushort off, ushort v)
    {
        if (off == 0xFFFF) throw new CpuFault(13);
        uint lin = Memory.Linear(S[seg], off);
        Mem.Write8(lin, (byte)v);
        Mem.Write8(lin + 1, (byte)(v >> 8));
    }

    public void Push(ushort v)
    {
        ushort sp = (ushort)(R[SP] - 2);
        WriteWord(SS, sp, v);
        R[SP] = sp;
    }

    public ushort Pop()
    {
        ushort v = ReadWord(SS, R[SP]);
        R[SP] += 2;
        return v;
    }

    // ---------------------------------------------------------------- instruction fetch / decode state

    private int _fetched;
    private ushort _startIp;
    private int _segOverride; // -1 = none
    private int _rep;         // 0 none, 0xF2 repne, 0xF3 rep/repe

    private byte Fetch8()
    {
        if (++_fetched > 10) throw new CpuFault(13);
        byte b = Mem.Read8(S[CS], IP);
        IP++;
        return b;
    }

    private ushort Fetch16()
    {
        byte lo = Fetch8();
        return (ushort)(lo | (Fetch8() << 8));
    }

    // ModRM
    private int _mod, _reg, _rm;
    private int _eaSeg;
    private ushort _eaOff;

    private void DecodeModRm()
    {
        byte m = Fetch8();
        _mod = m >> 6;
        _reg = (m >> 3) & 7;
        _rm = m & 7;
        if (_mod == 3) return;
        int seg = DS;
        int ea;
        switch (_rm)
        {
            case 0: ea = R[BX] + R[SI]; break;
            case 1: ea = R[BX] + R[DI]; break;
            case 2: ea = R[BP] + R[SI]; seg = SS; break;
            case 3: ea = R[BP] + R[DI]; seg = SS; break;
            case 4: ea = R[SI]; break;
            case 5: ea = R[DI]; break;
            case 6:
                if (_mod == 0) { ea = Fetch16(); }
                else { ea = R[BP]; seg = SS; }
                break;
            default: ea = R[BX]; break;
        }
        if (_mod == 1) ea += (sbyte)Fetch8();
        else if (_mod == 2) ea += Fetch16();
        _eaOff = (ushort)ea;
        _eaSeg = _segOverride >= 0 ? _segOverride : seg;
    }

    private bool IsReg => _mod == 3;

    private byte ReadRm8() => IsReg ? GetReg8(_rm) : ReadByte(_eaSeg, _eaOff);
    private ushort ReadRm16() => IsReg ? R[_rm] : ReadWord(_eaSeg, _eaOff);

    private void WriteRm8(byte v)
    {
        if (IsReg) SetReg8(_rm, v); else WriteByte(_eaSeg, _eaOff, v);
    }

    private void WriteRm16(ushort v)
    {
        if (IsReg) R[_rm] = v; else WriteWord(_eaSeg, _eaOff, v);
    }

    private int DataSeg => _segOverride >= 0 ? _segOverride : DS;

    // ---------------------------------------------------------------- flags helpers

    private static readonly bool[] Parity = BuildParity();

    private static bool[] BuildParity()
    {
        var p = new bool[256];
        for (int i = 0; i < 256; i++)
        {
            int c = 0;
            for (int b = 0; b < 8; b++) c += (i >> b) & 1;
            p[i] = (c & 1) == 0;
        }
        return p;
    }

    private void SetSzp(int res, bool word)
    {
        int mask = word ? 0xFFFF : 0xFF;
        res &= mask;
        SetFlag(ZF, res == 0);
        SetFlag(SF, (res & (word ? 0x8000 : 0x80)) != 0);
        SetFlag(PF, Parity[res & 0xFF]);
    }

    /// <summary>Two-operand ALU group (ADD OR ADC SBB AND SUB XOR CMP). Returns the result (masked).</summary>
    private int Alu(int op, int a, int b, bool word)
    {
        int mask = word ? 0xFFFF : 0xFF;
        int sign = word ? 0x8000 : 0x80;
        int res;
        switch (op)
        {
            case 0: // ADD
            case 2: // ADC
            {
                int c = op == 2 && GetFlag(CF) ? 1 : 0;
                res = a + b + c;
                SetFlag(CF, res > mask);
                SetFlag(OF, ((a ^ res) & (b ^ res) & sign) != 0);
                SetFlag(AF, ((a ^ b ^ res) & 0x10) != 0);
                break;
            }
            case 3: // SBB
            case 5: // SUB
            case 7: // CMP
            {
                int c = op == 3 && GetFlag(CF) ? 1 : 0;
                res = a - b - c;
                SetFlag(CF, res < 0);
                SetFlag(OF, ((a ^ b) & (a ^ res) & sign) != 0);
                SetFlag(AF, ((a ^ b ^ res) & 0x10) != 0);
                break;
            }
            case 1: res = a | b; Logic(); break;
            case 4: res = a & b; Logic(); break;
            default: res = a ^ b; Logic(); break;
        }
        res &= mask;
        SetSzp(res, word);
        return res;

        void Logic()
        {
            SetFlag(CF, false);
            SetFlag(OF, false);
            SetFlag(AF, false);
        }
    }

    private int Inc(int a, bool word)
    {
        int mask = word ? 0xFFFF : 0xFF;
        int res = (a + 1) & mask;
        SetFlag(OF, res == (word ? 0x8000 : 0x80));
        SetFlag(AF, (res & 0xF) == 0);
        SetSzp(res, word);
        return res;
    }

    private int Dec(int a, bool word)
    {
        int mask = word ? 0xFFFF : 0xFF;
        int res = (a - 1) & mask;
        SetFlag(OF, res == (word ? 0x7FFF : 0x7F));
        SetFlag(AF, (res & 0xF) == 0xF);
        SetSzp(res, word);
        return res;
    }

    /// <summary>Shift/rotate group (ROL ROR RCL RCR SHL SHR SAL SAR); count masked to 5 bits like the 286.</summary>
    private int Shift(int op, int a, int count, bool word)
    {
        count &= 0x1F;
        if (count == 0) return a;
        int bits = word ? 16 : 8;
        int mask = word ? 0xFFFF : 0xFF;
        int sign = word ? 0x8000 : 0x80;
        int res = a;
        switch (op)
        {
            case 0: // ROL
                for (int i = 0; i < count; i++) res = ((res << 1) | (res >> (bits - 1))) & mask;
                SetFlag(CF, (res & 1) != 0);
                SetFlag(OF, (((res & sign) != 0) ^ GetFlag(CF)));
                return res;
            case 1: // ROR
                for (int i = 0; i < count; i++) res = ((res >> 1) | ((res & 1) << (bits - 1))) & mask;
                SetFlag(CF, (res & sign) != 0);
                SetFlag(OF, ((res ^ (res << 1)) & sign) != 0);
                return res;
            case 2: // RCL
                for (int i = 0; i < count; i++)
                {
                    int c = GetFlag(CF) ? 1 : 0;
                    SetFlag(CF, (res & sign) != 0);
                    res = ((res << 1) | c) & mask;
                }
                SetFlag(OF, (((res & sign) != 0) ^ GetFlag(CF)));
                return res;
            case 3: // RCR
                for (int i = 0; i < count; i++)
                {
                    int c = GetFlag(CF) ? sign : 0;
                    SetFlag(CF, (res & 1) != 0);
                    res = (res >> 1) | c;
                }
                SetFlag(OF, ((res ^ (res << 1)) & sign) != 0);
                return res;
            case 4: // SHL
            case 6: // SAL (alias)
            {
                long r = (long)a << count;
                SetFlag(CF, ((r >> bits) & 1) != 0);
                res = (int)(r & mask);
                SetFlag(OF, (((res & sign) != 0) ^ GetFlag(CF)));
                SetFlag(AF, false);
                SetSzp(res, word);
                return res;
            }
            case 5: // SHR
                SetFlag(CF, ((a >> (count - 1)) & 1) != 0);
                res = count >= bits ? 0 : a >> count;
                SetFlag(OF, (a & sign) != 0);
                SetFlag(AF, false);
                SetSzp(res, word);
                return res;
            default: // SAR
            {
                int sa = word ? (short)a : (sbyte)a;
                SetFlag(CF, ((sa >> Math.Min(count - 1, 31)) & 1) != 0);
                res = (sa >> Math.Min(count, 31)) & mask;
                SetFlag(OF, false);
                SetFlag(AF, false);
                SetSzp(res, word);
                return res;
            }
        }
    }

    private bool Condition(int cc) => cc switch
    {
        0 => GetFlag(OF),
        1 => !GetFlag(OF),
        2 => GetFlag(CF),
        3 => !GetFlag(CF),
        4 => GetFlag(ZF),
        5 => !GetFlag(ZF),
        6 => GetFlag(CF) || GetFlag(ZF),
        7 => !GetFlag(CF) && !GetFlag(ZF),
        8 => GetFlag(SF),
        9 => !GetFlag(SF),
        10 => GetFlag(PF),
        11 => !GetFlag(PF),
        12 => GetFlag(SF) != GetFlag(OF),
        13 => GetFlag(SF) == GetFlag(OF),
        14 => GetFlag(ZF) || GetFlag(SF) != GetFlag(OF),
        _ => !GetFlag(ZF) && GetFlag(SF) == GetFlag(OF),
    };

    // ---------------------------------------------------------------- interrupts

    /// <summary>Vectors through the IVT (pushes FLAGS, CS, IP; clears IF and TF).</summary>
    public void Interrupt(int vector, ushort returnIp)
    {
        ushort newIp = Mem.Read16(0, (ushort)(vector * 4));
        ushort newCs = Mem.Read16(0, (ushort)(vector * 4 + 2));
        Push(_flags);
        Push(S[CS]);
        Push(returnIp);
        SetFlag(IF, false);
        SetFlag(TF, false);
        S[CS] = newCs;
        IP = newIp;
    }

    /// <summary>Delivers an external (hardware) interrupt between instructions.</summary>
    public void HardwareInterrupt(int vector)
    {
        Halted = false;
        Interrupt(vector, IP);
    }

    // ---------------------------------------------------------------- execution

    /// <summary>Executes one instruction (a complete REP string instruction counts as one).</summary>
    public void Step()
    {
        if (Halted) return;
        InterruptShadow = false;
        Snapshot();
        _startIp = IP;
        _fetched = 0;
        _segOverride = -1;
        _rep = 0;
        try
        {
            Execute();
        }
        catch (CpuFault f)
        {
            Restore();
            Interrupt(f.Vector, _startIp);
        }
        InstructionCount++;
    }

    private void Execute()
    {
        while (true)
        {
            byte op = Fetch8();
            switch (op)
            {
                case 0x26: _segOverride = ES; continue;
                case 0x2E: _segOverride = CS; continue;
                case 0x36: _segOverride = SS; continue;
                case 0x3E: _segOverride = DS; continue;
                case 0xF0: continue; // LOCK
                case 0xF2: _rep = 0xF2; continue;
                case 0xF3: _rep = 0xF3; continue;
            }
            ExecuteOpcode(op);
            return;
        }
    }

    private void ExecuteOpcode(byte op)
    {
        if (op < 0x40 && (op & 7) < 6)
        {
            int aluOp = op >> 3;
            switch (op & 7)
            {
                case 0: { DecodeModRm(); int r = Alu(aluOp, ReadRm8(), GetReg8(_reg), false); if (aluOp != 7) WriteRm8((byte)r); return; }
                case 1: { DecodeModRm(); int r = Alu(aluOp, ReadRm16(), R[_reg], true); if (aluOp != 7) WriteRm16((ushort)r); return; }
                case 2: { DecodeModRm(); int r = Alu(aluOp, GetReg8(_reg), ReadRm8(), false); if (aluOp != 7) SetReg8(_reg, (byte)r); return; }
                case 3: { DecodeModRm(); int r = Alu(aluOp, R[_reg], ReadRm16(), true); if (aluOp != 7) R[_reg] = (ushort)r; return; }
                case 4: { int r = Alu(aluOp, (byte)R[AX], Fetch8(), false); if (aluOp != 7) SetReg8(0, (byte)r); return; }
                case 5: { int r = Alu(aluOp, R[AX], Fetch16(), true); if (aluOp != 7) R[AX] = (ushort)r; return; }
            }
        }

        switch (op)
        {
            case 0x06: Push(S[ES]); return;
            case 0x07: S[ES] = Pop(); return;
            case 0x0E: Push(S[CS]); return;
            case 0x16: Push(S[SS]); return;
            case 0x17: S[SS] = Pop(); InterruptShadow = true; return;
            case 0x1E: Push(S[DS]); return;
            case 0x1F: S[DS] = Pop(); return;
            case 0x27: Daa(); return;
            case 0x2F: Das(); return;
            case 0x37: Aaa(); return;
            case 0x3F: Aas(); return;
        }

        if (op >= 0x40 && op <= 0x47) { bool c = GetFlag(CF); R[op & 7] = (ushort)Inc(R[op & 7], true); SetFlag(CF, c); return; }
        if (op >= 0x48 && op <= 0x4F) { bool c = GetFlag(CF); R[op & 7] = (ushort)Dec(R[op & 7], true); SetFlag(CF, c); return; }
        if (op >= 0x50 && op <= 0x57) { Push(R[op & 7]); return; } // PUSH SP pushes the old value on the 286
        if (op >= 0x58 && op <= 0x5F) { ushort v = Pop(); R[op & 7] = v; return; }
        if (op >= 0x70 && op <= 0x7F) { sbyte d = (sbyte)Fetch8(); if (Condition(op & 0x0F)) IP = (ushort)(IP + d); return; }
        if (op >= 0x91 && op <= 0x97) { (R[AX], R[op & 7]) = (R[op & 7], R[AX]); return; }
        if (op >= 0xB0 && op <= 0xB7) { SetReg8(op & 7, Fetch8()); return; }
        if (op >= 0xB8 && op <= 0xBF) { R[op & 7] = Fetch16(); return; }
        if (op >= 0xD8 && op <= 0xDF) { Esc(); return; }

        switch (op)
        {
            case 0x60: // PUSHA
            {
                ushort sp = R[SP];
                Push(R[AX]); Push(R[CX]); Push(R[DX]); Push(R[BX]);
                Push(sp); Push(R[BP]); Push(R[SI]); Push(R[DI]);
                return;
            }
            case 0x61: // POPA
                R[DI] = Pop(); R[SI] = Pop(); R[BP] = Pop(); Pop();
                R[BX] = Pop(); R[DX] = Pop(); R[CX] = Pop(); R[AX] = Pop();
                return;
            case 0x62: // BOUND
            {
                DecodeModRm();
                if (IsReg) throw new CpuFault(6);
                short lo = (short)ReadWord(_eaSeg, _eaOff);
                short hi = (short)ReadWord(_eaSeg, (ushort)(_eaOff + 2));
                short v = (short)R[_reg];
                if (v < lo || v > hi) throw new CpuFault(5);
                return;
            }
            case 0x68: Push(Fetch16()); return;
            case 0x69: { DecodeModRm(); short a = (short)ReadRm16(); short b = (short)Fetch16(); R[_reg] = Imul16(a, b); return; }
            case 0x6A: Push((ushort)(sbyte)Fetch8()); return;
            case 0x6B: { DecodeModRm(); short a = (short)ReadRm16(); short b = (sbyte)Fetch8(); R[_reg] = Imul16(a, b); return; }
            case 0x6C: StringOp(StrIns, false); return;
            case 0x6D: StringOp(StrIns, true); return;
            case 0x6E: StringOp(StrOuts, false); return;
            case 0x6F: StringOp(StrOuts, true); return;

            case 0x80:
            case 0x82:
            { DecodeModRm(); int a = ReadRm8(); int b = Fetch8(); int r = Alu(_reg, a, b, false); if (_reg != 7) WriteRm8((byte)r); return; }
            case 0x81:
            { DecodeModRm(); int a = ReadRm16(); int b = Fetch16(); int r = Alu(_reg, a, b, true); if (_reg != 7) WriteRm16((ushort)r); return; }
            case 0x83:
            { DecodeModRm(); int a = ReadRm16(); int b = (ushort)(sbyte)Fetch8(); int r = Alu(_reg, a, b, true); if (_reg != 7) WriteRm16((ushort)r); return; }
            case 0x84: { DecodeModRm(); Alu(4, ReadRm8(), GetReg8(_reg), false); return; }
            case 0x85: { DecodeModRm(); Alu(4, ReadRm16(), R[_reg], true); return; }
            case 0x86: { DecodeModRm(); byte a = ReadRm8(); byte b = GetReg8(_reg); WriteRm8(b); SetReg8(_reg, a); return; }
            case 0x87: { DecodeModRm(); ushort a = ReadRm16(); ushort b = R[_reg]; WriteRm16(b); R[_reg] = a; return; }
            case 0x88: DecodeModRm(); WriteRm8(GetReg8(_reg)); return;
            case 0x89: DecodeModRm(); WriteRm16(R[_reg]); return;
            case 0x8A: DecodeModRm(); SetReg8(_reg, ReadRm8()); return;
            case 0x8B: DecodeModRm(); R[_reg] = ReadRm16(); return;
            case 0x8C:
                DecodeModRm();
                if (_reg > 3) throw new CpuFault(6);
                WriteRm16(S[_reg]);
                return;
            case 0x8D:
                DecodeModRm();
                if (IsReg) throw new CpuFault(6);
                R[_reg] = _eaOff;
                return;
            case 0x8E:
            {
                DecodeModRm();
                if (_reg == CS || _reg > 3) throw new CpuFault(6);
                S[_reg] = ReadRm16();
                if (_reg == SS) InterruptShadow = true;
                return;
            }
            case 0x8F:
            {
                DecodeModRm();
                if (_reg != 0) throw new CpuFault(6);
                ushort v = ReadWord(SS, R[SP]);
                // the 286 computes the destination address before SP is incremented
                if (IsReg) { R[SP] += 2; R[_rm] = v; }
                else { WriteWord(_eaSeg, _eaOff, v); R[SP] += 2; }
                return;
            }
            case 0x90: return;
            case 0x98: R[AX] = (ushort)(sbyte)(byte)R[AX]; return;
            case 0x99: R[DX] = (ushort)((R[AX] & 0x8000) != 0 ? 0xFFFF : 0); return;
            case 0x9A: { ushort off = Fetch16(); ushort seg = Fetch16(); Push(S[CS]); Push(IP); S[CS] = seg; IP = off; return; }
            case 0x9B: return; // WAIT
            case 0x9C: Push(_flags); return;
            case 0x9D: Flags = Pop(); return;
            case 0x9E: _flags = (ushort)((_flags & 0xFF00) | (R[AX] >> 8 & 0xD5) | 0x02); return;
            case 0x9F: SetReg8(4, (byte)_flags); return;
            case 0xA0: { ushort off = Fetch16(); SetReg8(0, ReadByte(DataSeg, off)); return; }
            case 0xA1: { ushort off = Fetch16(); R[AX] = ReadWord(DataSeg, off); return; }
            case 0xA2: { ushort off = Fetch16(); WriteByte(DataSeg, off, (byte)R[AX]); return; }
            case 0xA3: { ushort off = Fetch16(); WriteWord(DataSeg, off, R[AX]); return; }
            case 0xA4: StringOp(StrMovs, false); return;
            case 0xA5: StringOp(StrMovs, true); return;
            case 0xA6: StringOp(StrCmps, false); return;
            case 0xA7: StringOp(StrCmps, true); return;
            case 0xA8: Alu(4, (byte)R[AX], Fetch8(), false); return;
            case 0xA9: Alu(4, R[AX], Fetch16(), true); return;
            case 0xAA: StringOp(StrStos, false); return;
            case 0xAB: StringOp(StrStos, true); return;
            case 0xAC: StringOp(StrLods, false); return;
            case 0xAD: StringOp(StrLods, true); return;
            case 0xAE: StringOp(StrScas, false); return;
            case 0xAF: StringOp(StrScas, true); return;

            case 0xC0: { DecodeModRm(); int a = ReadRm8(); int c = Fetch8(); WriteRm8((byte)Shift(_reg, a, c, false)); return; }
            case 0xC1: { DecodeModRm(); int a = ReadRm16(); int c = Fetch8(); WriteRm16((ushort)Shift(_reg, a, c, true)); return; }
            case 0xC2: { ushort n = Fetch16(); ushort ip = Pop(); R[SP] += n; IP = ip; return; }
            case 0xC3: IP = Pop(); return;
            case 0xC4:
            case 0xC5:
            {
                DecodeModRm();
                if (IsReg) throw new CpuFault(6);
                ushort off = ReadWord(_eaSeg, _eaOff);
                ushort seg = ReadWord(_eaSeg, (ushort)(_eaOff + 2));
                R[_reg] = off;
                S[op == 0xC4 ? ES : DS] = seg;
                return;
            }
            case 0xC6:
            { DecodeModRm(); if (_reg != 0) throw new CpuFault(6); WriteRm8(Fetch8()); return; }
            case 0xC7:
            { DecodeModRm(); if (_reg != 0) throw new CpuFault(6); WriteRm16(Fetch16()); return; }
            case 0xC8: Enter(); return;
            case 0xC9: R[SP] = R[BP]; R[BP] = Pop(); return;
            case 0xCA: { ushort n = Fetch16(); ushort ip = Pop(); ushort cs = Pop(); R[SP] += n; IP = ip; S[CS] = cs; return; }
            case 0xCB: { ushort ip = Pop(); ushort cs = Pop(); IP = ip; S[CS] = cs; return; }
            case 0xCC: Interrupt(3, IP); return;
            case 0xCD: { byte n = Fetch8(); Interrupt(n, IP); return; }
            case 0xCE: if (GetFlag(OF)) Interrupt(4, IP); return;
            case 0xCF: { ushort ip = Pop(); ushort cs = Pop(); ushort f = Pop(); IP = ip; S[CS] = cs; Flags = f; return; }

            case 0xD0: { DecodeModRm(); WriteRm8((byte)Shift(_reg, ReadRm8(), 1, false)); return; }
            case 0xD1: { DecodeModRm(); WriteRm16((ushort)Shift(_reg, ReadRm16(), 1, true)); return; }
            case 0xD2: { DecodeModRm(); WriteRm8((byte)Shift(_reg, ReadRm8(), (byte)R[CX], false)); return; }
            case 0xD3: { DecodeModRm(); WriteRm16((ushort)Shift(_reg, ReadRm16(), (byte)R[CX], true)); return; }
            case 0xD4: Aam(Fetch8()); return;
            case 0xD5: Aad(Fetch8()); return;
            case 0xD7: { ushort off = (ushort)(R[BX] + (byte)R[AX]); SetReg8(0, ReadByte(DataSeg, off)); return; }

            case 0xE0: { sbyte d = (sbyte)Fetch8(); R[CX]--; if (R[CX] != 0 && !GetFlag(ZF)) IP = (ushort)(IP + d); return; }
            case 0xE1: { sbyte d = (sbyte)Fetch8(); R[CX]--; if (R[CX] != 0 && GetFlag(ZF)) IP = (ushort)(IP + d); return; }
            case 0xE2: { sbyte d = (sbyte)Fetch8(); R[CX]--; if (R[CX] != 0) IP = (ushort)(IP + d); return; }
            case 0xE3: { sbyte d = (sbyte)Fetch8(); if (R[CX] == 0) IP = (ushort)(IP + d); return; }
            case 0xE4: SetReg8(0, Io.In8(Fetch8())); return;
            case 0xE5: { byte p = Fetch8(); R[AX] = In16(p); return; }
            case 0xE6: Io.Out8(Fetch8(), (byte)R[AX]); return;
            case 0xE7: { byte p = Fetch8(); Out16(p, R[AX]); return; }
            case 0xE8: { ushort d = Fetch16(); Push(IP); IP = (ushort)(IP + d); return; }
            case 0xE9: { ushort d = Fetch16(); IP = (ushort)(IP + d); return; }
            case 0xEA: { ushort off = Fetch16(); ushort seg = Fetch16(); S[CS] = seg; IP = off; return; }
            case 0xEB: { sbyte d = (sbyte)Fetch8(); IP = (ushort)(IP + d); return; }
            case 0xEC: SetReg8(0, Io.In8(R[DX])); return;
            case 0xED: R[AX] = In16(R[DX]); return;
            case 0xEE: Io.Out8(R[DX], (byte)R[AX]); return;
            case 0xEF: Out16(R[DX], R[AX]); return;

            case 0xF1:
                if (HostTrap is null) throw new CpuFault(6);
                HostTrap(this, Fetch8());
                return;
            case 0xF4: Halted = true; return;
            case 0xF5: SetFlag(CF, !GetFlag(CF)); return;
            case 0xF6: Group3(false); return;
            case 0xF7: Group3(true); return;
            case 0xF8: SetFlag(CF, false); return;
            case 0xF9: SetFlag(CF, true); return;
            case 0xFA: SetFlag(IF, false); return;
            case 0xFB: SetFlag(IF, true); InterruptShadow = true; return;
            case 0xFC: SetFlag(DF, false); return;
            case 0xFD: SetFlag(DF, true); return;
            case 0xFE:
            {
                DecodeModRm();
                bool c = GetFlag(CF);
                if (_reg == 0) WriteRm8((byte)Inc(ReadRm8(), false));
                else if (_reg == 1) WriteRm8((byte)Dec(ReadRm8(), false));
                else throw new CpuFault(6);
                SetFlag(CF, c);
                return;
            }
            case 0xFF: Group5(); return;
        }

        throw new CpuFault(6);
    }

    private ushort In16(ushort port) => (ushort)(Io.In8(port) | (Io.In8((ushort)(port + 1)) << 8));

    private void Out16(ushort port, ushort v)
    {
        Io.Out8(port, (byte)v);
        Io.Out8((ushort)(port + 1), (byte)(v >> 8));
    }

    private void Esc()
    {
        // No coprocessor: ESC opcodes only compute (and bounds-check) their memory operand.
        DecodeModRm();
        if (!IsReg && _eaOff == 0xFFFF) throw new CpuFault(13);
    }

    private ushort Imul16(short a, short b)
    {
        int r = a * b;
        bool ovf = r != (short)r;
        SetFlag(CF, ovf);
        SetFlag(OF, ovf);
        SetSzp(r, true);
        return (ushort)r;
    }

    private void Enter()
    {
        ushort size = Fetch16();
        int level = Fetch8() & 0x1F;
        Push(R[BP]);
        ushort frame = R[SP];
        if (level > 0)
        {
            for (int i = 1; i < level; i++)
            {
                R[BP] -= 2;
                Push(ReadWord(SS, R[BP]));
            }
            Push(frame);
        }
        R[BP] = frame;
        R[SP] -= size;
    }

    private void Group3(bool word)
    {
        DecodeModRm();
        switch (_reg)
        {
            case 0:
            case 1: // TEST (reg 1 is an alias on the 286)
                if (word) { int a = ReadRm16(); Alu(4, a, Fetch16(), true); }
                else { int a = ReadRm8(); Alu(4, a, Fetch8(), false); }
                return;
            case 2: // NOT
                if (word) WriteRm16((ushort)~ReadRm16()); else WriteRm8((byte)~ReadRm8());
                return;
            case 3: // NEG
                if (word) { int a = ReadRm16(); int r = Alu(5, 0, a, true); WriteRm16((ushort)r); SetFlag(CF, a != 0); }
                else { int a = ReadRm8(); int r = Alu(5, 0, a, false); WriteRm8((byte)r); SetFlag(CF, a != 0); }
                return;
            case 4: // MUL
                if (word)
                {
                    uint r = (uint)R[AX] * ReadRm16();
                    R[AX] = (ushort)r; R[DX] = (ushort)(r >> 16);
                    bool hi = R[DX] != 0;
                    SetFlag(CF, hi); SetFlag(OF, hi);
                    SetSzp((int)(r & 0xFFFF), true);
                }
                else
                {
                    int r = (byte)R[AX] * ReadRm8();
                    R[AX] = (ushort)r;
                    bool hi = (r & 0xFF00) != 0;
                    SetFlag(CF, hi); SetFlag(OF, hi);
                    SetSzp(r & 0xFF, false);
                }
                return;
            case 5: // IMUL
                if (word)
                {
                    int r = (short)R[AX] * (short)ReadRm16();
                    R[AX] = (ushort)r; R[DX] = (ushort)(r >> 16);
                    bool ovf = r != (short)r;
                    SetFlag(CF, ovf); SetFlag(OF, ovf);
                    SetSzp(r & 0xFFFF, true);
                }
                else
                {
                    int r = (sbyte)(byte)R[AX] * (sbyte)ReadRm8();
                    R[AX] = (ushort)r;
                    bool ovf = r != (sbyte)r;
                    SetFlag(CF, ovf); SetFlag(OF, ovf);
                    SetSzp(r & 0xFF, false);
                }
                return;
            case 6: // DIV
                if (word)
                {
                    uint d = ReadRm16();
                    uint n = ((uint)R[DX] << 16) | R[AX];
                    if (d == 0) throw new CpuFault(0);
                    uint q = n / d;
                    if (q > 0xFFFF) throw new CpuFault(0);
                    R[AX] = (ushort)q; R[DX] = (ushort)(n % d);
                }
                else
                {
                    uint d = ReadRm8();
                    uint n = R[AX];
                    if (d == 0) throw new CpuFault(0);
                    uint q = n / d;
                    if (q > 0xFF) throw new CpuFault(0);
                    R[AX] = (ushort)(((n % d) << 8) | q);
                }
                return;
            default: // IDIV
                if (word)
                {
                    int d = (short)ReadRm16();
                    int n = (int)(((uint)R[DX] << 16) | R[AX]);
                    if (d == 0) throw new CpuFault(0);
                    long q = (long)n / d;
                    if (q > short.MaxValue || q < short.MinValue) throw new CpuFault(0);
                    R[AX] = (ushort)q; R[DX] = (ushort)(n % d);
                }
                else
                {
                    int d = (sbyte)ReadRm8();
                    int n = (short)R[AX];
                    if (d == 0) throw new CpuFault(0);
                    int q = n / d;
                    if (q > sbyte.MaxValue || q < sbyte.MinValue) throw new CpuFault(0);
                    R[AX] = (ushort)((((n % d) & 0xFF) << 8) | (q & 0xFF));
                }
                return;
        }
    }

    private void Group5()
    {
        DecodeModRm();
        switch (_reg)
        {
            case 0: { bool c = GetFlag(CF); WriteRm16((ushort)Inc(ReadRm16(), true)); SetFlag(CF, c); return; }
            case 1: { bool c = GetFlag(CF); WriteRm16((ushort)Dec(ReadRm16(), true)); SetFlag(CF, c); return; }
            case 2: { ushort t = ReadRm16(); Push(IP); IP = t; return; }
            case 3:
            {
                if (IsReg) throw new CpuFault(6);
                ushort off = ReadWord(_eaSeg, _eaOff);
                ushort seg = ReadWord(_eaSeg, (ushort)(_eaOff + 2));
                Push(S[CS]); Push(IP);
                S[CS] = seg; IP = off;
                return;
            }
            case 4: IP = ReadRm16(); return;
            case 5:
            {
                if (IsReg) throw new CpuFault(6);
                ushort off = ReadWord(_eaSeg, _eaOff);
                ushort seg = ReadWord(_eaSeg, (ushort)(_eaOff + 2));
                S[CS] = seg; IP = off;
                return;
            }
            case 6: Push(ReadRm16()); return;
            default: throw new CpuFault(6);
        }
    }

    // ---------------------------------------------------------------- string instructions

    private const int StrMovs = 0, StrCmps = 1, StrStos = 2, StrLods = 3, StrScas = 4, StrIns = 5, StrOuts = 6;

    private void StringOp(int kind, bool word)
    {
        int step = (GetFlag(DF) ? -1 : 1) * (word ? 2 : 1);
        if (_rep == 0)
        {
            StringOnce(kind, word, step);
            return;
        }
        bool conditional = kind == StrCmps || kind == StrScas;
        while (R[CX] != 0)
        {
            StringOnce(kind, word, step);
            R[CX]--;
            Snapshot(); // a fault in a later iteration keeps the progress made so far
            _snapIp = _startIp;
            if (conditional && GetFlag(ZF) != (_rep == 0xF3)) break;
        }
    }

    private void StringOnce(int kind, bool word, int step)
    {
        switch (kind)
        {
            case StrMovs:
                if (word) WriteWord(ES, R[DI], ReadWord(DataSeg, R[SI]));
                else WriteByte(ES, R[DI], ReadByte(DataSeg, R[SI]));
                R[SI] = (ushort)(R[SI] + step); R[DI] = (ushort)(R[DI] + step);
                break;
            case StrCmps:
                if (word) Alu(7, ReadWord(DataSeg, R[SI]), ReadWord(ES, R[DI]), true);
                else Alu(7, ReadByte(DataSeg, R[SI]), ReadByte(ES, R[DI]), false);
                R[SI] = (ushort)(R[SI] + step); R[DI] = (ushort)(R[DI] + step);
                break;
            case StrStos:
                if (word) WriteWord(ES, R[DI], R[AX]); else WriteByte(ES, R[DI], (byte)R[AX]);
                R[DI] = (ushort)(R[DI] + step);
                break;
            case StrLods:
                if (word) R[AX] = ReadWord(DataSeg, R[SI]); else SetReg8(0, ReadByte(DataSeg, R[SI]));
                R[SI] = (ushort)(R[SI] + step);
                break;
            case StrScas:
                if (word) Alu(7, R[AX], ReadWord(ES, R[DI]), true);
                else Alu(7, (byte)R[AX], ReadByte(ES, R[DI]), false);
                R[DI] = (ushort)(R[DI] + step);
                break;
            case StrIns:
                if (word) WriteWord(ES, R[DI], In16(R[DX])); else WriteByte(ES, R[DI], Io.In8(R[DX]));
                R[DI] = (ushort)(R[DI] + step);
                break;
            default: // OUTS
                if (word) Out16(R[DX], ReadWord(DataSeg, R[SI])); else Io.Out8(R[DX], ReadByte(DataSeg, R[SI]));
                R[SI] = (ushort)(R[SI] + step);
                break;
        }
    }

    // ---------------------------------------------------------------- BCD

    private void Daa()
    {
        int al = (byte)R[AX];
        bool oldCf = GetFlag(CF);
        int oldAl = al;
        SetFlag(CF, false);
        if ((al & 0x0F) > 9 || GetFlag(AF))
        {
            al += 6;
            SetFlag(CF, oldCf || al > 0xFF);
            SetFlag(AF, true);
        }
        else SetFlag(AF, false);
        if (oldAl > 0x99 || oldCf) { al += 0x60; SetFlag(CF, true); }
        else SetFlag(CF, false);
        SetReg8(0, (byte)al);
        SetSzp(al & 0xFF, false);
    }

    private void Das()
    {
        int al = (byte)R[AX];
        bool oldCf = GetFlag(CF);
        int oldAl = al;
        SetFlag(CF, false);
        if ((al & 0x0F) > 9 || GetFlag(AF))
        {
            al -= 6;
            SetFlag(CF, oldCf || al < 0);
            SetFlag(AF, true);
        }
        else SetFlag(AF, false);
        if (oldAl > 0x99 || oldCf) { al -= 0x60; SetFlag(CF, true); }
        SetReg8(0, (byte)al);
        SetSzp(al & 0xFF, false);
    }

    private void Aaa()
    {
        if ((R[AX] & 0x0F) > 9 || GetFlag(AF))
        {
            R[AX] += 0x106;
            SetFlag(AF, true); SetFlag(CF, true);
        }
        else { SetFlag(AF, false); SetFlag(CF, false); }
        R[AX] &= 0xFF0F;
    }

    private void Aas()
    {
        if ((R[AX] & 0x0F) > 9 || GetFlag(AF))
        {
            R[AX] -= 0x106;
            SetFlag(AF, true); SetFlag(CF, true);
        }
        else { SetFlag(AF, false); SetFlag(CF, false); }
        R[AX] &= 0xFF0F;
    }

    private void Aam(byte imm)
    {
        if (imm == 0) throw new CpuFault(0);
        int al = (byte)R[AX];
        R[AX] = (ushort)(((al / imm) << 8) | (al % imm));
        SetSzp(al % imm, false);
    }

    private void Aad(byte imm)
    {
        int al = (byte)R[AX], ah = R[AX] >> 8;
        int res = (al + ah * imm) & 0xFF;
        R[AX] = (ushort)res;
        SetSzp(res, false);
    }
}
