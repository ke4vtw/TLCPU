using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCPU.Interfaces;

namespace TLCPU
{

    public class TLAssembly : IOpcodes
    {

        public List<Opcode> List { get; set; }

        public TLAssembly()
        {

            List = new List<Opcode>();
            var code = 0;

            List.Add(NOP = new Opcode(code++, "NOP", "No Operation. Do nothing."));

            code = 100;

            #region Load Registers
            List.Add(LODAX = new Opcode(code++, "LODAX", "Load AX register",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Registers.AX = cpu.Parameter(1)));

            List.Add(LODBX = new Opcode(code++, "LODBX", "Load BX register",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Registers.BX = cpu.Parameter(1)));

            List.Add(LODCX = new Opcode(code++, "LODCX", "Load CX register",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Registers.CX = cpu.Parameter(1)));

            List.Add(LODDX = new Opcode(code++, "LODDX", "Load DX register",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Registers.DX = cpu.Parameter(1)));

            List.Add(LODEX = new Opcode(code++, "LODEX", "Load EX register",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Registers.EX = cpu.Parameter(1)));
            #endregion

            #region Comparisons
            List.Add(CMP = new Opcode(code++, "CMP", "Compare Specified Locations",
                new[] { new Parameter("LValue", ParameterType.Value, "LValue"), new Parameter("RValue", ParameterType.Value, "RValue") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.RAM[cpu.Parameter(2)]));
            List.Add(CMPAX = new Opcode(code++, "CMPAX", "Compare AX to specified location.",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.Registers.AX));
            List.Add(CMPBX = new Opcode(code++, "CMPBX", "Compare AX to specified location.",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.Registers.BX));
            List.Add(CMPCX = new Opcode(code++, "CMPCX", "Compare AX to specified location.",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.Registers.CX));
            List.Add(CMPDX = new Opcode(code++, "CMPDX", "Compare AX to specified location.",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.Registers.DX));
            List.Add(CMPEX = new Opcode(code++, "CMPEX", "Compare AX to specified location.",
                new[] { new Parameter("Value", ParameterType.Value, "Value") },
                cpu => cpu.Flags.Compare = cpu.RAM[cpu.Parameter(1)] == cpu.Registers.EX));
            #endregion

            #region Jumps
            // Subtract two from these jumps, because the CPU will add 2 for the instruction itself.
            List.Add(GOTO = new Opcode(code++, "GOTO", "Jump to the specified address.",
                new[] { new Parameter("Address", ParameterType.Address, "Value") },
                cpu => cpu.Jump(cpu.Parameter(1))));

            List.Add(JMPEQ = new Opcode(code++, "JMPEQ", "Jump to the specified address if compare is true.",
                new[] { new Parameter("Address", ParameterType.Address, "Value") },
                cpu => { if (cpu.Flags.Compare) { cpu.Jump(cpu.Parameter(1)); } }));

            List.Add(JMPNE = new Opcode(code++, "JMPNE", "Jump to the specified address if compare is false.",
                new[] { new Parameter("Address", ParameterType.Address, "Value") },
                cpu => { if (!cpu.Flags.Compare) { cpu.Jump(cpu.Parameter(1)); } }));

            List.Add(LOOP = new Opcode(code++, "LOOP", "If CX > 0, dec cx and jump to specified address",
                new[] { new Parameter("Address", ParameterType.Address, "Value") },
                cpu => { if (cpu.Registers.CX > 0) { cpu.Registers.CX--; cpu.Jump(cpu.Parameter(1)); } }));

            List.Add(CALL = new Opcode(code++, "CALL", "Push Reg.IP and jump to the specified address.",
                new[] { new Parameter("Address", ParameterType.Address, "Value") },
                cpu => { cpu.Call(cpu.Registers.IP + 2, cpu.Parameter(1)); }));

            List.Add(RETR = new Opcode(code++, "RETR", "Pop address pushed by CALL and resume processing.",
                operation: cpu => { cpu.Return(); }));
            #endregion

            #region Stack Manipulation
            List.Add(PUSH = new Opcode(code++, "PUSH", "Push /parm/ to the stack.", new[] { new Parameter("Value", ParameterType.Value, "Value") }, cpu => cpu.Push(cpu.Parameter(1))));
            List.Add(PUSHAX = new Opcode(code++, "PUSHAX", "Push AX to the stack.", operation: cpu => cpu.Push(cpu.Registers.AX)));
            List.Add(PUSHBX = new Opcode(code++, "PUSHBX", "Push BX to the stack.", operation: cpu => cpu.Push(cpu.Registers.BX)));
            List.Add(PUSHCX = new Opcode(code++, "PUSHCX", "Push CX to the stack.", operation: cpu => cpu.Push(cpu.Registers.CX)));
            List.Add(PUSHDX = new Opcode(code++, "PUSHDX", "Push DX to the stack.", operation: cpu => cpu.Push(cpu.Registers.DX)));
            List.Add(PUSHEX = new Opcode(code++, "PUSHEX", "Push EX to the stack.", operation: cpu => cpu.Push(cpu.Registers.EX)));

            List.Add(POP = new Opcode(code++, "POP", "Pop value on stack to memory address.", new[] { new Parameter("Value", ParameterType.Value, "Value") }, cpu => cpu.RAM[cpu.Parameter(1)] = cpu.Pop()));
            List.Add(POPAX = new Opcode(code++, "POPAX", "Pop value on stack to AX.", operation: cpu => cpu.Registers.AX = cpu.Pop()));
            List.Add(POPBX = new Opcode(code++, "POPBX", "Pop value on stack to BX.", operation: cpu => cpu.Registers.BX = cpu.Pop()));
            List.Add(POPCX = new Opcode(code++, "POPCX", "Pop value on stack to CX.", operation: cpu => cpu.Registers.CX = cpu.Pop()));
            List.Add(POPDX = new Opcode(code++, "POPDX", "Pop value on stack to DX.", operation: cpu => cpu.Registers.DX = cpu.Pop()));
            List.Add(POPEX = new Opcode(code++, "POPEX", "Pop value on stack to EX.", operation: cpu => cpu.Registers.EX = cpu.Pop()));

            List.Add(SADD = new Opcode(code++, "SADD", "Pop two stack values, add together, and push back to stack.", operation: cpu => cpu.Push(cpu.Pop() + cpu.Pop())));
            List.Add(SSUB = new Opcode(code++, "SSUB", "Pop two stack values, subtract the first pushed from the last, and push back to stack.", operation: cpu => cpu.Push(cpu.Pop() - cpu.Pop())));
            List.Add(SMUL = new Opcode(code++, "SMUL", "Pop two stack values, multiply, and push back to stack.", operation: cpu => cpu.Push(cpu.Pop() * cpu.Pop())));
            #endregion

            List.Add(END = new Opcode(65535, "END", "Exit CPU", operation: cpu => cpu.Interrupt(TLAssembly.END)));

        }

        public Opcode Find(string sourceLine)
        {
            var parts = sourceLine.Split(' ').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            var code = parts[0] + ".";
            for (var i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith(":"))
                {
                    code += 'A';
                } else
                {
                    code += 'V';
                }
            }

            return List.FirstOrDefault(p => p.UniqueID == code) ?? NOP;
        }

