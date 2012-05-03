﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class ControllerAction : SaveableEditableItem
    {
        public ControllerAction(String name)
            :base(name)
        {

        }

        public override string Type
        {
            get
            {
                return "Action";
            }
        }

        protected ControllerAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
