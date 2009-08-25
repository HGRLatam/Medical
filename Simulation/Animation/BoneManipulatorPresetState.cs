﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class BoneManipulatorPresetState : PresetState
    {
        private List<BoneManipulatorStateEntry> positions = new List<BoneManipulatorStateEntry>();

        public BoneManipulatorPresetState(String name, String category, String imageName)
            :base(name, category, imageName)
        {

        }

        public void addPosition(BoneManipulatorStateEntry entry)
        {
            positions.Add(entry);
        }

        public override void applyToState(MedicalState state)
        {
            foreach (BoneManipulatorStateEntry position in positions)
            {
                state.BoneManipulator.addPosition(position.clone());
            }
        }

        #region Saveable Members

        private const String POSITION_BASE = "Position";

        protected BoneManipulatorPresetState(LoadInfo info)
            :base(info)
        {
            info.RebuildList<BoneManipulatorStateEntry>(POSITION_BASE, positions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<BoneManipulatorStateEntry>(POSITION_BASE, positions);
        }

        #endregion
    }
}
