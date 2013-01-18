﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using libRocketPlugin;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class RmlEditableProperty : EditableProperty
    {
        public delegate Browser CreateBrowser(EditUICallback uiCallback);

        private String name;
        private String value;
        private CreateBrowser browserBuildCallback;

        public event Action<RmlEditableProperty> ValueChanged;

        public RmlEditableProperty(String name, String value, CreateBrowser browserBuildCallback = null)
        {
            this.name = name;
            this.value = value;
            this.browserBuildCallback = browserBuildCallback;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            switch (column)
            {
                case 0:
                    return null;
                case 1:
                    return browserBuildCallback(uiCallback);
            }
            return null;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return name;
                case 1:
                    return value;
            }
            return null;
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return name;
                case 1:
                    return value;
            }
            return null;
        }

        public bool hasBrowser(int column)
        {
            switch (column)
            {
                case 0:
                    return false;
                case 1:
                    return browserBuildCallback != null;
            }
            return false;
        }

        public void setValue(int column, object value)
        {
            switch (column)
            {
                case 0:
                    name = value.ToString();
                    break;
                case 1:
                    this.value = value.ToString();
                    break;
            }
            fireValueChanged();
        }

        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case 0:
                    name = value.ToString();
                    break;
                case 1:
                    this.value = value;
                    break;
            }
            fireValueChanged();
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public String Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        private void fireValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this);
            }
        }
    }
}