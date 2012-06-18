﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.GUI
{
    public class ExpandingGenericEditorView : MyGUIView
    {
        public ExpandingGenericEditorView(String name, EditInterface editInterface, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Right;
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        protected ExpandingGenericEditorView(LoadInfo info)
            : base(info)
        {

        }
    }
}
