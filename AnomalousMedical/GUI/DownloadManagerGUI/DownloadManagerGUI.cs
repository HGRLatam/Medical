﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.Net;
using System.IO;
using System.Globalization;
using Logging;
using Engine;
using System.Threading;
using Medical.Controller;
using System.Drawing;

namespace Medical.GUI
{
    class DownloadManagerGUI : AbstractFullscreenGUIPopup
    {
        private InstallPanel installPanel;
        private ButtonGrid pluginGrid;
        private DownloadingPanel downloadPanel;
        private Widget readingInfo;
        private UninstallPanel uninstallPanel;

        private AtlasPluginManager pluginManager;
        private DownloadController downloadController;
        private bool activeNotDisposed = true;
        private bool addedInstalledPlugins = false;
        private bool readingServerPluginInfo = false;
        private bool displayRestartMessage = false;
        private bool autoStartUpdate = false;
        private String restartMessage = "";
        private DownloadManagerServer downloadServer;
        private NotificationGUIManager notificationManager;

        public DownloadManagerGUI(AtlasPluginManager pluginManager, DownloadManagerServer downloadServer, DownloadController downloadController, GUIManager guiManager)
            : base("Medical.GUI.DownloadManagerGUI.DownloadManagerGUI.layout", guiManager)
        {
            this.pluginManager = pluginManager;
            this.downloadController = downloadController;
            this.downloadServer = downloadServer;
            downloadServer.DownloadFound += new Action<ServerPluginDownloadInfo>(downloadServer_DownloadFound);
            downloadServer.FinishedReadingDownloads += new Action(downloadServer_FinishedReadingDownloads);
            this.notificationManager = guiManager.NotificationManager;

            pluginGrid = new ButtonGrid((ScrollView)widget.findWidget("PluginScrollList"), new ButtonGridListLayout());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);
            pluginGrid.ItemActivated += new EventHandler(pluginGrid_ItemActivated);
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Updates");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Pending Uninstall");
            pluginGrid.defineGroup("Pending Install");
            pluginGrid.defineGroup("Installed");

            installPanel = new InstallPanel(widget.findWidget("InstallPanel"));
            installPanel.InstallItem += new EventHandler(installPanel_InstallItem);
            installPanel.Visible = false;

            downloadPanel = new DownloadingPanel(widget.findWidget("DownloadingPanel"));
            downloadPanel.CancelDownload += new EventHandler(downloadPanel_CancelDownload);
            downloadPanel.Visible = false;

            uninstallPanel = new UninstallPanel(widget.findWidget("UninstallPanel"));
            uninstallPanel.UninstallItem += new EventHandler(uninstallPanel_UninstallItem);
            uninstallPanel.Visible = false;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            readingInfo = widget.findWidget("ReadingInfo");
            readingInfo.Visible = false;

