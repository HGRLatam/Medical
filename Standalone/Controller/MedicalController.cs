﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using OgrePlugin;
using Engine.Platform;
using Engine.Renderer;
using System.Threading;
using System.Xml;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using System.IO;
using BulletPlugin;
using MyGUIPlugin;
using Medical.GUI;
using SoundPlugin;
using Medical.Controller;
using System.Globalization;
using libRocketPlugin;
using BEPUikPlugin;
using Anomalous.libRocketWidget;
using Anomalous.OSPlatform;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;
using Anomalous.GuiFramework.Editor;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Anomalous.GuiFramework.Debugging;

namespace Medical
{
    public delegate void LoopUpdate(Clock time);

    public sealed class MedicalController : IDisposable
    {
        //Engine
        private PluginManager pluginManager;
        private LogFileListener logListener;

        //Platform
        private NativeSystemTimer systemTimer;
        private NativeUpdateTimer mainTimer;
        private EventManager eventManager;
        private NativeInputHandler inputHandler;
        private EventUpdateListener eventUpdate;
        private MedicalUpdate medicalUpdate;
        private IServiceCollection services = new ServiceCollection();

        //Performance
        private NativeSystemTimer performanceMetricTimer;

        //Controller
        private MedicalSceneController medicalScene;
        private FrameClearManager frameClearManager;

        //Serialization
        private XmlSaver xmlSaver = new XmlSaver();

        //Scene
        private String currentSceneFile;
        private String currentSceneDirectory;

        public event LoopUpdate OnLoopUpdate;

        public MedicalController(NativeOSWindow mainWindow)
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.LogFile);
            Log.Default.addLogListener(logListener);
            Log.ImportantInfo("Running from directory {0}", FolderFinder.ExecutableFolder);

            //Create pluginmanager
            pluginManager = new PluginManager(MedicalConfig.ConfigFile, services);

            //Configure the filesystem
            VirtualFileSystem archive = VirtualFileSystem.Instance;

            //Setup microcode cache load
            OgreInterface.MicrocodeCachePath = Path.Combine(FolderFinder.LocalPrivateDataFolder, "ShaderCache.mcc");
            OgreInterface.AllowMicrocodeCacheLoad = MedicalConfig.LastShaderVersion == UnifiedMaterialBuilder.Version;
            OgreInterface.TrackMemoryLeaks = MedicalConfig.TrackMemoryLeaks;
            MedicalConfig.LastShaderVersion = UnifiedMaterialBuilder.Version;

            MyGUIInterface.EventLayerKey = EventLayers.Gui;
            MyGUIInterface.CreateGuiGestures = MedicalConfig.EnableMultitouch && PlatformConfig.TouchType == TouchType.Screen;
            MyGUIInterface.TrackMemoryLeaks = MedicalConfig.TrackMemoryLeaks;

            RuntimePlatformInfo.addPath(MedicalConfig.OpenGLESEmulatorPath);

            //Configure plugins
            pluginManager.OnConfigureDefaultWindow = delegate (out WindowInfo defaultWindow)
            {
                //Setup main window
                defaultWindow = new WindowInfo(mainWindow, "Primary");
                defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
                defaultWindow.MonitorIndex = 0;

                if (MedicalConfig.EngineConfig.Fullscreen)
                {
                    mainWindow.setSize(MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
                    mainWindow.ExclusiveFullscreen = true;
                    defaultWindow.Width = MedicalConfig.EngineConfig.HorizontalRes;
                    defaultWindow.Height = MedicalConfig.EngineConfig.VerticalRes;
                }
                else
                {
                    mainWindow.Maximized = true;
                }
                mainWindow.show();
            };

            GuiFrameworkCamerasInterface.CameraTransitionTime = MedicalConfig.CameraTransitionTime;
            GuiFrameworkCamerasInterface.DefaultCameraButton = MedicalConfig.CameraMouseButton;
            GuiFrameworkCamerasInterface.MoveCameraEventLayer = EventLayers.Cameras;
            GuiFrameworkCamerasInterface.SelectWindowEventLayer = EventLayers.AfterGui;
            GuiFrameworkCamerasInterface.ShortcutEventLayer = EventLayers.Gui;
            GuiFrameworkCamerasInterface.TouchType = PlatformConfig.TouchType;
            GuiFrameworkCamerasInterface.PanKey = PlatformConfig.PanKey;

            GuiFrameworkEditorInterface.ToolsEventLayers = EventLayers.Tools;

            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BulletInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(NativePlatformPlugin).Assembly);
            pluginManager.addPluginAssembly(typeof(MyGUIInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(RocketInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SoundPluginInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BEPUikInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SimulationPlugin).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(RocketWidgetInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkCamerasInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkEditorInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkDebuggingInterface).Assembly);
            pluginManager.initializePlugins();

            performanceMetricTimer = new NativeSystemTimer();
            PerformanceMonitor.setupEnabledState(performanceMetricTimer);

            //Intialize the platform
            BulletInterface.Instance.ShapeMargin = 0.005f;
            systemTimer = new NativeSystemTimer();

            mainTimer = new NativeUpdateTimer(systemTimer);

            if (OgreConfig.VSync && MedicalConfig.EngineConfig.FPSCap < 300)
            {
                //Use a really high framerate cap if vsync is on since it will cap our 
                //framerate for us. If the user has requested a higher rate use it anyway.
                mainTimer.FramerateCap = 300;
            }
            else
            {
                mainTimer.FramerateCap = MedicalConfig.EngineConfig.FPSCap;
            }

            inputHandler = new NativeInputHandler(mainWindow, MedicalConfig.EnableMultitouch);
            eventManager = new EventManager(inputHandler, Enum.GetValues(typeof(EventLayers)));
            eventUpdate = new EventUpdateListener(eventManager);
            mainTimer.addUpdateListener(eventUpdate);
            pluginManager.setPlatformInfo(mainTimer, eventManager);
            medicalUpdate = new MedicalUpdate(this);
            mainTimer.addUpdateListener(medicalUpdate);

            //Initialize controllers
            medicalScene = new MedicalSceneController(pluginManager);
            frameClearManager = new FrameClearManager(OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget);

            SoundConfig.initialize(MedicalConfig.ConfigFile);

            GuiFrameworkInterface.Instance.handleCursors(mainWindow);
            SoundPluginInterface.Instance.setResourceWindow(mainWindow);

            TouchMouseGuiForwarder = new TouchMouseGuiForwarder(eventManager, inputHandler, systemTimer, mainWindow, EventLayers.Last);
            TouchMouseGuiForwarder.ForwardTouchesAsMouse = PlatformConfig.ForwardTouchAsMouse;
            var myGuiKeyboard = new MyGUIOnscreenKeyboardManager(TouchMouseGuiForwarder);
            var rocketKeyboard = new RocketWidgetOnscreenKeyboardManager(TouchMouseGuiForwarder);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (frameClearManager != null)
            {
                frameClearManager.Dispose();
            }
            if (medicalScene != null)
            {
                unloadSceneAndResources();
            }
            if (eventManager != null)
            {
                eventManager.Dispose();
            }
            if (inputHandler != null)
            {
                inputHandler.Dispose();
            }
            if (systemTimer != null)
            {
                systemTimer.Dispose();
            }
            if (performanceMetricTimer != null)
            {
                PerformanceMonitor.destroyEnabledState();
                performanceMetricTimer.Dispose();
            }
            if (pluginManager != null)
            {
                //This is the main engine plugin manager, it should be last unless subsystems need to be shutdown before any additional disposing
                pluginManager.Dispose();
            }

            Log.Info("Medical Controller Shutdown");
            logListener.Dispose();
        }

