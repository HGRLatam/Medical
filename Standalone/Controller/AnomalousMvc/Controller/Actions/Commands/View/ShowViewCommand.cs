﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class ShowViewCommand : ActionCommand
    {
        public ShowViewCommand()
        {
            
        }

        public ShowViewCommand(String view)
        {
            this.View = view;
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueShowView(View);
        }

        [EditableView]
        public String View { get; set; }

        public override string Type
        {
            get
            {
                return "Show View";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/ViewShowIcon";
            }
        }

        protected ShowViewCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
