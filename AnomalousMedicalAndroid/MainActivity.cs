﻿using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Anomalous.OSPlatform;
using Anomalous.OSPlatform.Android;
using Android.Views.InputMethods;
using Android.Content.PM;
using System.Collections.Generic;
using Android.Text;
using Engine;
using Medical;
using System.IO;

namespace AndroidBaseApp
{
	[Activity (Label = "Anomalous Medical", MainLauncher = true, Icon = "@drawable/icon", Theme="@android:style/Theme.NoTitleBar.Fullscreen", 
		ConfigurationChanges= ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout,
		WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
	[MetaData("android.app.lib_name", Value = AndroidPlatformPlugin.LibraryName)]
	public class MainActivity : AndroidActivity
	{
		static MainActivity()
		{
			Java.Lang.JavaSystem.LoadLibrary ("openal");
		}

		public MainActivity()
			:base(AnomalousMedicalAndroid.Resource.Layout.Main, AnomalousMedicalAndroid.Resource.Id.editText1)
		{

		}

		protected override void createApp ()
		{
			NativePlatformPlugin.StaticInitialize();

			#if DEBUG
			Logging.Log.Default.addLogListener (new Logging.LogConsoleListener ());
			#endif

			OtherProcessManager.OpenUrlInBrowserOverride = openUrl;

			String obbWildcard = String.Format ("main.*.{0}.obb", BaseContext.ApplicationInfo.PackageName.ToString ());

			var anomalous = new AnomalousController()
			{
				PrimaryArchive = Directory.EnumerateFiles(Application.Context.ObbDir.AbsolutePath, obbWildcard, SearchOption.AllDirectories).FirstOrDefault()
			};
			anomalous.OnInitCompleted += HandleOnInitCompleted;
			anomalous.run();
		}

		void HandleOnInitCompleted (AnomalousController anomalousController, StandaloneController controller)
		{
			setInputHandler(controller.MedicalController.InputHandler);
		}

		void openUrl(String url)
		{
			Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
			this.StartActivity(intent);
		}
	}
}

