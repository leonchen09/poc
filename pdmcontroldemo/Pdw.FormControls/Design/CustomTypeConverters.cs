using System;
using System.Collections;
using System.ComponentModel;
using Pdw.FormControls.Properties;

namespace Pdw.FormControls.Design
{
    public sealed class ListDefaultConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
           object result = null;

            if (destinationType == typeof(string) && value is IList)
            {
                IList list = value as IList;

                result = list!= null && list.Count != 0 ? Resources.Format_Converter_Default : string.Empty;
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }

    public sealed class TemplateItemCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType == typeof(string) && value is TemplateItemCollection)
            {
                TemplateItemCollection options = value as TemplateItemCollection;

                result = options != null && options.Count != 0 ? string.Format(Resources.Format_Converter_TemplateItemCollection, options.Count) : string.Empty;
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }

    public sealed class TemplateItemConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object result = null;
            
            if (destinationType == typeof(string) && value is TemplateItem)
            {
                TemplateItem option = value as TemplateItem;

                if (option == null || (string.IsNullOrEmpty(option.Caption) && string.IsNullOrEmpty(option.Value)))
                {
                    result = string.Empty;
                }
                else
                {
                    string format = option.Selected ? Resources.Format_Converter_TemplateItem_Selected : Resources.Format_Converter_TemplateItem;

                    result = string.Format(format, option.Caption, option.Value);
                }
                
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }

    public sealed class EmptyStringConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return null;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
