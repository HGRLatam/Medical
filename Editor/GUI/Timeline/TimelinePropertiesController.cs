﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;
using MyGUIPlugin;
using Logging;
using Engine.Editing;
using Engine.Platform;
using Engine;

namespace Medical.GUI
{
    class TimelinePropertiesController : IDisposable
    {
        enum InputEvents
        {
            Save,
        }

        private static MessageEvent save;

        static TimelinePropertiesController()
        {
            save = new MessageEvent(InputEvents.Save);
            save.addButton(KeyboardButtonCode.KC_LCONTROL);
            save.addButton(KeyboardButtonCode.KC_S);
            DefaultEvents.registerDefaultEvent(save);
        }

        private TimelineProperties timelineProperties;
        private TimelineObjectProperties timelineObjectProperties;
        private TimelineFileExplorer timelineFileExplorer;
        private TimelineObjectExplorer timelineObjectExplorer;
        private MedicalUICallback medicalUICallback;
        private TimelineUICallbackExtensions uiCallbackExtensions;
        private ObjectEditor timelineObjectEditor;
        private BrowserWindow browserWindow;
        private QuestionEditor questionEditor;
        private TimelineFileBrowserDialog fileBrowserDialog;
        public event SingleArgumentEvent<TimelinePropertiesController, Timeline> CurrentTimelineChanged;

        private TimelineController editorTimelineController;
        private TimelineController mainTimelineController;
        private TimelineDocumentHandler documentHandler;
        private DocumentController documentController;

        private Timeline currentTimeline;

        private bool visible = false;
        private bool togglingVisible = false;

        public event EventDelegate<TimelinePropertiesController> VisibilityChanged;

        public TimelinePropertiesController(StandaloneController standaloneController, EditorPlugin editorPlugin)
        {
            mainTimelineController = standaloneController.TimelineController;
            mainTimelineController.TimelinePlaybackStopped += new EventHandler(mainTimelineController_TimelinePlaybackStopped);

            GUIManager guiManager = standaloneController.GUIManager;
            editorTimelineController = editorPlugin.TimelineController;
            editorTimelineController.ResourceLocationChanged += new EventHandler(editorTimelineController_ResourceLocationChanged);
            editorTimelineController.TimelinePlaybackStopped += new EventHandler(editorTimelineController_TimelinePlaybackStopped);

            this.documentController = standaloneController.DocumentController;
            documentHandler = new TimelineDocumentHandler(this);
            documentController.addDocumentHandler(documentHandler);

            fileBrowserDialog = new TimelineFileBrowserDialog(editorTimelineController);
            editorTimelineController.FileBrowser = fileBrowserDialog;
            guiManager.addManagedDialog(fileBrowserDialog);

            timelineProperties = new TimelineProperties(editorTimelineController, editorPlugin, guiManager, this, fileBrowserDialog, standaloneController.Clipboard);
            guiManager.addManagedDialog(timelineProperties);

            timelineFileExplorer = new TimelineFileExplorer(editorTimelineController, standaloneController.DocumentController, this);
            timelineFileExplorer.Closed += new EventHandler(timelineFileExplorer_Closed);
            guiManager.addManagedDialog(timelineFileExplorer);

            browserWindow = new BrowserWindow();
            guiManager.addManagedDialog(browserWindow);

            questionEditor = new QuestionEditor(fileBrowserDialog, editorTimelineController);
            guiManager.addManagedDialog(questionEditor);

            medicalUICallback = new MedicalUICallback(browserWindow);
            uiCallbackExtensions = new TimelineUICallbackExtensions(standaloneController, medicalUICallback, editorTimelineController, browserWindow, questionEditor);

            timelineObjectExplorer = new TimelineObjectExplorer(medicalUICallback);
            timelineObjectExplorer.Enabled = false;
            guiManager.addManagedDialog(timelineObjectExplorer);

            timelineObjectProperties = new TimelineObjectProperties(medicalUICallback);
            guiManager.addManagedDialog(timelineObjectProperties);

            timelineObjectEditor = new ObjectEditor(timelineObjectExplorer.EditInterfaceTree, timelineObjectProperties.PropertiesTable, medicalUICallback);

            createNewTimeline();

            save.FirstFrameUpEvent += new MessageEventCallback(save_FirstFrameUpEvent);
        }

        public void Dispose()
        {
            documentController.removeDocumentHandler(documentHandler);
            timelineObjectProperties.Dispose();
            timelineFileExplorer.Dispose();
            timelineProperties.Dispose();
            timelineObjectExplorer.Dispose();
            questionEditor.Dispose();
            browserWindow.Dispose();
            fileBrowserDialog.Dispose();
        }

        public event EventHandler Closed
        {
            add
            {
                timelineFileExplorer.Closed += value;
            }
            remove
            {
                timelineFileExplorer.Closed -= value;
            }
        }

