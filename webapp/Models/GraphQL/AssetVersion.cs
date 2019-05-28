using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.GraphQL
{
    public class AssetVersion
    {
        public string Version { get; }
        public DateTime DateUpdated { get; }

        public AssetVersion(string version, DateTime dateUpdated)
        {
            Version = version;
            DateUpdated = dateUpdated;
        }
    }
}
