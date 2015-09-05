using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class Flags
    {

        public Flags()
        {
            IsRunning = false;
            Compare = false;
            Jumped = false;
        }

        public bool IsRunning { get; set; }
        public bool Compare { get; set; }
        public bool Jumped { get; set; }

        public override string ToString()
        {
            var results = "|";
            results += string.Format("EXE: {0} | ", IsRunning ? "1" : "0");
            results += string.Format("CMP: {0} |", Compare ? "1" : "0");
            results += string.Format("JMP: {0} |", Jumped ? "1" : "0");
            return results;
        }

    }
}
