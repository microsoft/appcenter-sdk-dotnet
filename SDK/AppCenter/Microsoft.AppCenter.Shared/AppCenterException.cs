// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// Represents App Center's errors that occur during execution.
    /// </summary>
    public class AppCenterException : Exception
    {
        /// <summary>
        /// Initializes a new instance with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AppCenterException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public AppCenterException(string message, Exception innerException) : base(message, innerException) { }
    }
}
