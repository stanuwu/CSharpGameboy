using csharp_gameboy.Memory;
using csharp_gameboy.Util;

namespace csharp_gameboy.Compute
{
    public class CPU
    {
        // flag conditions
        public enum Conditions
        {
            Z,
            NZ,
            C,
            NC,
            A
        }

        // cpu vars
        private bool _halted;

        // ime flag
        private bool _ime;

        // program pointer
        private ushort _pc;

        // 8 bit registers
        private byte _registerA;
        private byte _registerB;
        private byte _registerC;
        private byte _registerD;
        private byte _registerE;
        private byte _registerF;
        private byte _registerH;
        private byte _registerL;

        // stack pointer
        private ushort _sp; // auto formatter doesnt like _sp :(

        public CPU(MemoryManager memoryManager)
        {
            MemoryManager = memoryManager;
        }

        public MemoryManager MemoryManager { get; }

        // read / write registers
        public byte ReadA()
        {
            return _registerA;
        }

        public void WriteA(byte value)
        {
            _registerA = value;
        }

        public byte ReadB()
        {
            return _registerB;
        }

        public void WriteB(byte value)
        {
            _registerB = value;
        }

        public byte ReadC()
        {
            return _registerC;
        }

        public void WriteC(byte value)
        {
            _registerC = value;
        }

        public byte ReadD()
        {
            return _registerD;
        }

        public void WriteD(byte value)
        {
            _registerD = value;
        }

        public byte ReadE()
        {
            return _registerE;
        }

        public void WriteE(byte value)
        {
            _registerE = value;
        }

        public bool ReadFlagCarry()
        {
            return MathUtil.IsBitSet(_registerF, 4);
        }

        public void SetFlagCarry(bool state)
        {
            _registerF = MathUtil.SetOrClearBit(_registerF, state, 4);
        }

        public bool ReadFlagHalfCarry()
        {
            return MathUtil.IsBitSet(_registerF, 5);
        }

        public void SetFlagHalfCarry(bool state)
        {
            _registerF = MathUtil.SetOrClearBit(_registerF, state, 5);
        }

        public bool ReadFlagSubtraction()
        {
            return MathUtil.IsBitSet(_registerF, 6);
        }

        public void SetFlagSubtraction(bool state)
        {
            _registerF = MathUtil.SetOrClearBit(_registerF, state, 6);
        }

        public bool ReadFlagZero()
        {
            return MathUtil.IsBitSet(_registerF, 7);
        }

        public void SetFlagZero(bool state)
        {
            _registerF = MathUtil.SetOrClearBit(_registerF, state, 7);
        }

        public byte ReadFlagsRegister()
        {
            return _registerF;
        }

        public byte ReadH()
        {
            return _registerH;
        }

        public void WriteH(byte value)
        {
            _registerH = value;
        }

        public byte ReadL()
        {
            return _registerL;
        }

        public void WriteL(byte value)
        {
            _registerL = value;
        }

        public ushort ReadAF()
        {
            return (ushort)((_registerA << 8) | _registerF);
        }

        public void WriteAF(ushort value)
        {
            _registerA = (byte)(value >> 8);
            _registerF = (byte)value;
        }

        public ushort ReadBC()
        {
            return (ushort)((_registerB << 8) | _registerC);
        }

        public void WriteBC(ushort value)
        {
            _registerB = (byte)(value >> 8);
            _registerC = (byte)value;
        }

        public ushort ReadDE()
        {
            return (ushort)((_registerD << 8) | _registerE);
        }

        public void WriteDE(ushort value)
        {
            _registerD = (byte)(value >> 8);
            _registerE = (byte)value;
        }

        public ushort ReadHL()
        {
            return (ushort)((_registerH << 8) | _registerL);
        }

        public void WriteHL(ushort value)
        {
            _registerH = (byte)(value >> 8);
            _registerL = (byte)value;
        }

        public ushort ReadPC()
        {
            return _pc;
        }

        public ushort ReadSP()
        {
            return _sp;
        }

        public void WriteSP(ushort sp)
        {
            _sp = sp;
        }

        // abstract cpu operations

        public byte Adc8(byte a, byte b)
        {
            var c = (byte)(a + b + MathUtil.BoolAsByte(ReadFlagCarry()));
            SetFlagsAdc8(c, a, b, MathUtil.BoolAsByte(ReadFlagCarry()));
            return c;
        }

