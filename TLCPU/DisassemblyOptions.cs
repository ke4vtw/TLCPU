using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class DisassemblyOptions
    {
        public DisassemblyOptions()
        {
            IncludeInitialAddress = true;
        }
        public bool IncludeInitialAddress { get; set; }
    }
}
