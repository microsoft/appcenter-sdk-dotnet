using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Read options.
    /// </summary>
    public class ReadOptions : BaseOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.ReadOptions"/> class.
        /// </summary>
        public ReadOptions() : base()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.ReadOptions"/> class.
        /// </summary>
        /// <param name="ttl">Ttl.</param>
        public ReadOptions(int ttl) : base(ttl)
        {
            
        }

        /// <summary>
        /// Creates the infinite cache options.
        /// </summary>
        /// <returns>The infinite cache options.</returns>
        public static ReadOptions CreateInfiniteCacheOptions()
        {
            return new ReadOptions(BaseOptions.INFINITE);
        }

        /// <summary>
        /// Creates the no cache options.
        /// </summary>
        /// <returns>The no cache options.</returns>
        public static ReadOptions CreateNoCacheOptions()
        {
            return new ReadOptions(BaseOptions.NO_CACHE);
        }

  
        /// <summary>
        /// Ises the expired.
        /// </summary>
        /// <returns>Whether a document is expired.</returns>
        /// <param name="expiredAt">ExpiredAt timestamp of when the document is expired.</param>
        public static Boolean IsExpired(long expiredAt)
        {
            if (expiredAt == BaseOptions.INFINITE)
            {
                return false;
            }
            return DateTime.Now.Millisecond >= expiredAt;
        }
    }
}