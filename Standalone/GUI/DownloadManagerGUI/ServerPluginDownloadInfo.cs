﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Medical.Controller;
using Engine.Threads;

namespace Medical.GUI
{
    class ServerPluginDownloadInfo : ServerDownloadInfo
    {
        private DownloadManagerServer server;
        private String description = null;
        private bool readingDescriptionFromServer = false;

        public ServerPluginDownloadInfo(DownloadManagerServer server, long pluginId, String name, ServerDownloadStatus status)
            :base(status)
        {
            this.server = server;
            this.PluginId = pluginId;
            this.Name = name;
            Dependencies = new List<long>();
        }

        protected override void doStartDownload(DownloadController downloadController)
        {
            Download = downloadController.downloadPlugin(PluginId, this);
        }

        public override bool dependsOn(ServerDownloadInfo testAsDependency)
        {
            ServerPluginDownloadInfo other = testAsDependency as ServerPluginDownloadInfo;
            return other != null && Dependencies.Contains(other.PluginId);
        }

        public override bool shouldAutoDownlaod(IEnumerable<long> autoDownloadIds)
        {
            return autoDownloadIds.Contains(PluginId);
        }

        public override void downloadCompleted(Download download)
        {
            PluginDownload pluginDownload = (PluginDownload)download;
            //The plugin has been installed, so we need to remove it from the download list.
            if (pluginDownload.Successful)
            {
                if (pluginDownload.LoadedSucessfully)
                {
                    server.removeDetectedPlugin(this);
                }
                else
                {
                    requestRestart("A plugin you have downloaded requires Anomalous Medical to restart.", false);
                }
            }
            base.downloadCompleted(download);
        }

        public override DownloadGUIInfo createClientDownloadInfo()
        {
			PluginDownload pluginDownload = (PluginDownload)Download;
            AtlasPlugin plugin = pluginDownload.Plugin;
            if (pluginDownload.LoadedSucessfully && plugin != null)
            {
                if (plugin.AllowUninstall)
                {
                    return new UninstallInfo(plugin);
                }
                else
                {
                    return null;
                }
            }
            return new UpdateInfo(ImageKey, Name, String.Format("You must restart Anomalous Medical to finish installing {0}. You may install more things by selecting them and clicking install before restarting.", Name), ServerDownloadStatus.PendingInstall, false);
        }

        public long PluginId { get; set; }

        public List<long> Dependencies { get; private set; }

        public override string MoreInfoURL
        {
            get
            {
                return String.Format(MedicalConfig.ProductPageBaseURL, PluginId);
            }
        }

        public override void getDescription(DescriptionFoundCallback descriptionFoundCallback)
        {
            if (description != null)
            {
                descriptionFoundCallback.Invoke(description, this);
            }
            else if (!readingDescriptionFromServer)
            {
                readingDescriptionFromServer = true;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    description = server.readLicenseFromServer(PluginId);
                    if (description != null)
                    {
                        ThreadManager.invoke(new Action(delegate()
                        {
                            descriptionFoundCallback.Invoke(description, this);
                        }));
                    }
                    else
                    {
                        readingDescriptionFromServer = false;
                        ThreadManager.invoke(new Action(delegate()
                        {
                            descriptionFoundCallback.Invoke("There was an error reading this license from the download server. You are still bound to the license. Please visit www.anomalousmedical.com for more information.\n\nYou can click this item again to attempt to read the license off the server again.", this);
                        }));
                    }
                });
            }
        }
    }
}