        public void saveCrashLog()
        {
            if (logListener != null)
            {
                DateTime now = DateTime.Now;
                String crashFile = String.Format(CultureInfo.InvariantCulture, "{0}/log {1}-{2}-{3} {4}.{5}.{6}.log", MedicalConfig.CrashLogDirectory, now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second);
                logListener.saveCrashLog(crashFile);
            }
        }

        /// <summary>
        /// Stop the loop and begin the process of shutting down the program.
        /// </summary>
        public void shutdown()
        {
            mainTimer.stopLoop();
        }

        /// <summary>
        /// Attempt to open the given scene file. Will return true if the scene was loaded correctly.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>True if the scene was loaded, false on an error.</returns>
        public IEnumerable<SceneBuildStatus> openScene(String filename)
        {
            foreach (var status in openScene(filename, new VirtualFilesystemResourceProvider()))
            {
                yield return status;
            }
        }

        public IEnumerable<SceneBuildStatus> openScene(String filename, ResourceProvider resourceProvider)
        {
            medicalScene.destroyScene();
            if (resourceProvider.exists(filename))
            {
                currentSceneFile = VirtualFileSystem.GetFileName(filename);
                currentSceneDirectory = VirtualFileSystem.GetDirectoryName(filename);
                using (Stream file = resourceProvider.openFile(filename))
                {
                    XmlTextReader textReader = null;
                    ScenePackage scenePackage = null;
                    try
                    {
                        yield return new SceneBuildStatus()
                        {
                            Message = "Loading Scene File"
                        };
                        textReader = new XmlTextReader(file);
                        scenePackage = xmlSaver.restoreObject(textReader) as ScenePackage;
                    }
                    finally
                    {
                        if (textReader != null)
                        {
                            textReader.Close();
                        }
                    }
                    if (scenePackage != null)
                    {
                        foreach (var status in medicalScene.loadScene(scenePackage, SceneBuildOptions.SingleUseDefinitions))
                        {
                            yield return status;
                        }
                    }
                }
            }
            else
            {
                Log.Error("Could not load scene {0}.", filename);
            }
        }

        public void unloadSceneAndResources()
        {
            medicalScene.destroyScene();
            medicalScene.clearResources();
        }

        public void addSimObject(SimObjectBase simObject)
        {
            medicalScene.addSimObject(simObject);
        }

        internal void _sendUpdate(Clock clock)
        {
            if (OnLoopUpdate != null)
            {
                OnLoopUpdate.Invoke(clock);
            }
        }

        public EventManager EventManager
        {
            get
            {
                return eventManager;
            }
        }

        public NativeInputHandler InputHandler
        {
            get
            {
                return inputHandler;
            }
        }

        public NativeUpdateTimer MainTimer
        {
            get
            {
                return mainTimer;
            }
        }

        public SimScene CurrentScene
        {
            get
            {
                return medicalScene.CurrentScene;
            }
        }

        public IEnumerable<SimObjectBase> SimObjects
        {
            get
            {
                return medicalScene.SimObjects;
            }
        }

        public SimObject getSimObject(String name)
        {
            return medicalScene.getSimObject(name);
        }

        public String CurrentSceneFile
        {
            get
            {
                return currentSceneFile;
            }
        }

        public String CurrentSceneDirectory
        {
            get
            {
                return currentSceneDirectory;
            }
        }

        public PluginManager PluginManager
        {
            get
            {
                return pluginManager;
            }
        }

        public TouchMouseGuiForwarder TouchMouseGuiForwarder { get; private set; }
    }
}
