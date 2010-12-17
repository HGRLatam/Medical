﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropProperties : TimelineDataPanel, MovableObject
    {
        private ShowPropAction showProp;
        private bool comboUninitialized = true;
        private SimObjectMover simObjectMover;

        private ComboBox propTypes;
        private Edit translationEdit;
        private Edit rotationEdit;
        private ButtonGroup toolButtonGroup = new ButtonGroup();

        private Button translationButton;
        private Button rotationButton;

        public ShowPropProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowPropProperties.layout")
        {
            propTypes = mainWidget.findWidget("PropTypeCombo") as ComboBox;
            propTypes.EventComboChangePosition += new MyGUIEvent(propTypes_EventComboChangePosition);

            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;
            translationEdit.EventEditSelectAccept += new MyGUIEvent(translationEdit_EventEditSelectAccept);

            rotationEdit = mainWidget.findWidget("RotationEdit") as Edit;
            rotationEdit.EventEditSelectAccept += new MyGUIEvent(rotationEdit_EventEditSelectAccept);

            translationButton = mainWidget.findWidget("TranslationButton") as Button;
            toolButtonGroup.addButton(translationButton);

            rotationButton = mainWidget.findWidget("RotationButton") as Button;
            toolButtonGroup.addButton(rotationButton);

            toolButtonGroup.SelectedButton = translationButton;
            toolButtonGroup.SelectedButtonChanged += new EventHandler(toolButtonGroup_SelectedButtonChanged);
        }

        public override void setCurrentData(TimelineData data)
        {
            showProp = (ShowPropAction)((TimelineActionData)data).Action;
            if (comboUninitialized)
            {
                simObjectMover = showProp.TimelineController.SimObjectMover;
                PropFactory propFactory = showProp.TimelineController.PropFactory;
                foreach (String propName in propFactory.PropNames)
                {
                    propTypes.addItem(propName);
                }
                comboUninitialized = false;
            }
            uint index = propTypes.findItemIndexWith(showProp.PropType);
            if (index != ComboBox.Invalid)
            {
                propTypes.SelectedIndex = index;
            }
            translationEdit.Caption = showProp.Translation.ToString();
            Vector3 euler = showProp.Rotation.getEuler();
            euler *= 57.2957795f;
            rotationEdit.Caption = euler.ToString();
            simObjectMover.setActivePlanes(MovementAxis.All, MovementPlane.All);
            simObjectMover.addMovableObject("Prop", this);
            simObjectMover.ShowMoveTools = toolButtonGroup.SelectedButton == translationButton;
            simObjectMover.ShowRotateTools = toolButtonGroup.SelectedButton == rotationButton;
        }

        public override void editingCompleted()
        {
            showProp = null;
            simObjectMover.removeMovableObject(this);
            simObjectMover.ShowMoveTools = false;
            simObjectMover.ShowRotateTools = false;
        }

        void propTypes_EventComboChangePosition(Widget source, EventArgs e)
        {
            showProp.PropType = propTypes.getItemNameAt(propTypes.SelectedIndex);
        }

        void translationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 trans = new Vector3();
            trans.setValue(translationEdit.Caption);
            showProp.Translation = trans;
        }

        void rotationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 euler = new Vector3();
            euler.setValue(rotationEdit.Caption);
            euler *= 0.0174532925f;
            Quaternion rotation = new Quaternion(euler.x, euler.y, euler.z);
            showProp.Rotation = rotation;
        }

        void toolButtonGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            simObjectMover.ShowMoveTools = toolButtonGroup.SelectedButton == translationButton;
            simObjectMover.ShowRotateTools = toolButtonGroup.SelectedButton == rotationButton;
        }

        #region MovableObject Members

        public Vector3 ToolTranslation
        {
            get { return showProp.Translation; }
        }

        public void move(Vector3 offset)
        {
            showProp.Translation += offset;
            translationEdit.Caption = showProp.Translation.ToString();
        }

        public Quaternion ToolRotation
        {
            get { return showProp.Rotation; }
        }

        public bool ShowTools
        {
            get { return true; }
        }

        public void rotate(ref Quaternion newRot)
        {
            showProp.Rotation = newRot;
            Vector3 euler = showProp.Rotation.getEuler();
            euler *= 57.2957795f;
            rotationEdit.Caption = euler.ToString();
        }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }

        #endregion
    }
}
