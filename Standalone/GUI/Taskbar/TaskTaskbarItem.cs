﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class TaskTaskbarItem : TaskbarItem
    {
        private Task item;
        private PopupMenu popupMenu;

        public event EventDelegate<TaskTaskbarItem> PinToTaskbar;

        public TaskTaskbarItem(Task item)
            :base(item.Name, item.IconName)
        {
            this.item = item;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            item.clicked();
        }

        public override void rightClicked(Widget source, EventArgs e)
        {
            popupMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 10, 10, Align.Default, "Info", "");
            customizeMenu();
            if (popupMenu.getItemCount() > 0)
            {
                IntVector2 position = findGoodPosition(popupMenu.Width, popupMenu.Height);
                popupMenu.setPosition(position.x, position.y);
                popupMenu.Closed += new MyGUIEvent(popupMenu_Closed);
                popupMenu.ItemAccept += new MyGUIEvent(popupMenu_ItemAccept);
            }
            else
            {
                Gui.Instance.destroyWidget(popupMenu);
            }
        }

        public Task Task
        {
            get
            {
                return item;
            }
        }

        protected virtual void customizeMenu()
        {
            addMenuItem("Pin", firePinTask);
        }

        protected void addMenuItem(String name, TaskMenuDelegate action)
        {
            MenuItem item = popupMenu.addItem(name);
            item.UserObject = action;
        }

        void popupMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs evt = (MenuCtrlAcceptEventArgs)e;
            ((TaskMenuDelegate)evt.Item.UserObject).Invoke();
        }

        void popupMenu_Closed(Widget source, EventArgs e)
        {
            Gui.Instance.destroyWidget(source);
        }

        void firePinTask()
        {
            if (PinToTaskbar != null)
            {
                PinToTaskbar.Invoke(this);
            }
        }
    }
}
