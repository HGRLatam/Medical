﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class GUITimelineTaskManager
    {
        private Taskbar timelineGUITaskbar;
        private TaskController taskController;

        private bool addTasksAsAdded = false;

        private Dictionary<Task, TimelineTaskbarItem> timelineTaskbarItems = new Dictionary<Task, TimelineTaskbarItem>();

        public GUITimelineTaskManager(Taskbar timelineGUITaskbar, TaskController taskController)
        {
            this.timelineGUITaskbar = timelineGUITaskbar;
            this.taskController = taskController;

            taskController.TaskAdded += new TaskDelegate(TaskController_TaskAdded);
            taskController.TaskRemoved += new TaskDelegate(TaskController_TaskRemoved);
        }

        public void loadActiveTaskElements()
        {
            foreach (Task task in taskController.Tasks)
            {
                if (task.ShowOnTimelineTaskbar)
                {
                    addTimelineTaskbarItem(task);
                }
            }
            addTasksAsAdded = true;
        }

        void TaskController_TaskRemoved(Task task)
        {
            //Check to see that the task should show up on the timeline taskbar
            if (task.ShowOnTimelineTaskbar)
            {
                TimelineTaskbarItem item = timelineTaskbarItems[task];
                timelineTaskbarItems.Remove(task);
                timelineGUITaskbar.removeItem(item);
            }
        }

        void TaskController_TaskAdded(Task task)
        {
            //Check to see that the task should show up on the timeline taskbar, if this is done as tasks are added the first one's icon won't work, so this hack
            //works around that.
            if (addTasksAsAdded && task.ShowOnTimelineTaskbar)
            {
                addTimelineTaskbarItem(task);
            }
        }

        private void addTimelineTaskbarItem(Task task)
        {
            TimelineTaskbarItem timelineTaskbarItem = new TimelineTaskbarItem(task);
            timelineGUITaskbar.addItem(timelineTaskbarItem);
            timelineTaskbarItems.Add(task, timelineTaskbarItem);
        }
    }
}
