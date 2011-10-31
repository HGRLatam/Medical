﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AnatomyTagManager
    {
        private Dictionary<String, AnatomyTagGroup> anatomyTagGroups = new Dictionary<string, AnatomyTagGroup>();

        public AnatomyTagManager()
        {

        }

        public void setupPropertyGroups(IEnumerable<AnatomyTagProperties> properties)
        {
            foreach (AnatomyTagProperties prop in properties)
            {
                AnatomyTagGroup group = new AnatomyTagGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                anatomyTagGroups.Add(prop.Name, group);
            }
        }

        public void addAnatomyIdentifier(AnatomyIdentifier anatomyIdentifier)
        {
            foreach (AnatomyTag tag in anatomyIdentifier.Tags)
            {
                AnatomyTagGroup tagGroup;
                if (!anatomyTagGroups.TryGetValue(tag.Tag, out tagGroup))
                {
                    tagGroup = new AnatomyTagGroup(tag.Tag);
                    anatomyTagGroups.Add(tag.Tag, tagGroup);
                }
                tagGroup.addAnatomy(anatomyIdentifier);
            }
        }

        public void clear()
        {
            foreach (AnatomyTagGroup tagGroup in anatomyTagGroups.Values)
            {
                tagGroup.Dispose();
            }
            anatomyTagGroups.Clear();
        }

        public IEnumerable<AnatomyTagGroup> Groups
        {
            get
            {
                return anatomyTagGroups.Values;
            }
        }
    }
}
