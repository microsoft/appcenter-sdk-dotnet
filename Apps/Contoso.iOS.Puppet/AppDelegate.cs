﻿using Foundation;
using UIKit;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System;

namespace Contoso.iOS.Puppet
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            MobileCenter.LogLevel = LogLevel.Verbose;
<<<<<<< HEAD
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Start("b889c4f2-9ac2-4e2e-ae16-dae54f2c5899", typeof(Analytics), typeof(Crashes));
=======

            //MobileCenter.SetLogUrl("http://in-integration.dev.avalanch.es:8081");
            MobileCenter.Start("b5ac8947-ee9d-4f05-8009-426687eb2381", typeof(Analytics), typeof(Crashes));

>>>>>>> 9f6978352ecf7bcc3ea454acc5a8d9a5ea36d08c
            try
            {
                ThrowAnException();
            }
            catch (Exception e)
            {
                MobileCenterLog.Verbose("THETAG", "THEMESSAGE", e);
            }

            Analytics.Enabled = true;
            System.Diagnostics.Debug.WriteLine("ANALYTICS: " + Analytics.Enabled.ToString());
            return true;
        }
        private void ThrowAnException()
        {
            throw new Exception();
        }
        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

