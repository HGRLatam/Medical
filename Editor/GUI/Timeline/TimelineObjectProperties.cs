﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineObjectProperties : MDIDialog
    {
        private ResizingTable table;
        private PropertiesTable propertiesTable;

        public TimelineObjectProperties()
            : base("Medical.GUI.Timeline.TimelineObjectProperties.layout")
        {
            table = new ResizingTable(window.findWidget("ScrollView") as ScrollView);
            propertiesTable = new PropertiesTable(table);

            this.Resized += new EventHandler(TimelineObjectProperties_Resized);
        }

        public override void Dispose()
        {
            propertiesTable.Dispose();
            table.Dispose();
            base.Dispose();
        }

        public PropertiesTable PropertiesTable
        {
            get
            {
                return propertiesTable;
            }
        }

        void TimelineObjectProperties_Resized(object sender, EventArgs e)
        {
            table.layout();
        }
    }
}