        public byte Add8(byte a, byte b)
        {
            var c = (byte)(a + b);
            SetFlagsAdd8(c, a, b);
            return c;
        }

        public ushort Add16(ushort a, ushort b)
        {
            var c = (ushort)(a + b);
            SetFlagsAdd16(c, a, b);
            return c;
        }

        public ushort AddHL(ushort a, ushort b)
        {
            var c = (ushort)(a + b);
            SetFlagsAddHL(c, a, b);
            return c;
        }

        public ushort Add16s(ushort a, short b)
        {
            var c = (ushort)(a + b);
            SetFlagsAddSP(c, a, (ushort)b);
            return c;
        }

        public byte Sub8(byte a, byte b)
        {
            var c = (byte)(a - b);
            SetFlagsSub8(c, a, b);
            return c;
        }

        public byte Sbc8(byte a, byte b)
        {
            var carry = MathUtil.BoolAsByte(ReadFlagCarry());
            var b2 = (byte)(b + carry);
            var c = (byte)(a - b2);
            SetFlagsSub8(c, a, b, carry);
            return c;
        }

        public byte And8(byte a, byte b)
        {
            var c = (byte)(a & b);
            SetFlagsAnd8(c);
            return c;
        }

        public byte Or8(byte a, byte b)
        {
            var c = (byte)(a | b);
            SetFlagsOr8(c);
            return c;
        }

        public byte Xor8(byte a, byte b)
        {
            var c = (byte)(a ^ b);
            SetFlagsOr8(c);
            return c;
        }

        public void Cp8(byte a, byte b)
        {
            var c = (byte)(a - b);
            SetFlagsSub8(c, a, b);
        }

        public byte Inc8(byte a)
        {
            var c = (byte)(a + 1);
            SetFlagsInc8(c, a);
            return c;
        }

        public ushort Inc16(ushort a)
        {
            var c = (ushort)(a + 1);
            return c;
        }

        public byte Dec8(byte a)
        {
            var c = (byte)(a - 1);
            SetFlagsDec8(c, a);
            return c;
        }

        public ushort Dec16(ushort a)
        {
            var c = (ushort)(a - 1);
            return c;
        }

        public ushort DecSP(ushort a)
        {
            var c = (ushort)(a - 1);
            return c;
        }

        public void Ccf()
        {
            SetFlagSubtraction(false);
            SetFlagHalfCarry(false);
            SetFlagCarry(!ReadFlagCarry());
        }

        public void Scf()
        {
            SetFlagSubtraction(false);
            SetFlagHalfCarry(false);
            SetFlagCarry(true);
        }

        public byte Rr(byte a)
        {
            var carry = ReadFlagCarry();
            SetFlagCarry(MathUtil.IsBitSet(a, 0));
            SetFlagsRotate();
            var c = (byte)((a >> 1) + (MathUtil.BoolAsByte(carry) << 7));
            SetFlagZero(c == 0);
            return c;
        }

        public byte RrA()
        {
            var carry = ReadFlagCarry();
            SetFlagCarry(MathUtil.IsBitSet(ReadA(), 0));
            SetFlagsRotate();
            var c = (byte)((ReadA() >> 1) + (MathUtil.BoolAsByte(carry) << 7));
            return c;
        }

        public byte Rl(byte a)
        {
            var carry = ReadFlagCarry();
            SetFlagCarry(MathUtil.IsBitSet(a, 7));
            SetFlagsRotate();
            var c = (byte)((a << 1) + MathUtil.BoolAsByte(carry));
            SetFlagZero(c == 0);
            return c;
        }

        public byte RlA()
        {
            var carry = ReadFlagCarry();
            SetFlagCarry(MathUtil.IsBitSet(ReadA(), 7));
            SetFlagsRotate();
            var c = (byte)((ReadA() << 1) + MathUtil.BoolAsByte(carry));
            SetFlagZero(false);
            return c;
        }

        public byte Rrc(byte a)
        {
            var carry = MathUtil.IsBitSet(a, 0);
            var bit = MathUtil.BoolAsByte(carry);
            SetFlagCarry(carry);
            SetFlagsRotate();
            var c = (byte)((a >> 1) + (bit << 7));
            SetFlagZero(c == 0);
            return c;
        }

