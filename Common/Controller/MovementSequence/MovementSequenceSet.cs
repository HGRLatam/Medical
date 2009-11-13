﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class MovementSequenceSet : IDisposable
    {
        private LinkedList<MovementSequenceGroup> groups = new LinkedList<MovementSequenceGroup>();

        public MovementSequenceSet()
        {

        }

        public void Dispose()
        {
            foreach (MovementSequenceGroup group in groups)
            {
                group.Dispose();
            }
        }

        public void addGroup(MovementSequenceGroup group)
        {
            groups.AddLast(group);
        }

        public void removeGroup(MovementSequenceGroup group)
        {
            groups.Remove(group);
        }

        public IEnumerable<MovementSequenceGroup> Groups
        {
            get
            {
                return groups;
            }
        }
    }
}
