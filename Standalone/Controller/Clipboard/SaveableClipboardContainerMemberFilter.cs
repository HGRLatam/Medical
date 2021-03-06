﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Reflection;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// The filter for SaveableClipboardContainers ReflectedSaver.
    /// </summary>
    class SaveableClipboardContainerMemberFilter : MemberScannerFilter
    {
        public bool allowMember(MemberWrapper wrapper)
        {
            return !(wrapper.getCustomAttributes(typeof(DoNotSaveAttribute), true).Any()
                || wrapper.getWrappedType().GetCustomAttributes(typeof(DoNotSaveAttribute), true).Any()
                || wrapper.getWrappedType().GetCustomAttributes(typeof(NativeSubsystemTypeAttribute), true).Any()
                || !wrapper.getWrappedType().IsSubclassOf(typeof(Delegate)));
        }

        /// <summary>
        /// This function determines if the given type should be scanned for
        /// members. It will return true if the member should be accepted.
        /// </summary>
        /// <param name="type">The type to potentially scan for members.</param>
        /// <returns>True if the type should be scanned.</returns>
        public bool allowType(Type type)
        {
            return typeof(Saveable).IsAssignableFrom(type);
        }
    }
}
