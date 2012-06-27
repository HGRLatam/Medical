﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;
using Engine.Attributes;
using Logging;
using Medical.Model;

namespace Medical.Controller.AnomalousMvc
{
    public partial class AnomalousMvcContext : Saveable, IDisposable
    {
        private ControllerCollection controllers;
        private ViewCollection views;
        private AnomalousMvcCore core;
        private ResourceProvider resourceProvider;
        private ModelCollection models;

        //State recording stuff
        private ModelMemory modelMemory = new ModelMemory();

        //Action queue stuff
        private String queuedTimeline = null;
        private bool allowShutdown = true;
        private Queue<String> queuedActions = new Queue<string>();
        private ViewHost runningActionViewHost;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
            models = new ModelCollection();
        }

        /// <summary>
        /// Called at the end of the context's lifecycle when it will no longer
        /// be used again ever. This is always called when a context is done
        /// compared to shutdown, which is only called if the context shuts down
        /// naturally.
        /// </summary>
        public void Dispose()
        {
            if (!String.IsNullOrEmpty(DisposeAction))
            {
                runFinalAction(DisposeAction);
            }
        }

        /// <summary>
        /// Called when the context starts running for the first time.
        /// </summary>
        public void startup()
        {
            foreach (MvcModel model in models)
            {
                modelMemory.add(model.Name, model);
            }
            if (!String.IsNullOrEmpty(StartupAction))
            {
                runAction(StartupAction);
            }
        }

        /// <summary>
        /// Called when the context finishes running normally. This means that
        /// it was closed down naturally and has been removed from the stack.
        /// This will not be called if another context with the same RuntimeName
        /// was added to the stack, but dispose will be called in that case.
        /// </summary>
        public void shutdown()
        {
            if (!String.IsNullOrEmpty(ShutdownAction))
            {
                runFinalAction(ShutdownAction);
            }
        }

        /// <summary>
        /// Called when coming back out of suspend mode.
        /// </summary>
        public void resume()
        {
            if (!String.IsNullOrEmpty(ResumeAction))
            {
                runAction(ResumeAction);
            }
        }

        /// <summary>
        /// Called when the context is being suspended. Suspended means that the
        /// context can still be brought back to life and has just been pushed
        /// onto the stack and closed down temporarily until the higher level
        /// contexts close. It is possible to go from suspend to dispose without
        /// going through shutdown if another context with the same RuntimeName
        /// is added to the stack.
        /// </summary>
        public void suspend()
        {
            if (!String.IsNullOrEmpty(SuspendAction))
            {
                runFinalAction(SuspendAction);
            }
        }

        /// <summary>
        /// Called when the context becomes focused or active. This will be
        /// called after startup and after resume and means that the context is
        /// open and ready to recieve input.
        /// </summary>
        public void focus()
        {
            if (!String.IsNullOrEmpty(FocusAction))
            {
                runFinalAction(FocusAction);
            }
        }

        /// <summary>
        /// Called when the context is shutting down or suspending. It will be
        /// called before those events and means the context is becoming
        /// inactive.
        /// </summary>
        public void blur()
        {
            if (!String.IsNullOrEmpty(BlurAction))
            {
                runFinalAction(BlurAction);
            }
        }

        public void stopPlayingTimeline()
        {
            core.stopPlayingTimeline();
        }

        public void playTimeline(String timelineName)
        {
            core.playTimeline(timelineName);
        }

        public void runAction(string address, ViewHost viewHost = null)
        {
            runningActionViewHost = viewHost;
            queuedTimeline = null;

            doRunAction(address);

            if (queuedTimeline != null)
            {
                playTimeline(queuedTimeline);
            }

            core.processViewChanges();

            if (queuedActions.Count > 0)
            {
                runAction(queuedActions.Dequeue(), viewHost);
            }
            else
            {
                checkShutdownConditions();
            }
        }

        public void queueRunAction(String address)
        {
            queuedActions.Enqueue(address);
        }

        public void queueTimeline(string timeline)
        {
            queuedTimeline = timeline;
        }

        public bool isViewOpen(String name)
        {
            return core.isViewOpen(name);
        }