        public Opcode Find(int code)
        {
            return List.FirstOrDefault(p => p.Code == code) ?? NOP;
        }

        #region Properties
        public static Opcode NOP { get; private set; }

        public static Opcode LODAX { get; private set; }
        public static Opcode LODBX { get; private set; }
        public static Opcode LODCX { get; private set; }
        public static Opcode LODDX { get; private set; }
        public static Opcode LODEX { get; private set; }

        public static Opcode GOTO { get; private set; }
        public static Opcode JMPEQ { get; private set; }
        public static Opcode JMPNE { get; private set; }

        public static Opcode CMP { get; private set; }
        public static Opcode CMPAX { get; private set; }
        public static Opcode CMPBX { get; private set; }
        public static Opcode CMPCX { get; private set; }
        public static Opcode CMPDX { get; private set; }
        public static Opcode CMPEX { get; private set; }

        public static Opcode CALL { get; private set; }
        public static Opcode RETR { get; private set; }

        public static Opcode LOOP { get; private set; }

        public static Opcode PUSH { get; private set; }
        public static Opcode PUSHAX { get; private set; }
        public static Opcode PUSHBX { get; private set; }
        public static Opcode PUSHCX { get; private set; }
        public static Opcode PUSHDX { get; private set; }
        public static Opcode PUSHEX { get; private set; }

        public static Opcode POP { get; private set; }
        public static Opcode POPAX { get; private set; }
        public static Opcode POPBX { get; private set; }
        public static Opcode POPCX { get; private set; }
        public static Opcode POPDX { get; private set; }
        public static Opcode POPEX { get; private set; }

        public static Opcode SADD { get; private set; }
        public static Opcode SSUB { get; private set; }
        public static Opcode SMUL { get; private set; }

        public static Opcode END { get; private set; }
        #endregion
    }
}
