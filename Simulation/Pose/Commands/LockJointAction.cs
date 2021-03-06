﻿using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class LockJointAction : PoseCommandActionBase
    {
        [Editable]
        private String limitSimObjectName = "this";

        [Editable]
        private String limitName;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikLimit limit;

        protected override void link()
        {
            var limitSimObject = Owner.getOtherSimObject(limitSimObjectName);
            if(limitSimObject == null)
            {
                blacklist("Cannot find LimitSimObject named '{0}'", limitSimObjectName);
            }

            limit = limitSimObject.getElement(limitName) as BEPUikLimit;
            if (limit == null)
            {
                blacklist("Cannot find BEPUik limit '{0}' on '{1}'", limitName, limitSimObjectName);
            }

            base.link();
        }

        public override void posingStarted()
        {
            limit.Locked = true;
        }

        public override void posingEnded()
        {
            limit.Locked = false;
        }
    }
}
