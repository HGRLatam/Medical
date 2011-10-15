﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ServerPluginDownloadInfo : ServerDownloadInfo
    {
        private DownloadManagerServer server;

        public ServerPluginDownloadInfo(DownloadManagerServer server, int pluginId, String name, ServerDownloadStatus status)
            :base(status)
        {
            this.server = server;
            this.PluginId = pluginId;
            this.Name = name;
        }

        protected override void doStartDownload(DownloadController downloadController)
        {
            Download = downloadController.downloadPlugin(PluginId, this);
        }

        public override void downloadCompleted(Download download)
        {
            PluginDownload pluginDownload = (PluginDownload)download;
            //The plugin has been installed, so we need to remove it from the download list.
            if (pluginDownload.LoadedSucessfully)
            {
                server.removeDetectedPlugin(this);
            }
            else
            {
                requestRestart("You must restart Anomalous Medical in order to use some of the plugins you have downloaded.");
            }
            base.downloadCompleted(download);
        }

        public int PluginId { get; set; }
    }
}
