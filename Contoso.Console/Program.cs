using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
namespace Contoso
{
    class Program
    {
        static void Main(string[] args)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start("66daf822-7957-4e78-8f8f-cdf7b8510a17", typeof(Analytics));
            Analytics.TrackEvent("Herro");
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
