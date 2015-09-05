using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCPU.Interfaces;

namespace TLCPU
{
    public class CPU
    {

        public static CPU Boot()
        {
            return new CPU(4096, 8192);
        }

        public static CPU Boot(int stacksize, int ramsize, int callStackSize = 128)
        {
            return new CPU(stacksize, ramsize, callStackSize);
        }

        public static CPU Boot(IOpcodes language, int stacksize, int ramsize, int callStackSize = 128)
        {
            var cpu = Boot(stacksize, ramsize, callStackSize);
            cpu.Language = language;
            return cpu;
        }

        public int StackSize { get; private set; }
        public int RAMSize { get; private set; }
        public int CallStackSize { get; private set; }
        public IOpcodes Language { get; private set; }
        public Flags Flags { get; private set; }

        protected CPU(int stacksize, int ramsize, int callStackSize = 128)
        {
            Language = new TLAssembly();
            RAMSize = ramsize;
            StackSize = stacksize;
            CallStackSize = callStackSize;

            Registers = new Registers();

            RAM = new int[RAMSize];
            Stack = new int[StackSize];
            CallStack = new int[CallStackSize];
            Flags = new Flags();

            Registers.SP = 0;
            Registers.IP = 0;
            ListenForInterrupt(op =>
            {
                if (op.Mnemonic == "END")
                {
                    Flags.IsRunning = false;
                }
            });
        }

        public Registers Registers { get; private set; }

        public int[] Stack { get; private set; }

        public int[] CallStack { get; private set; }

        public int[] RAM;

        public void Execute(int address)
        {
            Flags.IsRunning = true;
            Registers.IP = address;
            Console.WriteLine(Registers.ToString());
            Console.WriteLine("=============================");
            while (Flags.IsRunning)
            {
                RunCycle();
            }
            Console.WriteLine("=============================");
            Console.WriteLine(Registers.ToString());
        }

        public void RunCycle()
        {
            Opcode inst = null;
            try
            {
                inst = ReadInstruction();
                Console.Write(string.Format("{0:X4}: {1,-25}", Registers.IP, inst.ToString()));
                inst.Execute(this);
                Console.WriteLine("     ; " + this.ToString());
                if (Flags.Jumped)
                {
                    Flags.Jumped = false;
                }
                else
                {
                    Registers.IP += inst.Length;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (inst != null)
                {
                    var msg = string.Format("INST: {0} ", inst.Mnemonic) + Registers.ToString();
                    throw new Exception(msg, ex);
                }
                else
                {
                    throw;
                }
            }
        }

        protected Opcode ReadInstruction()
        {
            var code = RAM[Registers.IP];
            //var opcode = Opcodes.List.FirstOrDefault(c => c.Code == code) ?? TLAssembly.NOP;
            var opcode = Language.Find(code);
            //opcode.Data.Clear();
            //for (var i = 0; i < opcode.Length; i++)
            for (var i = 1; i < opcode.Length; i++)
            {
                //opcode.Data.Add(RAM[Registers.IP + i]);
                opcode.Parameters[i - 1].Value = RAM[Registers.IP + i];
            }
            return opcode;
        }

        public int Parameter(int index)
        {
            return RAM[Registers.IP + index];
        }

        public void Push(int value)
        {
            Stack[Registers.SP++] = value;
        }

        public int Pop()
        {
            return Stack[--Registers.SP];
        }

        protected void PushCall(int value)
        {
            CallStack[Registers.CS++] = value;
        }

        protected int PopCall()
        {
            return CallStack[--Registers.CS];
        }

        public void Jump(int address)
        {
            Flags.Jumped = true;
            Registers.IP = address;
        }

        public void Call(int returnAddress, int subRoutineAddress)
        {
            PushCall(returnAddress);
            Jump(subRoutineAddress);
        }

        public void Return()
        {
            Jump(PopCall());
        }

        private List<Action<Opcode>> InterruptHandlers = new List<Action<Opcode>>();

        public void ListenForInterrupt(Action<Opcode> handler)
        {
            InterruptHandlers.Add(handler);
        }

        public void Interrupt(Opcode code)
        {
            foreach (var handler in InterruptHandlers)
            {
                handler(code);
            }
        }

        public override string ToString()
        {
            var results = "REGISTERS: " + Registers.ToString() + "   FLAGS: " + Flags.ToString() + "   STACK HEAD: " + (Registers.SP == 0 ? "--" : Stack[Registers.SP - 1].ToString()); ;
            return results;
        }

    }

}
