using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Word = Microsoft.Office.Interop.Word;
using System.Linq;
using System.Linq.Expressions;

namespace Pdw.FormControls
{
    /// <summary>
    /// The base class for Pdw form controls.
    /// </summary>
    [DataContract]
    [KnownType(typeof(DataBinding))]
    public abstract partial class ControlBase : IDisposable
    {
        #region Constants

        /// <summary>
        /// The config string of the build-in StringCollectionEditor.
        /// </summary>
        protected const string BuildInStringCollectionEditor = "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        /// <summary>
        /// The spliter of the key-value DataBinding data.
        /// </summary>
        protected const char Spliter = '\n';

        /// <summary>
        /// The prefix string of ObjectId
        /// </summary>
        private const string ObjectIdPrefix = "pdm_";

        /// <summary>
        /// The prefix string of BookmarkName
        /// </summary>
        private const string BookmarkNamePrefix = "bm_";

        #endregion

        #region Fields

        /// <summary>
        /// Value field for property "ObjectId".
        /// </summary>
        private string _objectId;

        /// <summary>
        /// Value field for property "BookmarkName". 
        /// </summary>
        private string _bookmarkName;

        /// <summary>
        /// Value field for property "DataBind". 
        /// </summary>
        private bool _dataBind;

        /// <summary>
        /// Value field for property "Width". 
        /// </summary>
        private float _width = 100;

        /// <summary>
        /// Value field for property "Height".
        /// </summary>
        private float _height = 30;

        /// <summary>
        /// The control's inline shape object in the document.
        /// </summary>
        private Word.InlineShape _shape;

        /// <summary>
        /// Value field for property "Enable". 
        /// </summary>
        private bool _enable = true;

        /// <summary>
        /// Value field for property "Scripts".
        /// </summary>
        private List<string> _scripts = new List<string>();

        /// <summary>
        /// Value field for property "DataBindings".
        /// </summary>
        private DatabindingCollection _bindings = new DatabindingCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Gets control's displayed component in the document. 
        /// </summary>
        [Browsable(false)]
        public Control Component { get; private set; }

        /// <summary>
        /// Gets the Id of the ActiveX object which associate with the control in the document.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public string ObjectId
        {
            get
            {
                if (string.IsNullOrEmpty(_objectId))
                {
                    _objectId = string.Format(SharedConstants.SimpleStringConcatFormat, ObjectIdPrefix, Guid.NewGuid().ToString("N"));
                }

                return _objectId;
            }
            private set
            {
                _objectId = value;
            }
        }

