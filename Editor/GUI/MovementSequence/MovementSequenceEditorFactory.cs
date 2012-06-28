﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class MovementSequenceEditorFactory : ViewHostComponentFactory
    {
        MovementSequenceController movementSequenceController;
        EditorController editorController;
        SaveableClipboard clipboard;

        public MovementSequenceEditorFactory(MovementSequenceController movementSequenceController, EditorController editorController, SaveableClipboard clipboard)
        {
            this.movementSequenceController = movementSequenceController;
            this.editorController = editorController;
            this.clipboard = clipboard;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is MovementSequenceEditorView)
            {
                return new MovementSequenceEditor(movementSequenceController, clipboard, editorController, viewHost, (MovementSequenceEditorView)view);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
