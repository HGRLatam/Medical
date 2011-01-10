﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;
using OgreWrapper;
using Engine.Attributes;
using BulletPlugin;

namespace Medical
{
    public abstract class StaticTooth : Tooth
    {
        [Editable]
        private String sceneNodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private String actorName = "Actor";

        [Editable]
        private String jointName = "Joint";

        [Editable]
        private String transparencyInterface = "Alpha";

        [Editable]
        private bool extracted = false;

        protected bool loose = false;

        protected SceneNodeElement sceneNodeElement;
        protected Entity entity;

        private TransparencyInterface transparency;
        protected RigidBody actorElement;
        protected Generic6DofConstraintElement joint;
        protected Vector3 startingLocation;
        protected Quaternion startingRotation;
        private Vector3 offset = Vector3.Zero;
        private Quaternion rotationOffset = Quaternion.Identity;
        private int adaptTeeth = 0;

        private bool toolHighlight = false;

        [DoNotCopy]
        [DoNotSave]
        protected List<Tooth> collidingTeeth = new List<Tooth>(5);

        [DoNotCopy]
        [DoNotSave]
        private bool showTools = false;

        protected override void constructed()
        {
            sceneNodeElement = Owner.getElement(sceneNodeName) as SceneNodeElement;
            if (sceneNodeElement == null)
            {
                blacklist("Could not find SceneNodeElement {0}.", sceneNodeName);
            }
            else
            {
                entity = sceneNodeElement.getNodeObject(entityName) as Entity;
                if (entity == null)
                {
                    blacklist("Could not find Entity {0}.", entityName);
                }
            }
            actorElement = Owner.getElement(actorName) as RigidBody;
            if (actorElement == null)
            {
                blacklist("Could not find Actor {0}.", actorName);
            }
            actorElement.MaxContactDistance = 0.05f;
            actorElement.setActivationState(ActivationState.DisableDeactivation);
            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }
            else
            {
                startingLocation = joint.getFrameOffsetOriginA();
                startingRotation = joint.getFrameOffsetBasisA();
            }
            transparency = Owner.getElement(transparencyInterface) as TransparencyInterface;
            if (transparency == null)
            {
                blacklist("Could not find TransparencyInterface {0}", transparencyInterface);
            }
            TeethController.addTooth(Owner.Name, this);
            HighlightColor = Color.White;
        }

        protected override void destroy()
        {
            TeethController.removeTooth(Owner.Name);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            bool highlight = false;
            if (TeethController.HighlightContacts)
            {
                highlight = MakingContact;
            }
            if (adaptTeeth > 0)
            {
                switch (adaptTeeth)
                {
                    case 1:
                        applyAdaptation(ToothType.Top, true);
                        break;
                    case 2:
                        applyAdaptation(ToothType.Top, false);
                        break;
                    case 3:
                        applyAdaptation(ToothType.Bottom, true);
                        break;
                    case 4:
                        applyAdaptation(ToothType.Bottom, false);
                        break;
                    default:
                        adaptTeeth = 0;
                        break;
                }
                adaptTeeth++;
            }

            if (toolHighlight)
            {
                HighlightColor = Color.Red;
            }
            else
            {
                HighlightColor = highlight ? Color.Blue : Color.White;
            }
        }

        protected abstract void looseChanged(bool loose);

        protected abstract void applyAdaptation(ToothType type, bool adapt);

        [DoNotCopy]
        public override bool Adapt
        {
            get
            {
                return adaptTeeth > 0;
            }
            set
            {
                //Start adaptation or do nothing if started.
                if (value)
                {
                    if (adaptTeeth == 0)
                    {
                        adaptTeeth = 1;
                    }
                }
                //Stop adaptation or do nothing if stopped.
                else
                {
                    if (adaptTeeth > 0)
                    {
                        adaptTeeth = 0;
                        applyAdaptation(ToothType.Top, false);
                        applyAdaptation(ToothType.Bottom, false);
                    }
                }
            }
        }

        [DoNotCopy]
        private Color HighlightColor
        {
            set
            {
                entity.getSubEntity(0).setCustomParameter(1, new Quaternion(value.r, value.g, value.b, value.a));
            }
        }

        [DoNotCopy]
        public override bool Extracted
        {
            get
            {
                return extracted;
            }
            set
            {
                //Put the tooth back if extracted
                if (this.extracted && !value)
                {
                    extracted = false;
                    actorElement.clearCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(true);
                    transparency.DisableOnHidden = true;
                }
                //Extract the tooth if it is in the scene
                else if (!this.extracted && value)
                {
                    extracted = true;
                    actorElement.raiseCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(false);
                    transparency.DisableOnHidden = false;
                }
            }
        }

        [DoNotCopy]
        public override bool Loose
        {
            get
            {
                return loose;
            }
            set
            {
                loose = value;
                looseChanged(loose);
            }
        }

        [DoNotCopy]
        public override Vector3 Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                joint.setFrameOffsetA(startingLocation + offset);
            }
        }

        [DoNotCopy]
        public override Quaternion Rotation
        {
            get
            {
                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
                joint.setFrameOffsetA(rotationOffset * startingRotation);
            }
        }

        [DoNotCopy]
        public override bool MakingContact
        {
            get
            {
                foreach (Tooth tooth in collidingTeeth)
                {
                    if (!tooth.Extracted)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #region MovableObject Members

        [DoNotCopy]
        public override Vector3 ToolTranslation
        {
            get
            {
                return Owner.Translation;
            }
        }

        public override void move(Vector3 offset)
        {
            Offset += offset;
        }

        [DoNotCopy]
        public override Quaternion ToolRotation
        {
            get
            {
                return rotationOffset;
            }
        }

        public override void rotate(ref Quaternion newRot)
        {
            Rotation = newRot;
        }

        [DoNotCopy]
        public override bool ShowTools
        {
            get
            {
                return showTools && !extracted;
            }
            set
            {
                showTools = value;
            }
        }

        public override void alertToolHighlightStatus(bool highlighted)
        {
            toolHighlight = highlighted;
        }

        #endregion
    }
}
