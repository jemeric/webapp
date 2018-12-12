using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webapp.Util.Dto
{
    public class NPMExternal
    {
        [JsonProperty("key")]
        public string Key { get; set;  }
        [JsonProperty("package")]
        public string Package { get; set; }
        [JsonProperty("assets")]
        public NPMAsset[] Assets { get; set; }
    }
}
