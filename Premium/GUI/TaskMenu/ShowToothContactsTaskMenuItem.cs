﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ShowToothContactsTaskMenuItem : TaskMenuItem
    {
        public ShowToothContactsTaskMenuItem()
            : base("Show Occlusal Contacts", "ShowTeethContactsIcon", TaskMenuCategories.Simulation)
        {

        }

        public override void clicked()
        {
            TeethController.HighlightContacts = !TeethController.HighlightContacts;
        }
    }
}
