﻿using System;
using ExpansionDownloader.Service;
using ExpansionDownloader;
using ExpansionDownloader.Client;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Content.PM;
using Engine.Threads;
using ExpansionDownloader.Database;
using System.Linq;

namespace AnomalousMedicalAndroid
{
    public class ObbDownloader : IDownloaderClient
    {
        private IDownloaderService downloaderService;
        private IDownloaderServiceConnection downloaderServiceConnection;

        public event Action DownloadFailed;
        public event Action DownloadSucceeded;
        public event Action<String, int, int> DownloadProgressUpdated;

        private Activity activity;

        public ObbDownloader(Activity activity)
        {
            this.activity = activity;
        }

        public bool AreExpansionFilesDelivered()
        {
            var downloads = DownloadsDatabase.GetDownloads();
            //True if we have no associated downloads or all downloads match the expected values.
            return downloads.Any() && downloads.All(x => Helpers.DoesFileExist(activity, x.FileName, x.TotalBytes, false));
        }

        public void GetExpansionFiles()
        {
            try
            {
                activity.RunOnUiThread(() =>
                    {
//                        for(int i = 0; i <= 10; ++i)
//                        {
//                            OnDownloadProgress(new DownloadProgressInfo(10 << 8, i << 8, 500, 500));
//                            System.Threading.Thread.Sleep(1000);
//                        }
//                        OnDownloadStateChanged(DownloaderState.Completed);
//
//                        return;
                        // Build the intent that launches this activity.
                        Intent launchIntent = activity.Intent;
                        var intent = new Intent(activity, typeof(MainActivity));
                        //intent.SetFlags(ActivityFlags. | ActivityFlags.ClearTop);
                        intent.SetAction(launchIntent.Action);

                        if (launchIntent.Categories != null)
                        {
                            foreach (string category in launchIntent.Categories)
                            {
                                intent.AddCategory(category);
                            }
                        }

                        // Build PendingIntent used to open this activity when user 
                        // taps the notification.
                        PendingIntent pendingIntent = PendingIntent.GetActivity(activity, 0, intent, PendingIntentFlags.UpdateCurrent);

                        // Request to start the download
                        DownloadServiceRequirement startResult = DownloaderService.StartDownloadServiceIfRequired(activity, pendingIntent, typeof(AnomalousMedicalDownloaderService));

                        // The DownloaderService has started downloading the files, 
                        // show progress otherwise, the download is not needed so  we 
                        // fall through to starting the actual app.
                        if (startResult != DownloadServiceRequirement.NoDownloadRequired)
                        {
                            downloaderServiceConnection = ClientMarshaller.CreateStub(this, typeof(AnomalousMedicalDownloaderService));
                            downloaderServiceConnection.Connect(activity);
                        }
                        else
                        {
                            ThreadManager.invoke(() =>
                            {
                                if(DownloadSucceeded != null)
                                {
                                    DownloadSucceeded.Invoke();
                                }
                            });
                        }
                    });
            }
            catch (PackageManager.NameNotFoundException e)
            {
                //Debug.WriteLine("Cannot find own package! MAYDAY!");
                e.PrintStackTrace();
            }
        }

        #region IDownloaderClient implementation

        public void OnDownloadProgress(DownloadProgressInfo progress)
        {
            String progressMessage = String.Format("Downloading expansion files. {0} ({1}) completed, {2} remaining", 
                Helpers.GetDownloadProgressPercent(progress.OverallProgress, progress.OverallTotal), 
                Helpers.GetDownloadProgressString(progress.OverallProgress, progress.OverallTotal), 
                Helpers.GetTimeRemaining(progress.TimeRemaining));

            ThreadManager.invoke(() =>
            {
                if(DownloadProgressUpdated != null)
                {
                    DownloadProgressUpdated.Invoke(progressMessage, (int)(progress.OverallProgress >> 8), (int)(progress.OverallTotal >> 8));
                }
            });
        }

        public void OnDownloadStateChanged(DownloaderState newState)
        {
            Console.WriteLine(newState.ToString());
            LastStateMessage = Helpers.GetDownloaderStringFromState(activity, newState);
            switch (newState)
            {
                case DownloaderState.Failed:
                case DownloaderState.FailedCanceled:
                case DownloaderState.FailedFetchingUrl:
                case DownloaderState.FailedSdCardFull:
                case DownloaderState.FailedUnlicensed:
                    ThreadManager.invoke(() =>
                    {
                        if (DownloadFailed != null)
                        {
                            DownloadFailed.Invoke();
                        }
                    });
                    break;
                case DownloaderState.Completed:
                    ThreadManager.invoke(() =>
                    {
                        if (DownloadSucceeded != null)
                        {
                            DownloadSucceeded.Invoke();
                        }
                    });
                    break;
            }
        }

        public void OnServiceConnected(Messenger m)
        {
            this.downloaderService = ServiceMarshaller.CreateProxy(m);
            this.downloaderService.OnClientUpdated(this.downloaderServiceConnection.GetMessenger());
            Console.WriteLine("Service connected");
        }

        public String LastStateMessage { get; private set; }

        #endregion
    }
}

