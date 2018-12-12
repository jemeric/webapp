using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webapp.Util.Dto
{
    public class NPMAsset
    {
        [JsonProperty("productionPath")]
        public string ProductionPath { get; set; }
        [JsonProperty("developmentPath")]
        public string DevelopmentPath { get; set; }
    }
}
