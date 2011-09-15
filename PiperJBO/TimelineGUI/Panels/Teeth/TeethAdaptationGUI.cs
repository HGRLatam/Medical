﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class TeethAdaptationGUI : TimelineWizardPanel
    {
        private TeethMovementGUI teethMovementPanel;

        private Button undoButton;
        private Button resetButton;

        //GridPropertiesControl gridPropertiesControl;

        public TeethAdaptationGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Teeth.TeethAdaptationGUI.layout", wizard)
        {
            teethMovementPanel = new TeethMovementGUI(widget);

            undoButton = widget.findWidget("TeethAdaptationPanel/UndoButton") as Button;
            resetButton = widget.findWidget("TeethAdaptationPanel/ResetButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);

            //gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            //gridPropertiesControl.GridSpacing = 2;
        }

        public override void Dispose()
        {
            TeethController.showTeethTools(false, false);
            //controller.MeasurementGrid.Visible = false;
            teethMovementPanel.disableAllButtons();
            base.Dispose();
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

            leftCP.setLocation(leftCP.NeutralLocation);
            rightCP.setLocation(rightCP.NeutralLocation);
            movingMuscle.changeForce(TeethController.AdaptForce);
            movingMuscleTarget.Offset = Vector3.Zero;

            //gridPropertiesControl.Origin = TeethController.getToothCenter();
            //gridPropertiesControl.updateGrid();
            teethMovementPanel.setDefaultTools();
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the teeth alignment to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                TeethController.setAllOffsets(Vector3.Zero);
                TeethController.setAllRotations(Quaternion.Identity);
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the teeth alignment to before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                TeethState undo = timelineWizard.StateBlender.UndoState.Teeth;
                foreach (ToothState toothState in undo.StateEnum)
                {
                    Tooth tooth = TeethController.getTooth(toothState.Name);
                    tooth.Offset = toothState.Offset;
                    tooth.Rotation = toothState.Rotation;
                }
            }
        }
    }
}
