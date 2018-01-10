using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    ///  Represents a select control, allows the user to select an item or items from a drop-down list.
    /// </summary>
    [DataContract]
    [KnownType(typeof(TemplateItem))]
    public class Select : VisibleControl, ISelectable
    {
        /// <summary>
        /// The default display member of the control.
        /// </summary>
        private const string DefaultDisplayMember = "Caption";

        /// <summary>
        /// The default value member of the control.
        /// </summary>
        private const string DefaultValueMember = "Value";

        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 99.75F;

        /// <summary>
        /// The default height of the control.
        /// </summary>
        private const float DefaultHeight = 15.75F;

        /// <summary>
        /// Value field for property "Options". 
        /// </summary>
        private TemplateItemCollection _options = new TemplateItemCollection();

        /// <summary>
        /// Ctor
        /// </summary>
        internal Select() 
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple options can be 
        /// selected at once for control's HTML object.
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Multiple)]
        [LocalizedDescription(ResourceStrings.Property_Description_Multiple)]
        public bool Multiple { get; set; }

        /// <summary>
        /// Gets or sets the Value attribute for control's HTML object.
        /// (No effect for this control).
        /// </summary>
        [Browsable(false)]
        public override string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether binds a data source to control.
        /// </summary>
        public override bool DataBind
        {
            get
            {
                return base.DataBind;
            }
            set
            {
                if (!value)
                {
                    DataBindingSelectPath = string.Empty;
                    DataBindingSelectKey = string.Empty;
                }

                base.DataBind = value;
            }
        }

        /// <summary>
        /// Gets or sets a well-formed data binding profile of selected field for the control.
        /// </summary>
        [DataMember]
        [ReadOnly(true)]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_BindingSelectPath)]
        [LocalizedDescription(ResourceStrings.Property_Description_BindingSelectPath)]
        public string DataBindingSelectPath { get; set; }

        /// <summary>
        /// Gets or sets a key associate with the selected data binding field.
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public string DataBindingSelectKey { get; set; }
        
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
        /// Gets the option collection for the control. 
        /// </summary>
        [DataMember]
        [Editor(typeof(SimpleInputEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TemplateItemCollectionConverter))]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Options)]
        [LocalizedDescription(ResourceStrings.Property_Description_Options)]
        public TemplateItemCollection Items
        {
            get
            {
                return _options;
            }
        }

        /// <summary>
        /// Gets control's html object type.
        /// </summary>
        protected override HtmlTagType TagType { get { return HtmlTagType.Select; } }

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
            //adds TemplateItemsChanged event handler for user entered Options
            Items.TemplateItemsChanged += new EventHandler(Options_SelectOptionsChanged);

            //adds a comboBox control to document
            var control = collection.AddComboBox(range, width, height, ObjectId);
            
            //initialize control properties
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            control.DisplayMember = DefaultDisplayMember;
            control.ValueMember = DefaultValueMember;

            shape = control.InlineShape;

            return control;
        }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteCustomAttributes(System.Xml.XmlWriter writer)
        {
            base.OnWriteCustomAttributes(writer);

            if (Multiple)
            {
                writer.WriteStartAttribute("multiple");
                writer.WriteEndAttribute();
            }
        }

        /// <summary>
        /// Performs additional custom processes when writing Xsl data binding template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteDatabindingTemplate(System.Xml.XmlWriter writer)
        {
            //get data binding configs
            DataBinding valuesBinding = DataBindings[SharedConstants.DataBinding_DefaultKey];
            DataBinding selectBinding = DataBindings[SharedConstants.DataBinding_SelectKey];

            //get a config array which contains binding data path and its parent path for each configs
            string[] valuesSplitArray = valuesBinding.Split('/');
            string[] selectSplitArray = selectBinding.Split('/');

            //get each binding path from binding config array
            string valuesNodePath = valuesSplitArray[0];
            string valuesPath = valuesSplitArray[1];
            string selectNodePath = selectSplitArray[0];
            string selectPath = selectSplitArray[1];

            //<xsl:for-each select="xpath">
            writer.WriteStartXslForEachElement(valuesNodePath);

            //<xsl:variable name="value" select="substring-before({0}, '&#xA')" />
            string valueSelector = string.Format(SharedConstants.XpathFuncFormat_SubstringBefore, valuesPath, Spliter);
            writer.WriteStartXslVariableElement("value", valueSelector);
            writer.WriteEndElement();

            //<xsl:variable name="caption" select="substring-after({0}, '&#xA')" />
            string captionSelector = string.Format(SharedConstants.XpathFuncFormat_SubstringAfter, valuesPath, Spliter);
            writer.WriteStartXslVariableElement("caption", captionSelector);
            writer.WriteEndElement();

            //<option>
            writer.WriteStartElement("option");

            //<xsl:attribute name="value">
            writer.WriteStartXslAttributeElement("value");

            //<xsl:value-of select="$value" />  - "Option's Value"
            writer.WriteXslValueOfElement("$value");

            //</xsl:attribute>
            writer.WriteEndElement();

            //<xsl:for-each select="xpath">
            writer.WriteStartXslForEachElement(selectNodePath);

            //<xsl:if test="$value = $selected">
            writer.WriteStartXslIfElement(string.Format("$value = {0}", selectPath));

            //<xsl:attribute name="selected" />  - "Option selected"
            writer.WriteStartXslAttributeElement("selected");
            writer.WriteEndElement();

            //</xsl:if>
            writer.WriteEndElement();
            //</xsl:for-each>
            writer.WriteEndElement();

            //<xsl:value-of select="$caption"/> - "Option's Caption"
            writer.WriteXslValueOfElement("$caption");

            //</option>
            writer.WriteEndElement();

            //</xsl:for-each>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Performs additional custom processes when writing inner node of the html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteInnerTemplate(System.Xml.XmlWriter writer)
        {
            //if there is no data source bound to control then generate 
            //html option object, else clearup user entered options. 
            if (!DataBind)
            {
                foreach (TemplateItem option in Items)
                {
                    //<option>
                    writer.WriteStartElement("option");
                    writer.WriteAttributeString("value", option.Value);

                    if (option.Selected)
                    {
                        writer.WriteStartAttribute("select");
                        writer.WriteEndAttribute();
                    }

                    writer.WriteString(option.Caption);

                    //<option>
                    writer.WriteEndElement();
                }
            }
            else
            {
                Items.Clear();
            }
        }

        /// <summary>
        /// Control option's TemplateItemsChanged event handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An System.EventArgs that contains no event data.</param>
        private void Options_SelectOptionsChanged(object sender, EventArgs e)
        {
            //get control and clearup its items
            ComboBox control = Component as ComboBox;
            control.Items.Clear();

            //if there are options user entered then add them to control item collection
            if (Items.Count != 0)
            {
                //get option array
                var optionList = Items.Cast<TemplateItem>().ToArray();

                //add option array to control 
                control.Items.AddRange(optionList);

                //set the last selected option as control's selected item
                control.SelectedItem = optionList.Where(v => v.Selected).LastOrDefault();
            }
        }
    }
}
