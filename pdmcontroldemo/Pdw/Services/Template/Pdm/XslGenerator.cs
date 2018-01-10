using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using mshtml;
using Pdw.Core;
using Pdw.FormControls;
using Pdw.FormControls.Extension;
using ProntoDoc.Framework.CoreObject;
using VstoWord = Microsoft.Office.Tools.Word;
using Wkl = Pdw.WKL.DataController.MainController;
using Word = Microsoft.Office.Interop.Word;
using System;
using Pdw.Properties;

namespace Pdw.Services.Template.Pdm
{
    public class XslGenerator : Base.BaseXslGenerator
    {
        private List<PartBookmark> _bookmarks;
        private string _htmlFileName;

        private const string ValueBindingKey = "value";
        private const string SelectBindingKey = "select";
        private const string WordXmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private const string ClassId = "CLSID:A37BBB42-E8C1-4E09-B9CA-F009CE620C08";
        
        private const string Form_Action = "/PRONTOMOBILE";
        private const string Form_Method = "get";
        private const string Hidden_Url_Name = "ProntoActuallyUrl";
        private const string Hidden_Object_Name = "ObjectName";
        private const string Hidden_Url_Value = "pdx_pdm_posturl";
        private const string Hidden_Object_Value = "pdx_pdm_objectname";

        private const string Format_Base64ImageSrc = "data:image/{0};base64,{1}";

        private const string Wartermark_CssClass = "watermark";
        private const string Wartermark_Tag = "div";

        private static readonly XName WordXmlBodyName = null;
        private static readonly XName WordXmlStartBookmarkName = null;
        private static readonly XName WordXmlNameAttribute = null;

        private static readonly string XslRootPath = null;
        private static readonly string WartermarkStyle = null;
        private static readonly Encoding UTF8Encoding = null;

        static XslGenerator()
        {
            WordXmlBodyName = XName.Get("body", WordXmlNamespace);
            WordXmlStartBookmarkName = XName.Get("bookmarkStart", WordXmlNamespace);
            WordXmlNameAttribute = XName.Get("name", WordXmlNamespace);

            XslRootPath = string.Format("/{0}", FrameworkConstants.PdmDataRootName);
            UTF8Encoding = new UTF8Encoding(false);

            WartermarkStyle = Resources.Pdm_WartermarkStyle;
        }

        public XslGenerator(InternalBookmark ibookmark, string htmlFileName)
        {
            _isAddSection = Wkl.MainCtrl.CommonCtrl.CommonProfile.IsAddSection;

            _htmlFileName = htmlFileName;
            _ibm = ibookmark;
            _foreach = new List<ForeachItem>();
        }

        public string GenXsl()
        {
            Word.Document doc = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;

            VstoWord.Document vstoDoc = Globals.Factory.GetVstoObject(doc);

            HTMLDocument htmlDoc = ReadHtmlDocument(_htmlFileName);

            PrepareBookmark();

            string xsl = SaveAsXsl(htmlDoc, vstoDoc);

            return xsl;
        }

        private HTMLDocument ReadHtmlDocument(string fileName)
        {
            HTMLDocument htmlDoc = null;

            using (StreamReader reader = new StreamReader(fileName, UTF8Encoding))
            {
                string html = reader.ReadToEnd();

                htmlDoc = new HTMLDocument();

                IHTMLDocument2 docToWritein = htmlDoc as IHTMLDocument2;

                docToWritein.write(new object[] { html });
            }

            return htmlDoc;
        }

        public string SaveAsXsl(HTMLDocument htmlDoc, VstoWord.Document doc)
        {
            string content = null;

            try
            {
                if (htmlDoc.hasChildNodes())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (XmlWriter xslWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = UTF8Encoding }))
                        {
                            //<?xml version="1.0" encoding = "utf-8" standalone="yes"?> 
                            //<xsl:stylesheet version="1.0"> 
                            xslWriter.WriteStartXsltDocument();

                            bool indent = false;

#if DEBUG
                            indent = true;
#endif

                            //<xsl:output method="xml" indent="yes" /> 
                            xslWriter.WriteXslOutputElement(indent);

                            //<xsl:template match="root path"> TODO:PdwDataRootName
                            xslWriter.WriteStartXslTemplateElement(XslRootPath);

                            IEnumerable<ControlBase> controls = doc.Controls.GetValidItems();

                            GenerateWatermarkElement(htmlDoc);

                            //TODO:CONFIRM THIS FUNCTION
                            CreateHtmlTemplate(xslWriter, htmlDoc.firstChild, controls);

                            //<xsl:template />
                            xslWriter.WriteEndElement();

                            //<xsl:stylesheet />
                            xslWriter.WriteEndXsltDocument();
                        }

