﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;
using OgreWrapper;
using OgrePlugin;

namespace Medical
{
    public partial class AnatomyIdentifier : BehaviorInterface, Anatomy
    {
        [Editable]
        private String anatomicalName;

        [Editable]
        private bool allowGroupSelection = true;

        [Editable]
        bool showInBasicVersion = false;

        [Editable]
        bool showInTextSearch = true;

        [Editable]
        bool showInClickSearch = true;

        [Editable]
        bool showInTree = false;

        [Editable]
        private String region;

        [Editable]
        private String classification;

        [Editable]
        private String nodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private Vector3 previewCameraDirection = Vector3.Backward;

        [DoNotSave]
        private List<String> tags = new List<String>();

        [DoNotSave]
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        [DoNotSave]
        private List<String> systems = new List<String>();

        [DoNotSave]
        private List<String> connectedTo = new List<String>();

        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        [DoNotCopy]
        [DoNotSave]
        private Entity entity;

        [DoNotCopy]
        [DoNotSave]
        private LinkedList<Anatomy> relatedAnatomy = new LinkedList<Anatomy>();

        [DoNotCopy]
        [DoNotSave]
        private TransparencyAnatomyCommand transparencyCommand;

        protected override void constructed()
        {
            
        }

        protected override void link()
        {
            String errorMessage = null;
            foreach (AnatomyCommand command in commands)
            {
                if (!command.link(Owner, this, ref errorMessage))
                {
                    blacklist("SimObject {0} AnatomyIdentifier {1} failed to link command {2}. Reason: {3}", Owner.Name, AnatomicalName, command.UIText, errorMessage);
                }
            }
            SceneNodeElement nodeElement = Owner.getElement(nodeName) as SceneNodeElement;
            if (nodeElement == null)
            {
                blacklist("SimObject {0} AnatomyIdentifier {1} cannot find node named {2}", Owner.Name, AnatomicalName, nodeName);
            }
            entity = nodeElement.getNodeObject(entityName) as Entity;
            if (entity == null)
            {
                blacklist("SimObject {0} AnatomyIdentifier {1} Node {2} cannot find entity named {3}", Owner.Name, AnatomicalName, nodeName, entityName);
            }
            AnatomyManager.addAnatomy(this);
        }

        protected override void destroy()
        {
            AnatomyManager.removeAnatomy(this);
            foreach (AnatomyCommand command in commands)
            {
                command.destroy();
            }
        }

        public void addRelatedAnatomy(Anatomy anatomy)
        {
            relatedAnatomy.AddLast(anatomy);
        }

        public String AnatomicalName
        {
            get
            {
                return anatomicalName;
            }
        }

        [DoNotCopy]
        public IEnumerable<String> Tags
        {
            get
            {
                return tags;
            }
        }

        public void addTag(String tag)
        {
            tags.Add(tag);
            editInterface.safeAlertSubInterfaceDataContentsChanged(tags);
        }

        public void removeTag(String tag)
        {
            tags.Remove(tag);
            editInterface.safeAlertSubInterfaceDataContentsChanged(tags);
        }