        public byte RrcA()
        {
            var carry = MathUtil.IsBitSet(ReadA(), 0);
            var bit = MathUtil.BoolAsByte(carry);
            SetFlagCarry(carry);
            SetFlagsRotate();
            var c = (byte)((ReadA() >> 1) + (bit << 7));
            SetFlagZero(false);
            return c;
        }

        public byte Rlc(byte a)
        {
            var carry = MathUtil.IsBitSet(a, 7);
            var bit = MathUtil.BoolAsByte(carry);
            SetFlagCarry(carry);
            SetFlagsRotate();
            var c = (byte)((a << 1) + bit);
            SetFlagZero(c == 0);
            return c;
        }

        public byte RlcA()
        {
            var carry = MathUtil.IsBitSet(ReadA(), 7);
            var bit = MathUtil.BoolAsByte(carry);
            SetFlagCarry(carry);
            SetFlagsRotate();
            var c = (byte)((ReadA() << 1) + bit);
            SetFlagZero(false);
            return c;
        }

        public void Cpl()
        {
            WriteA((byte)~ReadA());
            SetFlagHalfCarry(true);
            SetFlagSubtraction(true);
        }

        public void Bit(byte pos, byte a)
        {
            SetFlagZero(!MathUtil.IsBitSet(a, pos));
            SetFlagSubtraction(false);
            SetFlagHalfCarry(true);
        }

        public byte Res(byte pos, byte a)
        {
            return MathUtil.SetOrClearBit(a, false, pos);
        }

        public byte Set(byte pos, byte a)
        {
            return MathUtil.SetOrClearBit(a, true, pos);
        }

        public byte Srl(byte a)
        {
            SetFlagCarry(MathUtil.IsBitSet(a, 0));
            var c = (byte)(a >> 1);
            SetFlagsShift(c);
            return c;
        }

        public byte Sra(byte a)
        {
            var b7 = (byte)(a >> 7);
            SetFlagCarry(MathUtil.IsBitSet(a, 0));
            var c = (byte)((a >> 1) + (b7 << 7));
            SetFlagsShift(c);
            return c;
        }

        public byte Sla(byte a)
        {
            SetFlagCarry(MathUtil.IsBitSet(a, 7));
            var c = (byte)(a << 1);
            SetFlagsShift(c);
            return c;
        }

        public byte Swap(byte a)
        {
            var c = (byte)((a >> 4) + (byte)(a << 4));
            SetFlagCarry(false);
            SetFlagsShift(c);
            return c;
        }

        public void Jpl()
        {
            _pc = ReadHL();
        }

        public void Jp(Conditions c, ushort pc)
        {
            if (EvalCondition(c)) _pc = pc;
            else _pc += 3;
        }

        public void Jr(Conditions c, byte pc)
        {
            if (EvalCondition(c)) _pc = (ushort)(_pc + MathUtil.ToSigned(pc) + 2);
            else _pc += 2;
        }

        public void Push(ushort a)
        {
            _sp -= 1;
            WriteMem8(_sp, (byte)(a >> 8));
            _sp -= 1;
            WriteMem8(_sp, (byte)a);
        }

        public ushort Pop()
        {
            ushort byte1 = ReadMem8(_sp);
            _sp += 1;
            ushort byte2 = ReadMem8(_sp);
            _sp += 1;

            return (ushort)((byte2 << 8) | byte1);
        }

        public void PopAf()
        {
            var byte1 = ReadMem8(_sp);
            _sp += 1;
            var byte2 = ReadMem8(_sp);
            _sp += 1;

            SetFlagZero(MathUtil.IsBitSet(byte1, 7));
            SetFlagSubtraction(MathUtil.IsBitSet(byte1, 6));
            SetFlagHalfCarry(MathUtil.IsBitSet(byte1, 5));
            SetFlagCarry(MathUtil.IsBitSet(byte1, 4));
            WriteA(byte2);
        }

        public void PushAf()
        {
            Push(ReadAF());
        }

        public void Call(Conditions c)
        {
            var nextPc = (ushort)(_pc + 3);
            if (EvalCondition(c))
            {
                Push(nextPc);
                _pc = ReadArg12();
            }
            else
            {
                _pc = nextPc;
            }
        }

