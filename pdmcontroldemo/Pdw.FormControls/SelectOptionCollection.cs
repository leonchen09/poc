using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Pdw.FormControls
{
    [CollectionDataContract(Name = "Options", ItemName="Option")]
    public class SelectOptionCollection : CollectionBase, ICustomTypeDescriptor
    {
        public event EventHandler SelectOptionsChanged;

        public void Add(SelectOption option)
        {
            List.Add(option);
        }

        public void Remove(SelectOption option)
        {
            List.Remove(option);
        }

        public SelectOption this[int index]
        {
            get
            {
                return List[index] as SelectOption;
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            SelectOptionsChanged(this, EventArgs.Empty);
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
                SelectOption option = List[index] as SelectOption;

                SelectOptionPropertyDescriptor descriptor = new SelectOptionPropertyDescriptor(option, index);
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
