﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class StartWizardTaskMenuItem : TaskMenuItem
    {
        private PiperJBOAtlasPlugin piperAtlasPlugin;
        private StateWizard wizard;

        public StartWizardTaskMenuItem(PiperJBOAtlasPlugin piperAtlasPlugin, StateWizard wizard)
            :base(wizard.Name, wizard.ImageKey, TaskMenuCategories.Exams)
        {
            this.piperAtlasPlugin = piperAtlasPlugin;
            this.wizard = wizard;
        }

        public override void clicked()
        {
            piperAtlasPlugin.startWizard(wizard);
        }
    }
}