        /// <summary>
        /// Create a new project. You can optionally delete the old project.
        /// </summary>
        /// <param name="filename">The file name of the new project.</param>
        /// <param name="deleteOld">True to delete any existing project first.</param>
        public void createNewProject(String filename, bool deleteOld, bool asFolder)
        {
            try
            {
                if (deleteOld)
                {
                    File.Delete(filename);
                }
                createProject(filename, asFolder);
                updateWindowCaption();
                if (asFolder)
                {
                    documentController.addToRecentDocuments(Path.Combine(filename, TimelineController.INDEX_FILE_NAME));
                }
                else
                {
                    documentController.addToRecentDocuments(filename);
                }
            }
            catch (Exception ex)
            {
                String errorMessage = String.Format("Error creating new project {0}.", ex.Message);
                Log.Error(errorMessage);
                MessageBox.show(errorMessage, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        public void openProject(String projectPath)
        {
            if (projectPath.EndsWith(".tix"))
            {
                editorTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(Path.GetDirectoryName(projectPath));
            }
            else if (projectPath.EndsWith(".tl"))
            {
                editorTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(Path.GetDirectoryName(projectPath));
                openTimelineFile(projectPath);
            }
            else
            {
                editorTimelineController.ResourceProvider = new TimelineZipResources(projectPath);
            }
            updateWindowCaption();
        }

        public void openTimelineFile(String filename)
        {
            try
            {
                CurrentTimeline = editorTimelineController.openTimeline(filename);
            }
            catch (Exception ex)
            {
                Log.Default.printException(ex);
                MessageBox.show(String.Format("Error loading timeline {0}.\n{1}", filename, ex.Message), "Load Timeline Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            editorTimelineController.saveTimeline(timeline, filename);
            updateWindowCaption();
        }

        public void createNewTimeline()
        {
            CurrentTimeline = new Timeline();
        }

        public void togglePlayPreview(float startTime)
        {
            if (editorTimelineController.Playing)
            {
                editorTimelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                timelineProperties.stopEditing();
                editorTimelineController.startPlayback(currentTimeline, startTime, false);
            }
        }

        public void togglePlayFull()
        {
            if (currentTimeline != null)
            {
                timelineProperties.stopEditing();
                mainTimelineController.ResourceProvider = editorTimelineController.ResourceProvider.clone();
                mainTimelineController.startPlayback(currentTimeline, true);
            }
        }

        internal void cut()
        {
            timelineProperties.cut();
        }

        public void copy()
        {
            timelineProperties.copy();
        }

        public void paste()
        {
            timelineProperties.paste();
        }

        internal void selectAll()
        {
            timelineProperties.selectAll();
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                if (currentTimeline != value)
                {
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
                    }
                    currentTimeline = value;
                    editorTimelineController.setAsTimelineController(currentTimeline);
                    if (CurrentTimelineChanged != null)
                    {
                        CurrentTimelineChanged.Invoke(this, currentTimeline);
                    }
                    timelineProperties.setCurrentTimeline(currentTimeline);
                    timelineObjectEditor.EditInterface = currentTimeline.getEditInterface();
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded += currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
                    }
                }
            }
        }

        public String CurrentTimelineFile
        {
            get
            {
                return currentTimeline != null ? currentTimeline.SourceFile : null;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    togglingVisible = true;
                    visible = value;
                    if (timelineProperties.Visible != value)
                    {
                        timelineProperties.Visible = value;
                    }
                    if (timelineFileExplorer.Visible != value)
                    {
                        timelineFileExplorer.Visible = value;
                    }
                    if (timelineObjectExplorer.Visible != value)
                    {
                        timelineObjectExplorer.Visible = value;
                    }
                    if (timelineObjectProperties.Visible != value)
                    {
                        timelineObjectProperties.Visible = value;
                    }
                    if (VisibilityChanged != null)
                    {
                        VisibilityChanged.Invoke(this);
                    }
                    togglingVisible = false;
                }
            }
        }

        private void createProject(string projectName, bool asFolder)
        {
            if (asFolder)
            {
                if (!Directory.Exists(projectName))
                {
                    Directory.CreateDirectory(projectName);
                }
                using (XmlTextWriter fileStream = new XmlTextWriter(new FileStream(Path.Combine(projectName, TimelineController.INDEX_FILE_NAME), FileMode.Create), Encoding.Default))
                {
                    TimelineIndex index = new TimelineIndex();
                    XmlSaver xmlSaver = new XmlSaver();
                    xmlSaver.saveObject(index, fileStream);
                }
                editorTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(projectName);
            }
            else
            {
                using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(projectName))
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.Default);
                        xmlWriter.Formatting = Formatting.Indented;
                        TimelineIndex index = new TimelineIndex();
                        XmlSaver xmlSaver = new XmlSaver();
                        xmlSaver.saveObject(index, xmlWriter);
                        xmlWriter.Flush();
                        memStream.Seek(0, SeekOrigin.Begin);
                        ionicZip.AddEntry(TimelineController.INDEX_FILE_NAME, memStream);
                        ionicZip.Save();
                    }
                }
                editorTimelineController.ResourceProvider = new TimelineZipResources(projectName);
            }
        }

        private void updateWindowCaption()
        {
            timelineFileExplorer.updateWindowCaption();
            timelineProperties.updateWindowCaption();
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            timelineProperties.removeActionFromTimeline(e.Action);
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            timelineProperties.addActionToTimeline(e.Action);
        }

        void editorTimelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            timelineObjectExplorer.Enabled = timelineObjectProperties.Enabled = editorTimelineController.ResourceProvider != null;
        }

        void save_FirstFrameUpEvent(EventManager eventManager)
        {
            if (timelineFileExplorer.Visible)
            {
                timelineFileExplorer.save();
            }
        }

        void resetCurrentTimelineController()
        {
            editorTimelineController.setAsTimelineController(currentTimeline);
        }

        void mainTimelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            resetCurrentTimelineController();
        }

        void editorTimelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            resetCurrentTimelineController();
        }

        void timelineFileExplorer_Closed(object sender, EventArgs e)
        {
            if (!togglingVisible)
            {
                visible = false;
                if (timelineProperties.Visible)
                {
                    timelineProperties.Visible = false;
                }
                if (timelineObjectExplorer.Visible)
                {
                    timelineObjectExplorer.Visible = false;
                }
                if (timelineObjectProperties.Visible)
                {
                    timelineObjectProperties.Visible = false;
                }
                if (VisibilityChanged != null)
                {
                    VisibilityChanged.Invoke(this);
                }
            }
        }
    }
}
