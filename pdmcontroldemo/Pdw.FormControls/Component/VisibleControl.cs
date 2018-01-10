using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;

namespace Pdw.FormControls
{
    /// <summary>
    /// The base class for controls, which are components with visual representation.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FontStyle))]
    [KnownType(typeof(GraphicsUnit))] 
    public abstract class VisibleControl : ControlBase
    {
        /// <summary>
        /// Value field for property "BackColor". 
        /// </summary>
        private Color _backColor;

        /// <summary>
        /// Value field for property "ForeColor". 
        /// </summary>
        private Color _foreColor;

        /// <summary>
        /// Value field for property "Font". 
        /// </summary>
        private Font _font;

        /// <summary>
        /// The color's name of the default background color.
        /// </summary>
        private const string DefaultBackColorName = "White";

        /// <summary>
        /// The color's name of the default foreground color.
        /// </summary>
        private const string DefaultForeColorName = "Black";

        /// <summary>
        /// The format of the css inline style which will be used to render html object.  
        /// </summary>
        private const string StyleFormat = "color:{0};background-color:{1};text-align:{2};{3}height:{4};width:{5};";

        /// <summary>
        ///  The default background color of the control.
        /// </summary>
        private static Color DefaultBackColor = Color.White;

        /// <summary>
        /// The default foreground color of the control.
        /// </summary>
        private static Color DefaultForeColor = Color.Black;

        /// <summary>
        /// The default font of the control.
        /// </summary>
        private static Font DefaultFont = SystemFonts.DefaultFont;

        /// <summary>
        /// Ctor
        /// </summary>
        protected VisibleControl() 
        {
            _backColor = DefaultBackColor;
            _foreColor = DefaultForeColor;
            _font = DefaultFont;
        }

        /// <summary>
        /// Gets or sets HTML css class name or inline style attribute for control's HTML object.
        /// </summary>
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Css)]
        [LocalizedDescription(ResourceStrings.Property_Description_Css)]
        public virtual string Css { get; set; }

        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Font)]
        [LocalizedDescription(ResourceStrings.Property_Description_Font)]
        public virtual Font Font
        {
            get
            {
                return Component.Font;
            }
            set
            {
                _font = value;

                DoActionOnComponent(() => Component.Font = value);
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
        [DataMember]
        [DefaultValue(typeof(Color), DefaultBackColorName)]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_BackColor)]
        [LocalizedDescription(ResourceStrings.Property_Description_BackColor)]
        public virtual Color BackColor 
        {
            get
            {
                return Component.BackColor;
            }
            set
            {
                _backColor = value;

                DoActionOnComponent(() => Component.BackColor = value);
            }
        }

        /// <summary>
        ///  Gets or sets the foreground color of the control. 
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DataMember]
        [DefaultValue(typeof(Color), DefaultForeColorName)]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_ForeColor)]
        [LocalizedDescription(ResourceStrings.Property_Description_ForeColor)]
        public virtual Color ForeColor
        {
            get
            {
                return Component.ForeColor;
            }
            set
            {
                _foreColor = value;

                DoActionOnComponent(() => Component.ForeColor = value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text on the control. 
        /// </summary>
        [DataMember]
        [DefaultValue(HorizontalAlignment.Left)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<HorizontalAlignment>))]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_TextAlign)]
        [LocalizedDescription(ResourceStrings.Property_Description_TextAlign)]
        public virtual HorizontalAlignment TextAlign { get; set; }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(XmlWriter writer)
        {
            // "disabled" attribute
            if (!Enabled)
            {
                writer.WriteAttributeString("disabled", "disabled");
            }

            // "class" attribute
            if (!string.IsNullOrEmpty(Css))
            {
                writer.WriteAttributeString("class", Css);
            }

            string customStyle = GetCustomStyle();

            // "style" attribute
            writer.WriteAttributeString("style", customStyle);
        }

        /// <summary>
        /// Get custom css inline style
        /// </summary>
        /// <param name="autoFit">
        /// A value indicating whether the control's html width and height should be calculated by browser.
        /// </param>
        /// <returns>Custom css inline style</returns>
        protected virtual string GetCustomStyle(bool autoFit = false) 
        {
            string widthString = autoFit ? "auto" : string.Format("{0}pt", Width);
            string heightString = autoFit ? "auto" : string.Format("{0}pt", Height);

            string style = string.Format(StyleFormat, ForeColor.ToString("RGB"), BackColor.ToString("RGB"),
                TextAlign.ToString().ToLower(), Font.ToString("W"), heightString, widthString);

            return style; 
        }

        /// <summary>
        /// Set the control's custom properties when it is attaching to a document.
        /// </summary>
        /// <param name="component">The component to set property.</param>
        protected override void SetAttachingComponentProperties(Control component)
        {
            Font = _font;
            BackColor = _backColor;
            ForeColor = _foreColor;

            base.SetAttachingComponentProperties(component);
        }
    }
}
