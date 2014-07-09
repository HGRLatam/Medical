﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Engine.Attributes;
using Engine.Behaviors.Animation;

namespace Medical
{
    class SimObjectFollowerWithRotation : Interface
    {
        [Editable]
        String targetSimObjectName;

        [Editable]
        String targetPositionBroadcasterName = "PositionBroadcaster";

        [DoNotCopy]
        [DoNotSave]
        SimObject targetSimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        [DoNotCopy]
        [DoNotSave]
        PositionBroadcaster broadcaster;

        protected override void link()
        {
            targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", targetSimObjectName);
            }

            broadcaster = targetSimObject.getElement(targetPositionBroadcasterName) as PositionBroadcaster;
            if (broadcaster == null)
            {
                blacklist("Cannot find target PositionBroadcaster '{0}' on SimObject '{1}'", targetPositionBroadcasterName, targetSimObjectName);
            }
            broadcaster.PositionChanged += broadcaster_PositionChanged;

            Quaternion inverseTargetRot = targetSimObject.Rotation.inverse();

            translationOffset = Owner.Translation - targetSimObject.Translation;
            translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

            rotationOffset = inverseTargetRot * Owner.Rotation;
        }

        protected override void destroy()
        {
            broadcaster.PositionChanged -= broadcaster_PositionChanged;
            base.destroy();
        }

        void broadcaster_PositionChanged(SimObject obj)
        {
            Vector3 trans = targetSimObject.Translation + Quaternion.quatRotate(targetSimObject.Rotation, translationOffset);
            Quaternion rotation = targetSimObject.Rotation * rotationOffset;
            updatePosition(ref trans, ref rotation);
        }
    }
}
