using System;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Crashes.Ingestion.Models;

namespace Contoso.Forms.Test.TizenMobile
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            ErrorReportPageUpdateCallback.RequestUpdate = () =>
            {
                Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
                {
                    ErrorReportPageUpdateCallback.Update(task.Result);
                });
            };

            base.OnCreate();
            MobileCenter.Configure("6825bcfe-3582-4d4a-b3a7-fe06154414a4");
            LoadApplication(new App());
        }

        static void Main(string[] args)
        {
            var app = new Program();
            global::Xamarin.Forms.Platform.Tizen.Forms.Init(app);
            app.Run(args);
        }
    }
}
