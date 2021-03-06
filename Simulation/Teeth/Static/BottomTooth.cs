﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Logging;
using BulletPlugin;
using Engine.ObjectManagement;
using Engine.Attributes;

namespace Medical
{
    class BottomTooth : StaticTooth
    {
        protected override void looseChanged(bool loose)
        {
            
        }

        protected override void constructed()
        {
            base.constructed();
            if (actorElement != null)
            {
                actorElement.ContactStarted += actorElement_ContactStarted;
                actorElement.ContactEnded += actorElement_ContactEnded;
            }
        }

        protected override void destroy()
        {
            base.destroy();
            actorElement.ContactStarted -= actorElement_ContactStarted;
            actorElement.ContactEnded -= actorElement_ContactEnded;
        }

        protected override void applyAdaptation(ToothType type, bool adapt)
        {
            if (adapt)
            {
                if (type == ToothType.Bottom && collidingTeeth.Count > 0)
                {
                    joint.setLinearLowerLimit(new Vector3(-1.0f, -1.0f, -1.0f));
                    joint.setLinearUpperLimit(new Vector3(1.0f, 1.0f, 1.0f));
                    joint.setAngularLowerLimit(new Vector3(-3.14f, -3.14f, -3.14f));
                    joint.setAngularUpperLimit(new Vector3(3.14f, 3.14f, 3.14f));
                }
            }
            else
            {
                RigidBody other = joint.RigidBodyA;
                Offset = Quaternion.quatRotate(other.PhysicsRotation.inverse(), actorElement.PhysicsTranslation - other.PhysicsTranslation) - startingLocation;
                Rotation = other.PhysicsRotation.inverse() * actorElement.PhysicsRotation * startingRotation.inverse();
                joint.setLinearLowerLimit(Vector3.Zero);
                joint.setLinearUpperLimit(Vector3.Zero);
                joint.setAngularLowerLimit(Vector3.Zero);
                joint.setAngularUpperLimit(Vector3.Zero);
            }
        }

        void actorElement_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            if (otherBody != null)
            {
                TopTooth otherTooth = otherBody.Owner.getElement("Behavior") as TopTooth;
                if (otherTooth != null)
                {
                    collidingTeeth.Add(otherTooth);
                }
                else
                {
                    Splint splint = otherBody.Owner.getElement(Splint.SplintBehaviorName) as Splint;
                    if (splint != null)
                    {
                        collidingSplints.Add(splint);
                    }
                }
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            TopTooth otherTooth = otherBody.Owner.getElement("Behavior") as TopTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Remove(otherTooth);
            }
            else
            {
                Splint splint = otherBody.Owner.getElement(Splint.SplintBehaviorName) as Splint;
                if (splint != null)
                {
                    collidingSplints.Remove(splint);
                }
            }
        }

        [DoNotCopy]
        public override bool IsTopTooth
        {
            get
            {
                return false;
            }
        }
    }
}