        [DoNotCopy]
        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                return commands;
            }
        }

        [DoNotCopy]
        public IEnumerable<Anatomy> RelatedAnatomy
        {
            get
            {
                return relatedAnatomy;
            }
        }

        [DoNotCopy]
        public IEnumerable<Anatomy> SelectableAnatomy
        {
            get
            {
                yield return this;
            }
        }

        public AxisAlignedBox WorldBoundingBox
        {
            get
            {
                return entity.getWorldBoundingBox();
            }
        }

        /// <summary>
        /// Override the ability to select as a group. This is useful for stuff
        /// that is itself a top level object that isn't part of a group.
        /// </summary>
        public bool AllowGroupSelection
        {
            get
            {
                return allowGroupSelection;
            }
        }

        public bool ShowInTextSearch
        {
            get
            {
                return showInTextSearch;
            }
        }

        public bool ShowInClickSearch
        {
            get
            {
                return showInClickSearch;
            }
        }

        public bool ShowInTree
        {
            get
            {
                return showInTree;
            }
        }

        public bool ShowInBasicVersion
        {
            get
            {
                return showInBasicVersion;
            }
        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            if (transparencyCommand != null)
            {
                transparencyCommand.smoothBlend(alpha, duration, easingFunction);
            }
        }

        public float CurrentAlpha
        {
            get
            {
                if(transparencyCommand != null)
                {
                    return transparencyCommand.CurrentAlpha;
                }
                return 0.0f;
            }
        }

        public IEnumerable<String> TransparencyNames
        {
            get
            {
                if(transparencyCommand != null)
                {
                    yield return transparencyCommand.TransparencyInterfaceName;
                }
                yield break;
            }
        }

        [DoNotCopy]
        public Vector3 PreviewCameraDirection
        {
            get
            {
                return previewCameraDirection;
            }
            set
            {
                previewCameraDirection = value;
            }
        }

        public void addCommand(AnatomyCommand command)
        {
            commands.Add(command);
            editInterface.safeAddSubInterfaceForObject(commands, command);
        }

        public void removeCommand(AnatomyCommand command)
        {
            commands.Remove(command);
            editInterface.safeRemoveSubInterfaceForObject(commands, command);
        }

        internal bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return entity.raycastPolygonLevel(ray, ref distanceOnRay);
        }

        internal void _setTransparencyCommand(TransparencyAnatomyCommand command)
        {
            this.transparencyCommand = command;
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList<String>("AnatomyTag", tags);
            info.ExtractList<AnatomyCommand>("AnatomyCommand", commands);
            info.ExtractList("System", systems);
            info.ExtractList("ConnectedTo", connectedTo);
        }

        //Erase this static property when finished upgrading
        static String[] classificationUpgrades = { "Bone", "Ligament", "Muscle" };
        static String[] regions = { "Arm", "Leg" };
        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            //info.RebuildList<String>("AnatomyTag", tags); //Add this back after conversion completed, can remove AnatomyTag class then also.
            info.RebuildList<AnatomyCommand>("AnatomyCommand", commands);
            info.RebuildList("System", systems);
            info.RebuildList("ConnectedTo", connectedTo);

            
            //Custom conversion code from tags to new style, remove this after updating
            List<AnatomyTag> anatomyTags = new List<AnatomyTag>();
            info.RebuildList<AnatomyTag>("AnatomyTag", anatomyTags);
            bool updateSystems = systems.Count == 0;
            List<AnatomyTag> toRemove = new List<AnatomyTag>();
            foreach (var tag in anatomyTags)
            {
                if (updateSystems && tag.Tag.Contains("System") && !systems.Contains(tag.Tag))
                {
                    systems.Add(tag.Tag);
                    toRemove.Add(tag);
                }
                if(classification == null && classificationUpgrades.Contains(tag.Tag))
                {
                    classification = tag.Tag;
                    toRemove.Add(tag);
                }
                if(region == null && regions.Contains(tag.Tag))
                {
                    region = tag.Tag;
                    toRemove.Add(tag);
                }
            }
            anatomyTags.RemoveAll(i => toRemove.Contains(i));
            tags.AddRange(anatomyTags.Select(t => t.Tag));
        }

        private static AnatomyCommandBrowser anatomyCommandBrowser = null;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);

            this.editInterface = editInterface;

            editInterface.addSubInterfaceForObject(tags, new StringListlikeEditInterface(tags, "Tags",
                validateCallback: () =>
                {
                    if (this.Tags.Any(t => String.IsNullOrWhiteSpace(t)))
                    {
                        throw new ValidationException("Cannot accept blank tags. Please remove any blank entries.");
                    }
                }));
            editInterface.addSubInterfaceForObject(systems, new StringListlikeEditInterface(systems, "Systems"));
            editInterface.addSubInterfaceForObject(connectedTo, new StringListlikeEditInterface(connectedTo, "Connected To"));

            //Commands
            EditInterface commandsEditInterface = new EditInterface("Commands");

            commandsEditInterface.addCommand(new EditInterfaceCommand("Add Command", cb =>
            {
                if (anatomyCommandBrowser == null)
                {
                    anatomyCommandBrowser = new AnatomyCommandBrowser();
                }
                cb.showBrowser(anatomyCommandBrowser, delegate(Object result, ref String errorMessage)
                {
                    Type commandType = result as Type;
                    if (commandType != null)
                    {
                        this.addCommand((AnatomyCommand)Activator.CreateInstance(commandType));
                        return true;
                    }
                    return false;
                });
            }));

            var commandEditInterfaces = commandsEditInterface.createEditInterfaceManager<AnatomyCommand>(i => i.createEditInterface(), Commands);
            commandEditInterfaces.addCommand(new EditInterfaceCommand("Remove", cb => this.removeCommand(commandsEditInterface.resolveSourceObject<AnatomyCommand>(cb.getSelectedEditInterface()))));
            editInterface.addSubInterfaceForObject(commands, commandsEditInterface);
        }
    }
}
