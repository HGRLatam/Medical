﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class TeethMovementGUI
    {
        private static String[] LEFT_LATERAL_TEETH = 
        {
            "Tooth01", "Tooth02", "Tooth03", "Tooth04", "Tooth05", "Tooth06", 
            "Tooth32", "Tooth31", "Tooth30", "Tooth29", "Tooth28", "Tooth27"
        };

        private static String[] MIDLINE_ANTERIOR_TEETH = 
        {
            "Tooth05", "Tooth06", "Tooth07", "Tooth08", "Tooth09", "Tooth10", 
            "Tooth11", "Tooth12",
            "Tooth28", "Tooth27", "Tooth26", "Tooth25", "Tooth24", "Tooth23", 
            "Tooth22", "Tooth21"
        };

        private static String[] RIGHT_LATERAL_TEETH = 
        {
            "Tooth16", "Tooth15", "Tooth14", "Tooth13", "Tooth12", "Tooth11",
            "Tooth17", "Tooth18", "Tooth19", "Tooth20", "Tooth21", "Tooth22"
        };

        private CheckButton adaptButton;
        private CheckButton moveButton;
        private CheckButton rotateButton;

        private Button topCameraButton;
        private Button bottomCameraButton;
        private Button leftLateralCameraButton;
        private Button midlineAnteriorCameraButton;
        private Button rightLateralCameraButton;

        private TeethAdaptationGUIData teethGUIData;
        private TimelineWizardPanel panel;

        public TeethMovementGUI(Widget mainWidget, TeethAdaptationGUIData teethGUIData, TimelineWizardPanel panel)
        {
            this.teethGUIData = teethGUIData;
            this.panel = panel;

            adaptButton = new CheckButton(mainWidget.findWidget("TeethMovmentPanel/AdaptButton") as Button);
            moveButton = new CheckButton(mainWidget.findWidget("TeethMovmentPanel/MoveButton") as Button);
            rotateButton = new CheckButton(mainWidget.findWidget("TeethMovmentPanel/RotateButton") as Button);

            adaptButton.CheckedChanged += new MyGUIEvent(adaptButton_CheckedChanged);
            moveButton.CheckedChanged += new MyGUIEvent(moveButton_CheckedChanged);
            rotateButton.CheckedChanged += new MyGUIEvent(rotateButton_CheckedChanged);

            topCameraButton = mainWidget.findWidget("TeethMovementPanel/TopCameraButton") as Button;
            bottomCameraButton = mainWidget.findWidget("TeethMovementPanel/BottomCameraButton") as Button;
            leftLateralCameraButton = mainWidget.findWidget("TeethMovementPanel/LeftLateralCameraButton") as Button;
            midlineAnteriorCameraButton = mainWidget.findWidget("TeethMovementPanel/MidlineAnteriorCameraButton") as Button;
            rightLateralCameraButton = mainWidget.findWidget("TeethMovementPanel/RightLateralCameraButton") as Button;

            topCameraButton.MouseButtonClick += new MyGUIEvent(topCameraButton_MouseButtonClick);
            bottomCameraButton.MouseButtonClick += new MyGUIEvent(bottomCameraButton_MouseButtonClick);
            leftLateralCameraButton.MouseButtonClick += new MyGUIEvent(leftLateralCameraButton_MouseButtonClick);
            midlineAnteriorCameraButton.MouseButtonClick += new MyGUIEvent(midlineAnteriorCameraButton_MouseButtonClick);
            rightLateralCameraButton.MouseButtonClick += new MyGUIEvent(rightLateralCameraButton_MouseButtonClick);
        }

        public void disableAllButtons()
        {
            adaptButton.Checked = false;
            moveButton.Checked = false;
            rotateButton.Checked = false;
        }

        public void setDefaultTools()
        {
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Y, MovementPlane.XY);
        }

        private void topCameraButton_MouseButtonClick(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, false);
            panel.applyCameraPosition(teethGUIData.TopCamera);
            panel.applyLayers(teethGUIData.TopCameraLayers);
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Z, MovementPlane.XZ);
        }

        private void bottomCameraButton_MouseButtonClick(object sender, EventArgs e)
        {
            TeethController.showTeethTools(false, true);
            panel.applyCameraPosition(teethGUIData.BottomCamera);
            panel.applyLayers(teethGUIData.BottomCameraLayers);
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Z, MovementPlane.XZ);
        }

        private void leftLateralCameraButton_MouseButtonClick(object sender, EventArgs e)
        {
            TeethController.showTeethTools(LEFT_LATERAL_TEETH);
            panel.applyCameraPosition(teethGUIData.LeftLateralCamera);
            panel.applyLayers(teethGUIData.AllTeethLayers);
            TeethController.TeethMover.setActivePlanes(MovementAxis.Y | MovementAxis.Z, MovementPlane.YZ); 
        }

        private void midlineAnteriorCameraButton_MouseButtonClick(object sender, EventArgs e)
        {
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            panel.applyCameraPosition(teethGUIData.MidlineAnteriorCamera);
            panel.applyLayers(teethGUIData.AllTeethLayers);
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Y, MovementPlane.XY);
        }

        private void rightLateralCameraButton_MouseButtonClick(object sender, EventArgs e)
        {
            TeethController.showTeethTools(RIGHT_LATERAL_TEETH);
            panel.applyCameraPosition(teethGUIData.RightLateralCamera);
            panel.applyLayers(teethGUIData.AllTeethLayers);
            TeethController.TeethMover.setActivePlanes(MovementAxis.Y | MovementAxis.Z, MovementPlane.YZ);
        }

        void adaptButton_CheckedChanged(Widget sender, EventArgs e)
        {
            TeethController.adaptAllTeeth(adaptButton.Checked);
            if (adaptButton.Checked)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
                MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

                leftCP.setLocation(leftCP.NeutralLocation);
                rightCP.setLocation(rightCP.NeutralLocation);
                movingMuscle.changeForce(1.0f);
                movingMuscleTarget.Offset = Vector3.Zero;

                moveButton.Checked = false;
                rotateButton.Checked = false;
            }
        }

        void rotateButton_CheckedChanged(Widget sender, EventArgs e)
        {
            TeethController.TeethMover.ShowRotateTools = rotateButton.Checked;
            if (rotateButton.Checked)
            {
                moveButton.Checked = false;
                adaptButton.Checked = false;
            }
        }

        void moveButton_CheckedChanged(Widget sender, EventArgs e)
        {
            TeethController.TeethMover.ShowMoveTools = moveButton.Checked;
            if (moveButton.Checked)
            {
                rotateButton.Checked = false;
                adaptButton.Checked = false;
            }
        }
    }
}
