using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class Registers
    {
        public int AX { get; set; }
        public int BX { get; set; }
        public int CX { get; set; }
        public int DX { get; set; }
        public int EX { get; set; }

        public int IP { get; set; }
        public int SP { get; set; }
        public int CS { get; set; }

        public override string ToString()
        {
            return string.Format("AX: {0:X4}  BX: {1:X4} CX:{2:X4} DX:{3:X4} EX:{4:X4}  IP:{5:X4}  SP:{6:X4} CS:{7:X4}", AX, BX, CX, DX, EX, IP, SP, CS);
        }

    }
}
