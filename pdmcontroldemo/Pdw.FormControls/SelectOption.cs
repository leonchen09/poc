using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Pdw.FormControls
{
    [DataContract]
    [TypeConverter(typeof(SelectOptionConverter))]
    public class SelectOption
    {
        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_OptionName)]
        [LocalizedDescription(ResourceStrings.Property_Description_OptionName)]
        public string Name { get; set; }

        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_OptionValue)]
        [LocalizedDescription(ResourceStrings.Property_Description_OptionValue)]
        public string Value { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))] 
        [LocalizedDisplayName(ResourceStrings.Property_Name_DefaultSelected)]
        [LocalizedDescription(ResourceStrings.Property_Description_DefaultSelected)]
        public bool DefaultSelected { get; set; }
    }
}
