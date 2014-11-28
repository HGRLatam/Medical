﻿using Engine;
using Engine.Platform;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    /// <summary>
    /// This is an MDIDialog that can be pinned, you must use one of the P window types for this. You can also set the
    /// user string PinByDefault on the layout to true or false to pin the dialog by default.
    /// </summary>
    public class PinableMDIDialog : MDIDialog
    {
        private CheckButton pinButton;

        public PinableMDIDialog(String layoutFile)
            :base(layoutFile)
        {
            setupPinButton();
        }

        public PinableMDIDialog(String layoutFile, String persistName)
            :base(layoutFile, persistName)
        {
            setupPinButton();
        }

        public override void Dispose()
        {
            InputManager.Instance.MouseButtonPressed -= InputManager_MouseButtonPressed;
            base.Dispose();
        }

        public bool Pinned
        {
            get
            {
                return pinButton.Checked;
            }
            set
            {
                pinButton.Checked = value;
            }
        }

        protected override void customDeserialize(ConfigSection section, ConfigFile configFile)
        {
            base.customDeserialize(section, configFile);
            pinButton.Checked = section.getValue("Pinned", () =>
            {
                bool pin = false;
                bool.TryParse(window.getUserString("PinByDefault"), out pin);
                return pin;
            });
        }

        protected override void customSerialize(ConfigSection section, ConfigFile configFile)
        {
            base.customSerialize(section, configFile);
            section.setValue("Pinned", pinButton.Checked);
        }

        protected virtual bool keepOpenFromPoint(int x, int y)
        {
            return false;
        }

        private void setupPinButton()
        {
            pinButton = new CheckButton(window.findWidgetChildSkin("PinButton") as Button);
            pinButton.CheckedChanged += pinButton_CheckedChanged;
            pinButton_CheckedChanged(null, EventArgs.Empty);
        }

        void pinButton_CheckedChanged(Widget source, EventArgs e)
        {
            if (pinButton.Checked)
            {
                InputManager.Instance.MouseButtonPressed -= InputManager_MouseButtonPressed;
            }
            else
            {
                InputManager.Instance.MouseButtonPressed += InputManager_MouseButtonPressed;
            }
        }

        void InputManager_MouseButtonPressed(int x, int y, MouseButtonCode button)
        {
            int left = window.AbsoluteLeft;
            int top = window.AbsoluteTop;
            int right = left + window.Width;
            int bottom = top + window.Height;
            if (x < left || x > right || y < top || y > bottom)
            {
                if(MDIManager != null && MDIManager.isControlWidgetAtPosition(x, y) || keepOpenFromPoint(x, y))
                {
                    return;
                }
                Visible = false;
            }
        }
    }
}
