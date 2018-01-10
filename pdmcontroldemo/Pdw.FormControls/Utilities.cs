using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using mshtml;
using Pdw.FormControls.Design;
using Pdw.FormControls.Extension;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    /// Utility class for Form Controls' process in the document.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// The XML element name of stored controls in the Custom Xml Parts. 
        /// </summary>
        private const string FormControlXmlRootElementName = "Controls";

        /// <summary>
        /// The property name of XmlPart's Id for serialized controls.
        /// </summary>
        private const string FormControlXmlPartIdPropertyName = "Pronto_FormControlXmlPartId";

        /// <summary>
        /// 
        /// </summary>
        private const string XmlPartContentNodePath = "//Content";

        private delegate string BookmarkProcessHandler(string bookmarkName, out bool isControlBookmark); 

        public static T AddControl<T>(this ControlCollection source, Word.Range range, float width = default(float), float height = default(float)) where T : ControlBase
        {
            if (range.Start == 0)
            {
                //TODO:EXCEPTION TYPE
                throw new NotSupportedException();
            }

            Type type = typeof(T);

            if (type.IsAbstract)
            {
                //TODO:EXCEPTION TYPE
                throw new Exception();
            }

            T formControl = (T)Activator.CreateInstance(type, true);

            AddControl(formControl, source, range, width, height);

            return formControl;
        }

        public static void RestoreControls(this Document doc)
        {
            CustomXMLPart xmlPart = GetCustomXmlPart(doc, FormControlXmlPartIdPropertyName, false, false);

            if (xmlPart == null)
            {
                //TODO: EXCEPTION TYPE
                throw new ArgumentNullException();
            }

            var bookmarks = doc.Bookmarks.Cast<Word.Bookmark>().Select(b =>
                new
                {
                    Name = b.Name, Range = b.Range
                });

            XmlNode root = GetSpecificCustomXmlRootNode(xmlPart);

            if ((bookmarks.Count() == 0 && root.ChildNodes.Count != 0) ||
                (bookmarks.Count() != 0 && root.ChildNodes.Count == 0))
            {
                //TODO: EXCEPTION TYPE
                throw new ArgumentException();
            }

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            foreach (XmlNode node in root.ChildNodes)
            {
                RecreateFormControl(doc, node.Name, node.OuterXml, executingAssembly, bookmarks);
            }
        }

        public static void SaveTemplate(this Document doc)
        {
            SaveControls(doc);

            doc.SaveEncoding = MsoEncoding.msoEncodingUTF8;
            doc.Save();

            doc.Controls.GetValidItems().Each(c => doc.Bookmarks[c.BookmarkName].Delete());

            doc.Saved = true;
        }

        public static IEnumerable<ControlBase> GetDataBoundItems(this ControlCollection source)
        {
            return source.GetValidItems().Where(c => c.DataBind);
        }

        public static IEnumerable<ControlBase> GetValidItems(this ControlCollection source)
        {
            return source.OfType<Control>().Where(c => c.Tag is ControlBase).Select(c => c.Tag as ControlBase).Where(c => c.IsValid);
        }

        private static void SaveControls(Document doc)
        {
            string xml = GenerateXmlForFormControls(doc);

            xml = FormatCustomXmlContent(xml, ContentType.FormControls);

            InsertCustomXml(doc, xml, FormControlXmlPartIdPropertyName);
        }
        
        private static string GenerateXmlForFormControls(Document doc)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = SharedConstants.UTF8Encoding }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(FormControlXmlRootElementName);

                    foreach (ControlBase control in doc.Controls.GetValidItems())
                    {
                        //TODO:doc.Controls.GetInlineShapeForControl NULL EXCEPTION
                        Word.Range range = doc.Controls.GetInlineShapeForControl(control.Component).Range;

                        if (range.Start < 0)
                        {
                            //TODO:EXCEPTION TYPE
                            throw new NotSupportedException();
                        }

                        range.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                        doc.Bookmarks.Add(control.BookmarkName, range);

                        writer.WriteRaw(control.ToXml());
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                return SharedConstants.UTF8Encoding.GetString(stream.ToArray());
            }
        }

        private static CustomXMLPart InsertCustomXml(Document doc, string xml, string propertyName)
        {
            CustomXMLPart xmlpart = GetCustomXmlPart(doc, propertyName);

            bool result = xmlpart.LoadXML(xml);

            if (result)
            {
                DocumentProperties propertySet = doc.CustomDocumentProperties;
                propertySet.Add(propertyName, false, MsoDocProperties.msoPropertyTypeString, xmlpart.Id);
            }
            else
            {
                //TODO:FAILED PROCESS
            }

            return xmlpart;
        }

        private static CustomXMLPart GetCustomXmlPart(Document doc, string xmlPartIdName, bool createNew = true, bool clearContent = true)
        {
            string xmlPartId = GetCustomProperty(doc, xmlPartIdName);
            CustomXMLPart xmlPart = null;

            if (string.IsNullOrEmpty(xmlPartId))
            {
                if (createNew)
                {
                    xmlPart = doc.CustomXMLParts.Add();
                }
            }
            else
            {
                xmlPart = doc.CustomXMLParts.SelectByID(xmlPartId);

                if (xmlPart == null)
                {
                    if (createNew)
                    {
                        xmlPart = doc.CustomXMLParts.Add();
                    }
                }
                else if (clearContent)
                {
                    xmlPart.Delete();
                    xmlPart = doc.CustomXMLParts.Add();
                }
            }

            return xmlPart;
        }

        private static string GetCustomProperty(Document doc, string propertyName)
        {
            string targetProperty = null;

            if (!string.IsNullOrEmpty(propertyName))
            {
                foreach (DocumentProperty property in doc.CustomDocumentProperties)
                {
                    if (property.Name.Is(propertyName))
                    {
                        targetProperty = property.Value;
                        break;
                    }
                }
            }

            return targetProperty;
        }

        private static void RecreateFormControl(Document doc, string controlName, string controlXml, Assembly formControlsAssembly, IEnumerable<dynamic> bookmarks)
        {
            string typeName = string.Format(SharedConstants.TypeFullNameFormat, formControlsAssembly.GetName().Name, controlName);

            Type controlType = formControlsAssembly.GetType(typeName);

            using (MemoryStream stream = new MemoryStream(SharedConstants.UTF8Encoding.GetBytes(controlXml)))
            {
                using (XmlReader nodeReader = XmlReader.Create(stream))
                {
                    DataContractSerializer serializer = new DataContractSerializer(controlType);

                    ControlBase control = serializer.ReadObject(nodeReader, true) as ControlBase;

                    var bookmark = bookmarks.Single(b => ((string)b.Name).Is(control.BookmarkName));

                    int start = (int)bookmark.Range.Start;

                    Word.Range rangeToInsert = doc.Range(start + 1, start + 1);

                    AddControl(control, doc.Controls, rangeToInsert);

                    doc.Bookmarks[bookmark.Name].Delete();
                }
            }
        }

        private static string FormatCustomXmlContent(string content, ContentType type)
        {
            XmlObject formatContent = new XmlObject(content, type);

            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = SharedConstants.UTF8Encoding }))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(XmlObject));
                    serializer.Serialize(writer, formatContent);
                }

                return SharedConstants.UTF8Encoding.GetString(stream.ToArray());
            }
        }

        private static XmlNode GetSpecificCustomXmlRootNode(CustomXMLPart xmlPart)
        {
            if (xmlPart == null)
            {
                throw new ArgumentNullException();
            }

            CustomXMLNode xmlNode = xmlPart.SelectSingleNode(XmlPartContentNodePath);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlNode.Text);

            XmlNode root = xmlDoc.SelectNodes("/*")[0];

            return root;
        }

        private static void AddControl(ControlBase control, ControlCollection collection, Word.Range range, float width = default(float), float height = default(float))
        {
            control.AttachTo(collection, range);

            if (width != default(float))
            {
                control.Width = width;
            }

            if (height != default(float))
            {
                control.Height = height;
            }
        }
    }
}
