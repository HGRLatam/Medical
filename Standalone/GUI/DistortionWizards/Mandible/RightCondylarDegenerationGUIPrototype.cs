﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RightCondylarDegenerationGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RightCondylarDegenerationGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new RightCondylarDegenerationGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new CondylarDegenerationGUIData();
        }

        public string Name
        {
            get { return "PiperJBO.RightCondylarDegenerationGUI"; }
        }
    }
}
