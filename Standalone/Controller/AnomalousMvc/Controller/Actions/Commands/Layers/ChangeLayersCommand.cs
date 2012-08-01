﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class ChangeLayersCommand : ActionCommand
    {
        public ChangeLayersCommand()
        {
            Layers = new EditableLayerState("ChangeLayers");
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.applyLayers(Layers);
        }

        protected override void createEditInterface()
        {
            editInterface = Layers.getEditInterface(Type, ReflectedEditInterface.DefaultScanner);
            editInterface.IconReferenceTag = Icon;
        }

        public EditableLayerState Layers { get; set; }

        public override string Type
        {
            get
            {
                return "Change Layers";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/LayersChangeIcon";
            }
        }

        protected ChangeLayersCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
