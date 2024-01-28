using System;

namespace csharp_gameboy.Memory
{
    public class MemoryManager
    {
        private const int RomSize = 0x800000;
        private const int RomBankSize = 0x4000;
        private const int Bank0Start = 0x0000;
        private const int Bank0End = 0x3FFF;
        private const int BankNStart = 0x4000;
        private const int BankNEnd = 0x7FFF;

        private readonly byte[] _rom = new byte[RomSize];
        private int LoadedBank;

        public MemoryManager()
        {
            IO = new IO();
            Memory = new Memory(this, IO);
        }

        public IO IO { get; }

        public Memory Memory { get; }
        public int RealRomSize { get; private set; }

        public void LoadRom(byte[] rom)
        {
            RealRomSize = Math.Min(rom.Length, RomSize);
            for (var i = 0; i < RealRomSize; i++) _rom[i] = rom[i];
            for (ushort i = Bank0Start; i <= Bank0End; i++) Memory.WriteRom(i, _rom[i]);
            Memory.SetMbcType(rom[0x0147]);
            Memory.SetRomSize(rom[0x0148]);
            Memory.SetRamSize(rom[0x0149]);
            Memory.HandleMbcBootup();
        }

        public void LoadBootRom(byte[] rom)
        {
            for (ushort i = 0; i <= 0xFF; i++) Memory.WriteBootRom(i, rom[i]);
        }

        public byte ReadRomBank(ushort address)
        {
            var add2 = address + RomBankSize * (LoadedBank - 1);
            return _rom[add2];
        }

        public void LoadRomBank(int bank)
        {
            if (LoadedBank == bank) return;
            if (bank == 0) bank = 1;
            LoadedBank = bank;
        }
    }
}