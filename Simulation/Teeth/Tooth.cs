﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;
using Engine.Attributes;
using BulletPlugin;

namespace Medical
{
    public enum ToothType
    {
        Top,
        Bottom,
    }

    public abstract class Tooth : Behavior
    {
        [DoNotCopy]
        public abstract bool Adapt
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract bool Extracted
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract bool Loose
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract Vector3 Offset
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract Quaternion Rotation
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract bool MakingToothContact
        {
            get;
        }

        [DoNotCopy]
        public abstract bool IsTopTooth { get; }

        [DoNotCopy]
        public abstract bool ShowTools
        {
            get;
            set;
        }

        [DoNotCopy]
        public abstract bool ToolHighlight
        {
            get;
            set;
        }
    }
}
