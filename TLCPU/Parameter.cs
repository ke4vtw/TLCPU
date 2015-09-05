using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCPU
{
    public class Parameter
    {

        public Parameter(string name, ParameterType type, string desc, int value = 0)
        {
            Name = name;
            ParameterType = type;
            Description = desc;
            Value = value;
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("type")]
        public ParameterType ParameterType { get; private set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("desc")]
        public string Description { get; private set; }

    }
}
