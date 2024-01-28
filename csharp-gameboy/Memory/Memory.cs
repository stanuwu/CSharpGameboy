using System;
using System.Collections.Generic;
using csharp_gameboy.Util;

namespace csharp_gameboy.Memory
{
    public class Memory
    {
        private const ushort VRam = 0x8000;
        private const ushort TileMapSize = 0x1800;

        private readonly byte[] _bootRom;

        private readonly byte[] _memory;
        private readonly MemoryManager _memoryManager;

        private readonly Dictionary<byte, byte[]> _ramBanks;

        private readonly byte[,,] _tileSetCache;
        private byte _bankingMode;
        private bool _boot = true;

        private byte _currentRamBank;
        private ushort _currentRomBank;
        private bool _mbcRamProtected = true;

        private byte _mbcType;

        private long _ramSize;
        private long _romSize;

        public Memory(MemoryManager memoryManager, IO io)
        {
            _memoryManager = memoryManager;
            IO = io;
            _memory = new byte[0xFFFF];
            _bootRom = new byte[0x4000];
            _ramBanks = new Dictionary<byte, byte[]>
            {
                { 0, new byte[4096] },
                { 1, new byte[4096] },
                { 2, new byte[4096] },
                { 3, new byte[4096] },
                { 4, new byte[4096] },
                { 5, new byte[4096] },
                { 6, new byte[4096] },
                { 7, new byte[4096] },
                { 8, new byte[4096] },
                { 9, new byte[4096] },
                { 10, new byte[4096] },
                { 11, new byte[4096] },
                { 12, new byte[4096] },
                { 13, new byte[4096] },
                { 14, new byte[4096] },
                { 15, new byte[4096] }
            };
            _tileSetCache = new byte[384, 8, 8];
        }

        private IO IO { get; }

        public void SwitchRamBank(byte bank)
        {
            if (_ramSize >= 8192) _currentRamBank = bank;
        }

        public void SwitchRomBankLower(byte bank)
        {
            // This is a very bad way of doing this but im just a bit too lazy to figure out a better way
            _currentRomBank = MathUtil.ClearBit(_currentRomBank, 0);
            _currentRomBank = MathUtil.ClearBit(_currentRomBank, 1);
            _currentRomBank = MathUtil.ClearBit(_currentRomBank, 2);
            _currentRomBank = MathUtil.ClearBit(_currentRomBank, 3);
            _currentRomBank = MathUtil.ClearBit(_currentRomBank, 4);
            bank = MathUtil.ClearBit(bank, 5);
            bank = MathUtil.ClearBit(bank, 6);
            bank = MathUtil.ClearBit(bank, 7);
            _currentRomBank += bank;
            _memoryManager.LoadRomBank(_currentRomBank);
        }

        public void SwitchRomBankUpper(byte bank)
        {
            // Also not good code!!
            if (_romSize >= 1024)
            {
                _currentRomBank = MathUtil.ClearBit(_currentRomBank, 5);
                _currentRomBank = MathUtil.ClearBit(_currentRomBank, 6);
                bank = MathUtil.ClearBit(bank, 2);
                bank = MathUtil.ClearBit(bank, 3);
                bank = MathUtil.ClearBit(bank, 4);
                bank = MathUtil.ClearBit(bank, 5);
                bank = MathUtil.ClearBit(bank, 6);
                bank = MathUtil.ClearBit(bank, 7);
                _currentRomBank += (byte)(bank << 5);
                _memoryManager.LoadRomBank(_currentRomBank);
            }
        }

        public void SwitchRomBankFull(byte bank)
        {
            _currentRomBank = bank;
            _memoryManager.LoadRomBank(_currentRomBank);
        }

        public void SwitchRomBankFullLower(byte bank)
        {
            var bit8 = MathUtil.IsBitSet(_currentRomBank, 8);
            _currentRomBank = bank;
            SwitchRomBankFullUpper(MathUtil.BoolAsByte(bit8));
            _memoryManager.LoadRomBank(_currentRomBank);
        }

        public void SwitchRomBankFullUpper(byte bank)
        {
            MathUtil.SetOrClearBit(_currentRomBank, MathUtil.IsBitSet(bank, 0), 8);
            _memoryManager.LoadRomBank(_currentRomBank);
        }

        public void HandleMbcBootup()
        {
            switch (_mbcType)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 5:
                    SwitchRamBank(0);
                    SwitchRomBankLower(1);
                    break;
            }
        }

        public byte ReadFromRomBank(ushort address)
        {
            return _memoryManager.ReadRomBank(address);
        }

