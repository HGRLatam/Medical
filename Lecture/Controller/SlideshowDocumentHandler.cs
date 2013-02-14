﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical;

namespace Lecture
{
    class SlideshowDocumentHandler : DocumentHandler
    {
        private EditorController editorController;

        public SlideshowDocumentHandler(EditorController editorController)
        {
            this.editorController = editorController;
        }

        public bool canReadFile(string filename)
        {
            return Path.GetExtension(filename).Equals(".show", StringComparison.InvariantCultureIgnoreCase) && File.Exists(filename);
        }

        public bool processFile(string filename)
        {
            editorController.openProject(Path.GetDirectoryName(filename), filename);
            return true;
        }

        public string getPrettyName(string filename)
        {
            return "Smart Lectures";
        }

        public string getIcon(string filename)
        {
            return "Lecture.Icon.SmartLectureIcon";
        }
    }
}