        public void Ret(Conditions c)
        {
            if (EvalCondition(c))
            {
                _pc = ReadMem16(ReadSP());
                _sp += 2;
            }
            else
            {
                _pc += 1;
            }
        }

        public void Nop()
        {
        }

        public void Halt()
        {
            if (_ime) _halted = true;
            // TODO
        }

        public void Stop()
        {
            // TODO
        }

        public void Di()
        {
            _ime = false;
        }

        public void Ei()
        {
            _pc++;
            Step();
            _ime = true;
        }

        public void Reti()
        {
            _pc = ReadMem16(ReadSP());
            _sp += 2;
            _ime = true;
        }

        public bool ReadIme()
        {
            return _ime;
        }

        public void Daa()
        {
            var a = ReadA();

            if (ReadFlagSubtraction())
            {
                if (ReadFlagCarry()) a -= 0x60;
                if (ReadFlagHalfCarry()) a -= 0x6;
            }
            else
            {
                if (ReadFlagCarry() || a > 0x99)
                {
                    a += 0x60;
                    SetFlagCarry(true);
                }

                if (ReadFlagHalfCarry() || (a & 0x0f) > 0x09) a += 0x6;
            }

            SetFlagZero(a == 0);
            SetFlagHalfCarry(false);
            WriteA(a);
        }

        public void Rst(ushort address)
        {
            var nextPc = (ushort)(_pc + 1);
            Push(nextPc);
            _pc = address;
        }

        // Setting flags for certain operations

