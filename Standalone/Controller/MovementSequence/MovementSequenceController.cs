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
using Engine;

namespace Medical.Controller
{
    /// <summary>
    /// An event for the MuscleSequenceController.
    /// </summary>
    /// <param name="controller">The controller that fired the event.</param>
    public delegate void MovementSequenceEvent(MovementSequenceController controller);
    public delegate void MovementSequenceGroupEvent(MovementSequenceController controller, MovementSequenceGroup group);
    public delegate void MovementSequenceInfoEvent(MovementSequenceController controller, MovementSequenceGroup group, MovementSequenceInfo sequenceInfo);

    /// <summary>
    /// This class manages loading and playback of movement sequences.
    /// </summary>
    public class MovementSequenceController
    {
        public event MovementSequenceGroupEvent GroupAdded;
        public event MovementSequenceInfoEvent SequenceAdded;
        public event MovementSequenceEvent CurrentSequenceChanged;
        public event MovementSequenceEvent PlaybackStarted;
        public event MovementSequenceEvent PlaybackStopped;
        public event MovementSequenceEvent PlaybackUpdate;

        private XmlSaver xmlSaver = new XmlSaver();
        private MovementSequence currentSequence;
        private MedicalController medicalController;
        private float currentTime = 0.0f;
        private bool playing = false;
        private MovementSequenceSet currentSequenceSet = new MovementSequenceSet();

        public MovementSequenceController(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        /// <summary>
        /// Load the specified sequence and return it.
        /// </summary>
        /// <param name="sequenceInfo">The filename of the sequence to load.</param>
        /// <returns>The loaded sequence.</returns>
        public MovementSequence loadSequence(MovementSequenceInfo sequenceInfo)
        {
            return sequenceInfo.loadSequence(xmlSaver);
        }

        /// <summary>
        /// Load the sequences in the specified directory. Will not make any
        /// changes if the directory is the currently loaded directory.
        /// </summary>
        public void loadSequenceDirectories(String baseDir, List<String> sequenceDirs)
        {
            CurrentSequence = null;
            currentSequenceSet = new MovementSequenceSet();
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            foreach (String sequenceDirBase in sequenceDirs)
            {
                String sequenceDir = baseDir + sequenceDirBase;
                if (archive.exists(sequenceDir))
                {
                    foreach (String directory in archive.listDirectories(sequenceDir, false, false))
                    {
                        String groupName = archive.getFileInfo(directory).Name;
                        foreach (String file in archive.listFiles(directory, false))
                        {
                            VirtualFileInfo fileInfo = archive.getFileInfo(file);
                            String fileName = fileInfo.Name;
                            if (fileName.EndsWith(".seq"))
                            {
                                VirtualFSMovementSequenceInfo info = new VirtualFSMovementSequenceInfo();
                                info.Name = fileName.Substring(0, fileName.Length - 4);
                                info.FileName = fileInfo.FullName;
                                addMovementSequence(groupName, info);
                            }
                        }
                    }
                }
            }
        }

        public void addMovementSequence(String groupName, MovementSequenceInfo info)
        {
            MovementSequenceGroup group = currentSequenceSet.getGroup(groupName);
            if (group == null)
            {
                group = new MovementSequenceGroup(groupName);
                currentSequenceSet.addGroup(group);
                if (GroupAdded != null)
                {
                    GroupAdded.Invoke(this, group);
                }
            }
            group.addSequence(info);
            if (SequenceAdded != null)
            {
                SequenceAdded.Invoke(this, group, info);
            }
        }

        /// <summary>
        /// Play the current sequence.
        /// </summary>
        public void playCurrentSequence()
        {
            if (currentSequence != null && !playing)
            {
                currentTime = 0.0f;
                playing = true;
                medicalController.FixedLoopUpdate += medicalController_FixedLoopUpdate;
                if (PlaybackStarted != null)
                {
                    PlaybackStarted.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Stop playing the current sequence.
        /// </summary>
        public void stopPlayback()
        {
            if (playing)
            {
                medicalController.FixedLoopUpdate -= medicalController_FixedLoopUpdate;
                playing = false;
                if (currentSequence != null)
                {
                    currentSequence.setPosition(0.0f);
                }
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Stops playing the current sequence, but does not reset the time back
        /// to 0. This will effectivly leave the scene as it was when the
        /// sequence was playing. Still fires the PlaybackStopped event.
        /// </summary>
        public void pausePlayback()
        {
            if (playing)
            {
                medicalController.FixedLoopUpdate -= medicalController_FixedLoopUpdate;
                playing = false;
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this);
                }
            }
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

        public bool Playing
        {
            get
            {
                return playing;
            }
        }

        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                if (currentSequence.Duration != 0.0f)
                {
                    currentTime %= currentSequence.Duration;
                }
                currentSequence.setPosition(currentTime);
            }
        }

        /// <summary>
        /// Update function during playback.
        /// </summary>
        /// <param name="time">The time delta.</param>
        void medicalController_FixedLoopUpdate(Clock time)
        {
            CurrentTime += (float)time.Seconds;
            if (PlaybackUpdate != null)
            {
                PlaybackUpdate.Invoke(this);
            }
        }
    }
}
