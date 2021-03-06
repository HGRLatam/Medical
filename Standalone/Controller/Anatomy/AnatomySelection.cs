﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class AnatomySelection
    {
        /// <summary>
        /// Called when the selected anatomy changes. Note that the passed anatomy can be null, which means no selection.
        /// </summary>
        public event Action<AnatomySelection> SelectedAnatomyChanged;

        /// <summary>
        /// Called when a selection wishes to display its anatomy.
        /// </summary>
        internal event Action<IEnumerable<Anatomy>> DisplaySelectedAnatomy;

        private HashSet<Anatomy> selectedAnatomy = new HashSet<Anatomy>();

        public void setSelection(Anatomy anatomy)
        {
            selectedAnatomy.Clear();
            if (anatomy != null)
            {
                selectedAnatomy.Add(anatomy);
            }
            fireSelectedAnatomyChanged();
        }

        public void addSelection(Anatomy anatomy)
        {
            if (anatomy != null)
            {
                selectedAnatomy.Add(anatomy);
                fireSelectedAnatomyChanged();
            }
        }

        public void removeSelection(Anatomy anatomy)
        {
            if (anatomy != null)
            {
                selectedAnatomy.Remove(anatomy);
                fireSelectedAnatomyChanged();
            }
        }

        public bool isSelected(Anatomy anatomy)
        {
            return selectedAnatomy.Contains(anatomy);
        }

        public Anatomy Anatomy
        {
            get
            {
                switch(selectedAnatomy.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return selectedAnatomy.First();
                    default:
                        AnatomySelectionGroup selectionGroup = new AnatomySelectionGroup(this.SelectedAnatomy);
                        selectionGroup.DisplayContents += selectionGroup_DisplayContents;
                        return selectionGroup;
                }
            }
        }

        public IEnumerable<Anatomy> SelectedAnatomy
        {
            get
            {
                return selectedAnatomy;
            }
        }

        public int Count
        {
            get
            {
                return selectedAnatomy.Count;
            }
        }

        private void fireSelectedAnatomyChanged()
        {
            if (SelectedAnatomyChanged != null)
            {
                SelectedAnatomyChanged.Invoke(this);
            }
        }

        void selectionGroup_DisplayContents(IEnumerable<Anatomy> items)
        {
            if (DisplaySelectedAnatomy != null)
            {
                DisplaySelectedAnatomy(items);
            }
        }
    }
}
