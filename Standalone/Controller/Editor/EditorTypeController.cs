﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical.GUI;

namespace Medical
{
    public abstract class EditorTypeController
    {
        public EditorTypeController(String extension, EditorController editorController)
        {
            this.Extension = extension;
            this.EditorController = editorController;
        }

        public String Extension { get; private set; }

        public EditorController EditorController { get; private set; }

        public abstract void openEditor(String file);

        public abstract T loadFile<T>(String file) where T : class;

        public abstract void closeFile(String file);

        public virtual ProjectItemTemplate createItemTemplate()
        {
            return null;
        }
    }
}
