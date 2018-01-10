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
    /// Represents a multiline text input control.
    /// </summary>
    [DataContract]
    public class TextArea : TextInputBase
    {
        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 99.75F;

        /// <summary>
        /// The default height of the control.
        /// </summary>
        private const float DefaultHeight = 96;

        /// <summary>
        /// The default rows of the textarea
        /// </summary>
        private const int DefaultRows = 2;

        /// <summary>
        /// Ctor
        /// </summary>
        internal TextArea() 
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            Rows = DefaultRows;
        }

        /// <summary>
        /// Gets or sets the height of the text area (in lines, HTML).
        /// </summary>
        [DataMember]
        [DefaultValue(2)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Rows)]
        [LocalizedDescription(ResourceStrings.Property_Description_Rows)]
        public int Rows { get; set; }

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
        protected override HtmlTagType TagType { get { return HtmlTagType.TextArea; } }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(System.Xml.XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            //TODO: ROWS ATTRIBUTE
        }

        /// <summary>
        /// Performs additional custom processes when writing Xsl data binding template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteDatabindingTemplate(System.Xml.XmlWriter writer)
        {
            //bind data to textarea's text

            //<xsl:value-of select="Xpath" />
            writer.WriteXslValueOfElement(DataBindings[SharedConstants.DataBinding_DefaultKey].Expression);
        }

        /// <summary>
        /// Performs additional custom processes when writing inner node of the html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteInnerTemplate(XmlWriter writer)
        {
            //if there is no data source bound to control then use "Value" property as 
            //textarea's content
            if (!DataBind)
            {
                writer.WriteString(Value);
            }
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
            //adds a rich text box control to the document
            var control = collection.AddRichTextBox(range, width, height, ObjectId);

            //initialize control properties
            control.ReadOnly = true;
            control.BackColor = Color.White;

            shape = control.InlineShape;

            return control;
        }
    }
}
