﻿using Engine.Saving;
using Medical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Lecture.GUI
{
    class SlideshowProjectTemplate : ProjectTemplate
    {
        private const String SlideshowName = "Slides.show";

        public String createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartSlideshowTask mvcTask = new StartSlideshowTask("Task", projectName, "", "Slideshows");
            mvcTask.SlideshowFile = SlideshowName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            Assembly editorAssembly = Assembly.GetExecutingAssembly();

            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", resourceProvider, EmbeddedTemplateNames.Assembly);
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SlideMasterStyles_rcss, "SlideMasterStyles.rcss", resourceProvider, EmbeddedTemplateNames.Assembly);

            Slideshow slideshow = new Slideshow();
            saveObject(slideshow, resourceProvider, SlideshowName);

            return SlideshowName;
        }

        private void saveObject(Saveable saveable, EditorResourceProvider resourceProvider, String filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(resourceProvider.openWriteStream(filename), Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
        }
    }
}