                        content = UTF8Encoding.GetString(stream.ToArray());

                        Debug.WriteLine(content);
                    }
                }
            }
            catch 
            {
                //TODO:EXCEPTION TYPE
                throw;
            }

            return content;
        }

        private void GenerateWatermarkElement(HTMLDocument htmlDoc)
        {
            IHTMLElement watermark = htmlDoc.createElement(Wartermark_Tag);
            watermark.className = Wartermark_CssClass;
            watermark.innerText = GetWatermarkContent();

            IHTMLDOMNode body = htmlDoc.body as IHTMLDOMNode;
            body.insertBefore(watermark as IHTMLDOMNode, body.firstChild);
        }

        private string GetWatermarkContent()
        {
            return "Pdm_Template_Watermark";
        }

        private void CreateHtmlTemplate(XmlWriter writer, IHTMLDOMNode htmlNode, IEnumerable<ControlBase> formControls, bool analyseParagraph = true)
        {
            //Element
            if (htmlNode.nodeType == 1)
            {
#if DEBUG
                if (analyseParagraph)
                {
                    Debug.WriteLine(string.Format("Current HTML Node:{0}.", htmlNode.nodeName));
                }
                else
                {
                    Debug.WriteLine(string.Format("Current HTML Element:{0}, analysed.", htmlNode.nodeName));
                }
#endif
                if (htmlNode is IHTMLParaElement && analyseParagraph)
                {
                    ProcessParagraphTag(writer, htmlNode, formControls);

                    return;
                }
                
                if (htmlNode is IHTMLObjectElement)
                {
                    IHTMLElement objElement = htmlNode as IHTMLElement;

                    ControlBase formControl = formControls.Single(c => c.ObjectId.Is(objElement.id));

                    SetDataBindingsForControl(formControl);

                    //Write control template
                    formControl.WriteToXslTemplate(writer);

                    return;
                }
                
                if (htmlNode is IHTMLAnchorElement)
                {
                    PartBookmark bookmark = ParseToPartBookmark(htmlNode as IHTMLElement);

                    if (bookmark != null)
                    {
                        string result = GenerateXslForBookmark(bookmark);

                        writer.WriteRaw(result);

                        return;
                    }
                }

                if (htmlNode is IHTMLImgElement)
                {
                    IHTMLImgElement image = htmlNode as IHTMLImgElement;

                    if (!string.IsNullOrEmpty(image.src))
                    {
                        string[] tempArray = image.src.Split(':');

                        string src = tempArray.Length == 2 ? tempArray[1] : image.src;

                        src = string.Format(@"{0}\{1}", Path.GetDirectoryName(_htmlFileName), src.Replace('/', '\\'));

                        image.src = GetBase64ImageSrc(src);
                    }
                }

                WriteStartHTMLElement(writer, htmlNode);

                if (htmlNode is IHTMLBodyElement)
                {
                    writer.WriteStartHtmlFormElement(Form_Method, Form_Action);

                    writer.WriteXslHtmlHiddenElement(Hidden_Url_Name, Hidden_Url_Value);
                    writer.WriteXslHtmlHiddenElement(Hidden_Object_Name, Hidden_Object_Value);
                }

                if (htmlNode is IHTMLStyleElement)
                {
                    IHTMLElement element = htmlNode as IHTMLElement;
                    writer.WriteString(element.innerHTML);
                    writer.WriteString(WartermarkStyle);
                }

                //Other tags
                foreach (IHTMLDOMNode child in htmlNode.childNodes)
                {
                    CreateHtmlTemplate(writer, child, formControls);
                }

                WriteEndHTMLElement(writer, htmlNode);
            }
            else
            {
                //Text or something else

                Debug.WriteLine(string.Format("Current HTML Node:{0}.", htmlNode.nodeName));

                //Text
                if (htmlNode.nodeType == 3)
                {
                    if (!IsRedundantSpaceNode(htmlNode))
                    {
                        writer.WriteString(htmlNode.nodeValue);
                    }
                }
            }
        }

        private void ProcessParagraphTag(XmlWriter writer, IHTMLDOMNode pNode, IEnumerable<ControlBase> formControls)
        {
            List<IHTMLDOMNode> contents = new List<IHTMLDOMNode>();
            var bookmarkMatchingStack = new Stack<KeyValuePair<IHTMLDOMNode, PartBookmark>>();

            MatchingControlBookmarks(bookmarkMatchingStack, pNode);

            if (bookmarkMatchingStack.Count == 0)
            {
                contents.Add(pNode);
            }
            else
            {
                IEnumerable<IHTMLDOMNode> docOrderNodes =  bookmarkMatchingStack.Reverse().Select(b => b.Key);
                Queue<IHTMLDOMNode> unmatchedBookmarkQueue = new Queue<IHTMLDOMNode>(docOrderNodes);
                List<IHTMLDOMNode> controlNodes = new List<IHTMLDOMNode>();

                while (unmatchedBookmarkQueue.Count != 0)
                {
                    IHTMLDOMNode controlNode = unmatchedBookmarkQueue.Peek();

                    IHTMLDOMNode wrapper = null;

                    GenerateParagraphContent(controlNode, ref wrapper, controlNodes, unmatchedBookmarkQueue, true);

                    if (wrapper != null && wrapper.hasChildNodes())
                    {
                        contents.Add(wrapper);
                    }

                    contents.AddRange(controlNodes);
                }

                if (pNode.hasChildNodes())
                {
                    contents.Add(pNode);
                }
            }

            foreach (IHTMLDOMNode content in contents)
            {
                CreateHtmlTemplate(writer, content, formControls, false);
            }
        }

        private void GenerateParagraphContent(IHTMLDOMNode node, ref IHTMLDOMNode wrapper, List<IHTMLDOMNode> controlNodes, Queue<IHTMLDOMNode> unmatchedBookmarkQueue, bool isControlNode = false)
        {
            IHTMLDOMNode parent = node.parentNode;
            IHTMLDOMNode newNode = parent.cloneNode(false);
            List<IHTMLDOMNode> discardingNodes = new List<IHTMLDOMNode>();
            bool hasProcessed = false;

            foreach (IHTMLDOMNode child in parent.childNodes)
            {
                if (child != node)
                {
                    if (child.nodeType == 3)
                    {
                        if (string.IsNullOrWhiteSpace(child.nodeValue) && unmatchedBookmarkQueue.Contains(child.nextSibling))
                        {
                            discardingNodes.Add(child);
                            continue;
                        }
                    }

                    if (isControlNode)
                    {
                        if (child.nodeType == 3 && hasProcessed)
                        {
                            break;
                        }

                        if (child.nodeType == 1)
                        {
                            IHTMLDOMNode controlNode = unmatchedBookmarkQueue.Count != 0 ? unmatchedBookmarkQueue.Peek() : null;

                            if (controlNode == child)
                            {
                                unmatchedBookmarkQueue.Dequeue();
                                controlNodes.Add(child.cloneNode(true));

                                discardingNodes.Add(child);

                                continue;
                            }
                            else if (hasProcessed)
                            {
                                break;
                            }
                        }
                    }

                    discardingNodes.Add(child);
                    newNode.appendChild(child.cloneNode(true));
                }
                else
                {
                    if (wrapper != null)
                    {
                        newNode.appendChild(wrapper);
                    }
                    {
                        discardingNodes.Add(child);
                    }

                    if (isControlNode)
                    {
                        controlNodes.Add(child.cloneNode(true));
                        unmatchedBookmarkQueue.Dequeue();
                        hasProcessed = true;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (IHTMLDOMNode child in discardingNodes)
            {
                parent.removeChild(child);
            }

            if (newNode.hasChildNodes())
            {
                wrapper = newNode;
            }

            if (!(parent is IHTMLParaElement))
            {
                GenerateParagraphContent(parent, ref wrapper, controlNodes, unmatchedBookmarkQueue);
            }
        }

        private void MatchingControlBookmarks(Stack<KeyValuePair<IHTMLDOMNode, PartBookmark>> controlMarkupStack, IHTMLDOMNode htmlNode)
        {
            IHTMLElement2 htmlElement = htmlNode as IHTMLElement2;
            IHTMLElementCollection anchors = htmlElement.getElementsByTagName("A");

            foreach (IHTMLElement anchor in anchors)
            {
                PartBookmark bookmark = ParseToPartBookmark(anchor);

                if (bookmark != null && bookmark.IsControlBookmark)
                {
                    IHTMLDOMNode node = anchor as IHTMLDOMNode;

                    if (controlMarkupStack.Count == 0)
                    {
                        controlMarkupStack.Push(new KeyValuePair<IHTMLDOMNode, PartBookmark>(node, bookmark));
                    }
                    else
                    {
                        KeyValuePair<IHTMLDOMNode, PartBookmark> previous = controlMarkupStack.Peek();

                        bool isMatch = (previous.Value.Type == BookmarkType.StartForeach && bookmark.Type == BookmarkType.EndForeach) ||
                            (previous.Value.Type == BookmarkType.StartIf && bookmark.Type == BookmarkType.EndIf);

                        isMatch = isMatch && previous.Key.parentNode == node.parentNode;

                        if (isMatch)
                        {
                            controlMarkupStack.Pop();
                        }
                        else
                        {
                            controlMarkupStack.Push(new KeyValuePair<IHTMLDOMNode, PartBookmark>(node, bookmark));
                        }
                    }
                }
            }
        }

        private void WriteStartHTMLElement(XmlWriter writer, IHTMLDOMNode htmlNode)
        {
            if (htmlNode.nodeType == 1)
            {
                writer.WriteStartElement(htmlNode.nodeName.ToLower());
                WriteTagAttributes(writer, htmlNode);
            }
        }

        private void WriteEndHTMLElement(XmlWriter writer, IHTMLDOMNode htmlNode)
        {
            if (htmlNode.nodeType == 1)
            {
                if (htmlNode is IHTMLMetaElement)
                {
                    writer.WriteEndElement();
                }
                else
                {
                    if (htmlNode is IHTMLBodyElement)
                    {
                        //close form tag
                        writer.WriteFullEndElement();
                    }

                    writer.WriteFullEndElement();
                }
            }
        }

        private void WriteTagAttributes(XmlWriter writer, IHTMLDOMNode htmlNode)
        {
            IEnumerable attributes = (htmlNode.attributes as IEnumerable).Cast<IHTMLDOMAttribute>().Where(a => a.specified);

            foreach (IHTMLDOMAttribute attribute in attributes)
            {
                string value = null;

                if (attribute.nodeValue is DBNull)
                {
                    try
                    {
                        IHTMLDOMNode newNode = htmlNode.cloneNode(false);
                        IHTMLElement newElement = newNode as IHTMLElement;

                        XElement xmlElement = XElement.Parse(newElement.outerHTML);
                        XAttribute xmlAttribute = xmlElement.Attribute(attribute.nodeName);

                        value = xmlAttribute.Value;
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    value = attribute.nodeValue;
                }

                writer.WriteAttributeString(attribute.nodeName, value);
            }
        }

        private PartBookmark ParseToPartBookmark(IHTMLElement htmlElement)
        {
            PartBookmark bookmark = null;

            IHTMLAnchorElement anchor = htmlElement as IHTMLAnchorElement;

            if (anchor != null && !string.IsNullOrEmpty(anchor.name))
            {
                bookmark = new PartBookmark(anchor.name, _ibm);
            }

            return bookmark;
        }

        private void PrepareBookmark()
        {
            string wordXmlContent = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.WordOpenXML;

            XDocument xdoc = XDocument.Parse(wordXmlContent);
            XElement body = xdoc.Descendants(WordXmlBodyName).Single();

            _bookmarks = body.Descendants().Where(IsBookmark).Select(CreatePartBookmark).ToList();
        }

        private string GenerateXslForBookmark(string bookmarkName)
        {
            PartBookmark bookmark = new PartBookmark(bookmarkName, _ibm);

            return GenerateXslForBookmark(bookmark);
        }

        private string GenerateXslForBookmark(PartBookmark bookmark)
        {
            string result = null;
            string bizName = null;

            if (!bookmark.IsDelete)
            {
                bool isIf = bookmark.Type == BookmarkType.StartIf;
                Relations relations = null;

                switch (bookmark.Type)
                {
                    case BookmarkType.StartForeach:
                        ProcessStartForeachBookmark(bookmark, ref relations);
                        bizName = string.Empty;
                        goto case BookmarkType.Select;
                    case BookmarkType.StartIf:
                    case BookmarkType.Select:
                    case BookmarkType.Image:
                    case BookmarkType.PdeTag:
                    case BookmarkType.PdeChart:
                        string path = GetXslPath(bookmark, isIf, bizName);
                        result = string.Format(bookmark.XslString, path);
                        break;
                    case BookmarkType.EndForeach:
                        if (_foreach.Count > 0)
                        {
                            _foreach.RemoveAt(_foreach.Count - 1);
                        }
                        goto case BookmarkType.EndIf;
                    case BookmarkType.EndIf:
                        result = bookmark.XslString;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private void SetDataBindingsForControl(ControlBase control)
        {
            if (control.DataBind)
            {
                if (control is ISelectable)
                {
                    string selectedKey = ((ISelectable)control).DataBindingSelectKey;

                    if (!string.IsNullOrEmpty(selectedKey))
                    {
                        string selectPath = GetXslPath(selectedKey);

                        SetDataBindings(control, SelectBindingKey, selectPath);
                    }
                }

                string path = GetXslPath(control.DataBindingKey);

                SetDataBindings(control, ValueBindingKey, path);
            }
        }

        private void ProcessStartForeachBookmark(PartBookmark bookmark, ref Relations relations)
        {
            int index = _bookmarks.FindIndex(b => b.Key.Equals(bookmark.Key, System.StringComparison.OrdinalIgnoreCase));
            int foreachIndex = _foreach.Count + 1;

            //TODO:CHECK
            string variant = Core.MarkupConstant.XslVariableImage + index.ToString();

            InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(bookmark.Key);
            relations = GetRelations(bookmark);

            ForeachItem foreachItem = new ForeachItem(index, _bookmarks, relations, ibmDomain.SelectedTables, foreachIndex, variant, string.Empty);
            _foreach.Add(foreachItem);
        }

        private string GetXslPath(string bookmarkName)
        {
            PartBookmark bookmark = new PartBookmark(bookmarkName, _ibm);

            return GetXslPath(bookmark, false);
        }

        private string GetXslPath(PartBookmark bookmark, bool isIf = false, string bizName = null, Relations relations = null)
        {
            List<string> tables = GetSelectedTables(bookmark);
            relations = relations ?? GetRelations(bookmark);

            bizName = bizName == null ? bookmark.BizName : bizName;

            string path = GetXslPath(bookmark, bizName, isIf, tables, relations);

            return path;
        }

        private void SetDataBindings(ControlBase control, string key, string value)
        {
            var binding = control.DataBindings[key];

            if (binding == null)
            {
                control.DataBindings.Add(key, value);
            }
            else
            {
                binding.Expression = value;
            }
        }

        private bool IsBookmark(XElement element)
        {
            return element.Name == WordXmlStartBookmarkName;
        }

        private bool IsRedundantSpaceNode(IHTMLDOMNode node)
        {
            bool result = false;

            if (node.nodeType == 3 && string.IsNullOrWhiteSpace(node.nodeValue))
            {
                IHTMLDOMNode sibling = node.nextSibling;

                if (sibling != null && sibling is IHTMLAnchorElement)
                {
                    PartBookmark bookmark = ParseToPartBookmark(sibling as IHTMLElement);

                    if (bookmark != null && bookmark.IsControlBookmark)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private PartBookmark CreatePartBookmark(XElement element)
        {
            XAttribute attribute =  element.Attribute(WordXmlNameAttribute);

            return new PartBookmark(attribute.Value, _ibm);
        }

        private string GetBase64ImageSrc(string filePath)
        {
            string base64ImgSrc = null;
            
            try
            {
                string base64ImgString = null;
                string extension = Path.GetExtension(filePath).Substring(1);

                using (Stream stream = File.OpenRead(filePath))
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    base64ImgString = Convert.ToBase64String(buffer);
                }

                base64ImgSrc = string.Format(Format_Base64ImageSrc, extension, base64ImgString);
            }
            catch (System.Exception)
            {
            }

            return base64ImgSrc;
        }
    }
}
