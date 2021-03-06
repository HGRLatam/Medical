﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    public class MuscleController
    {
        static Dictionary<String, MuscleBehavior> muscles = new Dictionary<string,MuscleBehavior>();
        private static MovingMuscleTarget movingTarget;

        internal static void addMuscle(String name, MuscleBehavior muscle)
        {
            muscles.Add(name, muscle);
        }

        internal static void removeMuscle(String name)
        {
            muscles.Remove(name);
        }

        public static void selectMuscle(String name, bool selected)
        {
            if (muscles.ContainsKey(name))
            {
                muscles[name].Selected = selected;
            }
            else
            {
                Log.Default.sendMessage("Could not find muscle {0}.", LogLevel.Warning, "MuscleController", name);
            }
        }

        public static void changeForce(float force)
        {
            foreach (MuscleBehavior muscle in muscles.Values)
            {
                if (muscle.Selected)
                {
                    muscle.changeForce(force);
                }
            }
        }

        public static void changeForce(String name, float force)
        {
            if (muscles.ContainsKey(name))
            {
                muscles[name].changeForce(force);
            }
            else
            {
                Log.Default.sendMessage("Could not find muscle {0}.", LogLevel.Warning, "MuscleController", name);
            }
        }

        public static MuscleBehavior getMuscle(String name)
        {
            MuscleBehavior ret;
            muscles.TryGetValue(name, out ret);
            return ret;
        }

        public static MovingMuscleTarget MovingTarget
        {
            get
            {
                return movingTarget;
            }
            internal set
            {
                movingTarget = value;
            }
        }
    }
}
