using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;

namespace Pdw.FormControls
{
    public abstract partial class ControlBase : ICustomTypeDescriptor
    {
        /// <summary>
        /// Gets the properties which will be changed to the ReadOnly status when data bound.
        /// </summary>
        /// <returns>The properties.</returns>
        protected virtual IEnumerable<PropertyReadOnlyConverter.ReadOnlyPropertyProfile<ControlBase>> GetPropertiesToChangeReadOnlyStatus()
        {
            yield return new PropertyReadOnlyConverter.ReadOnlyPropertyProfile<ControlBase>
            {
                Property = c => c.DataBindingPath,
                IsReadOnly = !DataBind
            };

            yield return new PropertyReadOnlyConverter.ReadOnlyPropertyProfile<ControlBase> 
            { 
                Property = c => c.Value, IsReadOnly = DataBind 
            };

            yield return new PropertyReadOnlyConverter.ReadOnlyPropertyProfile<ControlBase>
            {
                Property = c => c.DataBind,
                IsReadOnly = !DataBind
            };
        }

        public event PropertyChangedEventHandler DataDindPropertyChanged;

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
            List<PropertyDescriptor> originalDescriptors = TypeDescriptor.GetProperties(GetType()).Cast<PropertyDescriptor>().ToList();

            IEnumerable<PropertyReadOnlyConverter.ReadOnlyPropertyProfile<ControlBase>> propertiesToChange = GetPropertiesToChangeReadOnlyStatus();

            foreach (var property in propertiesToChange)
            {
                MemberExpression expression = null;

                if (property.Property.Body is MemberExpression)
                {
                    expression = property.Property.Body as MemberExpression;
                }
                else if (property.Property.Body is UnaryExpression)
                {
                    UnaryExpression bodyExpression = property.Property.Body as UnaryExpression;
                    expression = bodyExpression.Operand as MemberExpression;
                }

                if (expression == null || (expression.Member.ReflectedType != typeof(ControlBase) 
                    && !expression.Member.ReflectedType.IsSubclassOf(typeof(ControlBase))))
                {
                    throw new NotSupportedException();
                }

                PropertyDescriptor descriptor = originalDescriptors.Find(d => d.Name.Is(expression.Member.Name));
                PropertyDescriptor newDescriptor = PropertyReadOnlyConverter.Convert(descriptor, property.IsReadOnly);

                originalDescriptors.Remove(descriptor);
                originalDescriptors.Add(newDescriptor);
            }

            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(originalDescriptors.ToArray());

            return descriptors;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
