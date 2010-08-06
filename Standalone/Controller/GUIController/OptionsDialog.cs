﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using OgrePlugin;

namespace Medical.GUI
{
    class OptionsDialog : Dialog
    {
        private ComboBox aaCombo;
        private ComboBox resolutionCombo;
        private CheckButton fullscreenCheck;
        private CheckButton vsyncCheck;
        private static readonly char[] seps = { 'x' };

        public OptionsDialog(String layoutFile)
            :base(layoutFile)
        {
            this.Modal = true;
            this.SmoothShow = true;

            aaCombo = window.findWidget("AACombo") as ComboBox;
            resolutionCombo = window.findWidget("ResolutionCombo") as ComboBox;

            RenderSystem rs = Root.getSingleton().getRenderSystem();
            if (rs.hasConfigOption("FSAA"))
            {
                aaCombo.Enabled = true;
                ConfigOption configOption = rs.getConfigOption("FSAA");
                foreach (String value in configOption.PossibleValues)
                {
                    aaCombo.addItem(value);
                }
                aaCombo.SelectedIndex = aaCombo.findItemIndexWith(OgreConfig.FSAA);
            }
            else
            {
                aaCombo.Enabled = false;
            }
            if (rs.hasConfigOption("Video Mode"))
            {
                resolutionCombo.Enabled = true;
                ConfigOption configOption = rs.getConfigOption("Video Mode");
                foreach (String value in configOption.PossibleValues)
                {
                    if(value.Contains(" @ 32-bit colour"))
                    {
                        resolutionCombo.addItem(value.Replace(" @ 32-bit colour", ""));
                    }
                }
                resolutionCombo.SelectedIndex = 0;
            }

            fullscreenCheck = new CheckButton(window.findWidget("FullscreenCheck") as Button);
            vsyncCheck = new CheckButton(window.findWidget("VSyncCheck") as Button);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            aaCombo.SelectedIndex = aaCombo.findItemIndexWith(OgreConfig.FSAA);
            fullscreenCheck.Checked = MedicalConfig.EngineConfig.Fullscreen;
            vsyncCheck.Checked = OgreConfig.VSync;

            String resString = String.Format("{0} x {1}", MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            uint resIndex = resolutionCombo.findItemIndexWith(resString);
            if (resIndex == uint.MaxValue)
            {
                resolutionCombo.addItem(resString);
                resolutionCombo.SelectedIndex = resolutionCombo.getItemCount() - 1;
            }
            else
            {
                resolutionCombo.SelectedIndex = resIndex;
            }
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            OgreConfig.FSAA = aaCombo.getItemNameAt(aaCombo.SelectedIndex);
            OgreConfig.VSync = vsyncCheck.Checked;
            MedicalConfig.EngineConfig.Fullscreen = fullscreenCheck.Checked;
            String[] res = resolutionCombo.getItemNameAt(resolutionCombo.SelectedIndex).Split(seps, StringSplitOptions.RemoveEmptyEntries);
            MedicalConfig.EngineConfig.HorizontalRes = int.Parse(res[0]);
            MedicalConfig.EngineConfig.VerticalRes = int.Parse(res[1]);
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }
    }
}
