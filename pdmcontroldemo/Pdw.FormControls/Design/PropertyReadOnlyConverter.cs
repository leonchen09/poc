using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;

namespace Pdw.FormControls.Design
{
    public class PropertyReadOnlyConverter 
    {
        public static PropertyDescriptor Convert(PropertyDescriptor descriptorToModify, bool isReadOnly)
        {
            Attribute[] attributes = descriptorToModify.Attributes.Cast<Attribute>().ToArray();

            return new ReadOnlyPropertyDescriptor(descriptorToModify, attributes, isReadOnly);
        }

        protected class ReadOnlyPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor _descriptor;
            private Attribute[] _attributes;
            private bool _condition;

            public ReadOnlyPropertyDescriptor(PropertyDescriptor descriptor, Attribute[] attributes, bool condition)
                : base(descriptor.Name, attributes)
            {
                _descriptor = descriptor;
                _attributes = attributes;
                _condition = condition;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return _descriptor.ComponentType; }
            }

            public override object GetValue(object component)
            {
                return _descriptor.GetValue(component);
            }

            public override bool IsReadOnly
            {
                get { return _condition; }
            }

            public override Type PropertyType
            {
                get { return _descriptor.PropertyType; }
            }

            public override void ResetValue(object component)
            {
                _descriptor.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                _descriptor.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }

        public class ReadOnlyPropertyProfile<T>
        {
            public Expression<Func<T, dynamic>> Property { get; set; }

            public bool IsReadOnly { get; set; }
        }
    }
}
