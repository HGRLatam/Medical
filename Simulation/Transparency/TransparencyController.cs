﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TransparencyController
    {
        static Dictionary<RenderGroup, TransparencyGroup> groups = new Dictionary<RenderGroup, TransparencyGroup>();

        public static void addTransparencyObject(TransparencyInterface alphaObject)
        {
            if (!groups.ContainsKey(alphaObject.RenderGroup))
            {
                groups.Add(alphaObject.RenderGroup, new TransparencyGroup(alphaObject.RenderGroup));
            }
            groups[alphaObject.RenderGroup].addTransparencyObject(alphaObject);
        }

        public static void removeTransparencyObject(TransparencyInterface alphaObject)
        {
            if (groups.ContainsKey(alphaObject.RenderGroup))
            {
                TransparencyGroup group = groups[alphaObject.RenderGroup];
                group.removeTransparencyObject(alphaObject);
                if (group.isEmpty())
                {
                    groups.Remove(alphaObject.RenderGroup);
                }
            }
        }

        public static IEnumerable<TransparencyGroup> getGroupIter()
        {
            return groups.Values;
        }
    }
}
