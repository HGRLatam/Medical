﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using System.IO;
using Medical.Editor;

namespace Medical
{
    class MvcTypeController : SaveableTypeController<AnomalousMvcContext>
    {
        private EditorController editorController;
        private MvcEditorContext editorContext;
        public const String Icon = "EditorFileIcon/.mvc";

        public MvcTypeController(EditorController editorController)
            :base(".mvc", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
        }

        public override void openFile(string file)
        {
            AnomalousMvcContext context = (AnomalousMvcContext)loadObject(file);
            BrowserWindowController.setCurrentEditingMvcContext(context);

            editorContext = new MvcEditorContext(context, file, this);
            editorContext.Shutdown += new Action<MvcEditorContext>(editorContext_Shutdown);
            editorController.runEditorContext(editorContext.MvcContext);
        }

        public void saveFile(AnomalousMvcContext context, string file)
        {
            saveObject(file, context);
            editorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }
        
        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create MVC Context", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("MVC Context Name", "Enter a name for the MVC Context.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".mvc");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewContext(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewContext(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewContext(String filePath)
        {
            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            creatingNewFile(filePath);
            saveObject(filePath, mvcContext);
            openFile(filePath);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            if (editorContext != null)
            {
                editorContext.close();
            }
            closeCurrentCachedResource();
        }

        void editorContext_Shutdown(MvcEditorContext obj)
        {
            if (editorContext == obj)
            {
                editorContext = null;
            }
        }
    }
}
