using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;
using Pdw.FormControls.Properties;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    /// Represents a list control to display a list of items.
    /// </summary>
    [DataContract]
    [KnownType(typeof(TemplateItem))]
    public abstract class ListBox : VisibleControl, ISelectable
    {
        /// <summary>
        /// Value field for property "Column". 
        /// </summary>
        private int _column = 0;

        /// <summary>
        /// Value field for property "Width". 
        /// </summary>
        private float _width;

        /// <summary>
        /// A value indicating whether user entered a number of items.
        /// </summary>
        private bool _isCustomized = false;

        /// <summary>
        /// Value field for property "DataBind". 
        /// </summary>
        private bool _dataBind;

        /// <summary>
        /// Value field for property "Items". 
        /// </summary>
        private TemplateItemCollection _items = new TemplateItemCollection();

        /// <summary>
        /// Temporary items for display.
        /// </summary>
        private TemplateItemCollection _tempItems = new TemplateItemCollection();

        /// <summary>
        ///  The distance, in pixels, between the left edge of the item and the left edge of control's client area.
        /// </summary>
        private const int ItemControlLeft = 2;

        /// <summary>
        ///  The distance, in pixels, between the top edge of the item and the top edge of control's client area.
        /// </summary>
        private const int ItemControlTop = 20;

        /// <summary>
        /// The default width of the control.
        /// </summary>
        private const float DefaultWidth = 92.25F;

        /// <summary>
        /// The template item count of each column.
        /// </summary>
        protected const int TemplateItemCountPerColumn = 3;

        /// <summary>
        /// Ctor
        /// </summary>
        protected ListBox() 
        {
            Width = DefaultWidth;
        }

        /// <summary>
        /// Gets or sets the Value attribute for control's HTML object.
        /// (No effect for this control).
        /// </summary>
        [Browsable(false)]
        public override string Value { get; set; }

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
                //TODO:WIDTH LIMITATION

                if (_width != value)
                {
                    _width = value;

                    base.Width = value;

                    DoActionOnComponent(RerenderItemsWhenPropertyChanged);
                }
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
        /// Gets or sets how many columns that the control's HTML object displayed.
        /// </summary>
        [DefaultValue(0)]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Column)]
        [LocalizedDescription(ResourceStrings.Property_Description_Column)]
        public int Column 
        {
            get
            {
                return _column;
            }
            set
            {
                if (_column != value)
                {
                    _column = value;

                    DoActionOnComponent(RerenderItemsWhenPropertyChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Name attribute for each item of control's HTML object.
        /// </summary>
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_GroupName)]
        [LocalizedDescription(ResourceStrings.Property_Description_GroupName)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets the item collection of the control.
        /// </summary>
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [TypeConverter(typeof(TemplateItemCollectionConverter))]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Items)]
        [LocalizedDescription(ResourceStrings.Property_Description_Items)]
        public TemplateItemCollection Items 
        {
            get
            {
                return _items;
            }
        }

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
                //if DataBind property changed
                if (_dataBind != value)
                {
                    _dataBind = value;

                    if (!_dataBind)
                    {
                        DataBindingSelectPath = string.Empty;
                        DataBindingSelectKey = string.Empty;
                    }

                    //generate default template items when there is no user customized data.
                    if (_dataBind || Items.Count == 0)
                    {
                        Items.Clear();

                        CreateDefaultTemplateItems();

                        RenderItems(Component, _tempItems);
                    }

                    base.DataBind = value;
                }
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
        /// Gets control's html object type.
        /// </summary>
        protected override HtmlTagType TagType
        {
            get { return HtmlTagType.Div; }
        }

        /// <summary>
        /// Gets checkable control's html input object type.
        /// </summary>
        protected abstract InputTagType InputType { get; }

        /// <summary>
        /// Gets the caption for template item.
        /// </summary>
        protected virtual string TemplateItemDisplayContent
        {
            get
            {
                return _dataBind ? Resources.ListControl_TemplateItemName_DataBind : Resources.ListControl_TemplateItemName;
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
            //adds a panel to document
            Panel control = new Panel();
            ControlSite controlSite = collection.AddControl(control, range, width, height, ObjectId);

            shape = controlSite.InlineShape;

            //if there is no list item in the item collection then attach a new list box to document
            if (Items.Count == 0)
            {
                CreateDefaultTemplateItems();

                RenderItems(control, _tempItems);
            }
            else
            {
                RenderItems(control, Items, false);
            }

            Items.TemplateItemsChanged += new EventHandler(Items_TemplateItemsChanged);

            return control;
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

            //<span style="float:left">
            writer.WriteStartElement("span");
            writer.WriteAttributeString("style", "float:left");

            //<input type="checkbox" name=""  disabled="?">
            writer.WriteStartElement("input");
            writer.WriteAttributeString("type", InputType.ToString().ToLower());
            writer.WriteAttributeString("name", GroupName);

            if (!Enabled)
            {
                writer.WriteAttributeString("disabled", "disabled");
            }

            //<xsl:attribute name="value">
            writer.WriteStartXslAttributeElement("value");

            //<xsl:value-of select="$value" />  - "Checkbox's Value"
            writer.WriteXslValueOfElement("$value");

            //</xsl:attribute>
            writer.WriteEndElement();

            //<xsl:for-each select="xpath">
            writer.WriteStartXslForEachElement(selectNodePath);

            //<xsl:if test="$value = $selected">
            writer.WriteStartXslIfElement(string.Format("$value = {0}", selectPath));

            //<xsl:attribute name="checked" />  - "Checkbox Checked"
            writer.WriteStartXslAttributeElement("checked");
            writer.WriteEndElement();

            //</xsl:if>
            writer.WriteEndElement();
            //</xsl:for-each>
            writer.WriteEndElement();
            //</input>
            writer.WriteEndElement();

            //<xsl:value-of select="$caption"/> - "Checkbox's Caption"
            writer.WriteXslValueOfElement("$caption");

            //</span>
            writer.WriteEndElement();

            //line feed
            if (Column > 0)
            {
                //<xsl:if test="position() mod Column = 0">
                writer.WriteStartXslIfElement(string.Format("position() mod {0} = 0", Column));

                //<br />
                writer.WriteStartElement("br");
                writer.WriteEndElement();

                //<div style="clear:both"/>
                writer.WriteStartElement("div");
                writer.WriteAttributeString("style", "clear:both");
                writer.WriteEndElement();

                //</xsl:if>
                writer.WriteEndElement();
            }
            
            //</xsl:for-each>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Performs additional custom processes when writing inner node of the html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected override void OnWriteInnerTemplate(System.Xml.XmlWriter writer)
        {
            //if there is no data bound to control
            if (!DataBind)
            {
                //render html object for each list item
                for (int index = 0; index < Items.Count; index++)
                {
                    TemplateItem item = Items[index];

                    //<span style="float:left">
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("style", "float:left");

                    //<input type="InputType", name="GroupName", value="Value">
                    writer.WriteStartElement("input");
                    writer.WriteAttributeString("type", InputType.ToString().ToLower());
                    writer.WriteAttributeString("name", GroupName);
                    writer.WriteAttributeString("value", item.Value);

                    //add "checked" attribute when item is selected
                    if (item.Selected)
                    {
                        writer.WriteStartAttribute("checked");
                        writer.WriteEndAttribute();
                    }

                    //</input>
                    writer.WriteEndElement();

                    writer.WriteString(item.Caption);

                    //</span>
                    writer.WriteEndElement();

                    //line feed
                    if (Column > 0 && (index + 1) % Column == 0)
                    {
                        //<br />
                        writer.WriteStartElement("br");
                        writer.WriteEndElement();

                        //<div style="clear:both"></div>
                        writer.WriteStartElement("div");
                        writer.WriteAttributeString("style", "clear:both");
                        writer.WriteFullEndElement();
                    }
                }
            }
            else
            {
                //clearup user customized data when data bound
                Items.Clear();
            }
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
            return base.GetCustomStyle() + "float:left;";
        }

        /// <summary>
        /// Get a template control which be generated using a specified template item.
        /// </summary>
        /// <param name="index">The index of specified template item in its collection</param>
        /// <param name="item">A specified template item used for make a template control</param>
        /// <returns>Control which will be displayed in the ListBox</returns>
        protected abstract Control GetTemplateItemControl(int index, TemplateItem item);

        /// <summary>
        /// Get the width of the default item template's caption.
        /// </summary>
        /// <param name="content">Content to get its width.</param>
        /// <returns>The width of specified content.</returns>
        protected virtual float GetDefaultTemplateItemControlWidth(string content)
        {
            float width = 0;

            //put the content to a checkbox to calculate the width
            using (var control = new System.Windows.Forms.CheckBox())
            {
                control.Text = content;
                control.AutoSize = true;

                width = control.PreferredSize.Width;
            }

            return width;
        }

        /// <summary>
        /// Reander list items to list box control.
        /// </summary>
        /// <param name="component">List box control.</param>
        /// <param name="itemsToRender">Specified list box items.</param>
        /// <param name="setHeight">A value indicating whether calculates control's height by item's position.</param>
        private void RenderItems(Control component, TemplateItemCollection itemsToRender, bool setHeight = true)
        {
            int itemCount = itemsToRender.Count;
            int itemIndex = 0;
            int rowIndex = 0;
            int breakTimes = 0;

            component.Controls.Clear();

            while (itemIndex < itemCount)
            {
                int left = ItemControlLeft;
                int top = rowIndex * ItemControlTop;
                int index = 0;

                do
                {
                    Control control = GetTemplateItemControl(itemIndex, itemsToRender[itemIndex]);

                    if (left + control.PreferredSize.Width < Width || Column == 1)
                    {
                        component.Controls.Add(control);

                        control.Top = top;
                        control.Left = left;

                        left += control.Width;

                        itemIndex++;
                        index++;
                    }
                    else
                    {
                        component.Controls.Remove(control);
                        breakTimes++;
                        break;
                    }
                } while (itemIndex < itemCount && (Column == 0 || index < Column));

                rowIndex++;

                if (breakTimes > itemCount)
                {
                    break;
                }
            }

            if (setHeight)
            {
                component.Height = component.Controls[itemCount - 1].Bottom;
            }
        }

        /// <summary>
        /// Control item's TemplateItemsChanged event handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An System.EventArgs that contains no event data.</param>
        private void Items_TemplateItemsChanged(object sender, EventArgs e)
        {
            Component.Controls.Clear();

            RenderItems(Component, Items);

            //make a mark as there is user customized data in the control.
            _isCustomized = true;
        }

        /// <summary>
        /// Create default template items
        /// </summary>
        private void CreateDefaultTemplateItems()
        {
            //default template item content
            string content = TemplateItemDisplayContent;

            //content width
            float width = GetDefaultTemplateItemControlWidth(content);

            //the maximum of the column displayed in the control
            int maxColumn = (int)(DefaultWidth / width);

            if (maxColumn == 0)
            {
                maxColumn = 1;
            }

            //template items total count
            int count = Column < maxColumn && Column != 0 ? TemplateItemCountPerColumn * Column : TemplateItemCountPerColumn * maxColumn;

            _tempItems.Clear();

            for (int i = 0; i < count; i++)
            {
                _tempItems.Add(new TemplateItem { Caption = content });
            }
        }

        /// <summary>
        /// Rerender list items when list box control's some property changed.
        /// </summary>
        private void RerenderItemsWhenPropertyChanged()
        {
            TemplateItemCollection items = null;

            //if there is no user data then show the default template items
            //else show the user data.
            if (!_isCustomized)
            {
                CreateDefaultTemplateItems();

                items = _tempItems;
            }
            else
            {
                items = Items;
            }

            RenderItems(Component, items);
        }
    }
}
