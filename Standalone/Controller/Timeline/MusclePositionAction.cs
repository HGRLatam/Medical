﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class MusclePositionAction : TimelineAction
    {
        private MusclePosition targetState;

        private MusclePosition startState = new MusclePosition();
        private float lastTime = 0.0f;

        public MusclePositionAction()
            :this(new MusclePosition())
        {
            
        }

        public MusclePositionAction(MusclePosition targetState)
        {
            this.targetState = targetState;
            Duration = 1.0f;
        }

        public override void capture()
        {
            targetState.captureState();
        }

        public override void started(float timelineTime, Clock clock)
        {
            start();
        }

        public override void skipTo(float timelineTime)
        {
            start();
            lastTime = timelineTime;
            float blendFactor = (timelineTime - StartTime) / Duration;
            if (blendFactor > 1.0f)
            {
                blendFactor = 0.0f;
            }
            startState.blend(targetState, blendFactor);
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
            startState.blend(targetState, (timelineTime - StartTime) / Duration);
        }

        public override void editing()
        {
            start();
            targetState.preview();
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override void cleanup(CleanupInfo cleanupInfo)
        {

        }

        public override void reverseSides()
        {
            targetState.reverseSides();
        }

        public override bool Finished
        {
            get 
            {
                return lastTime > StartTime + Duration;
            }
        }

        private void start()
        {
            startState.captureState();
            lastTime = 0.0f;
            if(Duration == 0.0f)
            {
                Duration = 0.001f;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Capture", capture));
        }

        [Editable]
        public EasingFunction Easing
        {
            get
            {
                return targetState.Easing;
            }
            set
            {
                targetState.Easing = value;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Muscle Position";
            }
        }

        protected MusclePositionAction(LoadInfo info)
            :base(info)
        {
            targetState = info.GetValue<MusclePosition>("TargetState");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TargetState", targetState);
        }
    }
}