        private void SetFlagsAdd8(byte newValue, byte oldValue, byte opValue)
        {
            SetFlagCarry(newValue < oldValue);
            SetFlagHalfCarry((oldValue & 0xF) + (opValue & 0xF) > 0xF);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsAdc8(byte newValue, byte oldValue, byte opValue, byte carry)
        {
            SetFlagCarry((oldValue & 0xFF) + (opValue & 0xFF) + carry > 0xFF);
            SetFlagHalfCarry((oldValue & 0xF) + (opValue & 0xF) + carry > 0xF);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsAdd16(ushort newValue, ushort oldValue, ushort opValue)
        {
            SetFlagCarry(newValue < oldValue);
            SetFlagHalfCarry((oldValue & 0xF) + (opValue & 0xF) > 0xF);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsAddSP(ushort newValue, ushort oldValue, ushort opValue)
        {
            SetFlagCarry((oldValue & 0xFF) + (opValue & 0xFF) > 0xFF);
            SetFlagHalfCarry((oldValue & 0xF) + (opValue & 0xF) > 0xF);
            SetFlagSubtraction(false);
            SetFlagZero(false);
        }

        private void SetFlagsAddHL(ushort newValue, ushort oldValue, ushort opValue)
        {
            SetFlagCarry(newValue < oldValue);
            SetFlagHalfCarry((oldValue & 0xFFF) + (opValue & 0xFFF) > 0xFFF);
            SetFlagSubtraction(false);
        }

        private void SetFlagsSub8(byte newValue, byte oldValue, byte opValue, byte carry = 0)
        {
            SetFlagCarry(opValue + carry > oldValue);
            SetFlagHalfCarry((oldValue & 0x0F) < (opValue & 0x0F) + carry);
            SetFlagSubtraction(true);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsAnd8(byte newValue)
        {
            SetFlagCarry(false);
            SetFlagHalfCarry(true);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsOr8(byte newValue)
        {
            SetFlagCarry(false);
            SetFlagHalfCarry(false);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsInc8(byte newValue, byte oldValue)
        {
            SetFlagHalfCarry((oldValue & 0xF) + (1 & 0xF) > 0xF);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsDec8(byte newValue, byte oldValue)
        {
            SetFlagHalfCarry((oldValue & 0x0F) < (1 & 0x0F));
            SetFlagSubtraction(true);
            SetFlagZero(newValue == 0);
        }

        private void SetFlagsRotate()
        {
            SetFlagHalfCarry(false);
            SetFlagSubtraction(false);
            SetFlagZero(false);
        }

        private void SetFlagsShift(byte newValue)
        {
            SetFlagHalfCarry(false);
            SetFlagSubtraction(false);
            SetFlagZero(newValue == 0);
        }

        // Util
        public byte ReadArg1()
        {
            return MemoryManager.Memory.Read((ushort)(_pc + 1));
        }

        public byte ReadArg2()
        {
            return MemoryManager.Memory.Read((ushort)(_pc + 2));
        }

        public ushort ReadArg12()
        {
            return MathUtil.BytesToU16(ReadArg1(), ReadArg2());
        }

        public byte ReadMem8(ushort address)
        {
            return MemoryManager.Memory.Read(address);
        }

        public ushort ReadMem16(ushort address)
        {
            return MathUtil.BytesToU16(ReadMem8(address), ReadMem8((ushort)(address + 1)));
        }

        public void WriteMem8(ushort address, byte value)
        {
            MemoryManager.Memory.Write(address, value);
        }

        public void WriteMem16(ushort address, ushort value)
        {
            MemoryManager.Memory.Write(address, (byte)value);
            MemoryManager.Memory.Write((ushort)(address + 1), (byte)(value >> 8));
        }

        public bool EvalCondition(Conditions c)
        {
            switch (c)
            {
                case Conditions.Z:
                    return ReadFlagZero();
                case Conditions.NZ:
                    return !ReadFlagZero();
                case Conditions.C:
                    return ReadFlagCarry();
                case Conditions.NC:
                    return !ReadFlagCarry();
                case Conditions.A:
                    return true;
            }

            return false;
        }

        private bool DoInterrupts()
        {
            // Interrupts
            if (_ime)
            {
                var _ie = ReadMem8(0xFFFF);
                var _if = ReadMem8(0xFF0F);

                // VBlank
                if (MathUtil.IsBitSet(_ie, 0) && MathUtil.IsBitSet(_if, 0))
                {
                    _halted = false;
                    _ime = false;
                    WriteMem8(0xFF0F, MathUtil.ClearBit(_if, 0));
                    Push(_pc);
                    _pc = 0x40;
                    return true;
                }

                // LCD
                if (MathUtil.IsBitSet(_ie, 1) && MathUtil.IsBitSet(_if, 1))
                {
                    _halted = false;
                    _ime = false;
                    WriteMem8(0xFF0F, MathUtil.ClearBit(_if, 1));
                    Push(_pc);
                    _pc = 0x48;
                    return true;
                }

                // Timer
                if (MathUtil.IsBitSet(_ie, 2) && MathUtil.IsBitSet(_if, 2))
                {
                    _halted = false;
                    _ime = false;
                    WriteMem8(0xFF0F, MathUtil.ClearBit(_if, 2));
                    Push(_pc);
                    _pc = 0x50;
                    return true;
                }

                // Serial
                if (MathUtil.IsBitSet(_ie, 3) && MathUtil.IsBitSet(_if, 3))
                {
                    _halted = false;
                    _ime = false;
                    WriteMem8(0xFF0F, MathUtil.ClearBit(_if, 3));
                    Push(_pc);
                    _pc = 0x58;
                    return true;
                }

                // Joystick
                if (MathUtil.IsBitSet(_ie, 4) && MathUtil.IsBitSet(_if, 4))
                {
                    _halted = false;
                    _ime = false;
                    WriteMem8(0xFF0F, MathUtil.ClearBit(_if, 4));
                    Push(_pc);
                    _pc = 0x60;
                    return true;
                }
            }

            return false;
        }

        // Main CPU Function
        public bool Step()
        {
            if (DoInterrupts()) return true;

            if (_halted) return true;

            var instructionByte = MemoryManager.Memory.Read(_pc);

            var instruction = instructionByte == 0xCB
                ? Instructions.FromPrefixByte(MemoryManager.Memory.Read((ushort)(_pc + 1)))
                : Instructions.FromByte(instructionByte);

            if (instruction != null)
            {
                _pc = instruction.Invoke(this);
                return true;
            }

            if (instructionByte == 0xCB)
                MainWindow.Instance.Error("Instruction", $"Prefix Instruction Unknown: 0x{MemoryManager.Memory.Read((ushort)(_pc + 1)):X} at 0x{_pc + 1:X}");
            else
                MainWindow.Instance.Error("Instruction", $"Instruction Unknown: 0x{instructionByte:X} at 0x{_pc:X}");
            return false;
        }
    }
}