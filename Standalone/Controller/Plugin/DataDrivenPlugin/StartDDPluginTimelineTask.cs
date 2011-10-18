﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using System.IO;

namespace Medical
{
    class StartDDPluginTimelineTask : DDPluginTask
    {
        public StartDDPluginTimelineTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {
            ShowOnTaskbar = false;
        }

        public void registerPlugin(DDAtlasPlugin dataDrivenPlugin)
        {

        }

        public override void clicked()
        {
            TimelineController timelineController = Plugin.TimelineController;
            if (timelineController.MultiTimelinePlaybackInProgress)
            {
                MyGUIPlugin.MessageBox.show("Cannot start this task right now. Please close other timeline task.", "Cannot start task.", MyGUIPlugin.MessageBoxStyle.Ok | MyGUIPlugin.MessageBoxStyle.IconWarning);
            }
            else
            {
                timelineController.ResourceProvider = new TimelineVirtualFSResourceProvider(Path.Combine(Plugin.PluginRootFolder, TimelineDirectory));
                Timeline start = timelineController.openTimeline(StartupTimeline);
                timelineController.startPlayback(start);
            }
        }

        [Editable]
        public String TimelineDirectory { get; set; }

        [Editable]
        public String StartupTimeline { get; set; }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        #region Saveable Members

        protected StartDDPluginTimelineTask(LoadInfo info)
            :base(info)
        {
            TimelineDirectory = info.GetString("TimelineDirectory");
            StartupTimeline = info.GetString("StartupTimeline");
            ShowOnTaskbar = false;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TimelineDirectory", TimelineDirectory);
            info.AddValue("StartupTimeline", StartupTimeline);
        }

        #endregion
    }
}
