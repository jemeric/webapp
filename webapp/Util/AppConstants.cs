using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util
{
    public static class AppConstants
    {
        public const string webRoot = "ClientApp/dist";
        public static class CacheKeys
        {
            public const string lastUpdatedVersion = "availableVersion";
            public const string publishedVersion = "publishedVersion";
            public const string previouslyPublishedVersion = "prevPublishedVersion";
            public const string clockOffsetInMillis = "clockOffsetInMillis";
            public const string isCDNEnabled = "isCDNEnabled";
        }
    }
}
