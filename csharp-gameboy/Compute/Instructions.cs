using System;
using System.Collections.Generic;
using csharp_gameboy.Util;

namespace csharp_gameboy.Compute
{
    public class Instructions
    {
        private static readonly Dictionary<byte, Instruction> _instructions = new Dictionary<byte, Instruction>
        {
            // NOP
            {
                0x00, new Instruction("NOP", cpu => { cpu.Nop(); })
            },

            // STOP
            {
                0x10, new Instruction("STOP", cpu => { cpu.Stop(); })
            },

            // LD
            {
                0x01, new Instruction("LD BC,nn", cpu => { cpu.WriteBC(cpu.ReadArg12()); }, 3)
            },
            {
                0x02, new Instruction("LD (BC),A", cpu => { cpu.WriteMem8(cpu.ReadBC(), cpu.ReadA()); })
            },
            {
                0x06, new Instruction("LD B,n", cpu => { cpu.WriteB(cpu.ReadArg1()); }, 2)
            },
            {
                0x08, new Instruction("LD (nn),SP", cpu => { cpu.WriteMem16(cpu.ReadArg12(), cpu.ReadSP()); }, 3)
            },
            {
                0x0A, new Instruction("LD A,(BC)", cpu => { cpu.WriteA(cpu.ReadMem8(cpu.ReadBC())); })
            },
            {
                0x0E, new Instruction("LD C,n", cpu => { cpu.WriteC(cpu.ReadArg1()); }, 2)
            },
            {
                0x11, new Instruction("LD DE,nn", cpu => { cpu.WriteDE(cpu.ReadArg12()); }, 3)
            },
            {
                0x12, new Instruction("LD (DE),A", cpu => { cpu.WriteMem8(cpu.ReadDE(), cpu.ReadA()); })
            },
            {
                0x16, new Instruction("LD D,n", cpu => { cpu.WriteD(cpu.ReadArg1()); }, 2)
            },
            {
                0x1A, new Instruction("LD A,(DE)", cpu => { cpu.WriteA(cpu.ReadMem8(cpu.ReadDE())); })
            },
            {
                0x1E, new Instruction("LD E,n", cpu => { cpu.WriteE(cpu.ReadArg1()); }, 2)
            },
            {
                0x21, new Instruction("LD HL,nn", cpu => { cpu.WriteHL(cpu.ReadArg12()); }, 3)
            },
            {
                0x22, new Instruction("LDI (HL),A", cpu =>
                {
                    cpu.WriteMem8(cpu.ReadHL(), cpu.ReadA());
                    IncHL(cpu);
                })
            },
            {
                0x26, new Instruction("LD H,n", cpu => { cpu.WriteH(cpu.ReadArg1()); }, 2)
            },
            {
                0x2A, new Instruction("LDI A,(HL)", cpu =>
                {
                    cpu.WriteA(cpu.ReadMem8(cpu.ReadHL()));
                    IncHL(cpu);
                })
            },
            {
                0x2E, new Instruction("LD L,n", cpu => { cpu.WriteL(cpu.ReadArg1()); }, 2)
            },
            {
                0x31, new Instruction("LD SP,nn", cpu => { cpu.WriteSP(cpu.ReadArg12()); }, 3)
            },
            {
                0x32, new Instruction("LDD (HL),A", cpu =>
                {
                    cpu.WriteMem8(cpu.ReadHL(), cpu.ReadA());
                    DecHL(cpu);
                })
            },
            {
                0x36, new Instruction("LD (HL),n", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadArg1()); }, 2)
            },
            {
                0x3A, new Instruction("LDD A,(HL)", cpu =>
                {
                    cpu.WriteA(cpu.ReadMem8(cpu.ReadHL()));
                    DecHL(cpu);
                })
            },
            {
                0x3E, new Instruction("LD A,n", cpu => { cpu.WriteA(cpu.ReadArg1()); }, 2)
            },
            {
                0x40, new Instruction("LD B,B", cpu => { cpu.WriteB(cpu.ReadB()); })
            },
            {
                0x41, new Instruction("LD B,C", cpu => { cpu.WriteB(cpu.ReadC()); })
            },
            {
                0x42, new Instruction("LD B,D", cpu => { cpu.WriteB(cpu.ReadD()); })
            },
            {
                0x43, new Instruction("LD B,E", cpu => { cpu.WriteB(cpu.ReadE()); })
            },
            {
                0x44, new Instruction("LD B,H", cpu => { cpu.WriteB(cpu.ReadH()); })
            },
            {
                0x45, new Instruction("LD B,L", cpu => { cpu.WriteB(cpu.ReadL()); })
            },
            {
                0x46, new Instruction("LD B,(HL)", cpu => { cpu.WriteB(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x47, new Instruction("LD B,A", cpu => { cpu.WriteB(cpu.ReadA()); })
            },
            {
                0x48, new Instruction("LD C,B", cpu => { cpu.WriteC(cpu.ReadB()); })
            },
            {
                0x49, new Instruction("LD C,C", cpu => { cpu.WriteC(cpu.ReadC()); })
            },
            {
                0x4A, new Instruction("LD C,D", cpu => { cpu.WriteC(cpu.ReadD()); })
            },
            {
                0x4B, new Instruction("LD C,E", cpu => { cpu.WriteC(cpu.ReadE()); })
            },
            {
                0x4C, new Instruction("LD C,H", cpu => { cpu.WriteC(cpu.ReadH()); })
            },
            {
                0x4D, new Instruction("LD C,L", cpu => { cpu.WriteC(cpu.ReadL()); })
            },
            {
                0x4E, new Instruction("LD C,(HL)", cpu => { cpu.WriteC(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x4F, new Instruction("LD C,A", cpu => { cpu.WriteC(cpu.ReadA()); })
            },
            {
                0x50, new Instruction("LD D,B", cpu => { cpu.WriteD(cpu.ReadB()); })
            },
            {
                0x51, new Instruction("LD D,C", cpu => { cpu.WriteD(cpu.ReadC()); })
            },
            {
                0x52, new Instruction("LD D,D", cpu => { cpu.WriteD(cpu.ReadD()); })
            },
            {
                0x53, new Instruction("LD D,E", cpu => { cpu.WriteD(cpu.ReadE()); })
            },
            {
                0x54, new Instruction("LD D,H", cpu => { cpu.WriteD(cpu.ReadH()); })
            },
            {
                0x55, new Instruction("LD D,L", cpu => { cpu.WriteD(cpu.ReadL()); })
            },
            {
                0x56, new Instruction("LD D,(HL)", cpu => { cpu.WriteD(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x57, new Instruction("LD D,A", cpu => { cpu.WriteD(cpu.ReadA()); })
            },
            {
                0x58, new Instruction("LD E,B", cpu => { cpu.WriteE(cpu.ReadB()); })
            },
            {
                0x59, new Instruction("LD E,C", cpu => { cpu.WriteE(cpu.ReadC()); })
            },
            {
                0x5A, new Instruction("LD E,D", cpu => { cpu.WriteE(cpu.ReadD()); })
            },
            {
                0x5B, new Instruction("LD E,E", cpu => { cpu.WriteE(cpu.ReadE()); })
            },
            {
                0x5C, new Instruction("LD E,H", cpu => { cpu.WriteE(cpu.ReadH()); })
            },
            {
                0x5D, new Instruction("LD E,L", cpu => { cpu.WriteE(cpu.ReadL()); })
            },
            {
                0x5E, new Instruction("LD E,(HL)", cpu => { cpu.WriteE(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x5F, new Instruction("LD E,A", cpu => { cpu.WriteE(cpu.ReadA()); })
            },
            {
                0x60, new Instruction("LD H,B", cpu => { cpu.WriteH(cpu.ReadB()); })
            },
            {
                0x61, new Instruction("LD H,C", cpu => { cpu.WriteH(cpu.ReadC()); })
            },
            {
                0x62, new Instruction("LD H,D", cpu => { cpu.WriteH(cpu.ReadD()); })
            },
            {
                0x63, new Instruction("LD H,E", cpu => { cpu.WriteH(cpu.ReadE()); })
            },
            {
                0x64, new Instruction("LD H,H", cpu => { cpu.WriteH(cpu.ReadH()); })
            },
            {
                0x65, new Instruction("LD H,L", cpu => { cpu.WriteH(cpu.ReadL()); })
            },
            {
                0x66, new Instruction("LD H,(HL)", cpu => { cpu.WriteH(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x67, new Instruction("LD H,A", cpu => { cpu.WriteH(cpu.ReadA()); })
            },
            {
                0x68, new Instruction("LD L,B", cpu => { cpu.WriteL(cpu.ReadB()); })
            },
            {
                0x69, new Instruction("LD L,C", cpu => { cpu.WriteL(cpu.ReadC()); })
            },
            {
                0x6A, new Instruction("LD L,D", cpu => { cpu.WriteL(cpu.ReadD()); })
            },
            {
                0x6B, new Instruction("LD L,E", cpu => { cpu.WriteL(cpu.ReadE()); })
            },
            {
                0x6C, new Instruction("LD L,H", cpu => { cpu.WriteL(cpu.ReadH()); })
            },
            {
                0x6D, new Instruction("LD L,L", cpu => { cpu.WriteL(cpu.ReadL()); })
            },
            {
                0x6E, new Instruction("LD L,(HL)", cpu => { cpu.WriteL(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x6F, new Instruction("LD L,A", cpu => { cpu.WriteL(cpu.ReadA()); })
            },
            {
                0x70, new Instruction("LD (HL),B", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadB()); })
            },
            {
                0x71, new Instruction("LD (HL),C", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadC()); })
            },
            {
                0x72, new Instruction("LD (HL),D", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadD()); })
            },
            {
                0x73, new Instruction("LD (HL),E", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadE()); })
            },
            {
                0x74, new Instruction("LD (HL),H", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadH()); })
            },
            {
                0x75, new Instruction("LD (HL),L", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadL()); })
            },
            {
                0x77, new Instruction("LD (HL),A", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.ReadA()); })
            },
            {
                0x78, new Instruction("LD A,B", cpu => { cpu.WriteA(cpu.ReadB()); })
            },
            {
                0x79, new Instruction("LD A,C", cpu => { cpu.WriteA(cpu.ReadC()); })
            },
            {
                0x7A, new Instruction("LD A,D", cpu => { cpu.WriteA(cpu.ReadD()); })
            },
            {
                0x7B, new Instruction("LD A,E", cpu => { cpu.WriteA(cpu.ReadE()); })
            },
            {
                0x7C, new Instruction("LD A,H", cpu => { cpu.WriteA(cpu.ReadH()); })
            },
            {
                0x7D, new Instruction("LD A,L", cpu => { cpu.WriteA(cpu.ReadL()); })
            },
            {
                0x7E, new Instruction("LD A,(HL)", cpu => { cpu.WriteA(cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0x7F, new Instruction("LD A,A", cpu => { cpu.WriteA(cpu.ReadA()); })
            },
            {
                0xE0, new Instruction("LDH (n),A", cpu =>
                {
                    cpu.WriteMem8(
                        (ushort)(cpu.ReadArg1() + 0xFF00),
                        cpu.ReadA()
                    );
                }, 2)
            },
            {
                0xE2, new Instruction("LDH (C),A", cpu =>
                {
                    cpu.WriteMem8(
                        (ushort)(cpu.ReadC() + 0xFF00),
                        cpu.ReadA()
                    );
                })
            },
            {
                0xEA, new Instruction("LD (nn),A", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadArg12(),
                        cpu.ReadA()
                    );
                }, 3)
            },
            {
                0xF0, new Instruction("LDH A,(n)", cpu =>
                {
                    cpu.WriteA(
                        cpu.ReadMem8(
                            (ushort)(cpu.ReadArg1() + 0xFF00)
                        )
                    );
                }, 2)
            },
            {
                0xF2, new Instruction("LDH (C),A", cpu => { cpu.WriteA(cpu.ReadMem8((ushort)(cpu.ReadC() + 0xFF00))); })
            },
            {
                0xF8, new Instruction("LDHL SP,d", cpu =>
                {
                    var value = cpu.Add16s(cpu.ReadSP(), MathUtil.ToSigned(cpu.ReadArg1()));
                    cpu.SetFlagZero(false);
                    cpu.WriteHL(value);
                }, 2)
            },
            {
                0xF9, new Instruction("LD SP,HL", cpu => { cpu.WriteSP(cpu.ReadHL()); })
            },
            {
                0xFA, new Instruction("LD A,(nn)", cpu => { cpu.WriteA(cpu.ReadMem8(cpu.ReadArg12())); }, 3)
            },

            // INC
            {
                0x03, new Instruction("INC BC", cpu => { cpu.WriteBC(cpu.Inc16(cpu.ReadBC())); })
            },
            {
                0x04, new Instruction("INC B", cpu => { cpu.WriteB(cpu.Inc8(cpu.ReadB())); })
            },
            {
                0x0C, new Instruction("INC C", cpu => { cpu.WriteC(cpu.Inc8(cpu.ReadC())); })
            },
            {
                0x13, new Instruction("INC DE", cpu => { cpu.WriteDE(cpu.Inc16(cpu.ReadDE())); })
            },
            {
                0x14, new Instruction("INC D", cpu => { cpu.WriteD(cpu.Inc8(cpu.ReadD())); })
            },
            {
                0x1C, new Instruction("INC E", cpu => { cpu.WriteE(cpu.Inc8(cpu.ReadE())); })
            },
            {
                0x23, new Instruction("INC HL", cpu => { cpu.WriteHL(cpu.Inc16(cpu.ReadHL())); })
            },
            {
                0x24, new Instruction("INC H", cpu => { cpu.WriteH(cpu.Inc8(cpu.ReadH())); })
            },
            {
                0x2C, new Instruction("INC L", cpu => { cpu.WriteL(cpu.Inc8(cpu.ReadL())); })
            },
            {
                0x33, new Instruction("INC SP", cpu => { cpu.WriteSP(cpu.Inc16(cpu.ReadSP())); })
            },
            {
                0x34, new Instruction("INC (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Inc8(
                            cpu.ReadMem8(cpu.ReadHL()
                            )
                        )
                    );
                })
            },
            {
                0x3C, new Instruction("INC A", cpu => { cpu.WriteA(cpu.Inc8(cpu.ReadA())); })
            },

            // DEC
            {
                0x05, new Instruction("DEC B", cpu => { cpu.WriteB(cpu.Dec8(cpu.ReadB())); })
            },
            {
                0x0B, new Instruction("DEC BC", cpu => { cpu.WriteBC(cpu.Dec16(cpu.ReadBC())); })
            },
            {
                0x0D, new Instruction("DEC C", cpu => { cpu.WriteC(cpu.Dec8(cpu.ReadC())); })
            },
            {
                0x15, new Instruction("DEC D", cpu => { cpu.WriteD(cpu.Dec8(cpu.ReadD())); })
            },
            {
                0x1B, new Instruction("DEC DE", cpu => { cpu.WriteDE(cpu.Dec16(cpu.ReadDE())); })
            },
            {
                0x1D, new Instruction("DEC E", cpu => { cpu.WriteE(cpu.Dec8(cpu.ReadE())); })
            },
            {
                0x25, new Instruction("DEC H", cpu => { cpu.WriteH(cpu.Dec8(cpu.ReadH())); })
            },
            {
                0x2B, new Instruction("DEC HL", cpu => { cpu.WriteHL(cpu.Dec16(cpu.ReadHL())); })
            },
            {
                0x2D, new Instruction("DEC L", cpu => { cpu.WriteL(cpu.Dec8(cpu.ReadL())); })
            },
            {
                0x35, new Instruction("DEC (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Dec8(
                            cpu.ReadMem8(cpu.ReadHL()
                            )
                        )
                    );
                })
            },
            {
                0x3B, new Instruction("DEC SP", cpu => { cpu.WriteSP(cpu.DecSP(cpu.ReadSP())); })
            },
            {
                0x3D, new Instruction("DEC A", cpu => { cpu.WriteA(cpu.Dec8(cpu.ReadA())); })
            },

            // RLC
            {
                0x07, new Instruction("RLC A", cpu => { cpu.WriteA(cpu.RlcA()); })
            },

            // ADD
            {
                0x09, new Instruction("ADD HL,BC", cpu =>
                {
                    cpu.WriteHL(
                        cpu.AddHL(cpu.ReadHL(), cpu.ReadBC())
                    );
                })
            },
            {
                0x19, new Instruction("ADD HL,DE", cpu =>
                {
                    cpu.WriteHL(
                        cpu.AddHL(cpu.ReadHL(), cpu.ReadDE())
                    );
                })
            },
            {
                0x29, new Instruction("ADD HL,HL", cpu =>
                {
                    cpu.WriteHL(
                        cpu.AddHL(cpu.ReadHL(), cpu.ReadHL())
                    );
                })
            },
            {
                0x39, new Instruction("ADD HL,SP", cpu =>
                {
                    cpu.WriteHL(
                        cpu.AddHL(cpu.ReadHL(), cpu.ReadSP())
                    );
                })
            },
            {
                0x80, new Instruction("ADD A,B", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadB())
                    );
                })
            },
            {
                0x81, new Instruction("ADD A,C", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadC())
                    );
                })
            },
            {
                0x82, new Instruction("ADD A,D", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadD())
                    );
                })
            },
            {
                0x83, new Instruction("ADD A,E", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadE())
                    );
                })
            },
            {
                0x84, new Instruction("ADD A,H", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadH())
                    );
                })
            },
            {
                0x85, new Instruction("ADD A,L", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadL())
                    );
                })
            },
            {
                0x86, new Instruction("ADD A,(HL)", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))
                    );
                })
            },
            {
                0x87, new Instruction("ADD A,A", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadA())
                    );
                })
            },
            {
                0xC6, new Instruction("ADD A,n", cpu =>
                {
                    cpu.WriteA(
                        cpu.Add8(cpu.ReadA(), cpu.ReadArg1())
                    );
                }, 2)
            },
            {
                0xE8, new Instruction("ADD SP,d", cpu =>
                {
                    cpu.WriteSP(
                        cpu.Add16s(cpu.ReadSP(), MathUtil.ToSigned(cpu.ReadArg1()))
                    );
                }, 2)
            },

            // RRC
            {
                0x0F, new Instruction("RRC A", cpu => { cpu.WriteA(cpu.RrcA()); })
            },

            // RL
            {
                0x17, new Instruction("RL A", cpu => { cpu.WriteA(cpu.RlA()); })
            },

            // RR
            {
                0x1F, new Instruction("RR A", cpu => { cpu.WriteA(cpu.RrA()); })
            },

            // DAA
            {
                0x27, new Instruction("DAA", cpu => { cpu.Daa(); })
            },

            // CPL
            {
                0x2F, new Instruction("CPL", cpu => { cpu.Cpl(); })
            },

            // SCF
            {
                0x37, new Instruction("SCF", cpu => { cpu.Scf(); })
            },

            // CCF
            {
                0x3F, new Instruction("CCF", cpu => { cpu.Ccf(); })
            },

            // Halt
            {
                0x76, new Instruction("HALT", cpu => { cpu.Halt(); })
            },

            // ADC
            {
                0x88, new Instruction("ADC A,B", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0x89, new Instruction("ADC A,C", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0x8A, new Instruction("ADC A,D", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0x8B, new Instruction("ADC A,E", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0x8C, new Instruction("ADC A,H", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0x8D, new Instruction("ADC A,L", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0x8E, new Instruction("ADC A,(HL)", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0x8F, new Instruction("ADC A,A", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xCE, new Instruction("ADC A,n", cpu => { cpu.WriteA(cpu.Adc8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // SUB
            {
                0x90, new Instruction("SUB A,B", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0x91, new Instruction("SUB A,C", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0x92, new Instruction("SUB A,D", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0x93, new Instruction("SUB A,E", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0x94, new Instruction("SUB A,H", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0x95, new Instruction("SUB A,L", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0x96, new Instruction("SUB A,(HL)", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0x97, new Instruction("SUB A,A", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xD6, new Instruction("SUB A,n", cpu => { cpu.WriteA(cpu.Sub8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // SBC
            {
                0x98, new Instruction("SBC A,B", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0x99, new Instruction("SBC A,C", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0x9A, new Instruction("SBC A,D", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0x9B, new Instruction("SBC A,E", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0x9C, new Instruction("SBC A,H", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0x9D, new Instruction("SBC A,L", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0x9E, new Instruction("SBC A,(HL)", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0x9F, new Instruction("SBC A,A", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xDE, new Instruction("SBC A,n", cpu => { cpu.WriteA(cpu.Sbc8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // AND
            {
                0xA0, new Instruction("AND B", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0xA1, new Instruction("AND C", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0xA2, new Instruction("AND D", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0xA3, new Instruction("AND E", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0xA4, new Instruction("AND H", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0xA5, new Instruction("AND L", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0xA6, new Instruction("AND (HL)", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0xA7, new Instruction("AND A", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xE6, new Instruction("AND n", cpu => { cpu.WriteA(cpu.And8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // XOR
            {
                0xA8, new Instruction("XOR B", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0xA9, new Instruction("XOR C", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0xAA, new Instruction("XOR D", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0xAB, new Instruction("XOR E", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0xAC, new Instruction("XOR H", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0xAD, new Instruction("XOR L", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0xAE, new Instruction("XOR (HL)", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0xAF, new Instruction("XOR A", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xEE, new Instruction("XOR n", cpu => { cpu.WriteA(cpu.Xor8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // OR
            {
                0xB0, new Instruction("OR B", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadB())); })
            },
            {
                0xB1, new Instruction("OR C", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadC())); })
            },
            {
                0xB2, new Instruction("OR D", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadD())); })
            },
            {
                0xB3, new Instruction("OR E", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadE())); })
            },
            {
                0xB4, new Instruction("OR H", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadH())); })
            },
            {
                0xB5, new Instruction("OR L", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadL())); })
            },
            {
                0xB6, new Instruction("OR (HL)", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL()))); })
            },
            {
                0xB7, new Instruction("OR A", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadA())); })
            },
            {
                0xF6, new Instruction("OR n", cpu => { cpu.WriteA(cpu.Or8(cpu.ReadA(), cpu.ReadArg1())); }, 2)
            },

            // CP
            {
                0xB8, new Instruction("CP B", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadB()); })
            },
            {
                0xB9, new Instruction("CP C", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadC()); })
            },
            {
                0xBA, new Instruction("CP D", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadD()); })
            },
            {
                0xBB, new Instruction("CP E", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadE()); })
            },
            {
                0xBC, new Instruction("CP H", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadH()); })
            },
            {
                0xBD, new Instruction("CP L", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadL()); })
            },
            {
                0xBE, new Instruction("CP (HL)", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadMem8(cpu.ReadHL())); })
            },
            {
                0xBF, new Instruction("CP A", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadA()); })
            },
            {
                0xFE, new Instruction("CP n", cpu => { cpu.Cp8(cpu.ReadA(), cpu.ReadArg1()); }, 2)
            },

            // RET
            {
                0xC0, new Instruction("RET NZ", cpu => { cpu.Ret(CPU.Conditions.NZ); }, 0)
            },
            {
                0xC8, new Instruction("RET Z", cpu => { cpu.Ret(CPU.Conditions.Z); }, 0)
            },
            {
                0xC9, new Instruction("RET", cpu => { cpu.Ret(CPU.Conditions.A); }, 0)
            },
            {
                0xD0, new Instruction("RET NC", cpu => { cpu.Ret(CPU.Conditions.NC); }, 0)
            },
            {
                0xD8, new Instruction("RET C", cpu => { cpu.Ret(CPU.Conditions.C); }, 0)
            },

            // RETI
            {
                0xD9, new Instruction("RETI", cpu => { cpu.Reti(); }, 0)
            },

            // POP
            {
                0xC1, new Instruction("POP BC", cpu => { cpu.WriteBC(cpu.Pop()); })
            },
            {
                0xD1, new Instruction("POP DE", cpu => { cpu.WriteDE(cpu.Pop()); })
            },
            {
                0xE1, new Instruction("POP HL", cpu => { cpu.WriteHL(cpu.Pop()); })
            },
            {
                0xF1, new Instruction("POP AF", cpu => { cpu.PopAf(); })
            },

            // CALL
            {
                0xC4, new Instruction("CALL NZ,nn", cpu => { cpu.Call(CPU.Conditions.NZ); }, 0)
            },
            {
                0xCC, new Instruction("CALL Z,nn", cpu => { cpu.Call(CPU.Conditions.Z); }, 0)
            },
            {
                0xCD, new Instruction("CALL nn", cpu => { cpu.Call(CPU.Conditions.A); }, 0)
            },
            {
                0xD4, new Instruction("CALL NC,nn", cpu => { cpu.Call(CPU.Conditions.NC); }, 0)
            },
            {
                0xDC, new Instruction("CALL C,nn", cpu => { cpu.Call(CPU.Conditions.C); }, 0)
            },

            // PUSH
            {
                0xC5, new Instruction("PUSH BC", cpu => { cpu.Push(cpu.ReadBC()); })
            },
            {
                0xD5, new Instruction("PUSH DE", cpu => { cpu.Push(cpu.ReadDE()); })
            },
            {
                0xE5, new Instruction("PUSH HL", cpu => { cpu.Push(cpu.ReadHL()); })
            },
            {
                0xF5, new Instruction("PUSH AF", cpu => { cpu.PushAf(); })
            },

            // RST
            {
                0xC7, new Instruction("RST 0", cpu => { cpu.Rst(0x00); }, 0)
            },
            {
                0xCF, new Instruction("RST 8", cpu => { cpu.Rst(0x08); }, 0)
            },
            {
                0xD7, new Instruction("RST 10", cpu => { cpu.Rst(0x10); }, 0)
            },
            {
                0xDF, new Instruction("RST 18", cpu => { cpu.Rst(0x18); }, 0)
            },
            {
                0xE7, new Instruction("RST 20", cpu => { cpu.Rst(0x20); }, 0)
            },
            {
                0xEF, new Instruction("RST 28", cpu => { cpu.Rst(0x28); }, 0)
            },
            {
                0xF7, new Instruction("RST 30", cpu => { cpu.Rst(0x30); }, 0)
            },
            {
                0xFF, new Instruction("RST 38", cpu => { cpu.Rst(0x38); }, 0)
            },

            // DI
            {
                0xF3, new Instruction("DI", cpu => { cpu.Di(); })
            },

            // EI
            {
                0xFB, new Instruction("EI", cpu => { cpu.Ei(); }, 0)
            },

            // JR
            {
                0x18, new Instruction("JR n", cpu => { cpu.Jr(CPU.Conditions.A, cpu.ReadArg1()); }, 0)
            },

            {
                0x20, new Instruction("JR NZ,n", cpu => { cpu.Jr(CPU.Conditions.NZ, cpu.ReadArg1()); }, 0)
            },
            {
                0x28, new Instruction("JR Z,n", cpu => { cpu.Jr(CPU.Conditions.Z, cpu.ReadArg1()); }, 0)
            },
            {
                0x30, new Instruction("JR NC,n", cpu => { cpu.Jr(CPU.Conditions.NC, cpu.ReadArg1()); }, 0)
            },
            {
                0x38, new Instruction("JR C,n", cpu => { cpu.Jr(CPU.Conditions.C, cpu.ReadArg1()); }, 0)
            },

            // JP
            {
                0xC2, new Instruction("JP NZ,nn", cpu => { cpu.Jp(CPU.Conditions.NZ, cpu.ReadArg12()); }, 0)
            },
            {
                0xC3, new Instruction("JP nn", cpu => { cpu.Jp(CPU.Conditions.A, cpu.ReadArg12()); }, 0)
            },
            {
                0xCA, new Instruction("JP Z,nn", cpu => { cpu.Jp(CPU.Conditions.Z, cpu.ReadArg12()); }, 0)
            },
            {
                0xD2, new Instruction("JP NC,nn", cpu => { cpu.Jp(CPU.Conditions.NC, cpu.ReadArg12()); }, 0)
            },
            {
                0xDA, new Instruction("JP C,nn", cpu => { cpu.Jp(CPU.Conditions.C, cpu.ReadArg12()); }, 0)
            },
            {
                0xE9, new Instruction("JR (HL)", cpu => { cpu.Jpl(); }, 0)
            }
        };

        private static readonly Dictionary<byte, Instruction> _prefixInstructions = new Dictionary<byte, Instruction>
        {
            // RLC
            {
                0x00, new Instruction("RLC B", cpu => { cpu.WriteB(cpu.Rlc(cpu.ReadB())); }, 2)
            },
            {
                0x01, new Instruction("RLC C", cpu => { cpu.WriteC(cpu.Rlc(cpu.ReadC())); }, 2)
            },
            {
                0x02, new Instruction("RLC D", cpu => { cpu.WriteD(cpu.Rlc(cpu.ReadD())); }, 2)
            },
            {
                0x03, new Instruction("RLC E", cpu => { cpu.WriteE(cpu.Rlc(cpu.ReadE())); }, 2)
            },
            {
                0x04, new Instruction("RLC H", cpu => { cpu.WriteH(cpu.Rlc(cpu.ReadH())); }, 2)
            },
            {
                0x05, new Instruction("RLC L", cpu => { cpu.WriteL(cpu.Rlc(cpu.ReadL())); }, 2)
            },
            {
                0x06, new Instruction("RLC (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Rlc(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x07, new Instruction("RLC A", cpu => { cpu.WriteA(cpu.Rlc(cpu.ReadA())); }, 2)
            },

            // RRC
            {
                0x08, new Instruction("RRC B", cpu => { cpu.WriteB(cpu.Rrc(cpu.ReadB())); }, 2)
            },
            {
                0x09, new Instruction("RRC C", cpu => { cpu.WriteC(cpu.Rrc(cpu.ReadC())); }, 2)
            },
            {
                0x0A, new Instruction("RRC D", cpu => { cpu.WriteD(cpu.Rrc(cpu.ReadD())); }, 2)
            },
            {
                0x0B, new Instruction("RRC E", cpu => { cpu.WriteE(cpu.Rrc(cpu.ReadE())); }, 2)
            },
            {
                0x0C, new Instruction("RRC H", cpu => { cpu.WriteH(cpu.Rrc(cpu.ReadH())); }, 2)
            },
            {
                0x0D, new Instruction("RRC L", cpu => { cpu.WriteL(cpu.Rrc(cpu.ReadL())); }, 2)
            },
            {
                0x0E, new Instruction("RRC (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Rrc(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x0F, new Instruction("RRC A", cpu => { cpu.WriteA(cpu.Rrc(cpu.ReadA())); }, 2)
            },

            // RL
            {
                0x10, new Instruction("RL B", cpu => { cpu.WriteB(cpu.Rl(cpu.ReadB())); }, 2)
            },
            {
                0x11, new Instruction("RL C", cpu => { cpu.WriteC(cpu.Rl(cpu.ReadC())); }, 2)
            },
            {
                0x12, new Instruction("RL D", cpu => { cpu.WriteD(cpu.Rl(cpu.ReadD())); }, 2)
            },
            {
                0x13, new Instruction("RL E", cpu => { cpu.WriteE(cpu.Rl(cpu.ReadE())); }, 2)
            },
            {
                0x14, new Instruction("RL H", cpu => { cpu.WriteH(cpu.Rl(cpu.ReadH())); }, 2)
            },
            {
                0x15, new Instruction("RL L", cpu => { cpu.WriteL(cpu.Rl(cpu.ReadL())); }, 2)
            },
            {
                0x16, new Instruction("RL HL", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Rl(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x17, new Instruction("RL A", cpu => { cpu.WriteA(cpu.Rl(cpu.ReadA())); }, 2)
            },

            // RR
            {
                0x18, new Instruction("RR B", cpu => { cpu.WriteB(cpu.Rr(cpu.ReadB())); }, 2)
            },
            {
                0x19, new Instruction("RR C", cpu => { cpu.WriteC(cpu.Rr(cpu.ReadC())); }, 2)
            },
            {
                0x1A, new Instruction("RR D", cpu => { cpu.WriteD(cpu.Rr(cpu.ReadD())); }, 2)
            },
            {
                0x1B, new Instruction("RR E", cpu => { cpu.WriteE(cpu.Rr(cpu.ReadE())); }, 2)
            },
            {
                0x1C, new Instruction("RR H", cpu => { cpu.WriteH(cpu.Rr(cpu.ReadH())); }, 2)
            },
            {
                0x1D, new Instruction("RR L", cpu => { cpu.WriteL(cpu.Rr(cpu.ReadL())); }, 2)
            },
            {
                0x1E, new Instruction("RR HL", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Rr(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x1F, new Instruction("RR A", cpu => { cpu.WriteA(cpu.Rr(cpu.ReadA())); }, 2)
            },

            // SLA
            {
                0x20, new Instruction("SLA B", cpu => { cpu.WriteB(cpu.Sla(cpu.ReadB())); }, 2)
            },
            {
                0x21, new Instruction("SLA C", cpu => { cpu.WriteC(cpu.Sla(cpu.ReadC())); }, 2)
            },
            {
                0x22, new Instruction("SLA D", cpu => { cpu.WriteD(cpu.Sla(cpu.ReadD())); }, 2)
            },
            {
                0x23, new Instruction("SLA E", cpu => { cpu.WriteE(cpu.Sla(cpu.ReadE())); }, 2)
            },
            {
                0x24, new Instruction("SLA H", cpu => { cpu.WriteH(cpu.Sla(cpu.ReadH())); }, 2)
            },
            {
                0x25, new Instruction("SLA L", cpu => { cpu.WriteL(cpu.Sla(cpu.ReadL())); }, 2)
            },
            {
                0x26, new Instruction("SLA (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Sla(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x27, new Instruction("SLA A", cpu => { cpu.WriteA(cpu.Sla(cpu.ReadA())); }, 2)
            },

            // SRA
            {
                0x28, new Instruction("SRA B", cpu => { cpu.WriteB(cpu.Sra(cpu.ReadB())); }, 2)
            },
            {
                0x29, new Instruction("SRA C", cpu => { cpu.WriteC(cpu.Sra(cpu.ReadC())); }, 2)
            },
            {
                0x2A, new Instruction("SRA D", cpu => { cpu.WriteD(cpu.Sra(cpu.ReadD())); }, 2)
            },
            {
                0x2B, new Instruction("SRA E", cpu => { cpu.WriteE(cpu.Sra(cpu.ReadE())); }, 2)
            },
            {
                0x2C, new Instruction("SRA H", cpu => { cpu.WriteH(cpu.Sra(cpu.ReadH())); }, 2)
            },
            {
                0x2D, new Instruction("SRA L", cpu => { cpu.WriteL(cpu.Sra(cpu.ReadL())); }, 2)
            },
            {
                0x2E, new Instruction("SRA (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Sra(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x2F, new Instruction("SRA A", cpu => { cpu.WriteA(cpu.Sra(cpu.ReadA())); }, 2)
            },

            // SWAP
            {
                0x30, new Instruction("SWAP B", cpu => { cpu.WriteB(cpu.Swap(cpu.ReadB())); }, 2)
            },
            {
                0x31, new Instruction("SWAP C", cpu => { cpu.WriteC(cpu.Swap(cpu.ReadC())); }, 2)
            },
            {
                0x32, new Instruction("SWAP D", cpu => { cpu.WriteD(cpu.Swap(cpu.ReadD())); }, 2)
            },
            {
                0x33, new Instruction("SWAP E", cpu => { cpu.WriteE(cpu.Swap(cpu.ReadE())); }, 2)
            },
            {
                0x34, new Instruction("SWAP H", cpu => { cpu.WriteH(cpu.Swap(cpu.ReadH())); }, 2)
            },
            {
                0x35, new Instruction("SWAP L", cpu => { cpu.WriteL(cpu.Swap(cpu.ReadL())); }, 2)
            },
            {
                0x36, new Instruction("SWAP (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Swap(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x37, new Instruction("SWAP A", cpu => { cpu.WriteA(cpu.Swap(cpu.ReadA())); }, 2)
            },

            // SRL
            {
                0x38, new Instruction("SRL B", cpu => { cpu.WriteB(cpu.Srl(cpu.ReadB())); }, 2)
            },
            {
                0x39, new Instruction("SRL C", cpu => { cpu.WriteC(cpu.Srl(cpu.ReadC())); }, 2)
            },
            {
                0x3A, new Instruction("SRL D", cpu => { cpu.WriteD(cpu.Srl(cpu.ReadD())); }, 2)
            },
            {
                0x3B, new Instruction("SRL E", cpu => { cpu.WriteE(cpu.Srl(cpu.ReadE())); }, 2)
            },
            {
                0x3C, new Instruction("SRL H", cpu => { cpu.WriteH(cpu.Srl(cpu.ReadH())); }, 2)
            },
            {
                0x3D, new Instruction("SRL L", cpu => { cpu.WriteL(cpu.Srl(cpu.ReadL())); }, 2)
            },
            {
                0x3E, new Instruction("SRL (HL)", cpu =>
                {
                    cpu.WriteMem8(
                        cpu.ReadHL(),
                        cpu.Srl(
                            cpu.ReadMem8(
                                cpu.ReadHL()
                            )
                        )
                    );
                }, 2)
            },
            {
                0x3F, new Instruction("SRL A", cpu => { cpu.WriteA(cpu.Srl(cpu.ReadA())); }, 2)
            },

            // BIT 0
            {
                0x40, new Instruction("BIT 0,B", cpu => { cpu.Bit(0, cpu.ReadB()); }, 2)
            },
            {
                0x41, new Instruction("BIT 0,C", cpu => { cpu.Bit(0, cpu.ReadC()); }, 2)
            },
            {
                0x42, new Instruction("BIT 0,D", cpu => { cpu.Bit(0, cpu.ReadD()); }, 2)
            },
            {
                0x43, new Instruction("BIT 0,E", cpu => { cpu.Bit(0, cpu.ReadE()); }, 2)
            },
            {
                0x44, new Instruction("BIT 0,H", cpu => { cpu.Bit(0, cpu.ReadH()); }, 2)
            },
            {
                0x45, new Instruction("BIT 0,L", cpu => { cpu.Bit(0, cpu.ReadL()); }, 2)
            },
            {
                0x46, new Instruction("BIT 0,(HL)", cpu => { cpu.Bit(0, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x47, new Instruction("BIT 0,A", cpu => { cpu.Bit(0, cpu.ReadA()); }, 2)
            },

            // BIT 1
            {
                0x48, new Instruction("BIT 1,B", cpu => { cpu.Bit(1, cpu.ReadB()); }, 2)
            },
            {
                0x49, new Instruction("BIT 1,C", cpu => { cpu.Bit(1, cpu.ReadC()); }, 2)
            },
            {
                0x4A, new Instruction("BIT 1,D", cpu => { cpu.Bit(1, cpu.ReadD()); }, 2)
            },
            {
                0x4B, new Instruction("BIT 1,E", cpu => { cpu.Bit(1, cpu.ReadE()); }, 2)
            },
            {
                0x4C, new Instruction("BIT 1,H", cpu => { cpu.Bit(1, cpu.ReadH()); }, 2)
            },
            {
                0x4D, new Instruction("BIT 1,L", cpu => { cpu.Bit(1, cpu.ReadL()); }, 2)
            },
            {
                0x4E, new Instruction("BIT 1,(HL)", cpu => { cpu.Bit(1, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x4F, new Instruction("BIT 1,A", cpu => { cpu.Bit(1, cpu.ReadA()); }, 2)
            },

            // BIT 2
            {
                0x50, new Instruction("BIT 2,B", cpu => { cpu.Bit(2, cpu.ReadB()); }, 2)
            },
            {
                0x51, new Instruction("BIT 2,C", cpu => { cpu.Bit(2, cpu.ReadC()); }, 2)
            },
            {
                0x52, new Instruction("BIT 2,D", cpu => { cpu.Bit(2, cpu.ReadD()); }, 2)
            },
            {
                0x53, new Instruction("BIT 2,E", cpu => { cpu.Bit(2, cpu.ReadE()); }, 2)
            },
            {
                0x54, new Instruction("BIT 2,H", cpu => { cpu.Bit(2, cpu.ReadH()); }, 2)
            },
            {
                0x55, new Instruction("BIT 2,L", cpu => { cpu.Bit(2, cpu.ReadL()); }, 2)
            },
            {
                0x56, new Instruction("BIT 2,(HL)", cpu => { cpu.Bit(2, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x57, new Instruction("BIT 2,A", cpu => { cpu.Bit(2, cpu.ReadA()); }, 2)
            },

            // BIT 3
            {
                0x58, new Instruction("BIT 3,B", cpu => { cpu.Bit(3, cpu.ReadB()); }, 2)
            },
            {
                0x59, new Instruction("BIT 3,C", cpu => { cpu.Bit(3, cpu.ReadC()); }, 2)
            },
            {
                0x5A, new Instruction("BIT 3,D", cpu => { cpu.Bit(3, cpu.ReadD()); }, 2)
            },
            {
                0x5B, new Instruction("BIT 3,E", cpu => { cpu.Bit(3, cpu.ReadE()); }, 2)
            },
            {
                0x5C, new Instruction("BIT 3,H", cpu => { cpu.Bit(3, cpu.ReadH()); }, 2)
            },
            {
                0x5D, new Instruction("BIT 3,L", cpu => { cpu.Bit(3, cpu.ReadL()); }, 2)
            },
            {
                0x5E, new Instruction("BIT 3,(HL)", cpu => { cpu.Bit(3, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x5F, new Instruction("BIT 3,A", cpu => { cpu.Bit(3, cpu.ReadA()); }, 2)
            },

            // BIT 4
            {
                0x60, new Instruction("BIT 4,B", cpu => { cpu.Bit(4, cpu.ReadB()); }, 2)
            },
            {
                0x61, new Instruction("BIT 4,C", cpu => { cpu.Bit(4, cpu.ReadC()); }, 2)
            },
            {
                0x62, new Instruction("BIT 4,D", cpu => { cpu.Bit(4, cpu.ReadD()); }, 2)
            },
            {
                0x63, new Instruction("BIT 4,E", cpu => { cpu.Bit(4, cpu.ReadE()); }, 2)
            },
            {
                0x64, new Instruction("BIT 4,H", cpu => { cpu.Bit(4, cpu.ReadH()); }, 2)
            },
            {
                0x65, new Instruction("BIT 4,L", cpu => { cpu.Bit(4, cpu.ReadL()); }, 2)
            },
            {
                0x66, new Instruction("BIT 4,(HL)", cpu => { cpu.Bit(4, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x67, new Instruction("BIT 4,A", cpu => { cpu.Bit(4, cpu.ReadA()); }, 2)
            },

            // BIT 5
            {
                0x68, new Instruction("BIT 5,B", cpu => { cpu.Bit(5, cpu.ReadB()); }, 2)
            },
            {
                0x69, new Instruction("BIT 5,C", cpu => { cpu.Bit(5, cpu.ReadC()); }, 2)
            },
            {
                0x6A, new Instruction("BIT 5,D", cpu => { cpu.Bit(5, cpu.ReadD()); }, 2)
            },
            {
                0x6B, new Instruction("BIT 5,E", cpu => { cpu.Bit(5, cpu.ReadE()); }, 2)
            },
            {
                0x6C, new Instruction("BIT 5,H", cpu => { cpu.Bit(5, cpu.ReadH()); }, 2)
            },
            {
                0x6D, new Instruction("BIT 5,L", cpu => { cpu.Bit(5, cpu.ReadL()); }, 2)
            },
            {
                0x6E, new Instruction("BIT 5,(HL)", cpu => { cpu.Bit(5, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x6F, new Instruction("BIT 5,A", cpu => { cpu.Bit(5, cpu.ReadA()); }, 2)
            },

            // BIT 6
            {
                0x70, new Instruction("BIT 6,B", cpu => { cpu.Bit(6, cpu.ReadB()); }, 2)
            },
            {
                0x71, new Instruction("BIT 6,C", cpu => { cpu.Bit(6, cpu.ReadC()); }, 2)
            },
            {
                0x72, new Instruction("BIT 6,D", cpu => { cpu.Bit(6, cpu.ReadD()); }, 2)
            },
            {
                0x73, new Instruction("BIT 6,E", cpu => { cpu.Bit(6, cpu.ReadE()); }, 2)
            },
            {
                0x74, new Instruction("BIT 6,H", cpu => { cpu.Bit(6, cpu.ReadH()); }, 2)
            },
            {
                0x75, new Instruction("BIT 6,L", cpu => { cpu.Bit(6, cpu.ReadL()); }, 2)
            },
            {
                0x76, new Instruction("BIT 6,(HL)", cpu => { cpu.Bit(6, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x77, new Instruction("BIT 6,A", cpu => { cpu.Bit(6, cpu.ReadA()); }, 2)
            },

            // BIT 7
            {
                0x78, new Instruction("BIT 7,B", cpu => { cpu.Bit(7, cpu.ReadB()); }, 2)
            },
            {
                0x79, new Instruction("BIT 7,C", cpu => { cpu.Bit(7, cpu.ReadC()); }, 2)
            },
            {
                0x7A, new Instruction("BIT 7,D", cpu => { cpu.Bit(7, cpu.ReadD()); }, 2)
            },
            {
                0x7B, new Instruction("BIT 7,E", cpu => { cpu.Bit(7, cpu.ReadE()); }, 2)
            },
            {
                0x7C, new Instruction("BIT 7,H", cpu => { cpu.Bit(7, cpu.ReadH()); }, 2)
            },
            {
                0x7D, new Instruction("BIT 7,L", cpu => { cpu.Bit(7, cpu.ReadL()); }, 2)
            },
            {
                0x7E, new Instruction("BIT 7,(HL)", cpu => { cpu.Bit(7, cpu.ReadMem8(cpu.ReadHL())); }, 2)
            },
            {
                0x7F, new Instruction("BIT 7,A", cpu => { cpu.Bit(7, cpu.ReadA()); }, 2)
            },

            // RES 0
            {
                0x80, new Instruction("RES 0,B", cpu => { cpu.WriteB(cpu.Res(0, cpu.ReadB())); }, 2)
            },
            {
                0x81, new Instruction("RES 0,C", cpu => { cpu.WriteC(cpu.Res(0, cpu.ReadC())); }, 2)
            },
            {
                0x82, new Instruction("RES 0,D", cpu => { cpu.WriteD(cpu.Res(0, cpu.ReadD())); }, 2)
            },
            {
                0x83, new Instruction("RES 0,E", cpu => { cpu.WriteE(cpu.Res(0, cpu.ReadE())); }, 2)
            },
            {
                0x84, new Instruction("RES 0,H", cpu => { cpu.WriteH(cpu.Res(0, cpu.ReadH())); }, 2)
            },
            {
                0x85, new Instruction("RES 0,L", cpu => { cpu.WriteL(cpu.Res(0, cpu.ReadL())); }, 2)
            },
            {
                0x86, new Instruction("RES 0,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(0, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0x87, new Instruction("RES 0,A", cpu => { cpu.WriteA(cpu.Res(0, cpu.ReadA())); }, 2)
            },

            // RES 1
            {
                0x88, new Instruction("RES 1,B", cpu => { cpu.WriteB(cpu.Res(1, cpu.ReadB())); }, 2)
            },
            {
                0x89, new Instruction("RES 1,C", cpu => { cpu.WriteC(cpu.Res(1, cpu.ReadC())); }, 2)
            },
            {
                0x8A, new Instruction("RES 1,D", cpu => { cpu.WriteD(cpu.Res(1, cpu.ReadD())); }, 2)
            },
            {
                0x8B, new Instruction("RES 1,E", cpu => { cpu.WriteE(cpu.Res(1, cpu.ReadE())); }, 2)
            },
            {
                0x8C, new Instruction("RES 1,H", cpu => { cpu.WriteH(cpu.Res(1, cpu.ReadH())); }, 2)
            },
            {
                0x8D, new Instruction("RES 1,L", cpu => { cpu.WriteL(cpu.Res(1, cpu.ReadL())); }, 2)
            },
            {
                0x8E, new Instruction("RES 1,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(1, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0x8F, new Instruction("RES 1,A", cpu => { cpu.WriteA(cpu.Res(1, cpu.ReadA())); }, 2)
            },

            // RES 2
            {
                0x90, new Instruction("RES 2,B", cpu => { cpu.WriteB(cpu.Res(2, cpu.ReadB())); }, 2)
            },
            {
                0x91, new Instruction("RES 2,C", cpu => { cpu.WriteC(cpu.Res(2, cpu.ReadC())); }, 2)
            },
            {
                0x92, new Instruction("RES 2,D", cpu => { cpu.WriteD(cpu.Res(2, cpu.ReadD())); }, 2)
            },
            {
                0x93, new Instruction("RES 2,E", cpu => { cpu.WriteE(cpu.Res(2, cpu.ReadE())); }, 2)
            },
            {
                0x94, new Instruction("RES 2,H", cpu => { cpu.WriteH(cpu.Res(2, cpu.ReadH())); }, 2)
            },
            {
                0x95, new Instruction("RES 2,L", cpu => { cpu.WriteL(cpu.Res(2, cpu.ReadL())); }, 2)
            },
            {
                0x96, new Instruction("RES 2,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(2, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0x97, new Instruction("RES 2,A", cpu => { cpu.WriteA(cpu.Res(2, cpu.ReadA())); }, 2)
            },

            // RES 3
            {
                0x98, new Instruction("RES 3,B", cpu => { cpu.WriteB(cpu.Res(3, cpu.ReadB())); }, 2)
            },
            {
                0x99, new Instruction("RES 3,C", cpu => { cpu.WriteC(cpu.Res(3, cpu.ReadC())); }, 2)
            },
            {
                0x9A, new Instruction("RES 3,D", cpu => { cpu.WriteD(cpu.Res(3, cpu.ReadD())); }, 2)
            },
            {
                0x9B, new Instruction("RES 3,E", cpu => { cpu.WriteE(cpu.Res(3, cpu.ReadE())); }, 2)
            },
            {
                0x9C, new Instruction("RES 3,H", cpu => { cpu.WriteH(cpu.Res(3, cpu.ReadH())); }, 2)
            },
            {
                0x9D, new Instruction("RES 3,L", cpu => { cpu.WriteL(cpu.Res(3, cpu.ReadL())); }, 2)
            },
            {
                0x9E, new Instruction("RES 3,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(3, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0x9F, new Instruction("RES 3,A", cpu => { cpu.WriteA(cpu.Res(3, cpu.ReadA())); }, 2)
            },

            // RES 4
            {
                0xA0, new Instruction("RES 4,B", cpu => { cpu.WriteB(cpu.Res(4, cpu.ReadB())); }, 2)
            },
            {
                0xA1, new Instruction("RES 4,C", cpu => { cpu.WriteC(cpu.Res(4, cpu.ReadC())); }, 2)
            },
            {
                0xA2, new Instruction("RES 4,D", cpu => { cpu.WriteD(cpu.Res(4, cpu.ReadD())); }, 2)
            },
            {
                0xA3, new Instruction("RES 4,E", cpu => { cpu.WriteE(cpu.Res(4, cpu.ReadE())); }, 2)
            },
            {
                0xA4, new Instruction("RES 4,H", cpu => { cpu.WriteH(cpu.Res(4, cpu.ReadH())); }, 2)
            },
            {
                0xA5, new Instruction("RES 4,L", cpu => { cpu.WriteL(cpu.Res(4, cpu.ReadL())); }, 2)
            },
            {
                0xA6, new Instruction("RES 4,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(4, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xA7, new Instruction("RES 4,A", cpu => { cpu.WriteA(cpu.Res(4, cpu.ReadA())); }, 2)
            },

            // RES 5
            {
                0xA8, new Instruction("RES 5,B", cpu => { cpu.WriteB(cpu.Res(5, cpu.ReadB())); }, 2)
            },
            {
                0xA9, new Instruction("RES 5,C", cpu => { cpu.WriteC(cpu.Res(5, cpu.ReadC())); }, 2)
            },
            {
                0xAA, new Instruction("RES 5,D", cpu => { cpu.WriteD(cpu.Res(5, cpu.ReadD())); }, 2)
            },
            {
                0xAB, new Instruction("RES 5,E", cpu => { cpu.WriteE(cpu.Res(5, cpu.ReadE())); }, 2)
            },
            {
                0xAC, new Instruction("RES 5,H", cpu => { cpu.WriteH(cpu.Res(5, cpu.ReadH())); }, 2)
            },
            {
                0xAD, new Instruction("RES 5,L", cpu => { cpu.WriteL(cpu.Res(5, cpu.ReadL())); }, 2)
            },
            {
                0xAE, new Instruction("RES 5,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(5, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xAF, new Instruction("RES 5,A", cpu => { cpu.WriteA(cpu.Res(5, cpu.ReadA())); }, 2)
            },

            // RES 6
            {
                0xB0, new Instruction("RES 6,B", cpu => { cpu.WriteB(cpu.Res(6, cpu.ReadB())); }, 2)
            },
            {
                0xB1, new Instruction("RES 6,C", cpu => { cpu.WriteC(cpu.Res(6, cpu.ReadC())); }, 2)
            },
            {
                0xB2, new Instruction("RES 6,D", cpu => { cpu.WriteD(cpu.Res(6, cpu.ReadD())); }, 2)
            },
            {
                0xB3, new Instruction("RES 6,E", cpu => { cpu.WriteE(cpu.Res(6, cpu.ReadE())); }, 2)
            },
            {
                0xB4, new Instruction("RES 6,H", cpu => { cpu.WriteH(cpu.Res(6, cpu.ReadH())); }, 2)
            },
            {
                0xB5, new Instruction("RES 6,L", cpu => { cpu.WriteL(cpu.Res(6, cpu.ReadL())); }, 2)
            },
            {
                0xB6, new Instruction("RES 6,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(6, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xB7, new Instruction("RES 6,A", cpu => { cpu.WriteA(cpu.Res(6, cpu.ReadA())); }, 2)
            },

            // RES 7
            {
                0xB8, new Instruction("RES 7,B", cpu => { cpu.WriteB(cpu.Res(7, cpu.ReadB())); }, 2)
            },
            {
                0xB9, new Instruction("RES 7,C", cpu => { cpu.WriteC(cpu.Res(7, cpu.ReadC())); }, 2)
            },
            {
                0xBA, new Instruction("RES 7,D", cpu => { cpu.WriteD(cpu.Res(7, cpu.ReadD())); }, 2)
            },
            {
                0xBB, new Instruction("RES 7,E", cpu => { cpu.WriteE(cpu.Res(7, cpu.ReadE())); }, 2)
            },
            {
                0xBC, new Instruction("RES 7,H", cpu => { cpu.WriteH(cpu.Res(7, cpu.ReadH())); }, 2)
            },
            {
                0xBD, new Instruction("RES 7,L", cpu => { cpu.WriteL(cpu.Res(7, cpu.ReadL())); }, 2)
            },
            {
                0xBE, new Instruction("RES 7,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Res(7, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xBF, new Instruction("RES 7,A", cpu => { cpu.WriteA(cpu.Res(7, cpu.ReadA())); }, 2)
            },

            // SET 0
            {
                0xC0, new Instruction("SET 0,B", cpu => { cpu.WriteB(cpu.Set(0, cpu.ReadB())); }, 2)
            },
            {
                0xC1, new Instruction("SET 0,C", cpu => { cpu.WriteC(cpu.Set(0, cpu.ReadC())); }, 2)
            },
            {
                0xC2, new Instruction("SET 0,D", cpu => { cpu.WriteD(cpu.Set(0, cpu.ReadD())); }, 2)
            },
            {
                0xC3, new Instruction("SET 0,E", cpu => { cpu.WriteE(cpu.Set(0, cpu.ReadE())); }, 2)
            },
            {
                0xC4, new Instruction("SET 0,H", cpu => { cpu.WriteH(cpu.Set(0, cpu.ReadH())); }, 2)
            },
            {
                0xC5, new Instruction("SET 0,L", cpu => { cpu.WriteL(cpu.Set(0, cpu.ReadL())); }, 2)
            },
            {
                0xC6, new Instruction("SET 0,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(0, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xC7, new Instruction("SET 0,A", cpu => { cpu.WriteA(cpu.Set(0, cpu.ReadA())); }, 2)
            },

            // SET 1
            {
                0xC8, new Instruction("SET 1,B", cpu => { cpu.WriteB(cpu.Set(1, cpu.ReadB())); }, 2)
            },
            {
                0xC9, new Instruction("SET 1,C", cpu => { cpu.WriteC(cpu.Set(1, cpu.ReadC())); }, 2)
            },
            {
                0xCA, new Instruction("SET 1,D", cpu => { cpu.WriteD(cpu.Set(1, cpu.ReadD())); }, 2)
            },
            {
                0xCB, new Instruction("SET 1,E", cpu => { cpu.WriteE(cpu.Set(1, cpu.ReadE())); }, 2)
            },
            {
                0xCC, new Instruction("SET 1,H", cpu => { cpu.WriteH(cpu.Set(1, cpu.ReadH())); }, 2)
            },
            {
                0xCD, new Instruction("SET 1,L", cpu => { cpu.WriteL(cpu.Set(1, cpu.ReadL())); }, 2)
            },
            {
                0xCE, new Instruction("SET 1,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(1, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xCF, new Instruction("SET 1,A", cpu => { cpu.WriteA(cpu.Set(1, cpu.ReadA())); }, 2)
            },

            // SET 2
            {
                0xD0, new Instruction("SET 2,B", cpu => { cpu.WriteB(cpu.Set(2, cpu.ReadB())); }, 2)
            },
            {
                0xD1, new Instruction("SET 2,C", cpu => { cpu.WriteC(cpu.Set(2, cpu.ReadC())); }, 2)
            },
            {
                0xD2, new Instruction("SET 2,D", cpu => { cpu.WriteD(cpu.Set(2, cpu.ReadD())); }, 2)
            },
            {
                0xD3, new Instruction("SET 2,E", cpu => { cpu.WriteE(cpu.Set(2, cpu.ReadE())); }, 2)
            },
            {
                0xD4, new Instruction("SET 2,H", cpu => { cpu.WriteH(cpu.Set(2, cpu.ReadH())); }, 2)
            },
            {
                0xD5, new Instruction("SET 2,L", cpu => { cpu.WriteL(cpu.Set(2, cpu.ReadL())); }, 2)
            },
            {
                0xD6, new Instruction("SET 2,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(2, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xD7, new Instruction("SET 2,A", cpu => { cpu.WriteA(cpu.Set(2, cpu.ReadA())); }, 2)
            },

            // SET 3
            {
                0xD8, new Instruction("SET 3,B", cpu => { cpu.WriteB(cpu.Set(3, cpu.ReadB())); }, 2)
            },
            {
                0xD9, new Instruction("SET 3,C", cpu => { cpu.WriteC(cpu.Set(3, cpu.ReadC())); }, 2)
            },
            {
                0xDA, new Instruction("SET 3,D", cpu => { cpu.WriteD(cpu.Set(3, cpu.ReadD())); }, 2)
            },
            {
                0xDB, new Instruction("SET 3,E", cpu => { cpu.WriteE(cpu.Set(3, cpu.ReadE())); }, 2)
            },
            {
                0xDC, new Instruction("SET 3,H", cpu => { cpu.WriteH(cpu.Set(3, cpu.ReadH())); }, 2)
            },
            {
                0xDD, new Instruction("SET 3,L", cpu => { cpu.WriteL(cpu.Set(3, cpu.ReadL())); }, 2)
            },
            {
                0xDE, new Instruction("SET 3,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(3, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xDF, new Instruction("SET 3,A", cpu => { cpu.WriteA(cpu.Set(3, cpu.ReadA())); }, 2)
            },

            // SET 4
            {
                0xE0, new Instruction("SET 4,B", cpu => { cpu.WriteB(cpu.Set(4, cpu.ReadB())); }, 2)
            },
            {
                0xE1, new Instruction("SET 4,C", cpu => { cpu.WriteC(cpu.Set(4, cpu.ReadC())); }, 2)
            },
            {
                0xE2, new Instruction("SET 4,D", cpu => { cpu.WriteD(cpu.Set(4, cpu.ReadD())); }, 2)
            },
            {
                0xE3, new Instruction("SET 4,E", cpu => { cpu.WriteE(cpu.Set(4, cpu.ReadE())); }, 2)
            },
            {
                0xE4, new Instruction("SET 4,H", cpu => { cpu.WriteH(cpu.Set(4, cpu.ReadH())); }, 2)
            },
            {
                0xE5, new Instruction("SET 4,L", cpu => { cpu.WriteL(cpu.Set(4, cpu.ReadL())); }, 2)
            },
            {
                0xE6, new Instruction("SET 4,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(4, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xE7, new Instruction("SET 4,A", cpu => { cpu.WriteA(cpu.Set(4, cpu.ReadA())); }, 2)
            },

            // SET 5
            {
                0xE8, new Instruction("SET 5,B", cpu => { cpu.WriteB(cpu.Set(5, cpu.ReadB())); }, 2)
            },
            {
                0xE9, new Instruction("SET 5,C", cpu => { cpu.WriteC(cpu.Set(5, cpu.ReadC())); }, 2)
            },
            {
                0xEA, new Instruction("SET 5,D", cpu => { cpu.WriteD(cpu.Set(5, cpu.ReadD())); }, 2)
            },
            {
                0xEB, new Instruction("SET 5,E", cpu => { cpu.WriteE(cpu.Set(5, cpu.ReadE())); }, 2)
            },
            {
                0xEC, new Instruction("SET 5,H", cpu => { cpu.WriteH(cpu.Set(5, cpu.ReadH())); }, 2)
            },
            {
                0xED, new Instruction("SET 5,L", cpu => { cpu.WriteL(cpu.Set(5, cpu.ReadL())); }, 2)
            },
            {
                0xEE, new Instruction("SET 5,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(5, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xEF, new Instruction("SET 5,A", cpu => { cpu.WriteA(cpu.Set(5, cpu.ReadA())); }, 2)
            },

            // SET 6
            {
                0xF0, new Instruction("SET 6,B", cpu => { cpu.WriteB(cpu.Set(6, cpu.ReadB())); }, 2)
            },
            {
                0xF1, new Instruction("SET 6,C", cpu => { cpu.WriteC(cpu.Set(6, cpu.ReadC())); }, 2)
            },
            {
                0xF2, new Instruction("SET 6,D", cpu => { cpu.WriteD(cpu.Set(6, cpu.ReadD())); }, 2)
            },
            {
                0xF3, new Instruction("SET 6,E", cpu => { cpu.WriteE(cpu.Set(6, cpu.ReadE())); }, 2)
            },
            {
                0xF4, new Instruction("SET 6,H", cpu => { cpu.WriteH(cpu.Set(6, cpu.ReadH())); }, 2)
            },
            {
                0xF5, new Instruction("SET 6,L", cpu => { cpu.WriteL(cpu.Set(6, cpu.ReadL())); }, 2)
            },
            {
                0xF6, new Instruction("SET 6,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(6, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xF7, new Instruction("SET 6,A", cpu => { cpu.WriteA(cpu.Set(6, cpu.ReadA())); }, 2)
            },

            // SET 7
            {
                0xF8, new Instruction("SET 7,B", cpu => { cpu.WriteB(cpu.Set(7, cpu.ReadB())); }, 2)
            },
            {
                0xF9, new Instruction("SET 7,C", cpu => { cpu.WriteC(cpu.Set(7, cpu.ReadC())); }, 2)
            },
            {
                0xFA, new Instruction("SET 7,D", cpu => { cpu.WriteD(cpu.Set(7, cpu.ReadD())); }, 2)
            },
            {
                0xFB, new Instruction("SET 7,E", cpu => { cpu.WriteE(cpu.Set(7, cpu.ReadE())); }, 2)
            },
            {
                0xFC, new Instruction("SET 7,H", cpu => { cpu.WriteH(cpu.Set(7, cpu.ReadH())); }, 2)
            },
            {
                0xFD, new Instruction("SET 7,L", cpu => { cpu.WriteL(cpu.Set(7, cpu.ReadL())); }, 2)
            },
            {
                0xFE, new Instruction("SET 7,(HL)", cpu => { cpu.WriteMem8(cpu.ReadHL(), cpu.Set(7, cpu.ReadMem8(cpu.ReadHL()))); }, 2)
            },
            {
                0xFF, new Instruction("SET 7,A", cpu => { cpu.WriteA(cpu.Set(7, cpu.ReadA())); }, 2)
            }
        };

        private static void IncHL(CPU cpu)
        {
            cpu.WriteHL((ushort)(cpu.ReadHL() + 1));
        }

        private static void DecHL(CPU cpu)
        {
            cpu.WriteHL((ushort)(cpu.ReadHL() - 1));
        }

        public static Instruction FromByte(byte instruction)
        {
            return !_instructions.ContainsKey(instruction) ? null : _instructions[instruction];
        }

        public static Instruction FromPrefixByte(byte instruction)
        {
            return !_prefixInstructions.ContainsKey(instruction) ? null : _prefixInstructions[instruction];
        }

        public static string DisplayCurrentDebugInfo(CPU cpu)
        {
            var instructionByte = cpu.ReadMem8(cpu.ReadPC());
            var instruction = instructionByte == 0xCB
                ? FromPrefixByte(cpu.ReadMem8((ushort)(cpu.ReadPC() + 1)))
                : FromByte(instructionByte);
            if (instruction == null)
            {
                if (instructionByte == 0xCB)
                    MainWindow.Instance.Error("Instruction",
                        $"Prefix Instruction Unknown: 0x{cpu.ReadMem8((ushort)(cpu.ReadPC() + 1)):X} at 0x{cpu.ReadPC() + 1:X}");
                else
                    MainWindow.Instance.Error("Instruction", $"Instruction Unknown: 0x{instructionByte:X} at 0x{cpu.ReadPC():X}");
            }

            return $"PC: 0x{cpu.ReadPC():X} | Instruction: {instruction.Name}\n" +
                   $"A: 0x{cpu.ReadA():x2} | B: 0x{cpu.ReadB():x2} | C: 0x{cpu.ReadC():x2}\n" +
                   $"D: 0x{cpu.ReadD():x2} | E: 0x{cpu.ReadE():x2} | H: 0x{cpu.ReadH():x2}\n" +
                   $"L: 0x{cpu.ReadL():x2} | SP : 0x{cpu.ReadSP():x4} | F: 0x{cpu.ReadFlagsRegister():x2}\n" +
                   $"Boot Rom: {cpu.MemoryManager.Memory.IsBooting()} | DIV: {cpu.ReadMem8(0xFF04):x2} | TIMA: {cpu.ReadMem8(0xFF05):x2}\n" +
                   $"LY: {cpu.ReadMem8(0xFF44):x2} | SCY: {cpu.ReadMem8(0xFF42):x2} | IE: {cpu.ReadMem8(0xFFFF):x2} | IF: {cpu.ReadMem8(0xFF0F):x2} | IME: {cpu.ReadIme()}";
        }

        public class Instruction
        {
            public Instruction(string name, Action<CPU> action, byte length = 1)
            {
                Name = name;
                Action = action;
                Length = length;
            }

            public string Name { get; }
            private Action<CPU> Action { get; }
            private byte Length { get; }

            public ushort Invoke(CPU cpu)
            {
                Action.Invoke(cpu);
                return (ushort)(cpu.ReadPC() + Length);
            }
        }
    }
}