using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Write options.
    /// </summary>
    public class WriteOptions : BaseOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.WriteOptions"/> class.
        /// </summary>
        public WriteOptions() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.WriteOptions"/> class.
        /// </summary>
        /// <param name="ttl">Ttl.</param>
        public WriteOptions(int ttl): base(ttl)
        {
          
        }

        /// <summary>
        /// Creates the infinite cache options.
        /// </summary>
        /// <returns>The infinite cache options.</returns>
        public static WriteOptions CreateInfiniteCacheOptions()
        {
            return new WriteOptions(BaseOptions.INFINITE);
        }

        /// <summary>
        /// Creates the no cache options.
        /// </summary>
        /// <returns>The no cache options.</returns>
        public static WriteOptions CreateNoCacheOptions()
        {
            return new WriteOptions(BaseOptions.NO_CACHE);
        }
    }
}