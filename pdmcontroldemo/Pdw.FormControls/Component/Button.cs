using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    /// Represents a button control.
    /// </summary>
    [DataContract]
    public class Button : VisibleControl
    {
        /// <summary>
        /// Value field for property "Value".
        /// </summary>
        private string _value;

        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 75;

        /// <summary>
        /// The default height of the control.
        /// </summary>
        private const float DefaultHeight = 23.25F;

        /// <summary>
        /// The color's name of the default background color.
        /// </summary>
        private const string DefaultBackColorName = "Silver";

        /// <summary>
        /// The default background color of the control.
        /// </summary>
        private static Color DefaultBackColor = Color.Silver;

        /// <summary>
        /// The default alignment of the text on the control. 
        /// </summary>
        private static HorizontalAlignment DefaultTextAlign = HorizontalAlignment.Center;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Button() 
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            BackColor = DefaultBackColor;
            TextAlign = DefaultTextAlign;
        }

        /// <summary>
        /// Gets or sets the type of the button control.
        /// </summary>
        [DataMember]
        [DefaultValue(ButtonType.Button)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<ButtonType>))]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Type)]
        [LocalizedDescription(ResourceStrings.Property_Description_Type)]
        public ButtonType Type { get; set; }

        /// <summary>
        /// Gets control's html object type.
        /// </summary>
        protected override HtmlTagType TagType { get { return HtmlTagType.Input; } }

        /// <summary>
        /// Gets or sets a value indicating whether binds a data source to control. 
        /// (No effect for this control).
        /// </summary>
        [Browsable(false)]
        public override bool DataBind { get; set; }

        /// <summary>
        /// Gets or sets a well-formed data binding profile for the control. 
        /// (No effect for this control).
        /// </summary>
        [Browsable(false)]
        public override string DataBindingPath { get; set; }

        /// <summary>
        /// Gets or sets the current text in the button control.
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
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DefaultValue(typeof(Color), DefaultBackColorName)]
        public override System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text on the control. 
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        public override System.Windows.Forms.HorizontalAlignment TextAlign
        {
            get
            {
                return base.TextAlign;
            }
            set
            {
                base.TextAlign = value;
            }
        }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(System.Xml.XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            //"type" attribute
            writer.WriteAttributeString("type", Type.ToString().ToLower());
            //"value" attribute
            writer.WriteAttributeString("value", Value);
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
        protected override System.Windows.Forms.Control Attach(ControlCollection collection, Word.Range range, float width, float height, out Word.InlineShape shape)
        {
            ////adds a button box control to the document
            var control = collection.AddButton(range, width, height, ObjectId);

            shape = control.InlineShape;

            return control;
        }

        /// <summary>
        /// Set the control's custom properties when it is attaching to a document.
        /// </summary>
        /// <param name="component">The component to set property.</param>
        protected override void SetAttachingComponentProperties(System.Windows.Forms.Control component)
        {
            Value = _value;

            base.SetAttachingComponentProperties(component);
        }
    }
}
