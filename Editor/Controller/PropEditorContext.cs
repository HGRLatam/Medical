﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;
using Medical.Controller;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;

namespace Medical
{
    class PropEditorContext
    {
        public event Action<PropEditorContext> Focus;
        public event Action<PropEditorContext> Blur;

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private PropTypeController typeController;

        private PropDefinition propDefinition;

        public PropEditorContext(PropDefinition propDefinition, String file, PropTypeController typeController, EditorController editorController, GuiFrameworkUICallback uiCallback)
        {
            this.typeController = typeController;
            this.currentFile = file;
            this.propDefinition = propDefinition;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", propDefinition.EditInterface, editorController, uiCallback, true);
            genericPropertiesView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
            mvcContext.Views.add(genericPropertiesView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Plugin Definition File", "CommonToolstrip/Save", "File", "Editor/Save", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("MvcContext"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
                    })));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
                new CallbackAction("Focus", context =>
                    {
                        GlobalContextEventHandler.setEventContext(eventContext);
                        if (Focus != null)
                        {
                            Focus.Invoke(this);
                        }
                    }),
                new CallbackAction("Blur", context =>
                    {
                        GlobalContextEventHandler.disableEventContext(eventContext);
                        if (Blur != null)
                        {
                            Blur.Invoke(this);
                        }
                    }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            eventContext = new EventContext();
            ButtonEvent saveEvent = new ButtonEvent(EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);
        }

        public void close()
        {
            mvcContext.runAction("Editor/Close");
        }

        public AnomalousMvcContext MvcContext
        {
            get
            {
                return mvcContext;
            }
        }

        private void saveAll()
        {
            typeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            typeController.saveFile(propDefinition, currentFile);
        }
    }
}
