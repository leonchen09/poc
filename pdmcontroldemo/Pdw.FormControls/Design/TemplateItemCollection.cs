using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pdw.FormControls.Design
{
    public sealed class TemplateItemCollection : CollectionBase, ICustomTypeDescriptor
    {
        public event EventHandler TemplateItemsChanged;

        public void Add(TemplateItem item)
        {
            List.Add(item);
        }

        public void Remove(TemplateItem item)
        {
            List.Remove(item);
        }

        public TemplateItem this[int index]
        {
            get
            {
                return List[index] as TemplateItem;
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            if (TemplateItemsChanged != null)
            {
                TemplateItemsChanged(this, EventArgs.Empty);
            }
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(null);

            for (int index = 0; index < List.Count; index++)
            {
                TemplateItem option = List[index] as TemplateItem;

                TemplateItemPropertyDescriptor descriptor = new TemplateItemPropertyDescriptor(option, index);
                descriptors.Add(descriptor);
            }

            return descriptors;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
