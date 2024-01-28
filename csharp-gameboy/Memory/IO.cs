using System.Collections.Generic;
using csharp_gameboy.Compute;
using csharp_gameboy.Util;

namespace csharp_gameboy.Memory
{
    public class IO
    {
        private const int CpuHz = 4190000;
        private const int DivHz = 16384;
        private const int ScreenHz = 60;
        private const int Tac00 = 1024;
        private const int Tac01 = 16;
        private const int Tac10 = 64;
        private const int Tac11 = 256;

        private readonly Dictionary<ushort, byte> _hardwareRegisters = new Dictionary<ushort, byte>
        {
            // JOYP
            { 0xFF00, 0x0F },
            // SB
            { 0XFF01, 0 },
            // SC
            { 0XFF02, 0 },
            // DIV
            { 0XFF04, 0 },
            // TIMA
            { 0XFF05, 0 },
            // TMA
            { 0XFF06, 0 },
            // TAC
            { 0XFF07, 0 },
            // IF
            { 0XFF0F, 0 },

            // NR10
            { 0XFF10, 0 },
            // NR11
            { 0XFF11, 0 },
            // NR12
            { 0XFF12, 0 },
            // NR13
            { 0XFF13, 0 },
            // NR14
            { 0XFF14, 0 },
            // NR21
            { 0XFF16, 0 },
            // NR22
            { 0XFF17, 0 },
            // NR23
            { 0XFF18, 0 },
            // NR24
            { 0XFF19, 0 },
            // NR30
            { 0XFF1A, 0 },
            // NR31
            { 0XFF1B, 0 },
            // NR32
            { 0XFF1C, 0 },
            // NR33
            { 0XFF1D, 0 },
            // NR34
            { 0XFF1E, 0 },
            // NR41
            { 0XFF20, 0 },
            // NR42
            { 0XFF21, 0 },
            // NR43
            { 0XFF22, 0 },
            // NR44
            { 0XFF23, 0 },
            // NR50
            { 0XFF24, 0 },
            // NR51
            { 0XFF25, 0 },
            // NR52
            { 0XFF26, 0 },

            // WAVE RAM
            { 0XFF30, 0 },
            { 0XFF31, 0 },
            { 0XFF32, 0 },
            { 0XFF33, 0 },
            { 0XFF34, 0 },
            { 0XFF35, 0 },
            { 0XFF36, 0 },
            { 0XFF37, 0 },
            { 0XFF38, 0 },
            { 0XFF39, 0 },
            { 0XFF3A, 0 },
            { 0XFF3B, 0 },
            { 0XFF3C, 0 },
            { 0XFF3D, 0 },
            { 0XFF3E, 0 },
            { 0XFF3F, 0 },

            // LCDC
            { 0XFF40, 0 },
            // STAT
            { 0XFF41, 0 },
            // SCY
            { 0XFF42, 0 },
            // SCX
            { 0XFF43, 0 },
            // LY
            { 0XFF44, 0 },
            // LYC
            { 0XFF45, 0 },

            // DMA
            { 0XFF46, 0 },

            // BGP
            { 0XFF47, 0 },
            // OBR0
            { 0XFF48, 0 },
            // OBP1
            { 0XFF49, 0 },
            // WY
            { 0XFF4A, 0 },
            // WX
            { 0XFF4B, 0 },

            // PCM12
            { 0XFF76, 0 },
            // PCM34
            { 0XFF77, 0 },

            // IE
            { 0XFFFF, 0 }
        };

        private long _tick;

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0xFF04: // div
                    _hardwareRegisters[address] = 0x00;
                    return;
            }

