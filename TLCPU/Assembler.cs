using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class Assembler
    {

        public const string LabelFlag = ":";

        protected class Label
        {
            public Label()
            {
                References = new List<int>();
            }
            public string Name { get; set; }
            public int Address { get; set; }
            public List<int> References { get; set; }
        }
        protected CPU _cpu;
        protected List<Label> Labels = new List<Label>();

        public bool TraceOutput { get; set; }

        public Assembler(CPU cpu)
        {
            _cpu = cpu;
            TraceOutput = false;
        }

        public int Assemble(string program, int address = 0)
        {
            var lines = program.Split(Environment.NewLine.ToCharArray());
            Labels = new List<Label>();
            var currentAddress = address;
            foreach (var line in lines)
            {
                Trace(";" + line);
                currentAddress = Translate(line, currentAddress);
            }
            if (Labels.Any())
            {
                Trace("\n\nLABELS");
                foreach (var label in Labels)
                {
                    Trace("   {0,-25}{1,7}", label.Name, label.Address);
                    foreach (var addr in label.References)
                    {
                        _cpu.RAM[addr] = label.Address;
                    }
                }
            }
            return currentAddress;
        }

        public string Disassemble(int startAddress = 0, int endAddress = 0, IDictionary<int, string> knownLabels = null, DisassemblyOptions options = null)
        {
            var results = string.Empty;
            options = options ?? new DisassemblyOptions();
            var labels = knownLabels ?? new Dictionary<int, string>();

            if (endAddress == 0) { endAddress = _cpu.RAM.Length; }
            var currentAddress = startAddress;
            while (currentAddress < endAddress)
            {
                if (labels.ContainsKey(currentAddress))
                {
                    results += ":" + labels[currentAddress] + Environment.NewLine;
                }
                results += Disassemble(ref currentAddress, labels, options);
            }
            return results;
        }

        public string Disassemble(ref int address, IDictionary<int, string> knownLabels = null, DisassemblyOptions options = null)
        {
            var labels = knownLabels ?? new Dictionary<int, string>();
            options = options ?? new DisassemblyOptions();
            var results = options.IncludeInitialAddress ? string.Format("\t{0:X4}: ", address) : "\t";

            var instruction = _cpu.RAM[address];
            var opcode = Opcodes.List.FirstOrDefault(o => o.Code == instruction);
            if (opcode == null)
            {
                results += string.Format("!0x{0:X4}", instruction);
                address += 1;
            }
            else
            {
                results += string.Format("{0, -6}", opcode.Mnemonic);
                var idx = 1;
                foreach (var parm in opcode.Parameters)
                {
                    if (parm.ParameterType == ParameterType.Address)
                    {
                        var addr = _cpu.RAM[address + idx];
                        if (labels.ContainsKey(addr))
                        {
                            results += string.Format(" :{0}", labels[addr]);
                        }
                        else
                        {
                            results += string.Format(" {0:X4}", addr);
                        }
                    }
                    else
                    {
                        results += string.Format(" {0:X4}", _cpu.RAM[address + idx]);
                    }
                    idx++;
                }
                //			for (int i = 1; i < opcode.Length; i++) {
                //				results += string.Format(" {0:X4}", _cpu.RAM[address + i]);
                //			}
                results = string.Format("{0,-50};{1}", results, opcode.Description);
                address += opcode.Length;
            }
            return results + Environment.NewLine;
        }

        #region Private

        protected Label GetLabel(string name)
        {
            var label = Labels.FirstOrDefault(l => l.Name == name);
            if (label == null)
            {
                label = new Label { Name = name };
                Labels.Add(label);
            }
            return label;
        }

        protected int Translate(string line, int addr)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line)) return addr;
            var source = (line.Contains(";") ? line.Split(';')[0] : line).Trim();
            var comment = (line.Contains(";") ? line.Split(';')[1] : string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(source))
            {
                if (!string.IsNullOrWhiteSpace(comment)) Console.WriteLine(comment);
                return addr;
            }

            //Label Flags						
            if (source.StartsWith(LabelFlag))
            {
                var name = source.Trim();
                var label = GetLabel(name);
                label.Address = addr;
                return addr;
            }

            if (source.StartsWith("!"))
            {
                var codes = source.Trim().Replace("!", string.Empty);
                var values = codes.Split(',');
                foreach (var stringValue in values.Select(v => v.Trim()))
                {
                    var value = Convert.ToInt32(stringValue);
                    _cpu.RAM[addr++] = value;
                }
                return addr;
            }

            var parts = source.Split(' ').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

            var opcode = Opcodes.List.FirstOrDefault(o => o.Mnemonic == parts[0].ToUpperInvariant());
            if (opcode == null) throw new SyntaxErrorException("Syntax error: " + source);
            if (opcode.Length == parts.Length)
            {
                parts[0] = opcode.Code.ToString();
                foreach (var part in parts.Select(p => p.Trim()))
                {
                    if (part.StartsWith(LabelFlag))
                    {
                        var label = GetLabel(part);
                        label.References.Add(addr++);
                    }
                    else
                    {
                        _cpu.RAM[addr++] = Convert.ToInt32(part);
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException(string.Format("Parameter mismatch: {0}; Expected {1} parameters; found {2} ({3}).", source, opcode.Length - 1, parts.Length - 1, line));
            }
            return addr;
        }

        protected void Trace(string format, params object[] data)
        {
            if (TraceOutput) Console.WriteLine(format, data);
        }

        #endregion
    }

}
