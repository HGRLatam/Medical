﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Attributes;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public partial class DataModel : MvcModel, IDataProvider
    {
        public const String DefaultName = "DataModel";

        [DoNotSave]
        private Dictionary<String, DataModelItem> items = new Dictionary<String, DataModelItem>();

        public DataModel(String name = DefaultName)
            :base(name)
        {

        }

        public void addItem(DataModelItem item)
        {
            items.Add(item.Name, item);
            addItemDefinition(item);
        }

        public void removeItem(DataModelItem item)
        {
            items.Remove(item.Name);
            removeItemDefinition(item);
        }

        public bool hasItem(String name)
        {
            return items.ContainsKey(name);
        }

        public DataModelItem this[String key]
        {
            get
            {
                return items[key];
            }
        }

        public bool trySetValue(String key, String value)
        {
            DataModelItem item;
            if(items.TryGetValue(key, out item))
            {
                item.StringValue = value;
            }
            return false;
        }
        
        protected DataModel(LoadInfo info)
            :base(info)
        {
            info.RebuildDictionary<String, DataModelItem>("Item", items);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractDictionary<String, DataModelItem>("Item", items);
        }

        public string getValue(string name)
        {
            return items[name].StringValue;
        }

        public bool tryGetValue(string name, out string value)
        {
            if (hasItem(name))
            {
                value = getValue(name);
                return true;
            }
            value = null;
            return false;
        }

        public bool hasValue(string name)
        {
            return items.ContainsKey(name);
        }

        public IEnumerable<Tuple<string, string>> Iterator
        {
            get
            {
                foreach (DataModelItem item in items.Values)
                {
                    yield return Tuple.Create(item.Name, item.StringValue);
                }
            }
        }
    }

    partial class DataModel
    {
        private EditInterfaceManager<DataModelItem> itemEdits;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Numeric Data", addNumberData));
            editInterface.addCommand(new EditInterfaceCommand("Add Text Data", addTextData));
            editInterface.addCommand(new EditInterfaceCommand("Add True False Data", addTrueFalseData));
            editInterface.addCommand(new EditInterfaceCommand("Add Choice Data", addChoiceData));

            itemEdits = new EditInterfaceManager<DataModelItem>(editInterface);
            itemEdits.addCommand(new EditInterfaceCommand("Remove", removeItem));

            foreach (DataModelItem link in items.Values)
            {
                addItemDefinition(link);
            }
        }

        private void addNumberData(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name for this data item", delegate(String result, ref String errorMessage)
            {
                if (hasItem(result))
                {
                    errorMessage = String.Format("An item named {0} already exists. Please enter another.", result);
                    return false;
                }
                else
                {
                    addItem(new NumberData(result));
                    return true;
                }
            });
        }

        private void addTextData(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name for this data item", delegate(String result, ref String errorMessage)
            {
                if (hasItem(result))
                {
                    errorMessage = String.Format("An item named {0} already exists. Please enter another.", result);
                    return false;
                }
                else
                {
                    addItem(new TextData(result));
                    return true;
                }
            });
        }

        private void addChoiceData(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name for this data item", delegate(String result, ref String errorMessage)
            {
                if (hasItem(result))
                {
                    errorMessage = String.Format("An item named {0} already exists. Please enter another.", result);
                    return false;
                }
                else
                {
                    addItem(new ChoiceData(result));
                    return true;
                }
            });
        }

        private void addTrueFalseData(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name for this data item", delegate(String result, ref String errorMessage)
            {
                if (hasItem(result))
                {
                    errorMessage = String.Format("An item named {0} already exists. Please enter another.", result);
                    return false;
                }
                else
                {
                    addItem(new TrueFalseData(result));
                    return true;
                }
            });
        }

        private void removeItem(EditUICallback callback, EditInterfaceCommand command)
        {
            DataModelItem item = itemEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeItem(item);
        }

        private void addItemDefinition(DataModelItem item)
        {
            if (itemEdits != null)
            {
                itemEdits.addSubInterface(item, item.getEditInterface());
            }
        }

        private void removeItemDefinition(DataModelItem item)
        {
            if (itemEdits != null)
            {
                itemEdits.removeSubInterface(item);
            }
        }
    }
}