        /// <summary>
        /// Gets or sets the Id attribute for control's HTML object.
        /// </summary>
        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Id)]
        [LocalizedDescription(ResourceStrings.Property_Description_Id)]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name attribute for control's HTML object.
        /// </summary>
        [DataMember]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Name)]
        [LocalizedDescription(ResourceStrings.Property_Description_Name)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the Value attribute for control's HTML object.
        /// </summary>
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Value)]
        [LocalizedDescription(ResourceStrings.Property_Description_Value)]
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether binds a data source to control.
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_DataBind)]
        [LocalizedDescription(ResourceStrings.Property_Description_DataBind)]
        public virtual bool DataBind
        {
            get
            {
                return _dataBind;
            }
            set
            {
                _dataBind = value;

                if (!_dataBind)
                {
                    DataBindingPath = string.Empty;
                    DataBindingKey = string.Empty;
                }
                else
                {
                    Value = string.Empty;
                }

                if (DataDindPropertyChanged != null)
                {
                    DataDindPropertyChanged(this, new PropertyChangedEventArgs("DataBind"));
                }
            }
        }

        /// <summary>
        /// Gets the data bindings for the control.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public virtual DatabindingCollection DataBindings { get { return _bindings; } }

        /// <summary>
        /// Gets or sets a well-formed data binding profile for the control.
        /// </summary>
        [DataMember]
        //[Editor(typeof(SimpleInputEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(EmptyStringConverter))]
        [LocalizedCategory(ResourceStrings.Property_Category_Data)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_BindingPath)]
        [LocalizedDescription(ResourceStrings.Property_Description_BindingPath)]
        public virtual string DataBindingPath { get; set; }

        /// <summary>
        /// Gets or sets a user data associate with the data binding field.
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public string DataBindingKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control's HTML object can respond to user interaction.
        /// </summary>
        /// <remarks>
        /// Do not read this property unless the control is attached to a document.
        /// </remarks>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when try to get this property while the Component is not attached to a document.
        /// </exception>
        [DataMember]
        [DefaultValue(true)]
        [TypeConverter(typeof(LocalizedSimpleTypeConverter<Boolean>))]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Enabled)]
        [LocalizedDescription(ResourceStrings.Property_Description_Enabled)]
        public virtual bool Enabled
        {
            get
            {
                return Component.Enabled;
            }
            set
            {
                _enable = value;

                DoActionOnComponent(() => Component.Enabled = value);
            }
        }

        /// <summary>
        /// Gets the name of the bookmark which associate with the control's location in the document.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        internal string BookmarkName
        {
            get
            {
                if (string.IsNullOrEmpty(_bookmarkName))
                {
                    _bookmarkName = string.Format(SharedConstants.SimpleStringConcatFormat, BookmarkNamePrefix, ObjectId);
                }

                return _bookmarkName;
            }
            private set
            {
                _bookmarkName = value;
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
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Width)]
        [LocalizedDescription(ResourceStrings.Property_Description_Width)]
        public virtual float Width
        {
            get
            {
                return _shape.Width;
            }
            set
            {
                _width = value;

                if (_shape != null)
                {
                    _shape.Width = _width;
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
        [DataMember]
        [LocalizedCategory(ResourceStrings.Property_Category_Appearance)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_Height)]
        [LocalizedDescription(ResourceStrings.Property_Description_Height)]
        public virtual float Height
        {
            get
            {
                return _shape.Height;
            }
            set
            {
                _height = value;

                if (_shape != null)
                {
                    _shape.Height = _height;
                }
            }
        }

        /// <summary>
        /// Gets or sets the JavaScript associate with the control.
        /// </summary>
        [DataMember]
        [TypeConverter(typeof(ListDefaultConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(BuildInStringCollectionEditor, typeof(UITypeEditor))]
        [LocalizedCategory(ResourceStrings.Property_Category_Behavior)]
        [LocalizedDisplayName(ResourceStrings.Property_Name_JavaScript)]
        [LocalizedDescription(ResourceStrings.Property_Description_JavaScript)]
        public virtual List<string> JavaScript
        {
            get
            {
                return _scripts;
            }
            set
            {
                _scripts = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control's component is exist in the document.
        /// </summary>
        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                bool valid = true;

                if (_shape != null)
                {
                    try
                    {
                        _shape.GetType();
                    }
                    catch 
                    {
                        valid = false;
                    }
                }

                return valid;
            }
        }

        /// <summary>
        /// Gets control's html object type.
        /// </summary>
        protected abstract HtmlTagType TagType { get; }

        #endregion

        #region Release Resources

        /// <summary>
        /// Releases all resources used by the Component.
        /// </summary>
        public void Dispose()
        {
            if (Component != null)
            {
                Component.Dispose();
                Component = null;
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialize the control to XML.
        /// </summary>
        /// <returns>XML string</returns>
        internal string ToXml()
        {
            string xml = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                //initialize a serializer for current control type
                DataContractSerializer serializer = new DataContractSerializer(GetType());

                //serialize process
                serializer.WriteObject(stream, this);

                //get string content from stream format
                xml = Encoding.UTF8.GetString(stream.ToArray());
            }

            return xml;
        }

        #endregion

        #region Generate XSL

        /// <summary>
        /// Saves the control's content as a xsl template to the specified XmlWriter.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        public void WriteToXslTemplate(XmlWriter writer)
        {
            //<tag>
            writer.WriteStartElement(TagType.ToString().ToLower());

            //"id" attribute
            if (!string.IsNullOrEmpty(Id))
            {
                writer.WriteAttributeString("id", Id);
            }

            //"name" attribute
            if (!string.IsNullOrEmpty(Name))
            {
                writer.WriteAttributeString("name", Name);
            }

            OnWriteCustomAttributes(writer);

            if (DataBind)
            {
                //if the control is set to bind a data source while there is no 
                //data-binding config then throw an exception
                if (DataBindings == null || DataBindings.Count == 0)
                {
                    //TODO:EXCEPTION TYPE
                    throw new NullReferenceException();
                }

                OnWriteDatabindingTemplate(writer);
            }

            OnWriteInnerTemplate(writer);

            //</tag>
            writer.WriteEndElement();

            OnWriteToXslTemplateComplete(writer);
        }

        /// <summary>
        /// Performs additional custom processes when writing customized attributes for html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected virtual void OnWriteCustomAttributes(XmlWriter writer) { }

        /// <summary>
        /// Performs additional custom processes when writing Xsl data binding template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected virtual void OnWriteDatabindingTemplate(XmlWriter writer) { }

        /// <summary>
        /// Performs additional custom processes when writing inner node of the html object in xsl template.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected virtual void OnWriteInnerTemplate(XmlWriter writer) { }

        /// <summary>
        /// Performs additional custom processes when write to Xsl template finished.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save xsl template.</param>
        protected virtual void OnWriteToXslTemplateComplete(XmlWriter writer) { }

        #endregion

        #region Attachment

        /// <summary>
        /// Attach the control to a specified document's control collection.
        /// </summary>
        /// <param name="collection">The control collection to hold the control.</param>
        /// <param name="range">The location of the control.</param>
        internal void AttachTo(ControlCollection collection, Word.Range range) 
        {
            Component = Attach(collection, range, _width, _height, out _shape);

            SetAttachingComponentProperties(Component);

            //set control's ActiveX object name, will be rendered as 
            //<object id="ObjectId"> when document save as a html file
            _shape.OLEFormat.Object.Name = ObjectId;

            //set the current insertion point following the attaching component
            _shape.Application.Selection.SetRange(_shape.Range.End, _shape.Range.End);

            //make relationship between the control and its component in document 
            Component.Tag = this;
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
        protected abstract Control Attach(ControlCollection collection, Word.Range range, float width, float height, out Word.InlineShape shape);

        /// <summary>
        /// Set the control's custom properties when it is attaching to a document.
        /// </summary>
        /// <param name="component">The component to set property.</param>
        protected virtual void SetAttachingComponentProperties(Control component) 
        {
            Enabled = _enable;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Performs the specified action on component when component is not null.
        /// </summary>
        /// <param name="action">
        /// The System.Action delegate to perform on component.
        /// </param>
        protected void DoActionOnComponent(System.Action action)
        {
            //if the Component is not null then do the actoin
            if (Component != null)
            {
                action();
            }
        }

        #endregion
    }
}
