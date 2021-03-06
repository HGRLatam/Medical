﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Editing;
using Engine;
using Engine.Attributes;
using Engine.Saving;
using Engine.Reflection;

namespace Medical
{
    public class ChangeHandPosition : ShowPropSubAction
    {
        private PoseableHand hand;
        private bool finished = false;

        public ChangeHandPosition()
        {
            Thumb = new PoseableThumbAnimator();
            Index = new PoseableFingerAnimator();
            Middle = new PoseableFingerAnimator();
            Ring = new PoseableFingerAnimator();
            Pinky = new PoseableFingerAnimator();
        }

        public override void started(float timelineTime, Clock clock)
        {
            findHandBehavior();
        }

        public override void skipTo(float timelineTime)
        {
            findHandBehavior();
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            blend(timelineTime);
            finished = timelineTime > EndTime;
        }

        public override void editing(PropEditController propEditController)
        {
            findHandBehavior();
            Thumb.apply();
            Index.apply();
            Middle.apply();
            Ring.apply();
            Pinky.apply();
        }

        private void findHandBehavior()
        {
            hand = (PoseableHand)PropSimObject.getElement(PoseableHand.PoseableHandBehavior);
            
            Thumb.setFinger(hand.Thumb);
            Thumb.getStartingValues();
            
            Index.setFinger(hand.Index);
            Index.getStartingValues();
            
            Middle.setFinger(hand.Middle);
            Middle.getStartingValues();
            
            Ring.setFinger(hand.Ring);
            Ring.getStartingValues();

            Pinky.setFinger(hand.Pinky);
            Pinky.getStartingValues();
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }

        public PoseableThumbAnimator Thumb { get; set; }

        public PoseableFingerAnimator Index { get; set; }

        public PoseableFingerAnimator Middle { get; set; }

        public PoseableFingerAnimator Ring { get; set; }

        public PoseableFingerAnimator Pinky { get; set; }

        private void blend(float timelineTime)
        {
            float percentage = (timelineTime - StartTime) / Duration;
            if (percentage > 1.0f)
            {
                percentage = 1.0f;
            }

            Thumb.blend(percentage);
            Index.blend(percentage);
            Middle.blend(percentage);
            Ring.blend(percentage);
            Pinky.blend(percentage);
        }

        public override string TypeName
        {
            get
            {
                return "Hand Position";
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new GenericEditableProperty<ChangeHandPosition>("Hand Position", this));
            base.customizeEditInterface(editInterface);
        }

        #region Saveable Members

        protected ChangeHandPosition(LoadInfo info)
            :base (info)
        {
            Thumb = info.GetValue<PoseableThumbAnimator>("Thumb");
            Index = info.GetValue<PoseableFingerAnimator>("Index");
            Middle = info.GetValue<PoseableFingerAnimator>("Middle");
            Ring = info.GetValue<PoseableFingerAnimator>("Ring");
            Pinky = info.GetValue<PoseableFingerAnimator>("Pinky");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);

            info.AddValue("Thumb", Thumb);
            info.AddValue("Index", Index);
            info.AddValue("Middle", Middle);
            info.AddValue("Ring", Ring);
            info.AddValue("Pinky", Pinky);
        }

        #endregion
    }
}
