using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ip = Microsoft.Office.Interop.InfoPath;
using ipst = Microsoft.Office.Interop.InfoPath.SemiTrust;
using ipxml = Microsoft.Office.Interop.InfoPath.Xml;

namespace TestApp
{
    class Test
    {
        public void importXsd(ip.XDocument xdocument)
        {
            //xdocument.ImportFile(@"E:\ProntoDir\pdm\Pdmpocplugin.xsd");
            //xdocument.ImportDOM(@"E:\ProntoDir\pdm\Pdmpocplugin.xsd");
            //ip.Xml.
        }

        public ip.XDocument createApp()
        {
            ip.Application app = new ip.Application();
            
            //app.RegisterSolution(@"E:\ProntoDir\pdm\表单4.xsn");
            ip.XDocument doc = app.XDocuments.Open(@"E:\ProntoDir\pdm\form1\template.xml");
            ip.Xml.IXMLDOMDocument dom = doc.CreateDOM();
            dom.loadXML(@"E:\ProntoDir\pdm\Pdmpocplugin.xsd");
            return doc;

        }
    }
}
