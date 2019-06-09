using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Assets
{
    [Serializable]
    public class AssetVersion : IEquatable<AssetVersion>, IComparable<AssetVersion>
    {
        public string Version { get; }
        public DateTime DateUpdated { get; }

        public AssetVersion(string version, DateTime dateUpdated)
        {
            Version = version;
            DateUpdated = dateUpdated;
        }

        public bool Equals(AssetVersion other)
        {
            if (other == null) return false;
            return Version.Equals(other.Version);
        }

        public int CompareTo(AssetVersion other)
        {
            // null value means this object is greater
            if (other == null) return 1;
            return Version.CompareTo(other.Version);
        }
    }
}
