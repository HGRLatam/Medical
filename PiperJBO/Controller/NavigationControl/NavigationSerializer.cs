﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Engine;
using System.IO;
using Logging;
using Engine.Platform;
using System.Drawing;
using System.Drawing.Imaging;

namespace Medical
{
    class NavigationSerializer
    {
        private const String NAVIGATION_STATE_SET = "NavigationStateSet";
        private const String NAVIGATION_STATE = "NavigationState";
        private const String LOOK_AT = "LookAt";
        private const String TRANSLATION = "Translation";
        private const String NAME = "Name";
        private const String LINK = "Link";
        private const String BUTTON = "Button";
        private const String DESTINATION = "Destination";
        private const String SOURCE = "Source";
        private const String VISUAL_RADIUS = "VisualRadius";
        private const String RADIUS_START_OFFSET = "RadiusStartOffset";
        private const String HIDDEN = "IsHidden";
        private const String SHORTCUT_KEY = "ShortcutKey";
        private const String THUMBNAIL = "Thumbnail";

        private const String NAVIGATION_MENU = "NavigationMenu";
        private const String NAVIGATION_MENU_ENTRY = "NavigationMenuEntry";
        private const String TEXT = "Text";
        private const String BITMAP_SIZE = "Size";
        private const String LAYER_STATE = "LayerState";
        private const String MENU_ENTRY_NAVIGATION_STATE = "MenuEntryNavigationState";

