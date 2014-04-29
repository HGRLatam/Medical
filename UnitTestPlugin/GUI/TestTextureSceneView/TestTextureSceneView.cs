﻿using Engine;
using Medical;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPlugin.GUI
{
    class TestTextureSceneView : MDIDialog
    {
        private SingleSelectButtonGrid buttonGrid;
        private ScrollView scrollView;

        private EditBox secondsToSleepEdit;
        private EditBox numToUpdateEdit;

        private ButtonGridLiveThumbnailController liveThumbHost;

        private int count = 0;

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            Button addButton = (Button)window.findWidget("AddButton");
            addButton.MouseButtonClick += addButton_MouseButtonClick;

            Button removeButton = (Button)window.findWidget("RemoveButton");
            removeButton.MouseButtonClick +=removeButton_MouseButtonClick;

            scrollView = (ScrollView)window.findWidget("ScrollView");
            buttonGrid = new SingleSelectButtonGrid(scrollView);

            window.WindowChangedCoord += window_WindowChangedCoord;

            liveThumbHost = new ButtonGridLiveThumbnailController("TestRTT_", new IntSize2(200, 200), sceneViewController, buttonGrid, scrollView);

            numToUpdateEdit = (EditBox)window.findWidget("NumToUpdate");
            numToUpdateEdit.Caption = liveThumbHost.NumImagesToUpdate.ToString();
            secondsToSleepEdit = (EditBox)window.findWidget("SecondsToSleep");
            secondsToSleepEdit.Caption = liveThumbHost.SecondsToSleep.ToString();
            Button applyButton = (Button)window.findWidget("ApplyButton");
            applyButton.MouseButtonClick += applyButton_MouseButtonClick;
        }

        public override void Dispose()
        {
            liveThumbHost.Dispose();
            buttonGrid.Dispose();
            base.Dispose();
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            int numImagesToUpdate;
            if (int.TryParse(numToUpdateEdit.Caption, out numImagesToUpdate))
            {
                liveThumbHost.NumImagesToUpdate = numImagesToUpdate;
            }
            double secondsToSleep;
            if (double.TryParse(secondsToSleepEdit.Caption, out secondsToSleep))
            {
                liveThumbHost.SecondsToSleep = secondsToSleep;
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem item = buttonGrid.addItem("Main", count++.ToString());
            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
            liveThumbHost.itemAdded(item);
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            var selectedItem = buttonGrid.SelectedItem;
            if (selectedItem != null)
            {
                buttonGrid.removeItem(selectedItem);
                liveThumbHost.itemRemoved(selectedItem);
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            liveThumbHost.resizeAndLayout();
        }
    }
}