            this.Showing += new EventHandler(PluginManagerGUI_Showing);
        }

        public override void Dispose()
        {
            downloadServer.Dispose();
            activeNotDisposed = false;
            base.Dispose();
        }

        void PluginManagerGUI_Showing(object sender, EventArgs e)
        {
            if (!readingServerPluginInfo)
            {
                List<AtlasPlugin> installedPluginsList = new List<AtlasPlugin>();

                if (addedInstalledPlugins)
                {
                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        if (plugin.PluginId != -1)
                        {
                            installedPluginsList.Add(plugin);
                        }
                    }
                }
                else
                {
                    pluginGrid.SuppressLayout = true;

                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName, plugin.BrandingImageKey);
                        if (plugin.PluginId != -1)
                        {
                            item.UserObject = new UninstallInfo(plugin);
                            installedPluginsList.Add(plugin);
                        }
                    }
                    addedInstalledPlugins = true;

                    pluginGrid.SuppressLayout = false;
                    pluginGrid.layout();
                }

                readingServerPluginInfo = true;
                readingInfo.Visible = true;
                downloadServer.readPluginInfoFromServer(installedPluginsList, addNotInstalledPlugins);
            }
        }

        void downloadServer_DownloadFound(ServerPluginDownloadInfo download)
        {
            if (activeNotDisposed)
            {
                addDownloadToButtonGrid(download);
            }
        }

        void downloadServer_FinishedReadingDownloads()
        {
            if (activeNotDisposed)
            {
                readingServerPluginInfo = false;
                readingInfo.Visible = false;
            }
        }

        private void addNotInstalledPlugins(List<ServerDownloadInfo> downloadInfo)
        {
            //if (activeNotDisposed)
            //{
            //    pluginGrid.SuppressLayout = true;

            //    foreach (ServerDownloadInfo download in downloadInfo)
            //    {
            //        addDownloadToButtonGrid(download);
            //    }

            //    pluginGrid.SuppressLayout = false;
            //    pluginGrid.layout();

            //    readingServerPluginInfo = false;
            //    readingInfo.Visible = false;
            //}
        }

        private ButtonGridItem addDownloadToButtonGrid(ServerDownloadInfo download)
        {
            String group = "";
            switch (download.Status)
            {
                case ServerDownloadStatus.NotInstalled:
                    group = "Not Installed";
                    break;
                case ServerDownloadStatus.Update:
                    group = "Updates";
                    break;
            }
            ButtonGridItem item = pluginGrid.addItem(group, download.Name, download.ImageKey);
            item.UserObject = download;
            return item;
        }

        private void togglePanelVisibility()
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            if (selectedItem != null)
            {
                installPanel.Visible = selectedItem.GroupName == "Not Installed" || selectedItem.GroupName == "Updates";
                if (installPanel.Visible)
                {
                    installPanel.setDownloadInfo(selectedItem.UserObject as ServerDownloadInfo);
                }
                downloadPanel.Visible = selectedItem.GroupName == "Downloading";
                if (downloadPanel.Visible)
                {
                    downloadPanel.setDownloadInfo(selectedItem.UserObject as ServerDownloadInfo);
                }
                uninstallPanel.Visible = selectedItem.GroupName == "Installed" && selectedItem.UserObject is UninstallInfo;
                if (uninstallPanel.Visible)
                {
                    uninstallPanel.setInfo(selectedItem.UserObject as UninstallInfo);
                }
            }
            else
            {
                installPanel.Visible = false;
                downloadPanel.Visible = false;
                uninstallPanel.Visible = false;
            }
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            togglePanelVisibility();
        }

        void installPanel_InstallItem(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        void uninstallPanel_UninstallItem(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            UninstallInfo pluginInfo = selectedItem.UserObject as UninstallInfo;
            if (pluginInfo != null)
            {
                pluginInfo.uninstall(pluginManager);
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(selectedItem);
                pluginGrid.SuppressLayout = false;
                pluginGrid.addItem("Pending Uninstall", pluginInfo.Name, pluginInfo.ImageKey);
                notificationManager.showRestartNotification(String.Format("You must restart Anomalous Medical to finish uninstalling '{0}'.\nClick here to do this now.", pluginInfo.Name), pluginInfo.ImageKey, false);
            }
        }

        void pluginGrid_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && (pluginInfo.Download == null || pluginInfo.Download.Cancel))
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        private void downloadItem(ButtonGridItem selectedItem, ServerDownloadInfo downloadInfo)
        {
            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(selectedItem);
            ButtonGridItem downloadingItem = pluginGrid.addItem("Downloading", String.Format("{0}\n{1}", downloadInfo.Name, "Starting Download"), downloadInfo.ImageKey);
            downloadingItem.UserObject = downloadInfo;
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();

            downloadInfo.UserObject = downloadingItem;
            downloadInfo.startDownload(downloadController);
            subscribeToDownload(downloadInfo);
            pluginGrid.SelectedItem = downloadingItem;
        }

        void downloadPanel_CancelDownload(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && pluginInfo.Download != null)
            {
                pluginInfo.Download.cancelDownload(DownloadPostAction.DeleteFile);
            }
        }

        public override void setSize(int width, int height)
        {
            base.setSize(width, height);
            pluginGrid.layout();
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        #region DownloadUIDisplay Members

        void downloadInfo_UpdateStatus(ServerDownloadInfo downloadInfo, string status)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                downloadingItem.Caption = status;
            }
        }

        void downloadInfo_DownloadSuccessful(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                ButtonGridItem installedItem;
                if (downloadInfo.Download.DownloadedToSafeLocation)
                {
                    installedItem = pluginGrid.addItem("Pending Install", downloadInfo.Name, downloadInfo.ImageKey);
                }
                else
                {
                    installedItem = pluginGrid.addItem("Installed", downloadInfo.Name, downloadInfo.ImageKey);
                }
                installedItem.UserObject = downloadInfo.createUninstallInfo();

                if (!downloadController.Downloading && displayRestartMessage)
                {
                    notificationManager.showRestartNotification(restartMessage + "\nClick here to do this now.", "AnomalousMedical/Download", autoStartUpdate);
                    displayRestartMessage = false;
                    autoStartUpdate = false;
                }
                else if (!Visible)
                {
                    notificationManager.showNotification(String.Format("{0} has finished downloading.", downloadInfo.Name), "AnomalousMedical/Download");
                }
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
                if (reselectItem)
                {
                    pluginGrid.SelectedItem = installedItem;
                }
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadFailed(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                MessageBox.show(String.Format("There was an error downloading {0}.\nPlease try again later.", downloadInfo.Name), "Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                ButtonGridItem newItem = addDownloadToButtonGrid(downloadInfo);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
                if (reselectItem)
                {
                    pluginGrid.SelectedItem = newItem;
                }
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadCanceled(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                ButtonGridItem newItem = addDownloadToButtonGrid(downloadInfo);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
                if (reselectItem)
                {
                    pluginGrid.SelectedItem = newItem;
                }
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_RequestRestart(ServerDownloadInfo downloadInfo, string restartMessage, bool startPlatformUpdate)
        {
            if (!autoStartUpdate)
            {
                autoStartUpdate = startPlatformUpdate;
                displayRestartMessage = true;
                this.restartMessage = restartMessage;
            }
        }

        private void subscribeToDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled += downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed += downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful += downloadInfo_DownloadSuccessful;
            downloadInfo.RequestRestart += downloadInfo_RequestRestart;
            downloadInfo.UpdateStatus += downloadInfo_UpdateStatus;
        }

        private void unsubscribeFromDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled -= downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed -= downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful -= downloadInfo_DownloadSuccessful;
            downloadInfo.RequestRestart -= downloadInfo_RequestRestart;
            downloadInfo.UpdateStatus -= downloadInfo_UpdateStatus;
        }

        #endregion
    }
}
