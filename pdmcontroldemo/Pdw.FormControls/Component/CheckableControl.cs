using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;

namespace Pdw.FormControls
{
    /// <summary>
    /// Implements the basic functionality required by checkable controls.
    /// </summary>
    [DataContract]
    public abstract class CheckableControl : VisibleControl
    {
        /// <summary>
        /// Value field for property "Caption".
        /// </summary>
        private string _caption;

        /// <summary>
        /// Value field for property "Checked".
        /// </summary>
        private bool _checked;

        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 75;

        /// <summary>
        /// The default height of the control.
        /// </summary>
        private const float DefaultHeight = 23.25F;

        /// <summary>
        /// Ctor
        /// </summary>
        protected CheckableControl()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        /// <summary>
        /// Gets the exact control of current object which has a "Checked" property.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when control has not a "Checked" property.
        /// </exception>
        protected dynamic ExactControl
        {
            get
            {
                //try to get control's "Checked" property 
                PropertyInfo property = Component.GetType().GetProperty("Checked", typeof(bool));

                //if control has not a "Checked" property then throw an exception 
                if (property == null)
                {
                    throw new NotSupportedException();
                }

                return Component;
            }
        }

        /// <summary>
        /// Gets or set a value indicating whether the checkable control is in the checked state.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DataMember]
        [DefaultValue(false)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Checked)]
        [LocalizedDescription(ResourceStrings.Property_Description_Checked)]
        public bool Checked
        {
            get
            {
                return ExactControl.Checked; 
            }
            set
            {
                _checked = false;

                DoActionOnComponent(() => ExactControl.Checked = value);
            }
        }

        /// <summary>
        /// Gets or sets the caption of the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Caption)]
        [LocalizedDescription(ResourceStrings.Property_Description_Caption)]
        public string Caption 
        {
            get
            {
                return Component.Text; 
            }
            set
            {
                _caption = value;

                DoActionOnComponent(() => Component.Text = value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [Browsable(false)]
        [DefaultValue(DefaultHeight)]
        public override float Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [Browsable(false)]
        [DefaultValue(DefaultWidth)]
        public override float Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Gets control's html object type. Always return HtmlTagType.Input.
        /// </summary>
        protected override HtmlTagType TagType { get { return HtmlTagType.Input; } }

        /// <summary>
        /// Gets checkable control's html input object type.
        /// </summary>
        protected abstract InputTagType InputType { get; }

        /// <summary>
        /// Performs additional custom processes when writing Xsl data binding template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteDatabindingTemplate(System.Xml.XmlWriter writer)
        {
            //<xsl:if test="Boolean Expression">
            string expression = DataBindings[SharedConstants.DataBinding_DefaultKey].Expression;
            string condition = string.Format(SharedConstants.Format_EqualExpression, expression, Value);
            writer.WriteStartXslIfElement(condition);

            //<xsl:attribute name="checked" />
            writer.WriteStartXslAttributeElement("checked");
            writer.WriteEndElement();

            //</xsl:if>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(System.Xml.XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            //"type" attribute for input object
            writer.WriteAttributeString("type", InputType.ToString().ToLower());
            //"value" attribute
            writer.WriteAttributeString("value", Value);

            //if there is no data source bound to control and control is checked
            //then set "checked" attribute
            if (!DataBind && Checked)
            {
                writer.WriteStartAttribute("checked");
                writer.WriteEndAttribute();
            }
            else 
            {
                //set control is unchecked
                Checked = false;
            }
        }

        /// <summary>
        /// Performs additional custom processes when write to Xsl template finished.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteToXslTemplateComplete(System.Xml.XmlWriter writer)
        {
            //adds caption after input tag
            writer.WriteString(Caption);
        }

        /// <summary>
        /// Set the control's custom properties when it is attaching to a document.
        /// </summary>
        /// <param name="component">The component to set property.</param>
        protected override void SetAttachingComponentProperties(Control component)
        {
            Checked = _checked;
            Caption = _caption;

            base.SetAttachingComponentProperties(component);
        }

        /// <summary>
        /// Get custom css inline style
        /// </summary>
        /// <param name="autoFit">
        /// A value indicating whether the control's html width and height should be calculated by browser.
        /// </param>
        /// <returns>Custom css inline style</returns>
        protected override string GetCustomStyle(bool autoFit = false)
        {
            //set control's html object to auto match its size
            return base.GetCustomStyle(true);
        }
    }
}