        public static void writeNavigationStateSet(NavigationStateSet set, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(NAVIGATION_STATE_SET);
            foreach(NavigationState state in set.States)
            {
                xmlWriter.WriteStartElement(NAVIGATION_STATE);
                xmlWriter.WriteElementString(NAME, state.Name);
                xmlWriter.WriteElementString(TRANSLATION, state.Translation.ToString());
                xmlWriter.WriteElementString(LOOK_AT, state.LookAt.ToString());
                xmlWriter.WriteElementString(HIDDEN, state.Hidden.ToString());
                xmlWriter.WriteElementString(SHORTCUT_KEY, state.ShortcutKey.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement(NAVIGATION_MENU);
            foreach (NavigationMenuEntry entry in set.Menus.ParentEntries)
            {
                WriteNavMenuEntry(xmlWriter, entry);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public static void WriteNavMenuEntry(XmlWriter xmlWriter, NavigationMenuEntry entry)
        {
            xmlWriter.WriteStartElement(NAVIGATION_MENU_ENTRY);
            xmlWriter.WriteElementString(TEXT, entry.Text);
            xmlWriter.WriteElementString(LAYER_STATE, entry.LayerState);
            if (entry.NavigationState != null)
            {
                xmlWriter.WriteElementString(MENU_ENTRY_NAVIGATION_STATE, entry.NavigationState);
            }
            if (entry.Thumbnail != null)
            {
                writeThumbnail(xmlWriter, entry.Thumbnail);
            }
            if (entry.SubEntries != null)
            {
                foreach (NavigationMenuEntry subEntry in entry.SubEntries)
                {
                    WriteNavMenuEntry(xmlWriter, subEntry);
                }
            }
            xmlWriter.WriteEndElement();
        }

        private static void writeThumbnail(XmlWriter xmlWriter, Bitmap image)
        {
            xmlWriter.WriteStartElement(THUMBNAIL);
            using (MemoryStream memStream = new MemoryStream())
            {
                image.Save(memStream, ImageFormat.Png);
                byte[] buffer = memStream.GetBuffer();
                xmlWriter.WriteAttributeString(BITMAP_SIZE, buffer.Length.ToString());
                xmlWriter.WriteBinHex(buffer, 0, buffer.Length);
            }
            xmlWriter.WriteEndElement();
        }

        public static NavigationStateSet readNavigationStateSet(XmlReader xmlReader)
        {
            NavigationStateSet set = new NavigationStateSet();
            while (!isEndElement(xmlReader, NAVIGATION_STATE_SET) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAVIGATION_STATE)
                    {
                        readNavigationState(set, xmlReader);
                    }
                    else if (xmlReader.Name == NAVIGATION_MENU)
                    {
                        readNavigationMenu(set, xmlReader);
                    }
                }
            }
            return set;
        }

        private static void readNavigationState(NavigationStateSet set, XmlReader xmlReader)
        {
            String name = null;
            Vector3 position = Vector3.UnitZ;
            Vector3 lookAt = Vector3.Zero;
            bool hidden = false;
            KeyboardButtonCode shortcutKey = KeyboardButtonCode.KC_UNASSIGNED;
            while (!isEndElement(xmlReader, NAVIGATION_STATE) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAME)
                    {
                        name = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == TRANSLATION)
                    {
                        position = new Vector3(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == LOOK_AT)
                    {
                        lookAt = new Vector3(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == HIDDEN)
                    {
                        hidden = bool.Parse(xmlReader.ReadElementContentAsString());
                    }
                    else if (xmlReader.Name == SHORTCUT_KEY)
                    {
                        shortcutKey = (KeyboardButtonCode)Enum.Parse(typeof(KeyboardButtonCode), xmlReader.ReadElementContentAsString());
                    }
                }
            }
            if (name != null)
            {
                NavigationState navState = new NavigationState(name, lookAt, position, hidden, shortcutKey);
                set.addState(navState);
            }
        }

        public static void readNavigationMenu(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            while (!isEndElement(xmlReader, NAVIGATION_MENU) && xmlReader.Read())
            {
                if(isValidElement(xmlReader))
                {
                    if (xmlReader.Name == NAVIGATION_MENU_ENTRY)
                    {
                        readNavigationMenuParentEntry(navStateSet, xmlReader);
                    }
                }
            }
        }

        public static void readNavigationMenuParentEntry(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            navStateSet.Menus.addParentEntry(readNavMenuEntryData(navStateSet, xmlReader));
        }

        private static NavigationMenuEntry readNavMenuEntryData(NavigationStateSet navStateSet, XmlReader xmlReader)
        {
            NavigationMenuEntry menuEntry = new NavigationMenuEntry("");
            while (!isEndElement(xmlReader, NAVIGATION_MENU_ENTRY) && xmlReader.Read())
            {
                if (isValidElement(xmlReader))
                {
                    if (xmlReader.Name == TEXT)
                    {
                        menuEntry.Text = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == LAYER_STATE)
                    {
                        menuEntry.LayerState = xmlReader.ReadElementContentAsString();
                    }
                    else if (xmlReader.Name == THUMBNAIL)
                    {
                        menuEntry.Thumbnail = readThumbnail(xmlReader);
                    }
                    else if (xmlReader.Name == NAVIGATION_MENU_ENTRY)
                    {
                        menuEntry.addSubEntry(readNavMenuEntryData(navStateSet, xmlReader));
                    }
                    else if (xmlReader.Name == MENU_ENTRY_NAVIGATION_STATE)
                    {
                        menuEntry.NavigationState = xmlReader.ReadElementContentAsString();
                    }
                }
            }
            xmlReader.Read();
            return menuEntry;
        }

        private static Bitmap readThumbnail(XmlReader xmlReader)
        {
            int size = NumberParser.ParseInt(xmlReader.GetAttribute(BITMAP_SIZE));
            byte[] buffer = new byte[size];
            xmlReader.ReadElementContentAsBinHex(buffer, 0, size);
            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                return new Bitmap(memStream);
            }
        }

        private static bool isEndElement(XmlReader xmlReader, String elementName)
        {
            return xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.EndElement;
        }

        private static bool isValidElement(XmlReader xmlReader)
        {
            return xmlReader.NodeType == XmlNodeType.Element && !xmlReader.IsEmptyElement;
        }
    }
}