        public void queueShowView(String view)
        {
            try
            {
                core.queueShowView(views[view], this);
            }
            catch (KeyNotFoundException)
            {
                Log.Error("Cannot show a view named {0} because it does not exist.", view);
            }
        }

        public void queueCloseView()
        {
            core.queueCloseView(runningActionViewHost);
        }

        public void queueCloseAllViews()
        {
            core.queueCloseAllViews();
        }

        public void applyLayers(EditableLayerState layers)
        {
            core.applyLayers(layers);
        }

        public void applyPresetState(PresetState presetState, float duration)
        {
            core.applyPresetState(presetState, duration);
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            core.applyCameraPosition(cameraPosition);
        }

        public void setResourceProvider(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
        }

        public void saveCamera(String name)
        {
            CameraPosition cameraPosition = core.getCurrentCameraPosition();
            if (cameraPosition != null)
            {
                modelMemory.add(name, cameraPosition);
            }
            else
            {
                Log.Warning("Problem saving camera position for '{0}'. A position could not be generated.", name);
            }
        }

        public void restoreCamera(String name)
        {
            CameraPosition cameraPos = modelMemory.get<CameraPosition>(name);
            if (cameraPos != null)
            {
                core.applyCameraPosition(cameraPos);
            }
            else
            {
                Log.Warning("A camera named '{0}' cannot be found to restore.", name);
            }
        }

        public void saveLayers(String name)
        {
            LayerState layerState = new LayerState(name);
            layerState.captureState();
            modelMemory.add(name, layerState);
        }

        public void restoreLayers(String name)
        {
            LayerState layers = modelMemory.get<LayerState>(name);
            if (layers != null)
            {
                core.applyLayers(layers);
            }
            else
            {
                Log.Warning("A layer state named '{0}' cannot be found to restore.", name);
            }
        }

        public void saveMedicalState(String name)
        {
            MedicalState medicalState = core.generateMedicalState();
            modelMemory.add(name, medicalState);
        }

        public void restoreMedicalState(String name)
        {
            MedicalState medicalState = modelMemory.get<MedicalState>(name);
            if (medicalState != null)
            {
                core.applyMedicalState(medicalState);
            }
            else
            {
                Log.Warning("A medical state named '{0}' cannot be found to restore.", name);
            }
        }

        public void createMedicalState(MedicalStateInfoModel stateInfo)
        {
            core.createMedicalState(stateInfo);
        }

        public void saveViewLayout(String name)
        {
            StoredViewCollection storedViews = core.generateSavedViewLayout();
            modelMemory.add(name, storedViews);
        }

        public void restoreViewLayout(String name)
        {
            StoredViewCollection storedViews = modelMemory.get<StoredViewCollection>(name);
            if (storedViews != null)
            {
                core.restoreSavedViewLayout(storedViews, this);
            }
            else
            {
                Log.Warning("Could not restore a StoredViewModel named {0}. It cannot be found.", name);
            }
        }

        public void addModel(String name, Object model)
        {
            modelMemory.add(name, model);
        }

        public TypeName getModel<TypeName>(String name)
            where TypeName : class
        {
            return modelMemory.get<TypeName>(name);
        }

        [EditableAction]
        public String StartupAction { get; set; }

        [EditableAction]
        public String ShutdownAction { get; set; }

        [EditableAction]
        public String SuspendAction { get; set; }

        [EditableAction]
        public String ResumeAction { get; set; }

        [EditableAction]
        public String DisposeAction { get; set; }

        [EditableAction]
        public String FocusAction { get; set; }

        [EditableAction]
        public String BlurAction { get; set; }

        /// <summary>
        /// This is the unique name of the context. If another context is
        /// running with the same name it will be removed from the context
        /// manager when this context starts.
        /// </summary>
        public String RuntimeName { get; set; }

        public ControllerCollection Controllers
        {
            get
            {
                return controllers;
            }
        }

        public ViewCollection Views
        {
            get
            {
                return views;
            }
        }

        public ModelCollection Models
        {
            get
            {
                return models;
            }
        }

        internal AnomalousMvcCore Core
        {
            get
            {
                return core;
            }
            set
            {
                this.core = value;
            }
        }

