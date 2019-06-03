using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util.Dto.Configuration
{
    public class S3Configuration
    {
        public String Host { get; set; }
        public String Bucket { get; set; }
        public String AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
