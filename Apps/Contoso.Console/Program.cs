using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace Contoso
{
	class Program
	{
		private static void TrackEvent()
		{
			Console.Clear();
			Console.WriteLine("Tracking event. Input name:");
			var eventName = Console.ReadLine();
			Analytics.TrackEvent(eventName);
		}

		private static async void PrintStatus()
		{
			Console.WriteLine($"Analytics:\t{(await Analytics.IsEnabledAsync() ? "ON" : "OFF")}");
		}

		private static async void PrintMenu()
		{
			Console.Clear();
			Console.WriteLine("Contoso.Console application.");
			Console.WriteLine("Menu:");
			PrintStatus();
			Console.WriteLine($"a:\tChange Analytics state to {(await Analytics.IsEnabledAsync() ? "OFF" : "ON")}");
			Console.WriteLine("e:\tTrack event");
			Console.WriteLine("q:\tQuit");
		}

		static void Main(string[] args)
		{
			AppCenter.LogLevel = LogLevel.Verbose;
			AppCenter.Start("66daf822-7957-4e78-8f8f-cdf7b8510a17", typeof(Analytics));
			
			while (true)
			{
				PrintMenu();
				var selection = Console.ReadKey();
				switch (selection.KeyChar)
				{
					case 'e':
					case 'E':
						TrackEvent();
						break;
					case 'a':
					case 'A':
						Analytics.SetEnabledAsync(!Analytics.IsEnabledAsync().GetAwaiter().GetResult());
						break;
					case 'q':
					case 'Q':
						Environment.Exit(0);
						break;
				}
			}
		}
	}
}