        protected AnomalousMvcContext(LoadInfo info)
        {
            StartupAction = info.GetString("StartupAction");
            ShutdownAction = info.GetString("ShutdownAction");
            ResumeAction = info.GetString("ResumeAction", null);
            SuspendAction = info.GetString("SuspendAction", null);
            DisposeAction = info.GetString("DisposeAction", null);
            FocusAction = info.GetString("FocusAction", null);
            BlurAction = info.GetString("BlurAction", null);
            controllers = info.GetValue<ControllerCollection>("Controllers");
            views = info.GetValue<ViewCollection>("Views");
            models = info.GetValue<ModelCollection>("Models", null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("StartupAction", StartupAction);
            info.AddValue("ShutdownAction", ShutdownAction);
            info.AddValue("ResumeAction", ResumeAction);
            info.AddValue("SuspendAction", SuspendAction);
            info.AddValue("DisposeAction", DisposeAction);
            info.AddValue("FocusAction", FocusAction);
            info.AddValue("BlurAction", BlurAction);
            info.AddValue("Controllers", Controllers);
            info.AddValue("Views", views);
            info.AddValue("Models", models);
        }

        public void hideMainInterface(bool showSharedGui)
        {
            core.hideMainInterface(showSharedGui);
        }

        public void showMainInterface()
        {
            core.showMainInterface();
        }

        public void queueShutdown()
        {
            queueCloseAllViews();
            stopPlayingTimeline();
        }

        public bool AllowShutdown
        {
            get
            {
                return allowShutdown;
            }
            set
            {
                allowShutdown = value;
            }
        }

        public ResourceProvider ResourceProvider
        {
            get
            {
                return resourceProvider;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return core.MeasurementGrid;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return core.AnatomyController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return core.ImageRenderer;
            }
        }

        /// <summary>
        /// This is a special method to run the last action for a context. It
        /// does not allow views or timelines to be queued, but it can do a few
        /// things on shutdown.
        /// </summary>
        /// <param name="address"></param>
        internal void runFinalAction(String address)
        {
            doRunAction(address);
        }

        private void checkShutdownConditions()
        {
            if (allowShutdown)
            {
                //Check for shutdown conditions
                //No views are open
                if (!core.HasOpenViews)
                {
                    if (core.PlayingTimeline)
                    {
                        //Reevaluate again when the timeline stops playing
                        core.TimelineStopped += core_TimelineStopped;
                    }
                    else
                    {
                        //Not timelines playing and no views showing, shutdown
                        core.shutdownContext(this, true, true); //true, true
                    }
                }
            }
        }

        private void doRunAction(string address)
        {
            if (address != null)
            {
                int slashLoc = address.LastIndexOf('/');
                if (slashLoc != -1)
                {
                    try
                    {
                        String controllerName = address.Substring(0, slashLoc);
                        ++slashLoc;
                        String actionName = address.Substring(slashLoc, address.Length - slashLoc);
                        MvcController controller = controllers[controllerName];
                        controller.runAction(actionName, this);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Cannot run action {0}. Reason {1}", address, ex.Message);
                    }
                }
                else
                {
                    Log.Error("Malformed action address '{0}' the format must be 'Controller/Action' cannot run action", address);
                }
            }
            else
            {
                Log.Error("Address was null, cannot run action.", address);
            }
        }

        void core_TimelineStopped()
        {
            core.TimelineStopped -= core_TimelineStopped;
            checkShutdownConditions();
        }
    }

    public partial class AnomalousMvcContext
    {
        private EditInterface editInterface;

        public enum CustomQueries
        {
            Preview
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "MVC", null);// new EditInterface("MVC");
                editInterface.addSubInterface(views.getEditInterface("Views"));
                editInterface.addSubInterface(controllers.getEditInterface("Controllers"));
                editInterface.addSubInterface(models.getEditInterface("Models"));
                editInterface.addCommand(new EditInterfaceCommand("Preview", preview));
            }
            return editInterface;
        }

        private void preview(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runOneWayCustomQuery(CustomQueries.Preview, this);
        }
    }
}
