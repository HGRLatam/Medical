﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using Medical.Properties;

namespace Medical
{
    public class TeethStatePicker : DistortionWizard
    {
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;
        private String lastPresetDirectory;

        private TeethDistortionPicker teethDistortionPicker;
        private NotesPanel notesPanel;

        public TeethStatePicker(StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
        {
            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);
            statePicker = new StatePickerWizard(uiHost, temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            teethDistortionPicker = new TeethDistortionPicker();
            statePicker.addStatePanel(teethDistortionPicker);

            notesPanel = new NotesPanel(this.Name, imageRenderer);
            statePicker.addStatePanel(notesPanel);
        }

        public override void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
        }

        public override void setToDefault()
        {
            statePicker.setToDefault();
        }

        public override void sceneChanged(SimScene scene, string presetDirectory)
        {
            if (presetDirectory != lastPresetDirectory)
            {
                lastPresetDirectory = presetDirectory;
                teethDistortionPicker.initialize(presetDirectory + "/TeethPresets");
            }
        }

        public override void startWizard(DrawingWindow displayWindow)
        {
            statePicker.startWizard(displayWindow);
            statePicker.show();
        }

        public override string Name
        {
            get
            {
                return "Teeth Wizard";
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            alertStateCreated(state);
        }

        void statePicker_Finished()
        {
            alertWizardFinished();
        }
    }
}
