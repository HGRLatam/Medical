﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class RestoreCameraPositionCommand : ActionCommand
    {
        public RestoreCameraPositionCommand()
        {
            Name = "DefaultCamera";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.restoreCamera(Name);
            }
            else
            {
                Log.Warning("No name defined.");
            }
        }

        [Editable]
        public String Name { get; set; }

        public override string Type
        {
            get
            {
                return "Restore Camera Position";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/CameraRestorePositionIcon";
            }
        }

        protected RestoreCameraPositionCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
