using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Assets
{
    [Serializable]
    public class AssetVersion
    {
        public string Version { get; }
        public DateTime DateUpdated { get; }

        public AssetVersion(string version, DateTime dateUpdated)
        {
            Version = version;
            DateUpdated = dateUpdated;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                AssetVersion v = (AssetVersion)obj;
                return Version.Equals(v.Version);
            }
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }
    }
}
