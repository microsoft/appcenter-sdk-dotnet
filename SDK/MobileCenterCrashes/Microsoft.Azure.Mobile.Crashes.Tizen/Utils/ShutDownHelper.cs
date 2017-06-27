using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Azure.Mobile.Crashes.Utils
{
    public class ShutDownHelper
    {
        public static void ShutDown()
        {

            // TODO TIZEN look into error codes and proper shutdown process
            Process.GetCurrentProcess().Close();
            System.Environment.Exit(1);
        }
    }
}
