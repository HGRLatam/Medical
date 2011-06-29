﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;

namespace Medical
{
    class PoseableFingerSection
    {
        private Bone bone;
        private Radian yaw = 0.0f;
        private Radian pitch = 0.0f;
        private Quaternion startRotation;

        public PoseableFingerSection(Skeleton skeleton, String boneName)
        {
            bone = skeleton.getBone(boneName);
            if (bone == null)
            {
                throw new Exception(String.Format("Bone not found for finger section {0}", boneName));
            }
            bone.setManuallyControlled(true);
            startRotation = bone.getOrientation();
        }

        public Radian Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value;
                updateBone();
            }
        }

        public Radian Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = value;
                updateBone();
            }
        }

        public void updateBone()
        {
            //This does not use the right order.
            Quaternion rotation = new Quaternion(0, pitch, yaw);
            bone.setOrientation(startRotation * rotation);
            bone.needUpdate(true);
        }
    }
}
