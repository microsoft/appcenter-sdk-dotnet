using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public class BaseOptions
    {
     
        /// <summary>
        /// Cache does not expire.
        /// </summary>
        public static int INFINITE = -1;

        /// <summary>
        ///  Do not cache documents locally.
        /// </summary>
        public static int NO_CACHE = 0;

        /// <summary>
        /// Default caching value of one day.
        /// </summary>
        public static int DEFAULT_EXPIRATION_IN_SECONDS = 60 * 60 * 24;

        private int mTtl;

        public BaseOptions() : this(DEFAULT_EXPIRATION_IN_SECONDS)
        {

        }

        public BaseOptions(int ttl)
        {
            if (ttl < -1)
            {
                throw new Exception("Time-to-live should be greater than or equal to zero, or -1 for infinite.");
            }
            this.mTtl = ttl;
        }

        /// <summary>
        /// Gets the device time to live.
        /// </summary>
        /// <returns>document time-to-live in seconds (default to one day)..</returns>
        public int GetDeviceTimeToLive()
        {
            return mTtl;
        }
    }
}