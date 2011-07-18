﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class DialogOpenTaskMenuItem : TaskMenuItem
    {
        private Dialog dialog;

        public DialogOpenTaskMenuItem(Dialog dialog, String uniqueName, String name, String iconName, String category)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT)
        {
            
        }

        public DialogOpenTaskMenuItem(Dialog dialog, String uniqueName, String name, String iconName, String category, int weight)
            : this(dialog, uniqueName, name, iconName, category, weight, true)
        {
            
        }

        public DialogOpenTaskMenuItem(Dialog dialog, String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(dialog, uniqueName, name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {

        }

        public DialogOpenTaskMenuItem(Dialog dialog, String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar)
            : base(uniqueName, name, iconName, category)
        {
            this.dialog = dialog;
            this.Weight = weight;
            this.ShowOnTaskbar = showOnTaskbar;
            dialog.Closed += new EventHandler(dialog_Closed);
        }

        public override void clicked()
        {
            if (!dialog.Visible)
            {
                dialog.Visible = true;
            }
            else
            {
                dialog.bringToFront();
            }
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
