﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class DiscLockedPoleControl : BehaviorObject
    {
        [Editable]
        String poleBoneName;

        [DoNotCopy]
        [DoNotSave]
        Bone bone;

        [DoNotCopy]
        [DoNotSave]
        Vector3 offset;

        [DoNotCopy]
        [DoNotSave]
        ControlPointBehavior controlPoint;

        [DoNotCopy]
        [DoNotSave]
        SimObject owner;

        public DiscLockedPoleControl()
        {

        }

        protected DiscLockedPoleControl(LoadInfo info)
            :base(info)
        {

        }

        public bool findBone(Skeleton skeleton)
        {
            if (skeleton.hasBone(poleBoneName))
            {
                bone = skeleton.getBone(poleBoneName);
                bone.setManuallyControlled(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void initialize(ControlPointBehavior controlPoint, SimObject owner)
        {
            this.controlPoint = controlPoint;
            this.owner = owner;
            offset = bone.getDerivedPosition() + owner.Translation - (controlPoint.MandibleBonePosition + controlPoint.MandibleTranslation);
        }

        public void update()
        {
            Quaternion inverseOwnerRotation = owner.Rotation.inverse();
            Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + offset) + controlPoint.MandibleTranslation - owner.Translation;
            bone.setPosition(Quaternion.quatRotate(inverseOwnerRotation, translation));
            bone.setOrientation(controlPoint.MandibleBoneRotation * inverseOwnerRotation);
            bone.needUpdate(true);
        }
    }
}
