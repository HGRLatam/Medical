﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    class HighlightTeethAction : TimelineAction
    {
        public static readonly String Name = "Highlight Teeth";

        public HighlightTeethAction()
        {

        }

        public HighlightTeethAction(bool enable, float startTime)
        {
            this.EnableHighlight = enable;
            this.StartTime = startTime;
        }

        public override void started(float timelineTime, Clock clock)
        {
            TeethController.HighlightContacts = EnableHighlight;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override bool Finished
        {
            get { return true; }
        }

        public override String TypeName
        {
            get { return Name; }
        }

        public bool EnableHighlight { get; set; }

        #region Saveable

        private static readonly String ENABLE_HIGHLIGHT = "EnableHighlight";

        protected HighlightTeethAction(LoadInfo info)
            :base(info)
        {
            EnableHighlight = info.GetBoolean(ENABLE_HIGHLIGHT, false);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(ENABLE_HIGHLIGHT, EnableHighlight);
            base.getInfo(info);
        }

        #endregion
    }
}
