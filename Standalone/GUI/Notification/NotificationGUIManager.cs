﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class NotificationGUIManager : IDisposable
    {
        private Taskbar taskbar;
        private List<NotificationGUI> openNotifications = new List<NotificationGUI>();

        public NotificationGUIManager(Taskbar taskbar)
        {
            this.taskbar = taskbar;
        }

        public void Dispose()
        {
            foreach (NotificationGUI notification in openNotifications)
            {
                notification.Dispose();
            }
        }

        public void showNotification(String text, String imageKey)
        {
            NotificationGUI notification = new NotificationGUI(text, imageKey, this);
            positionNotification(notification);
        }

        public void showTaskNotification(String text, String imageKey, Task task)
        {
            NotificationGUI notification = new StartTaskNotification(text, imageKey, this, task);
            positionNotification(notification);
        }

        internal void notificationClosed(NotificationGUI notification)
        {
            openNotifications.Remove(notification);
            relayoutNotifications();
        }

        internal void hideAllNotifications()
        {
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.hide();
            }
        }

        internal void reshowAllNotifications()
        {
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.show(openNotification.Left, openNotification.Top);
            }
        }

        private void positionNotification(NotificationGUI notification)
        {
            int additionalHeightOffset = 0;
            foreach(NotificationGUI openNotification in openNotifications)
            {
                additionalHeightOffset += openNotification.Height;
            }
            openNotifications.Add(notification);
            switch (taskbar.Alignment)
            {
                case TaskbarAlignment.Top:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, taskbar.Height + additionalHeightOffset);
                    break;
                case TaskbarAlignment.Right:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width - taskbar.Width, additionalHeightOffset);
                    break;
                default:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, additionalHeightOffset);
                    break;
            }
        }

        private void relayoutNotifications()
        {
            int currentHeight = 0;
            if (taskbar.Alignment == TaskbarAlignment.Top)
            {
                currentHeight = taskbar.Height;
            }
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.setPosition(openNotification.Left, currentHeight);
                currentHeight += openNotification.Height;
            }
        }
    }
}
