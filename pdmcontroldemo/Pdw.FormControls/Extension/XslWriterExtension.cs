using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Pdw.FormControls.Design;

namespace Pdw.FormControls.Extension
{
    public static class XslWriterExtension
    {
        private const string XsltVersion = "1.0";
        private const string XsltOutputType = "html";
        private const string XsltElementPrefix = "xsl";

        private const string XsltElement_Attribute = "attribute";
        private const string XsltElement_Choose = "choose";
        private const string XsltElement_Comment = "comment";
        private const string XsltElement_ForEach = "for-each";
        private const string XsltElement_If = "if";
        private const string XsltElement_Otherwise = "otherwise";
        private const string XsltElement_Output = "output";
        private const string XsltElement_Sort = "sort";
        private const string XsltElement_StyleSheet = "stylesheet";
        private const string XsltElement_Template = "template";
        private const string XsltElement_ValueOf = "value-of";
        private const string XsltElement_Variable = "variable";
        private const string XsltElement_When = "when";

        private const string XsltAttribute_Indent = "indent";
        private const string XsltAttribute_Name = "name";
        private const string XsltAttribute_Select = "select";
        private const string XsltAttribute_Test = "test";
        private const string XsltAttribute_Match = "match";
        private const string XsltAttribute_Method = "method";
        private const string XsltAttribute_Version = "version";

        private const string HtmlAttribute_Action = "action";
        private const string HtmlAttribute_Method = "method";
        private const string HtmlAttribute_Value = "value";
        private const string HtmlAttribute_Name = "name";
        private const string HtmlAttribute_Type = "type";

        private const string XslNamespace = "http://www.w3.org/1999/XSL/Transform";

        public static void WriteStartXsltDocument(this XmlWriter writer)
        {
            //<?xml version="1.0" encoding = "utf-8" standalone="yes"?> 
            writer.WriteStartDocument(true);

            //<xsl:stylesheet version="1.0"> 
            writer.WriteStartElement(XsltElementPrefix, XsltElement_StyleSheet, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Version, XsltVersion);
        }

        public static void WriteEndXsltDocument(this XmlWriter writer)
        {
            //<xsl:stylesheet />
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public static void WriteXslOutputElement(this XmlWriter writer, bool indent = false)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_Output, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Method, XsltOutputType);

            string indentChoice = indent ? SharedConstants.Choice_Yes : SharedConstants.Choice_No;

            writer.WriteAttributeString(XsltAttribute_Indent, indentChoice);

            writer.WriteEndElement();
        }

        public static void WriteStartXslTemplateElement(this XmlWriter writer, string xpath)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_Template, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Match, xpath);
        }

        public static void WriteStartXslAttributeElement(this XmlWriter writer, string name)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_Attribute, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Name, name);
        }

        public static void WriteStartXslVariableElement(this XmlWriter writer, string name, string xpath)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_Variable, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Name, name);
            writer.WriteAttributeString(XsltAttribute_Select, xpath);
        }

        public static void WriteStartXslForEachElement(this XmlWriter writer, string xpath)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_ForEach, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Select, xpath);
        }

        public static void WriteStartXslIfElement(this XmlWriter writer, string condition)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_If, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Test, condition);
        }

        public static void WriteXslValueOfElement(this XmlWriter writer, string xpath)
        {
            writer.WriteStartElement(XsltElementPrefix, XsltElement_ValueOf, XslNamespace);
            writer.WriteAttributeString(XsltAttribute_Select, xpath);
            writer.WriteEndElement();
        }

        public static void WriteStartHtmlFormElement(this XmlWriter writer, string method, string action)
        {
            writer.WriteStartElement(HtmlTagType.Form.ToString().ToLower());
            writer.WriteAttributeString(HtmlAttribute_Action, action);
            writer.WriteAttributeString(HtmlAttribute_Method, method);
        }

        public static void WriteXslHtmlHiddenElement(this XmlWriter writer, string name, string valuePath)
        {
            writer.WriteStartElement(HtmlTagType.Input.ToString().ToLower());
            writer.WriteAttributeString(HtmlAttribute_Name, name);
            writer.WriteAttributeString(HtmlAttribute_Type, InputTagType.Hidden.ToString().ToLower());

            writer.WriteStartXslAttributeElement(HtmlAttribute_Value);
            writer.WriteXslValueOfElement(valuePath);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
