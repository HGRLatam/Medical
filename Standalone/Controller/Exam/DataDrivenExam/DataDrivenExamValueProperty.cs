﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    class DataDrivenExamValueProperty : EditableProperty
    {
        private String key;
        private Object value;

        public DataDrivenExamValueProperty(String key, Object value)
        {
            this.key = key;
            this.value = value;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return false;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return key;
                case 1:
                    return value.ToString();
                default:
                    return "";
            }
        }

        public void setValueStr(int column, string value)
        {
            
        }
    }
}
