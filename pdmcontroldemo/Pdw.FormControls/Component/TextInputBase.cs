using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Pdw.FormControls.Design;

namespace Pdw.FormControls
{
    /// <summary>
    /// Implements the basic functionality required by text controls.
    /// </summary>
    [DataContract]
    public abstract class TextInputBase : VisibleControl
    {
        /// <summary>
        /// Value field for property "Value".
        /// </summary>
        private string _value;

        /// <summary>
        /// The default maximum number of characters the user can type or paste into the control
        /// </summary>
        protected const int DefaultMaxLength = 20;

        /// <summary>
        /// Ctor
        /// </summary>
        protected TextInputBase() 
        {
            MaxLength = DefaultMaxLength;
        }

        /// <summary>
        /// Gets or sets the current text in the text input control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        public override string Value
        {
            get
            {
                return Component.Text; 
            }
            set
            {
                _value = value;

                DoActionOnComponent(() => Component.Text = value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters the use can type or paste into the text input control.
        /// </summary>
        [DataMember]
        [DefaultValue(DefaultMaxLength)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_MaxLength)]
        [LocalizedDescription(ResourceStrings.Property_Description_MaxLength)]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether text is read-only when the control is rendered to a html object.
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))]
        [LocalizedDisplayName(ResourceStrings.Property_Name_ReadOnly)]
        [LocalizedDescription(ResourceStrings.Property_Description_ReadOnly)]
        public virtual bool ReadOnly { get; set; }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(System.Xml.XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            // "maxlength" attribute
            writer.WriteAttributeString("maxlength", MaxLength.ToString());

            // "readonly" attribute
            if (ReadOnly)
            {
                writer.WriteAttributeString("readonly", "readonly");
            }
        }

        /// <summary>
        /// Set the control's custom properties when it is attaching to a document.
        /// </summary>
        /// <param name="component">The component to set property.</param>
        protected override void SetAttachingComponentProperties(Control component)
        {
            Value = _value;

            base.SetAttachingComponentProperties(component);
        }
    }
}
