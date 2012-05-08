﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.Platform;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// The core is the link from AnomalousMVC to the rest of the system.
    /// </summary>
    public class AnomalousMvcCore
    {
        private TimelineController timelineController;
        private ViewHostFactory viewHostFactory;

        private ViewHostManager viewHostManager;
        private ViewHost viewHost;
        private GUIManager guiManager;
        private StandaloneController standaloneController;

        private XmlSaver xmlSaver = new XmlSaver();

        public event Action TimelineStopped;

        public AnomalousMvcCore(StandaloneController standaloneController, ViewHostFactory viewHostFactory)//, UpdateTimer updateTimer, GUIManager guiManager, TimelineController timelineController)
        {
            this.standaloneController = standaloneController;
            this.timelineController = standaloneController.TimelineController;
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            this.guiManager = standaloneController.GUIManager;
            this.viewHostFactory = viewHostFactory;

            viewHostManager = new ViewHostManager(standaloneController.MedicalController.MainTimer, guiManager);
        }

        public void showView(View view, AnomalousMvcContext context)
        {
            viewHost = viewHostFactory.createViewHost(view, context);
            viewHostManager.requestOpen(viewHost);
        }

        public void closeView()
        {
            if (viewHost != null)
            {
                viewHostManager.requestClose(viewHost);
                viewHost = null;
            }
        }

        public bool HasOpenViews
        {
            get
            {
                return viewHost != null;
            }
        }

        public bool PlayingTimeline
        {
            get
            {
                return timelineController.Playing;
            }
        }

        public void stopPlayingTimeline()
        {
            if (timelineController.Playing)
            {
                timelineController.stopAndStartPlayback(null, false);
            }
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            Timeline timeline = timelineController.openTimeline(timelineName);
            if (timeline != null)
            {
                timeline.AutoFireMultiTimelineStopped = allowPlaybackStop;
                if (timelineController.Playing)
                {
                    timelineController.stopAndStartPlayback(timeline, true);
                }
                else
                {
                    timelineController.startPlayback(timeline);
                }
            }
            else
            {
                Log.Warning("AnomalousMvcCore playback: Error loading timeline '{0}'", timelineName);
            }
        }

        public void applyLayers(LayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }

        public void applyPresetState(PresetState presetState, float duration)
        {
            TemporaryStateBlender stateBlender = standaloneController.TemporaryStateBlender;
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            presetState.applyToState(createdState);
            stateBlender.startTemporaryBlend(createdState);
        }

        public void applyMedicalState(MedicalState medicalState)
        {
            standaloneController.TemporaryStateBlender.startTemporaryBlend(medicalState);
        }

        public MedicalState generateMedicalState()
        {
            return standaloneController.TemporaryStateBlender.createBaselineState();
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition);
            }
        }

        public CameraPosition getCurrentCameraPosition()
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                CameraPosition position = new CameraPosition();
                position.Translation = window.Translation;
                position.LookAt = window.LookAt;
                return position;
            }
            return null;
        }

        public AnomalousMvcContext loadContext(Stream stream)
        {
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                AnomalousMvcContext context = (AnomalousMvcContext)xmlSaver.restoreObject(xmlReader);
                context._setCore(this);
                return context;
            }
        }

        public void startRunningContext(AnomalousMvcContext context)
        {
            timelineController.MvcContext = context;
            context._setCore(this);
            context.runAction(context.StartupAction);
        }

        public void hideMainInterface(bool showSharedInterface)
        {
            guiManager.setMainInterfaceEnabled(false, showSharedInterface);
        }

        public void showMainInterface()
        {
            guiManager.setMainInterfaceEnabled(true, false);
        }

        public void shutdownContext(AnomalousMvcContext context)
        {
            timelineController.stopPlayback(false);
            context.runFinalAction(context.ShutdownAction);
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            if (TimelineStopped != null && !timelineController.HasQueuedTimeline)
            {
                TimelineStopped.Invoke();
            }
        }
    }
}
