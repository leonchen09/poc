using System;
using System.ComponentModel;

namespace Pdw.FormControls
{
    public class SelectOptionPropertyDescriptor : PropertyDescriptor
    {
        private SelectOption _option;
        private int _index;

        public SelectOptionPropertyDescriptor(SelectOption option, int index) : base((index + 1).ToString(), null)
        {
            _option = option;
            _index = index;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return typeof(SelectOptionCollection); }
        }

        public override object GetValue(object component)
        {
            return _option;
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(SelectOption); }
        }

        public override void ResetValue(object component) { }

        public override void SetValue(object component, object value) { }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}
