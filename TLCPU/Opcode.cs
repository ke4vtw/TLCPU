using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class Opcode
    {

        private static Action<CPU> NoAction = (cpu) => { };

        public Opcode(int code, string mnemonic, string description = "", IEnumerable<Parameter> parms = null, Action<CPU> operation = null)
        {
            Code = code;
            Mnemonic = mnemonic;
            Parameters = (parms ?? new List<Parameter>()).ToList();
            Description = description;
            Execute = operation ?? NoAction;
            //Data = new List<int>();
        }

        [JsonProperty("code")]
        public int Code { get; private set; }

        [JsonProperty("mnemonic")]
        public string Mnemonic { get; private set; }

        [JsonProperty("length")]
        public int Length
        {
            get
            {
                return Parameters.Count + 1; // Include OpCode
            }
        }

        [JsonProperty("desc")]
        public string Description { get; private set; }

        [JsonProperty("parameters")]
        public IList<Parameter> Parameters { get; private set; }

        [JsonIgnore]
        public string UniqueID { 
            get {
                var parmCodes = string.Empty;
                foreach (var p in Parameters)
                {
                    parmCodes += p.ParameterType.ToString().Substring(0, 1);
                }
               return string.Format("{0}.{1}", Mnemonic, parmCodes);
            }
        }

        [JsonIgnore]
        public Action<CPU> Execute { get; private set; }

        public override string ToString()
        {
            var results = Mnemonic + " ";
            //foreach (var d in Data.Skip(1))
            foreach (var d in Parameters)
            {
                results += d.Name + ":" + d.Value + " ";
            }
            return results;
        }

    }
}
