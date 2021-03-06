﻿using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Engine;
using Engine.Editing;
using Engine.Saving;
using libRocketPlugin;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using Medical.GUI.RmlWysiwyg.Elements;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideTriggerStrategy : ElementStrategy
    {
        private ElementTextEditor textEditor;
        private EditInterfaceEditor actionEditor;
        private Slide slide;
        private Browser actionTypeBrowser;
        private SlideAction currentAction;
        private UndoRedoBuffer undoBuffer;
        private EditInterfaceEditor appearanceEditor;
        private TextElementStyle elementStyle;
        private NotificationGUIManager notificationManager;
        private RunCommandsAction previewTriggerAction;
        private String primaryClassName;

        public event Action PreviewTrigger;

        /// <summary>
        /// Create a slide trigger strategy. The ActionTypeBrowser determines the slide action types that can be put on the slide.
        /// Be sure to set the DefaultSelection on this browser, this is used when the trigger has no action as the default.
        /// </summary>
        public SlideTriggerStrategy(Slide slide, Browser actionTypeBrowser, UndoRedoBuffer undoBuffer, String tag, String primaryClassName, String previewIconName, NotificationGUIManager notificationManager, RunCommandsAction previewTriggerAction)
            : base(tag, previewIconName, true)
        {
            this.primaryClassName = primaryClassName;
            this.previewTriggerAction = previewTriggerAction;
            this.undoBuffer = undoBuffer;
            this.slide = slide;
            this.actionTypeBrowser = actionTypeBrowser;
            this.notificationManager = notificationManager;
            ResizeHandles = ResizeType.Top | ResizeType.Height;
        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            elementStyle = new TextElementStyle(element, false);
            elementStyle.Changed += elementStyle_Changed;
            String rml = TextElementStrategy.DecodeFromHtml(element.InnerRml);
            textEditor = new ElementTextEditor(rml);
            String actionName = element.GetAttribute("onclick").StringValue;
            if (String.IsNullOrEmpty(actionName))
            {
                actionName = Guid.NewGuid().ToString();
                element.SetAttribute("onclick", actionName);
            }
            SlideAction action = slide.getAction(actionName);
            if (action == null)
            {
                action = ((Func<String, SlideAction>)actionTypeBrowser.DefaultSelection.Value)(actionName);
                slide.addAction(action);
            }

            //Make copy of action, this is really important, a lot of the function of this editor assumes this
            //is copied.
            SlideAction editingAction = CopySaver.Default.copy(action);

            EditInterface editInterface = setupEditInterface(editingAction, slide);
            actionEditor = new EditInterfaceEditor("Action", editInterface, uiCallback);
            appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(actionEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        private EditInterface setupEditInterface(SlideAction editingAction, Slide slide)
        {
            currentAction = editingAction;
            EditInterface editInterface = editingAction.getEditInterface();
            editingAction.ChangesMade += editingAction_ChangesMade;
            editInterface.addCommand(new EditInterfaceCommand("Change Type", callback =>
            {
                callback.showBrowser<Func<String, SlideAction>>(actionTypeBrowser, delegate(Func<String, SlideAction> result, ref string errorPrompt)
                {
                    currentAction.ChangesMade -= editingAction_ChangesMade;
                    SlideAction newAction = result(currentAction.Name);
                    newAction.ChangesMade += editingAction_ChangesMade;
                    actionEditor.EditInterface = setupEditInterface(newAction, slide);
                    editingAction_ChangesMade(newAction);
                    errorPrompt = "";
                    return true;
                });
            }));
            if (editingAction.AllowPreview)
            {
                editInterface.addCommand(new EditInterfaceCommand("Preview", callback =>
                    {
                        previewTriggerAction.clear();
                        currentAction.setupAction(slide, previewTriggerAction);
                        if (PreviewTrigger != null)
                        {
                            PreviewTrigger.Invoke();
                        }
                    }));
            }
            return editInterface;
        }

        void editingAction_ChangesMade(SlideAction obj)
        {
            String actionText = textEditor.Text;
            if (actionText == null)
            {
                actionText = "";
            }
            if (actionText.Length > 33)
            {
                actionText = actionText.Substring(0, 30) + "...";
            }

            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<SlideAction, SlideAction>(CopySaver.Default.copy(currentAction), slide.getAction(currentAction.Name),
                new TwoWayDelegateCommand<SlideAction, SlideAction>.Funcs()
                {
                    ExecuteFunc = (exec) =>
                    {
                        notificationManager.showNotification(String.Format("Changed trigger \"{0}\" action.", actionText), PreviewIconName, 3);
                        slide.replaceAction(exec);
                    },
                    UndoFunc = (undo) =>
                    {
                        notificationManager.showNotification(String.Format("Undid trigger \"{0}\" action.", actionText), PreviewIconName, 3);
                        slide.replaceAction(undo);
                    },
                }));
        }

        void elementStyle_Changed(StyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }

        public override HighlightProvider HighlightProvider
        {
            get
            {
                return elementStyle;
            }
        }

        public override void changeSizePreview(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            elementStyle.changeSize(newRect, resizeType, bounds);
            build(element);
        }

        public override Rect getStartingRect(Element selectedElement, out bool leftAnchor)
        {
            return elementStyle.createCurrentRect(selectedElement, out leftAnchor);
        }

        public override void applySizeChange(Element element)
        {
            appearanceEditor.alertChangesMade();
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component, out TwoWayCommand additionalUndoOperations)
        {
            additionalUndoOperations = null;
            return build(element);
        }

        private bool build(Element element)
        {
            String text = textEditor.Text;
            element.InnerRml = TextElementStrategy.EncodeToHtml(text);

            StringBuilder style = new StringBuilder();
            elementStyle.buildStyleAttribute(style);
            if (style.Length > 0)
            {
                element.SetAttribute("style", style.ToString());
            }
            else
            {
                element.RemoveAttribute("style");
            }

            StringBuilder classes = new StringBuilder();
            classes.AppendFormat("{0} ", primaryClassName);
            elementStyle.buildClassList(classes);
            if (classes.Length > 0)
            {
                element.SetAttribute("class", classes.ToString());
            }
            else
            {
                element.RemoveAttribute("class");
            }

            if (currentAction != null)
            {
                element.SetAttribute("onclick", currentAction.Name);
            }
            return true;
        }

        public override bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = element.InnerRml;
            if (String.IsNullOrEmpty(text))
            {
                component.deleteElement(element);
                return true;
            }
            return false;
        }
    }
}
