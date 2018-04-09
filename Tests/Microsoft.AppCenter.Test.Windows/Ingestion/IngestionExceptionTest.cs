using System;
using Microsoft.AppCenter.Ingestion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows.Ingestion
{
    [TestClass]
    public class IngestionExceptionTest
    {
        /// <summary>
        /// Validate that exception message is saving
        /// </summary>
        [TestMethod]
        public void CheckMessageError()
        {
            var exceptionMessage = "Test exception message";
            var ingestionException = new IngestionException(exceptionMessage);

            Assert.AreEqual(exceptionMessage, ingestionException.Message);
        }

        /// <summary>
        /// Validate that exception is saving as an internal exception
        /// </summary>
        [TestMethod]
        public void CheckInternalError()
        {
            var exceptionMessage = "Test exception message";
            var internalException = new Exception(exceptionMessage);
            var ingestionException = new IngestionException(internalException);

            Assert.AreSame(internalException, ingestionException.InnerException);
            Assert.AreEqual(exceptionMessage, ingestionException.InnerException.Message);
        }
    }
}
