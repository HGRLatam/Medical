﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class SetMusclePositionCommand : ActionCommand
    {
        public SetMusclePositionCommand()
        {
            MusclePosition = new MusclePosition();
            MusclePosition.captureState();
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.applyMusclePosition(MusclePosition, MedicalConfig.CameraTransitionTime);
        }

        protected override void createEditInterface()
        {
            editInterface = MusclePosition.getEditInterface(Type);
            editInterface.IconReferenceTag = Icon;
        }

        public MusclePosition MusclePosition { get; set; }

        public override string Type
        {
            get
            {
                return "Change Muscle Position";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MusclePositionChangeIcon";
            }
        }

        protected SetMusclePositionCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
