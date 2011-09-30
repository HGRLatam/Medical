﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class PluginDownload : Download
    {
        LicenseManager licenseManager;
        AtlasPluginManager atlasPluginManager;
        DownloadListener downloadListener;

        public PluginDownload(int pluginId, DownloadListener downloadListener, LicenseManager licenseManager, AtlasPluginManager atlasPluginManager)
        {
            this.PluginId = pluginId;
            this.licenseManager = licenseManager;
            this.atlasPluginManager = atlasPluginManager;
            this.downloadListener = downloadListener;
        }

        public override void completed(bool success)
        {
            this.Successful = success;
            if (success)
            {
                if (!licenseManager.allowFeature(PluginId) && !licenseManager.getNewLicense())
                {
                    ThreadManager.invoke(new Action(licenseServerReadFail));
                }
                else
                {
                    //Load plugin back on main thread
                    ThreadManager.invoke(new Action(delegate()
                    {
                        atlasPluginManager.addPlugin(Path.Combine(DestinationFolder, FileName));
                        atlasPluginManager.initialzePlugins();
                    }));
                }
            }
            ThreadManager.invoke(new Action(delegate()
            {
                downloadListener.downloadCompleted(this);
            }));
        }

        public override void updateStatus()
        {
            ThreadManager.invoke(new Action(delegate()
            {
                downloadListener.updateStatus(this);
            }));
        }

        public long PluginId { get; set; }

        void licenseServerReadFail()
        {
            MessageBox.show("There was an problem getting a new license. Please restart the program to use your new plugin.", "License Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        }

        public override string DestinationFolder
        {
            get
            {
                return MedicalConfig.PluginConfig.PluginsFolder;
            }
        }

        public override string Type
        {
            get
            {
                return "Plugin";
            }
        }

        public override string AdditionalArgs
        {
            get
            {
                return "pluginId=" + PluginId.ToString();
            }
        }
    }
}
