﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;

namespace Medical.GUI
{
    class ScratchArea : MDIDialog
    {
        private ScratchAreaController scratchAreaController;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;
        private GuiFrameworkUICallback uiCallback;

        public ScratchArea(ScratchAreaController scratchAreaController, GuiFrameworkUICallback uiCallback)
            :base("Medical.GUI.ScratchArea.ScratchArea.layout")
        {
            this.scratchAreaController = scratchAreaController;

            this.uiCallback = uiCallback;
            uiCallback.addCustomQuery<SaveableClipboard>(ScratchAreaCustomQueries.GetClipboard, getClipboardCallback);

            tree = new Tree((ScrollView)window.findWidget("TableScroll"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);
            editTreeView.EditInterface = scratchAreaController.EditInterface;
            editTreeView.EditInterfaceSelectionChanged += editTreeView_EditInterfaceSelectionChanged;

            this.Resized += new EventHandler(ScratchArea_Resized);
        }

        public override void Dispose()
        {
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        void ScratchArea_Resized(object sender, EventArgs e)
        {
            tree.layout();
        }

        void editTreeView_EditInterfaceSelectionChanged(EditInterfaceViewEventArgs evt)
        {
            uiCallback.SelectedEditInterface = evt.EditInterface;
        }

        void getClipboardCallback(SendResult<SaveableClipboard> resultCallback)
        {
            String error = null;
            resultCallback.Invoke(scratchAreaController.Clipboard, ref error);
        }
    }
}
