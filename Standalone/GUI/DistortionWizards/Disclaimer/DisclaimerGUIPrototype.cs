﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class DisclaimerGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public DisclaimerGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new TimelineWizardPanel("Medical.GUI.DistortionWizards.Disclaimer.DisclaimerGUI.layout", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.DisclaimerGUI"; }
        }
    }
}
