using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings;
using webapp.Util;
using webapp.Util.Extensions;

namespace webapp.Services
{
    public class SettingsService
    {
        private readonly IDistributedCache distributedCache;

        public SettingsService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public DateTime GetNow()
        {
            // TODO - allow this to be offset
            return new DateTime();
        }

        public async Task<AppClock> SetClockOffset(long clockOffsetInMillis)
        {
            await distributedCache.SetAsync<long>(AppConstants.CacheKeys.clockOffsetInMillis, clockOffsetInMillis, new DistributedCacheEntryOptions());
            return new AppClock(clockOffsetInMillis);
        }

        public async Task<long> GetClockOffset()
        {
            // default of long? would be nullable, default of long is 0 (which is fine in this case)
            return await distributedCache.GetAsync<long>(AppConstants.CacheKeys.clockOffsetInMillis);
        }

        public async Task<AppClock> GetClock()
        {
            // TO-DO - get from request context if available
            long offset = await GetClockOffset();
            return new AppClock(offset);
        }
    }
}
