using System;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace Contoso.Forms.Test.TizenMobile
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            LoadApplication(new App());

            try
            {
                MobileCenter.LogLevel = LogLevel.Verbose;
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "STARTING MOBILE CENTER API");
                MobileCenter.Start("6825bcfe-3582-4d4a-b3a7-fe06154414a4", typeof(Analytics), typeof(Crashes));
            }
            catch (System.Exception exc)
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"{exc}");
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            global::Xamarin.Forms.Platform.Tizen.Forms.Init(app);
            app.Run(args);
        }
    }
}
