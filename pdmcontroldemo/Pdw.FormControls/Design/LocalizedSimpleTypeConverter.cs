using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Pdw.FormControls.Properties;

namespace Pdw.FormControls.Design
{
    public sealed class LocalizedSimpleTypeConverter<T> : TypeConverter
    {
        private readonly Dictionary<string, T> Values;

        public LocalizedSimpleTypeConverter()
        {
            Type targetType = typeof(T);
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            Values = new Dictionary<string, T>();

            if (targetType == typeof(bool) || targetType.BaseType == typeof(Enum))
            {
                IEnumerable<string> names = GetFieldsNameList(targetType);

                foreach (string name in names)
                {
                    string resource = GetResource(targetType.Name, name);

                    if (resource != null)
                    {
                        T value = (T)converter.ConvertFrom(name);
                        Values.Add(resource, value);
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            bool toReturn = sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);

            return toReturn;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Values.Keys);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            dynamic toReturn = null;

            if (value is T)
            {
                toReturn = Values.Single(v => v.Value.Equals(value)).Key;
            }
            else
            {
                toReturn = base.ConvertTo(context, culture, value, destinationType);
            }

            return toReturn;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            dynamic toReturn;

            if (value is string)
            {
                toReturn = Values[(string)value];
            }
            else
            {
                toReturn = base.ConvertFrom(context, culture, value);
            }

            return toReturn;
        }

        private string GetResource(string typeName, string fieldName)
        {
            string resourceName = string.Format(SharedConstants.TypeFullNameFormat, typeName, fieldName);
            string resource = string.Empty;

            try
            {
                resource = Resources.ResourceManager.GetString(resourceName);
            }
            catch (Exception)
            {
                resource = fieldName;
            }

            return resource;
        }

        private IEnumerable<string> GetFieldsNameList(Type targetType)
        {
            IEnumerable<string> names = targetType == typeof(Boolean)
                ? new[] { Boolean.TrueString, Boolean.FalseString }
                : targetType.GetFields().Select(f => f.Name);

            return names;
        }
    }
}
