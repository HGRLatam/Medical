﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Properties;

namespace Medical.GUI
{
    public partial class FossaStatePanel : StatePickerPanel
    {
        public FossaStatePanel()
        {
            InitializeComponent();
            this.Text = "Fossa Flatness";
        }

        public override void applyToState(MedicalState state)
        {
            getRightFossaState(state.Fossa);
            getLeftFossaState(state.Fossa);
        }

        public override void setToDefault()
        {
            //rightEminenceNormal.Checked = true;
            //leftEminenceNormal.Checked = true;
        }

        private void getRightFossaState(FossaState fossaState)
        {
            if (rightEminenceNormal.Checked)
            {
                fossaState.addPosition("RightFossa", 0.0f);
            }
            else if (rightEminenceModerate.Checked)
            {
                fossaState.addPosition("RightFossa", 0.5f);
            }
            else if (rightEminenceSevere.Checked)
            {
                fossaState.addPosition("RightFossa", 1.0f);
            }
        }

        private void getLeftFossaState(FossaState fossaState)
        {
            if (leftEminenceNormal.Checked)
            {
                fossaState.addPosition("LeftFossa", 0.0f);
            }
            else if (leftEminenceModerate.Checked)
            {
                fossaState.addPosition("LeftFossa", 0.5f);
            }
            else if (leftEminenceSevere.Checked)
            {
                fossaState.addPosition("LeftFossa", 1.0f);
            }
        }

        private void rightEminenceNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rightEminenceNormal.Checked)
            {
                rightEminanceImage.Image = Resources.rightnormaleminance;
                showChanges(false);
            }
        }

        private void rightEminenceModerate_CheckedChanged(object sender, EventArgs e)
        {
            if (rightEminenceModerate.Checked)
            {
                rightEminanceImage.Image = Resources.rightsemiflateminance;
                showChanges(false);
            }
        }

        private void rightEminenceSevere_CheckedChanged(object sender, EventArgs e)
        {
            if (rightEminenceSevere.Checked)
            {
                showChanges(false);
                rightEminanceImage.Image = Resources.rightflateminance;
            }
        }

        private void leftEminenceNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (leftEminenceNormal.Checked)
            {
                leftEminanceImage.Image = Resources.leftnormaleminance;
                showChanges(false);
            }
        }

        private void leftEminenceModerate_CheckedChanged(object sender, EventArgs e)
        {
            if (leftEminenceModerate.Checked)
            {
                leftEminanceImage.Image = Resources.leftsemiflateminance;
                showChanges(false);
            }
        }

        private void leftEminenceSevere_CheckedChanged(object sender, EventArgs e)
        {
            if (leftEminenceSevere.Checked)
            {
                leftEminanceImage.Image = Resources.leftflateminance;
                showChanges(false);
            }
        }

        public override void recordOpeningState()
        {
            base.recordOpeningState();
        }

        public override void resetToOpeningState()
        {
            base.resetToOpeningState();
        }
    }
}
