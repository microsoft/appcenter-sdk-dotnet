// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{

    /**
    * A listener that is going to be notified about remote operations completion status.
    */
    public class DocumentError
    {

        /**
         * @param exception that occurred during the network state change.
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.DocumentError"/> class.
        /// </summary>
        /// <param name="exception">exception that occurred during the network state change..</param>
        public DocumentError(object exception)
        {
            Throwable = exception;
        }

        /// <summary>
        /// Gets the throwable.
        /// </summary>
        /// <value>Underlying exception.</value>
        public object Throwable { get; }
    }
}