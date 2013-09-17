﻿using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class ChooseStoreController
    {
        private LicenseManager licenseManager;
        private bool allowGetUserStores = true;

        public ChooseStoreController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            MvcController controller = context.Controllers["ChooseStore"];
            this.licenseManager = licenseManager;

            ((RunCommandsAction)controller.Actions["GetUserStores"]).addCommand(new CallbackCommand((executingContext) =>
            {
                getUserStores(executingContext);
            }));
        }

        private void getUserStores(AnomalousMvcContext executingContext)
        {
            if (allowGetUserStores)
            {
                allowGetUserStores = false;
                ViewHost viewHost = executingContext.RunningActionViewHost;
                ViewHostControl progressMessage = viewHost.findControl("ProgressMessage");
                ViewHostControl storeSelection = viewHost.findControl("StoreSelection");
                ViewHostControl errorMessage = viewHost.findControl("ErrorMessage");
                progressMessage.Visible = true;
                storeSelection.Visible = false;
                errorMessage.Visible = false;

                ThreadPool.QueueUserWorkItem((stateInfo) =>
                {
                    UserStoresModel userStores = null;
                    try
                    {
                        CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.StoreListUrl, licenseManager.User, licenseManager.MachinePassword);
                        userStores = serverConnection.makeRequestSaveableResponse(StoreManagerTypeFinder.Instance) as UserStoresModel;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
                    }
                    ThreadManager.invoke(() =>
                    {
                        if (viewHost.Open)
                        {
                            if (userStores != null)
                            {
                                progressMessage.Visible = false;
                                storeSelection.Visible = true;
                                ViewHostControl formElement = viewHost.findControl("StoreSelectionForms");
                                String formatString = formElement.Value;
                                StringBuilder sb = new StringBuilder();
                                foreach (var store in userStores.Stores)
                                {
                                    sb.AppendFormat(formatString, store.UniqueName, store.Name);
                                }
                                formElement.Value = sb.ToString();
                            }
                            else
                            {
                                progressMessage.Visible = false;
                                errorMessage.Visible = true;
                            }
                        }

                        //Set fetch allow back to true.
                        allowGetUserStores = true;
                    });
                });
            }
        }
    }
}