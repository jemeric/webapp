using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings
{
    public class AppClock
    {
        public long OffsetInMillis { get; }
        public DateTime CurrentTime {
            get {  return DateTime.Now.AddMilliseconds(OffsetInMillis); }
        }
        public AppClock(long offsetInMillis)
        {
            OffsetInMillis = offsetInMillis;
        }

    }
}
