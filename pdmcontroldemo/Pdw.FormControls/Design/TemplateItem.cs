using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Pdw.FormControls.Design
{
    [DataContract]
    [TypeConverter(typeof(TemplateItemConverter))]
    public sealed class TemplateItem
    {
        public TemplateItem()
        {
            Caption = string.Empty;
            Value = string.Empty;
        }

        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Caption)]
        [LocalizedDescription(ResourceStrings.Property_Description_Caption)]
        public string Caption { get; set; }

        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Value)]
        [LocalizedDescription(ResourceStrings.Property_Description_Value)]
        public string Value { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))] 
        [LocalizedDisplayName(ResourceStrings.Property_Name_Selected)]
        [LocalizedDescription(ResourceStrings.Property_Description_Selected)]
        public bool Selected { get; set; }
    }
}
