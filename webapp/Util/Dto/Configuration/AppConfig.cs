using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util.Dto.Configuration
{
    public class AppConfig
    {
        public AssetsConfig Assets { get; set; }
        public S3Config S3 { get; set; }
        public AuthorizationConfig Authorization { get; set; }
        public bool IsDistributed { get; set; }
        public string AppHostUrl { get; set; }
    }
}
