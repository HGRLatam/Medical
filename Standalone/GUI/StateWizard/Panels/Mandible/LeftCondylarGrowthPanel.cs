﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class LeftCondylarGrowthPanel : BoneManipulatorPanel
    {
        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        private GridPropertiesControl gridPropertiesControl;

        public LeftCondylarGrowthPanel(StateWizardPanelController controller)
            : base("Medical.GUI.StateWizard.Panels.Mandible.LeftCondylarGrowthPanel.layout", controller)
        {
            leftRamusHeightSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleGrowth/RamusHeightSlider") as VScroll);
            leftCondyleHeightSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleGrowth/CondyleHeightSlider") as VScroll);
            leftCondyleRotationSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleGrowth/CondyleRotationSlider") as VScroll);
            leftMandibularNotchSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleGrowth/MandibularNotchSlider") as VScroll);
            leftAntegonialNotchSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleGrowth/AntegonialNotchSlider") as VScroll);

            addBoneManipulator(leftRamusHeightSlider);
            addBoneManipulator(leftCondyleHeightSlider);
            addBoneManipulator(leftCondyleRotationSlider);
            addBoneManipulator(leftMandibularNotchSlider);
            addBoneManipulator(leftAntegonialNotchSlider);

            undoButton = mainWidget.findWidget("LeftCondyleGrowth/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("LeftCondyleGrowth/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            gridPropertiesControl.GridSpacing = 5;
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the condylar growth to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                setToDefault();
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the condylar growth to before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                resetToOpeningState();
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            gridPropertiesControl.updateGrid();
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            controller.MeasurementGrid.Visible = false;
        }
    }
}
