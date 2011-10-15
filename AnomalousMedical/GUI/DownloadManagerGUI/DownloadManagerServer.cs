﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using Medical.Controller;
using Logging;
using MyGUIPlugin;
using System.Globalization;
using Engine;
using System.Threading;

namespace Medical.GUI
{
    class DownloadManagerServer : IDisposable
    {
        private ImageAtlas serverImages = new ImageAtlas("PluginManagerServerImages", new Size2(100, 100), new Size2(1024, 1024));
        private LicenseManager licenseManager;
        List<ServerPluginDownloadInfo> detectedServerPlugins = new List<ServerPluginDownloadInfo>();

        public DownloadManagerServer(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void Dispose()
        {
            serverImages.Dispose();
        }

        public ImageAtlas ServerImages
        {
            get
            {
                return serverImages;
            }
        }

        public void readPluginInfoFromServer(List<int> installedPluginIds, Action<List<ServerDownloadInfo>> finishedCallback)
        {
            foreach (ServerPluginDownloadInfo plugin in detectedServerPlugins)
            {
                installedPluginIds.Add(plugin.PluginId);
            }
            Thread serverReadThread = new Thread(delegate()
            {
                StringBuilder sb = new StringBuilder();
                foreach (int pluginId in installedPluginIds)
                {
                    sb.Append(pluginId.ToString());
                    sb.Append(",");
                }
                String installedPluginsList = String.Empty;
                if (sb.Length > 0)
                {
                    installedPluginsList = sb.ToString(0, sb.Length - 1);
                }
                List<ServerDownloadInfo> pluginInfo = readServerPluginInfo(installedPluginsList);
                ThreadManager.invoke(finishedCallback, pluginInfo);
            });
            serverReadThread.Start();
        }

        internal void removeDetectedPlugin(ServerPluginDownloadInfo serverPluginDownloadInfo)
        {
            detectedServerPlugins.Remove(serverPluginDownloadInfo);
        }

        private List<ServerDownloadInfo> readServerPluginInfo(String commaSeparatedPluginList)
        {
            List<ServerDownloadInfo> downloadInfoList = new List<ServerDownloadInfo>();
            try
            {
                Version localVersion = AnomalousMainPlugin.Version;
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginInfoURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&version={2}&os={3}&list={4}", licenseManager.User, licenseManager.MachinePassword, localVersion, (int)PlatformConfig.OsId, commaSeparatedPluginList);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";

                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream serverDataStream = response.GetResponseStream())
                        {
                            //Download all the data from the server
                            using (MemoryStream localDataStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[8 * 1024];
                                int len;
                                while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    localDataStream.Write(buffer, 0, len);
                                }
                                localDataStream.Seek(0, SeekOrigin.Begin);
                                using (BinaryReader streamReader = new BinaryReader(localDataStream))
                                {
                                    String versionString = streamReader.ReadString();
                                    Version remoteVersion = new Version(versionString);
                                    if (remoteVersion > localVersion)
                                    {
                                        downloadInfoList.Add(new PlatformUpdateDownloadInfo(remoteVersion));
                                    }
                                    while (streamReader.PeekChar() != -1)
                                    {
                                        ServerPluginDownloadInfo pluginInfo = new ServerPluginDownloadInfo(this, streamReader.ReadInt32(), streamReader.ReadString(), ServerDownloadStatus.NotInstalled);
                                        downloadInfoList.Add(pluginInfo);
                                        String imageURL = streamReader.ReadString();
                                        if (!String.IsNullOrEmpty(imageURL))
                                        {
                                            using (Bitmap image = loadImageFromURL(imageURL))
                                            {
                                                if (image != null)
                                                {
                                                    ThreadManager.invokeAndWait(new Action(delegate()
                                                    {
                                                        pluginInfo.ImageKey = serverImages.addImage(pluginInfo, image);
                                                    }));
                                                }
                                            }
                                        }
                                        detectedServerPlugins.Add(pluginInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    MessageBox.show(String.Format("Error reading plugin data from the server. Please try again later.\nReason: {0}", e.Message), "Server Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }));
            }

            return downloadInfoList;
        }

        //Runs on background thread
        private Bitmap loadImageFromURL(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.WebsiteImagesBaseURL + url));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            return new Bitmap(responseStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    Log.Error("Could not load image from {0} because {1}.", url, e.Message);
                }));
            }
            return null;
        }
    }
}
