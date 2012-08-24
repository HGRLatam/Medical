﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using Engine;
using Medical;
using Engine.Performance;
using Engine.Platform;

namespace Developer.GUI
{
    class PerformanceGui : MDIDialog
    {
        private TextBox text;
        private CheckButton enabled;
        private StandaloneController standaloneController;

        public PerformanceGui(StandaloneController standaloneController)
            :base("Developer.GUI.Performance.PerformanceGui.layout")
        {
            this.standaloneController = standaloneController;

            text = (TextBox)window.findWidget("Text");
            enabled = new CheckButton((Button)window.findWidget("Enabled"));
            enabled.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            enabled.Checked = PerformanceMonitor.Enabled;
        }

        void enabled_CheckedChanged(Widget source, EventArgs e)
        {
            PerformanceMonitor.Enabled = enabled.Checked;
            if (enabled.Checked)
            {
                standaloneController.MedicalController.FixedLoopUpdate += MedicalController_FixedLoopUpdate;
            }
            else
            {
                standaloneController.MedicalController.FixedLoopUpdate -= MedicalController_FixedLoopUpdate;
            }
        }

        void MedicalController_FixedLoopUpdate(Clock time)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Timelapse timelapse in PerformanceMonitor.Timelapses)
            {
                sb.AppendFormat("{0}: {1}ms\n", timelapse.Name, timelapse.Duration);
            }
            text.Caption = sb.ToString();
        }
    }
}
