using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webapp.Util.Dto
{
    public class NPMExternal
    {
        [JsonProperty("global")]
        public string Global { get; set;  }
        [JsonProperty("module")]
        public string Module { get; set; }
        [JsonProperty("assets")]
        public NPMAsset[] Assets { get; set; }
    }
}
