﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using System.Windows.Forms;
using Engine.Platform;
using Engine.Renderer;
using WeifenLuo.WinFormsUI.Docking;
using Medical.GUI;
using Logging;
using Medical.Controller;

namespace Medical
{
    public delegate void DrawingWindowEvent(DrawingWindow window);
    public delegate void DrawingWindowRenderEvent(DrawingWindow window, bool currentCameraRender);

    public class DrawingWindowController : IDisposable
    {
        public event DrawingWindowEvent WindowCreated;
        public event DrawingWindowEvent WindowDestroyed;
        public event DrawingWindowEvent ActiveWindowChanged;

        private Dictionary<String, DrawingWindowHost> cameras = new Dictionary<String, DrawingWindowHost>();
        private bool camerasActive = false;
        private bool showStatsActive = false;
        private UpdateTimer mainTimer;
        private SimScene scene;
        private EventManager eventManager;
        private RendererPlugin rendererPlugin;
        private DrawingWindowHost activeDrawingWindow = null;
        private bool allowRotation = true;
        private bool allowZoom = true;
        private DockProvider dockProvider;

        public DrawingWindowController(DockProvider dockProvider)
        {
            this.dockProvider = dockProvider;
            dockProvider.ActiveDocumentChanged += new DockProviderEvent(dockProvider_ActiveDocumentChanged);
        }

        public void Dispose()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.Dispose();
                host.Dispose();
            }
        }

        public void initialize(EventManager eventManager, RendererPlugin rendererPlugin, ConfigFile configFile)
        {
            this.eventManager = eventManager;
            this.rendererPlugin = rendererPlugin;
        }

        public DrawingWindowHost restoreFromString(String persistString)
        {
            String name;
            Vector3 translation, lookAt;
            int bgColor;
            if (dockProvider.restoreFromString(persistString, out name, out translation, out lookAt, out bgColor))
            {
                DrawingWindowHost host = this.createDrawingWindowHost(name, translation, lookAt);
                host.DrawingWindow.BackColor = System.Drawing.Color.FromArgb(bgColor);
                return host;
            }
            else
            {
                return null;
            }
        }

        public void recreateAllWindows()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.recreateWindow();
            }
        }

        public void createFromPresets(DrawingWindowPresetSet presets)
        {
            StatusController.SetStatus("Recreating viewports...");
            closeAllWindows();
            DrawingWindowHost camera;
            foreach(DrawingWindowPreset preset in presets.getPresetEnum())
            {
                camera = addCamera(preset.Name, preset.Position, preset.LookAt);
            }
            DrawingWindowHost parent;
            foreach (DrawingWindowPreset preset in presets.getPresetEnum())
            {
                camera = cameras[preset.Name];
                parent = null;
                if (preset.ParentWindow != null)
                {
                    cameras.TryGetValue(preset.ParentWindow, out parent);
                    if (parent == null)
                    {
                        Log.Warning("Could not find parent window {0} for window {1}.", preset.ParentWindow, preset.Name);
                    }
                }
                if (parent == null)
                {
                    camera.ShowWindow();
                }
                else
                {
                    camera.ShowWindow(parent, preset.WindowPosition);
                }
            }
            StatusController.TaskCompleted();
        }

        public DrawingWindowHost createDrawingWindowHost(String name, Vector3 translation, Vector3 lookAt)
        {
            return addCamera(name, translation, lookAt);
        }

        public void destroyCameras()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.destroyCamera();
            }
            camerasActive = false;
            mainTimer = null;
            scene = null;
        }

        public void createCameras(UpdateTimer mainTimer, SimScene scene, String sceneDirectory)
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.createCamera(mainTimer, scene);
            }
            camerasActive = true;
            this.mainTimer = mainTimer;
            this.scene = scene;
        }

        public void showStats(bool show)
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.showStats(show);
            }
            showStatsActive = show;
        }

        public DrawingWindowHost getActiveWindow()
        {
            return activeDrawingWindow;
        }

        public void cloneActiveWindow()
        {
            DrawingWindowHost host = getActiveWindow();
            String cloneName = host.Text + " Clone";
            if (cameras.ContainsKey(cloneName))
            {
                MessageBox.Show(host.DrawingWindow, String.Format("Cannot create a clone. The window {0} already has a clone.", host.Text), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                DrawingWindowHost cloneHost = addCloneCamera(cloneName, host.DrawingWindow);
                //cloneHost.Show(host.Pane, DockAlignment.Right, 0.5);
                cloneHost.ShowWindow();
                cloneHost.Size = host.Size;
            }
        }

        public void closeAllWindows()
        {
            List<DrawingWindowHost> listCopy = new List<DrawingWindowHost>(cameras.Values);
            foreach (DrawingWindowHost host in listCopy)
            {
                host.Close();
            }
        }

        public bool AllowRotation
        {
            get
            {
                return allowRotation;
            }
            set
            {
                allowRotation = value;
            }
        }

        public bool AllowZoom
        {
            get
            {
                return allowZoom;
            }
            set
            {
                allowZoom = value;
            }
        }

        internal void _alertCameraDestroyed(DrawingWindowHost host)
        {
            if (WindowDestroyed != null)
            {
                WindowDestroyed.Invoke(host.DrawingWindow);
            }
            cameras.Remove(host.DrawingWindow.CameraName);
        }

        void dockProvider_ActiveDocumentChanged(DockProvider source)
        {
            DrawingWindowHost changed = source.ActiveDocument;
            if (changed != null)
            {
                activeDrawingWindow = changed;
                if (ActiveWindowChanged != null)
                {
                    ActiveWindowChanged.Invoke(activeDrawingWindow.DrawingWindow);
                }
            }
        }

        private DrawingWindowHost addCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            DrawingWindowHost cameraHost = dockProvider.createWindow(name, this);
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, cameraHost.DrawingWindow, eventManager);
            orbitCamera.AllowRotation = allowRotation;
            orbitCamera.AllowZoom = allowZoom;
            cameraHost.DrawingWindow.initialize(name, orbitCamera, rendererPlugin);
            initializeCamera(cameraHost);
            return cameraHost;
        }

        private DrawingWindowHost addCloneCamera(String name, DrawingWindow cloneWindow)
        {
            DrawingWindowHost cameraHost = dockProvider.createCloneWindow(name, this);
            CloneCamera cloneCamera = new CloneCamera(cloneWindow);
            cameraHost.DrawingWindow.initialize(name, cloneCamera, rendererPlugin);
            initializeCamera(cameraHost);
            return cameraHost;
        }

        private void initializeCamera(DrawingWindowHost cameraHost)
        {
            cameras.Add(cameraHost.DrawingWindow.CameraName, cameraHost);
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(cameraHost.DrawingWindow);
            }

            if (camerasActive)
            {
                cameraHost.DrawingWindow.createCamera(mainTimer, scene);
            }
            cameraHost.DrawingWindow.showStats(showStatsActive);
        }
    }
}