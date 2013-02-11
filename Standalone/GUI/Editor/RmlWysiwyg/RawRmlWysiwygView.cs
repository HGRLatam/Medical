﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Engine.Attributes;

namespace Medical.GUI
{
    public class RawRmlWysiwygView : MyGUIView
    {
        public event Action<RawRmlWysiwygView, RmlWysiwygComponent> ComponentCreated;

        [DoNotSave]
        private UndoRedoBuffer undoBuffer;

        [DoNotSave]
        private LinkedList<ElementStrategy> customStrategies = new LinkedList<ElementStrategy>();

        public RawRmlWysiwygView(String name, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, UndoRedoBuffer undoBuffer)
            :base(name)
        {
            this.UICallback = uiCallback;
            this.BrowserProvider = browserProvider;
            this.undoBuffer = undoBuffer;
        }

        public void addCustomStrategy(ElementStrategy strategy)
        {
            customStrategies.AddLast(strategy);
        }

        [Editable]
        public String Rml { get; set; }

        public Action<String> UndoRedoCallback { get; set; }

        public UndoRedoBuffer UndoBuffer
        {
            get
            {
                return undoBuffer;
            }
        }

        public IEnumerable<ElementStrategy> CustomElementStrategies
        {
            get
            {
                return customStrategies;
            }
        }

        public MedicalUICallback UICallback { get; private set; }

        public RmlWysiwygBrowserProvider BrowserProvider { get; private set; }

        internal void _fireComponentCreated(RmlWysiwygComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        protected RawRmlWysiwygView(LoadInfo info)
            :base (info)
        {

        }
    }
}
