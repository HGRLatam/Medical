﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class FossaGUILeftPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public FossaGUILeftPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new FossaGUI("LeftFossa", "Medical.GUI.DistortionWizards.Fossa.FossaGUILeft.layout", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.FossaGUILeft"; }
        }
    }
}
