﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine.Resources;
using System.Xml;
using Logging;
using Engine.Saving.XMLSaver;
using Engine.Platform;

namespace Medical.Controller
{
    /// <summary>
    /// An event for the MuscleSequenceController.
    /// </summary>
    /// <param name="controller">The controller that fired the event.</param>
    public delegate void MovementSequenceEvent(MovementSequenceController controller);

    /// <summary>
    /// This class manages loading and playback of movement sequences.
    /// </summary>
    public class MovementSequenceController : IDisposable
    {
        public event MovementSequenceEvent CurrentSequenceChanged;
        public event MovementSequenceEvent CurrentSequenceSetChanged;

        private XmlSaver xmlSaver = new XmlSaver();
        private MovementSequence currentSequence;
        private String currentDirectory;
        private MedicalController medicalController;
        private float currentTime = 0.0f;
        private bool playing = false;
        private MovementSequenceSet currentSequenceSet;

        public MovementSequenceController(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        public void Dispose()
        {
            if (currentSequenceSet != null)
            {
                currentSequenceSet.Dispose();
            }
        }

        /// <summary>
        /// Load the specified sequence and return it.
        /// </summary>
        /// <param name="filename">The filename of the sequence to load.</param>
        /// <returns>The loaded sequence.</returns>
        public MovementSequence loadSequence(String filename)
        {
            MovementSequence loadingSequence = null;
            try
            {
                using (Archive archive = FileSystem.OpenArchive(filename))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(filename, FileMode.Open, FileAccess.Read)))
                    {
                        loadingSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read muscle sequence {0}.\nReason: {1}.", filename, e.Message);
            }
            return loadingSequence;
        }

        /// <summary>
        /// Load the sequences in the specified directory. Will not make any
        /// changes if the directory is the currently loaded directory.
        /// </summary>
        /// <param name="sequenceDir">The directory to load.</param>
        public void loadSequenceSet(String sequenceDir)
        {
            if (sequenceDir != currentDirectory)
            {
                if (currentSequenceSet != null)
                {
                    currentSequenceSet.Dispose();
                }
                currentSequenceSet = new MovementSequenceSet();
                currentDirectory = sequenceDir;
                using (Archive archive = FileSystem.OpenArchive(sequenceDir))
                {
                    foreach (String directory in archive.listDirectories(sequenceDir, false, false))
                    {
                        MovementSequenceGroup group = new MovementSequenceGroup(archive.getFileInfo(directory).Name);
                        currentSequenceSet.addGroup(group);
                        foreach (String file in archive.listFiles(directory, false))
                        {
                            String fileName = archive.getFileInfo(file).Name;
                            MovementSequenceInfo info = new MovementSequenceInfo();
                            info.Name = fileName.Substring(0, fileName.Length - 4);
                            info.FileName = archive.getFullPath(file);
                            group.addSequence(info);
                        }
                    }
                }
                if (CurrentSequenceSetChanged != null)
                {
                    CurrentSequenceSetChanged.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Play the current sequence.
        /// </summary>
        public void playCurrentSequence()
        {
            if (currentSequence != null)
            {
                currentTime = 0.0f;
                subscibeToUpdates();
            }
        }

        /// <summary>
        /// Stop playing the current sequence.
        /// </summary>
        public void stopPlayback()
        {
            unsubscribeFromUpdates();
        }

        /// <summary>
        /// The sequence that is currently loaded for playback or manipulation.
        /// </summary>
        public MovementSequence CurrentSequence
        {
            get
            {
                return currentSequence;
            }
            set
            {
                if (currentSequence != value)
                {
                    currentSequence = value;
                    if (CurrentSequenceChanged != null)
                    {
                        CurrentSequenceChanged.Invoke(this);
                    }
                    currentTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// The set of all sequences.
        /// </summary>
        public MovementSequenceSet SequenceSet
        {
            get
            {
                return currentSequenceSet;
            }
        }

        /// <summary>
        /// Update function during playback.
        /// </summary>
        /// <param name="time">The time delta.</param>
        void medicalController_FixedLoopUpdate(Clock time)
        {
            currentTime += (float)time.Seconds;
            currentTime %= currentSequence.Duration;
            currentSequence.setPosition(currentTime);
        }

        /// <summary>
        /// Helper function to start listening for updates.
        /// </summary>
        private void subscibeToUpdates()
        {
            if (!playing)
            {
                playing = true;
                medicalController.FixedLoopUpdate += medicalController_FixedLoopUpdate;
            }
        }

        /// <summary>
        /// Helper function to stop listening for updates.
        /// </summary>
        private void unsubscribeFromUpdates()
        {
            if (playing)
            {
                medicalController.FixedLoopUpdate -= medicalController_FixedLoopUpdate;
                playing = false;
            }
        }
    }
}