            if (_hardwareRegisters.ContainsKey(address)) _hardwareRegisters[address] = data;
        }

        public bool CanHas(ushort address)
        {
            return _hardwareRegisters.ContainsKey(address);
        }

        public byte Read(ushort address)
        {
            return _hardwareRegisters[address];
        }

        public void Update(CPU cpu)
        {
            _tick++;

            // DIV
            if (_tick % (CpuHz / DivHz) == 0) _hardwareRegisters[0xFF04]++;

            // TIMA
            var tma = _hardwareRegisters[0xFF06];
            var tac = _hardwareRegisters[0xFF07];
            if (MathUtil.IsBitSet(tac, 2))
            {
                var ms = MathUtil.IsBitSet(tac, 0) ? 1 : 0;
                ms += MathUtil.IsBitSet(tac, 1) ? 2 : 0;

                var mode = 1;
                switch (ms)
                {
                    case 0:
                        mode = Tac00;
                        break;
                    case 1:
                        mode = Tac01;
                        break;
                    case 2:
                        mode = Tac10;
                        break;
                    case 3:
                        mode = Tac11;
                        break;
                }

                var frequency = CpuHz / mode;
                if (_tick % frequency == 0)
                {
                    var nv = _hardwareRegisters[0xFF05] + 1;
                    if (nv > 0xFF)
                    {
                        nv = tma;
                        _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 2);
                    }

                    _hardwareRegisters[0xFF05] = (byte)nv;
                }
            }

            // LYC
            if (_hardwareRegisters[0XFF44] == _hardwareRegisters[0xFF46])
                _hardwareRegisters[0xFF41] = MathUtil.SetBit(_hardwareRegisters[0xFF41], 2);
            else
                _hardwareRegisters[0xFF41] = MathUtil.ClearBit(_hardwareRegisters[0xFF41], 2);

            // OAM DMA
            if (_hardwareRegisters[0xFF46] != 0x00)
            {
                var source = _hardwareRegisters[0xFF46] * 0x100;
                var start = 0xFE00;
                for (var i = 0; i <= 0xDF; i++) cpu.MemoryManager.Memory.Write((ushort)(start + i), cpu.ReadMem8((ushort)(source + i)));

                _hardwareRegisters[0xFF46] = 0;
            }

            // Temporary Display Spoof
            if (_tick % (CpuHz / (ScreenHz * 144)) == 0)
            {
                var newY = _hardwareRegisters[0xFF44] + 1;
                _hardwareRegisters[0xFF44] = (byte)newY;
                if (newY > 153) _hardwareRegisters[0xFF44] = 0;
            }

            // VBlank Interrupt
            if (_hardwareRegisters[0xFF44] == 153) _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 0);

            // Gameboy Doctor
            /*
            _hardwareRegisters[0xFF44] = 0x90;
            */

            // Joypad Input
            var joypad = _hardwareRegisters[0xFF00];
            var joypadInterrupt = _hardwareRegisters[0xFF00];
            if (MathUtil.IsBitSet(joypad, 4))
            {
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyE, 3);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyQ, 2);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyDown, 1);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyRight, 0);
                _hardwareRegisters[0xFF00] = joypad;
            }
            else if (MathUtil.IsBitSet(joypad, 5))
            {
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyS, 3);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyW, 2);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyA, 1);
                joypad = MathUtil.SetOrClearBit(joypad, !MainWindow.Instance.KeyD, 0);
                _hardwareRegisters[0xFF00] = joypad;
            }
            else
            {
                _hardwareRegisters[0xFF00] = 0x0F;
            }

            if (
                (MathUtil.IsBitSet(joypadInterrupt, 0) && !MathUtil.IsBitSet(joypad, 0)) ||
                (MathUtil.IsBitSet(joypadInterrupt, 1) && !MathUtil.IsBitSet(joypad, 1)) ||
                (MathUtil.IsBitSet(joypadInterrupt, 2) && !MathUtil.IsBitSet(joypad, 2)) ||
                (MathUtil.IsBitSet(joypadInterrupt, 3) && !MathUtil.IsBitSet(joypad, 3))
            )
                _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 4);

            // Stat Interrupt
            _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], _hardwareRegisters[0XFF44] == _hardwareRegisters[0XFF45], 2);
            if (MathUtil.IsBitSet(_hardwareRegisters[0XFF41], 3))
            {
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], false, 0);
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], false, 1);
                _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 1);
            }
            else if (MathUtil.IsBitSet(_hardwareRegisters[0XFF41], 4))
            {
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], true, 0);
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], false, 1);
                _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 1);
            }
            else if (MathUtil.IsBitSet(_hardwareRegisters[0XFF41], 5))
            {
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], false, 0);
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], true, 1);
                _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 1);
            }
            else if (MathUtil.IsBitSet(_hardwareRegisters[0XFF41], 6) && _hardwareRegisters[0XFF44] == _hardwareRegisters[0XFF45])
            {
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], true, 0);
                _hardwareRegisters[0XFF41] = MathUtil.SetOrClearBit(_hardwareRegisters[0XFF41], true, 1);
                _hardwareRegisters[0xFF0F] = MathUtil.SetBit(_hardwareRegisters[0xFF0F], 1);
            }
        }
    }
}