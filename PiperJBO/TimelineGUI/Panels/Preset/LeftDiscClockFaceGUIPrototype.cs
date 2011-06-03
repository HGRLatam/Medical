﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LeftDiscClockFaceGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public LeftDiscClockFaceGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new PresetStateGUI("LeftDisc", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return null;
        }

        public string Name
        {
            get { return "PiperJBO.LeftDiscClockFaceGUI"; }
        }
    }
}
