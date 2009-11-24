﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Medical
{
    public class LayerState : Saveable, IDisposable
    {
        private String name;
        private LinkedList<LayerEntry> entries = new LinkedList<LayerEntry>();
        private bool hidden = false;
        private Bitmap thumbnail = null;

        public LayerState(String name)
        {
            this.name = name;
        }

        public void Dispose()
        {
            if (thumbnail != null)
            {
                thumbnail.Dispose();
                thumbnail = null;
            }
        }

        /// <summary>
        /// Capture the current state of the transparency controller.
        /// </summary>
        public void captureState()
        {
            entries.Clear();
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                foreach (TransparencyInterface trans in group.getTransparencyObjectIter())
                {
                    LayerEntry entry = new LayerEntry(trans);
                    entries.AddLast(entry);
                }
            }
        }

        internal void apply()
        {
            foreach (LayerEntry entry in entries)
            {
                entry.apply();
            }
        }

        internal void applyTemporaryUndisruptive()
        {
            foreach (LayerEntry entry in entries)
            {
                entry.applyTemporaryUndisruptive();
            }
        }

        public bool Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Set the thumbnail for this state. This image will NOT be copied, but
        /// it will be disposed when it is no longer needed. If you need to make
        /// a copy make one manually.
        /// </summary>
        public Bitmap Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        public IEnumerable<LayerEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        #region Saveable Members

        private const string NAME = "Name";
        private const string HIDDEN = "Hidden";
        private const string ENTRIES = "Entry";
        private const string THUMBNAIL = "Thumbnail";

        protected LayerState(LoadInfo info)
        {
            name = info.GetString(NAME);
            hidden = info.GetBoolean(HIDDEN);
            info.RebuildLinkedList<LayerEntry>(ENTRIES, entries);
            if (info.hasValue(THUMBNAIL))
            {
                using (MemoryStream memStream = new MemoryStream(info.GetBlob(THUMBNAIL)))
                {
                    thumbnail = new Bitmap(memStream);
                    memStream.Close();
                }
            }
            else
            {
                thumbnail = null;
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(HIDDEN, hidden);
            info.ExtractLinkedList<LayerEntry>(ENTRIES, entries);
            if (thumbnail != null)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    thumbnail.Save(memStream, ImageFormat.Png);
                    info.AddValue(THUMBNAIL, memStream.GetBuffer());
                    memStream.Close();
                }
            }
        }

        #endregion
    }
}