        public void HandleMbcWrite(ushort address, byte data)
        {
            switch (_mbcType)
            {
                // no mbc
                case 0:
                    break;

                // mbc1
                case 1:
                    if (address < 0x2000)
                    {
                        if (data << 4 == 0xA0) EnableMbcRam();
                        else DisableMbcRam();
                    }
                    else if (address < 0x4000)
                    {
                        SwitchRomBankLower(data);
                    }
                    else if (address < 0x6000)
                    {
                        if (_bankingMode > 0)
                            SwitchRamBank(data);
                        else
                            SwitchRomBankUpper(data);
                    }
                    else if (address < 0x8000)
                    {
                        _bankingMode = (byte)(MathUtil.IsBitSet(data, 0) ? 1 : 0);
                    }

                    break;

                // mbc2
                case 2:
                    if (address < 0x4000)
                    {
                        if (MathUtil.IsBitSet((byte)(address >> 8), 0))
                        {
                            SwitchRomBankLower(MathUtil.ClearBit(data, 4));
                        }
                        else
                        {
                            if (data << 4 == 0xA0) EnableMbcRam();
                            else DisableMbcRam();
                        }
                    }

                    break;

                // mbc3
                case 3:
                    if (address < 0x2000)
                    {
                        if (data << 4 == 0xA0) EnableMbcRam();
                        else DisableMbcRam();
                    }
                    else if (address < 0x4000)
                    {
                        data = MathUtil.ClearBit(data, 7);
                        SwitchRomBankFull(data);
                    }
                    else if (address < 0x6000)
                    {
                        if (data <= 0x03) SwitchRamBank(data);
                    }

                    break;

                // mbc5
                case 5:
                    if (address < 0x2000)
                    {
                        if (data << 4 == 0xA0) EnableMbcRam();
                        else DisableMbcRam();
                    }
                    else if (address < 0x3000)
                    {
                        SwitchRomBankFullLower(data);
                    }
                    else if (address < 0x4000)
                    {
                        SwitchRomBankFullUpper(data);
                    }
                    else if (address < 0x6000)
                    {
                        SwitchRamBank(data);
                    }

                    break;

                // not doing other mbcs for now
            }
        }

        public void EnableMbcRam()
        {
            _mbcRamProtected = false;
        }

        public void DisableMbcRam()
        {
            _mbcRamProtected = true;
        }

        public void SetRomSize(byte size)
        {
            _romSize = (long)(16384 * Math.Pow(2, size + 1));
        }

        public void SetRamSize(byte size)
        {
            switch (size)
            {
                case 0:
                    _ramSize = 0;
                    break;
                case 1:
                    _ramSize = -1;
                    break;
                case 2:
                    _ramSize = 8192;
                    break;
                case 3:
                    _ramSize = 32768;
                    break;
                case 4:
                    _ramSize = 131072;
                    break;
                case 5:
                    _ramSize = 65536;
                    break;
            }
        }

        public void SetMbcType(byte type)
        {
            switch (type)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                    _mbcType = 1;
                    break;
                case 0x05:
                case 0x06:
                    _mbcType = 2;
                    break;
                case 0x0F:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                    _mbcType = 3;
                    break;
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                    _mbcType = 5;
                    break;
                case 0x20:
                    _mbcType = 6;
                    break;
                case 0x22:
                    _mbcType = 7;
                    break;
                default:
                    _mbcType = 0;
                    break;
            }
        }

        public byte ReadTilePixel(int tile, int x, int y)
        {
            return _tileSetCache[tile, x, y];
        }

        private void CacheTileData(ushort index)
        {
            var normalizedIndex = index & 0xFFFE;
            var byte1 = Read((ushort)(VRam + normalizedIndex));
            var byte2 = Read((ushort)(VRam + normalizedIndex + 1));

            var tileIndex = index / 16;
            var rowIndex = index % 16 / 2;
            for (var pixelIndex = 0; pixelIndex < 8; pixelIndex++)
            {
                var mask = 1 << (7 - pixelIndex);
                var lsb = byte1 & mask;
                var msb = byte2 & mask;
                byte color = 0;
                if (lsb != 0 && msb != 0) color = 3;
                else if (msb != 0) color = 2;
                else if (lsb != 0) color = 1;

                _tileSetCache[tileIndex, rowIndex, pixelIndex] = color;
            }
        }

        public bool IsBooting()
        {
            return _boot;
        }

        public byte Read(ushort address)
        {
            // Read Boot Rom
            if (_boot)
                if (address <= 0xFF)
                    return _bootRom[address];

            // Read Rom Bank
            if (address >= 0x4000 && address < 0x8000) return ReadFromRomBank(address);

            // Read IO
            if (IO.CanHas(address)) return IO.Read(address);

            // Read Ram Bank
            if (address >= 0xA000 && address < 0xC000)
            {
                if (_mbcRamProtected) return 0;
                return _ramBanks[_currentRamBank][address - 0xA000];
            }

            return _memory[address];
        }

        public void WriteRom(ushort address, byte value)
        {
            _memory[address] = value;
        }

        public void Write(ushort address, byte value)
        {
            // Detatch Boot Rom
            if (_boot && address == 0xFF50 && value == 0x01)
            {
                _boot = false;
                MainWindow.Instance.Log("+ Booted");
            }

            // Write IO
            if (IO.CanHas(address))
            {
                IO.Write(address, value);
                return;
            }

            // Write MBC
            if (address < 0x8000) HandleMbcWrite(address, value);

            // Write Ram Bank
            if (address >= 0xA000 && address < 0xC000)
            {
                if (_mbcRamProtected) return;
                _ramBanks[_currentRamBank][address - 0xA000] = value;
                return;
            }

            _memory[address] = value;
            if (address > VRam && address < VRam + TileMapSize) CacheTileData((ushort)(address - VRam));
        }

        public void WriteBootRom(ushort address, byte value)
        {
            _bootRom[address] = value;
        }
    }
}