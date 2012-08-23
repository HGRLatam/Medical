﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.Threading;
using Medical.Controller;
using Logging;

namespace Medical.GUI
{
    class LicenseDialog : Dialog
    {
        /// <summary>
        /// This event will fire if the key is entered in the dialog sucessfully.
        /// </summary>
        public event EventHandler KeyEnteredSucessfully;

        /// <summary>
        /// This event will fire if the key could not be entered.
        /// </summary>
        public event EventHandler KeyInvalid;

        private EditBox userEdit;
        private EditBox passwordEdit;
        private string programName;
        private Button activateButton;
        private Button cancelButton;
        private CheckButton rememberPasswordButton;

        public LicenseDialog(String programName, String message)
            :base("Medical.GUI.LicenseDialog.LicenseDialog.layout")
        {
            this.programName = programName;

            TextBox prompt = (TextBox)window.findWidget("Prompt");
            if (message != null)
            {
                prompt.Caption = message;
            }
            else
            {
                prompt.Caption = prompt.Caption.Replace("$(PROGRAM_NAME)", programName);
            }

            userEdit = window.findWidget("UserText") as EditBox;
            userEdit.KeyButtonReleased += new MyGUIEvent(userEdit_KeyButtonReleased);
            
            passwordEdit = window.findWidget("PasswordText") as EditBox;
            passwordEdit.KeyButtonReleased += new MyGUIEvent(passwordEdit_KeyButtonReleased);

            activateButton = window.findWidget("ActivateButton") as Button;
            activateButton.MouseButtonClick += new MyGUIEvent(activateButton_MouseButtonClick);

            cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            rememberPasswordButton = new CheckButton(window.findWidget("RememberPassword") as Button);
            rememberPasswordButton.Checked = MedicalConfig.StoreCredentials;

            TextBox connectionLabel = window.findWidget("ConnectionLabel") as TextBox;
            connectionLabel.Caption = MedicalConfig.LicenseServerURL;

            Button forgotPassword = window.findWidget("ForgotPassword") as Button;
            forgotPassword.MouseButtonClick += new MyGUIEvent(forgotPassword_MouseButtonClick);

            Button register = window.findWidget("Register") as Button;
            register.MouseButtonClick += new MyGUIEvent(register_MouseButtonClick);
        }

        public String UserName
        {
            get
            {
                return userEdit.Caption;
            }
            set
            {
                userEdit.Caption = value;
            }
        }

        public bool SelectPasswordOnOpen { get; set; }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            if (SelectPasswordOnOpen)
            {
                InputManager.Instance.setKeyFocusWidget(passwordEdit);
            }
            else
            {
                InputManager.Instance.setKeyFocusWidget(userEdit);
            }
        }

        void passwordEdit_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            switch(ke.Key)
            {
                case Engine.Platform.KeyboardButtonCode.KC_TAB:
                    InputManager.Instance.setKeyFocusWidget(userEdit);
                    break;
                case Engine.Platform.KeyboardButtonCode.KC_RETURN:
                    if (activateButton.Enabled)
                    {
                        activateButton_MouseButtonClick(source, e);
                    }
                    break;
            }
        }

        void userEdit_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            switch (ke.Key)
            {
                case Engine.Platform.KeyboardButtonCode.KC_TAB:
                    InputManager.Instance.setKeyFocusWidget(passwordEdit);
                    break;
                case Engine.Platform.KeyboardButtonCode.KC_RETURN:
                    if (activateButton.Enabled)
                    {
                        activateButton_MouseButtonClick(source, e);
                    }
                    break;
            }
        }

        public byte[] License { get; private set; }

        void activateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MedicalConfig.StoreCredentials = rememberPasswordButton.Checked;
            activateButton.Enabled = false;
            cancelButton.Enabled = false;
            ThreadPool.QueueUserWorkItem(new WaitCallback(getLicense));
        }

        void forgotPassword_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.ForgotPasswordURL);
        }

        void register_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.RegisterURL);
        }

        void getLicense(Object ignored)
        {
            try
            {
                AnomalousLicenseServer licenseServer = new AnomalousLicenseServer(MedicalConfig.LicenseServerURL);
                License = licenseServer.createLicenseFile(userEdit.OnlyText, passwordEdit.OnlyText);
                if (License != null)
                {
                    ThreadManager.invoke(new Callback(licenseCaptured), null);
                }
                else
                {
                    ThreadManager.invoke(new Callback(licenseLoginFail), null);
                }
            }
            catch (AnomalousLicenseServerException alse)
            {
                ThreadManager.invoke(new CallbackString(licenseServerFail), alse.Message);
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new CallbackString(licenseServerFail), String.Format("Could not connect to license server. Please try again later.\nReason is {0}", e.Message));
            }
        }

        private delegate void Callback();
        private delegate void CallbackString(String message);

        void licenseCaptured()
        {
            try
            {
                if (KeyEnteredSucessfully != null)
                {
                    KeyEnteredSucessfully.Invoke(this, EventArgs.Empty);
                }
                this.close();
            }
            catch (LicenseInvalidException ex)
            {
                Log.Error("Invalid license returned from server. Reason: {0}", ex.Message);
                activateButton.Enabled = true;
                cancelButton.Enabled = true;
                passwordEdit.Caption = "";
                MessageBox.show(String.Format("License returned from server is invalid.\nReason: {0}\nPlease contact support at CustomerService@AnomalousMedical.com.", ex.Message), "Login Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void licenseLoginFail()
        {
            activateButton.Enabled = true;
            cancelButton.Enabled = true;
            passwordEdit.Caption = "";
            MessageBox.show("Could not get license file. Username or password is invalid.", "Login Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
        }

        void licenseServerFail(String message)
        {
            activateButton.Enabled = true;
            cancelButton.Enabled = true;
            MessageBox.show(message, "License Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
            if (KeyInvalid != null)
            {
                KeyInvalid.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
