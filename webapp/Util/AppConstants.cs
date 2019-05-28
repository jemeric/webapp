using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util
{
    public static class AppConstants
    {
        public static class CacheKeys
        {
            public const string lastUpdatedVersion = "availableVersion";
            public const string publishedVersion = "publishedVersion";
            public const string previouslyPublishedVersion = "prevPublishedVersion";
        }
    }
}
