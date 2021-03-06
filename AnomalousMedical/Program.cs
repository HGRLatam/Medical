﻿using Anomalous.OSPlatform;
using Anomalous.OSPlatform.Win32;
using DentalSim;
using Lecture;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Medical
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            WindowsRuntimePlatformInfo.Initialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;

            AnomalousController anomalous = null;
            try
            {
                anomalous = new AnomalousController();
                anomalous.AddAdditionalPlugins += HandleAddAdditionalPlugins;
                anomalous.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (anomalous != null)
                {
                    anomalous.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                MessageDialog.showErrorDialog(errorMessage, "Exception");
            }
            finally
            {
                if (anomalous != null)
                {
                    anomalous.Dispose();
                }
            }
        }

        static void HandleAddAdditionalPlugins(AnomalousController anomalousController, StandaloneController controller)
        {
            controller.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(controller)
            {
                AllowUninstall = false
            });

            controller.AtlasPluginManager.addPlugin(new DentalSimPlugin()
            {
                AllowUninstall = false
            });

            controller.AtlasPluginManager.addPlugin(new LecturePlugin()
            {
                AllowUninstall = false
            });

            controller.AtlasPluginManager.addPlugin(new Movement.MovementBodyAtlasPlugin()
            {
                AllowUninstall = false
            });
            controller.AtlasPluginManager.addPlugin(new Developer.DeveloperAtlasPlugin(controller)
            {
                AllowUninstall = false
            });
            controller.AtlasPluginManager.addPlugin(new EditorPlugin()
            {
                AllowUninstall = false
            });
        }
    }
}
