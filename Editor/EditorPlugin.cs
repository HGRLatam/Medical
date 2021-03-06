﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using Medical.Editor;
using System.Reflection;
using System.IO;
using Engine;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;

namespace Medical
{
    public class EditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        private ScratchArea scratchArea;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;
        private ScratchAreaController scratchAreaController;

        private AspectRatioTask aspectRatioTask;
        private ProjectExplorer projectExplorer;

        private EditorController editorController;
        private EditorUICallback editorUICallback;
        private PropEditController propEditController;
        private TypeControllerManager typeControllerManager;
        private EditorTaskbarFactory editorTaskbarFactory;

        public EditorPlugin()
        {
            AllowUninstall = true;

            Log.Info("Editor GUI Loaded");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    String assemblyFileName = new AssemblyName(args.Name).Name;
                    String embeddedResourceName = String.Format("Medical.Resources.{0}.dll", assemblyFileName);
                    try
                    {
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
                        {
                            if (stream != null)
                            {
                                Byte[] assemblyData = new Byte[stream.Length];
                                stream.Read(assemblyData, 0, assemblyData.Length);
                                return Assembly.Load(assemblyData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    return null;
                };
        }

        public void Dispose()
        {
            EditorConfig.save();
            projectExplorer.Dispose();
            scratchArea.Dispose();
            aspectRatioTask.Dispose();
            editorController.Dispose();
            propMover.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.EditorPlugin_MyGUI_Skin.xml");
            ResourceManager.Instance.load("Medical.Resources.EditorImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;
            guiManager.MainGUIShown += new Action(guiManager_MainGUIShown);
            guiManager.MainGUIHidden += new Action(guiManager_MainGUIHidden);

            //Prop Mover
            MedicalController medicalController = standaloneController.MedicalController;
            propMover = new SimObjectMover("Props", medicalController.PluginManager.RendererPlugin, medicalController.EventManager, standaloneController.SceneViewController);

            this.standaloneController = standaloneController;
            editorTimelineController = new TimelineController(standaloneController);
            standaloneController.giveGUIsToTimelineController(editorTimelineController);

            scratchAreaController = new ScratchAreaController(standaloneController.Clipboard);

            //Controller
            editorController = new EditorController(standaloneController, editorTimelineController);
            standaloneController.DocumentController.addDocumentHandler(new ProjectDocumentHandler(editorController));
            standaloneController.DocumentController.UnknownDocumentHander = new UnknownDocumentHandler(editorController);
            propEditController = new PropEditController(propMover);

            //UI Helpers
            editorUICallback = new EditorUICallback(standaloneController, editorController, propEditController);

            typeControllerManager = new TypeControllerManager(standaloneController, this);
            typeControllerManager.FilesystemWatcherCreated += typeControllerManager_FilesystemWatcherCreated;

            //Dialogs
            scratchArea = new ScratchArea(scratchAreaController, editorUICallback);
            guiManager.addManagedDialog(scratchArea);

            projectExplorer = new ProjectExplorer(editorController, typeControllerManager);
            guiManager.addManagedDialog(projectExplorer);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            aspectRatioTask = new AspectRatioTask(standaloneController.SceneViewController);

            if (MedicalConfig.ShowDeveloperTools)
            {
                taskController.addTask(new MDIDialogOpenTask(scratchArea, "Medical.ScratchArea", "Scratch Area", "EditorIcons.ScratchAreaIcon", TaskMenuCategories.Create));
                taskController.addTask(new MDIDialogOpenTask(projectExplorer, "Medical.EditorTools", "Editor Tools", "EditorIcons.EditorTools", TaskMenuCategories.Create));
                taskController.addTask(aspectRatioTask);
            }

            editorTaskbarFactory = new EditorTaskbarFactory(editorController);
            standaloneController.ViewHostFactory.addFactory(new EditorInfoBarFactory());
            standaloneController.ViewHostFactory.addFactory(new TextEditorComponentFactory());
            standaloneController.ViewHostFactory.addFactory(editorTaskbarFactory);
            CommonEditorResources.initialize(standaloneController);

            editorController.ProjectChanged += editorController_ProjectChanged;

            //Editor Views
            standaloneController.ViewHostFactory.addFactory(new OffsetSequenceEditorFactory(standaloneController.MedicalController, standaloneController.Clipboard));
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            if (editorController.ResourceProvider != null)
            {
                if (!projectExplorer.Visible)
                {
                    projectExplorer.Visible = true;
                }
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            propMover.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            propMover.sceneUnloading(scene);
        }

        public long PluginId
        {
            get
            {
                return 6;
            }
        }

        public String PluginName
        {
            get
            {
                return "Editor Tools";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "Editor/BrandingImage";
            }
        }

        public bool AllowUninstall { get; set; }

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }

        public SimObjectMover SimObjectMover
        {
            get
            {
                return propMover;
            }
        }

        public TimelineController TimelineController
        {
            get
            {
                return editorTimelineController;
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }

        public ProjectExplorer ProjectExplorer
        {
            get
            {
                return projectExplorer;
            }
        }

        public EditorUICallback UICallback
        {
            get
            {
                return editorUICallback;
            }
        }

        public PropEditController PropEditController
        {
            get
            {
                return propEditController;
            }
        }

        public EditorController EditorController
        {
            get
            {
                return editorController;
            }
        }

        void guiManager_MainGUIHidden()
        {
            propEditController.hideOpenProps();
        }

        void guiManager_MainGUIShown()
        {
            propEditController.showOpenProps();
        }

        void typeControllerManager_FilesystemWatcherCreated(EditorFilesystemWatcher obj)
        {
            editorTaskbarFactory.FilesystemWatcher = obj;
        }
    }
}
