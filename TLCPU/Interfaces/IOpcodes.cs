using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU.Interfaces
{
    public interface IOpcodes
    {
        List<Opcode> List { get; }
        
        Opcode Find(string sourceLine);
        Opcode Find(int code);
    }
}
