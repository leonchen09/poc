using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    /// Represents a single line text input control.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Font))]
    public class TextInput : TextInputBase
    {
        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 99.75F;

        /// <summary>
        /// The default height of the control.
        /// </summary>
        private const float DefaultHeight = 18;

        /// <summary>
        /// Ctor
        /// </summary>
        internal TextInput() 
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        /// <summary>
        /// Gets or sets a value indicating whether represents the control as a password input object.
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_IsPassword)]
        [LocalizedDescription(ResourceStrings.Property_Description_IsPassword)]
        public bool IsPassword { get; set; }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
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
        /// Gets control's html object type.
        /// </summary>
        protected override HtmlTagType TagType { get { return HtmlTagType.Input; } }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            // "type" attribute
            writer.WriteAttributeString("type", IsPassword ? InputTagType.Password.ToString().ToLower() : InputTagType.Text.ToString().ToLower());

            //if there is no data source bound to control then set "value" attribute
            if (!DataBind)
            {
                writer.WriteAttributeString("value", Value);
            }
        }

        /// <summary>
        /// Performs additional custom processes when writing Xsl data binding template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteDatabindingTemplate(System.Xml.XmlWriter writer)
        {
            //bind data to text input object's value attribute

            //<xsl:attribute name="value">
            writer.WriteStartXslAttributeElement("value");

            //<xsl:value-of select="Xpath" />
            writer.WriteXslValueOfElement(DataBindings[SharedConstants.DataBinding_DefaultKey].Expression);

            //</xsl:attribute>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Attach the control to a specified document's control collection using a specified process.
        /// </summary>
        /// <param name="collection">The control collection to hold the control.</param>
        /// <param name="range">The location of the control.</param>
        /// <param name="width">The width of the control.</param>
        /// <param name="height">The height of the control.</param>
        /// <param name="shape">The control's inline shape object in the document</param>
        /// <returns>The component of the attached control.</returns>
        protected override Control Attach(ControlCollection collection, Word.Range range, float width, float height, out Word.InlineShape shape)
        {
            //adds a text box control to the document 
            var control = collection.AddTextBox(range, width, height, ObjectId);

            //initialize control properties
            control.ReadOnly = true;
            control.BackColor = Color.White;
            control.Multiline = true;
            control.WordWrap = false;

            shape = control.InlineShape;

            return control;
        }
    }
}
