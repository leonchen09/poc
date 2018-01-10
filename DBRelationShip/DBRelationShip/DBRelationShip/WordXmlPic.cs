using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml;

namespace DBRelationShip
{
    class WordXmlPic
    {
        public static String convertToBase64(String filePath)
        {
            FileStream mPicture = new FileStream(filePath, FileMode.Open);
            byte[] picByte = new byte[mPicture.Length];
            mPicture.Read(picByte, 0, (int)mPicture.Length);
            String str = Convert.ToBase64String(picByte);
            return str;
        }

        public static void convertFromBase64(String base64Str, String fileName)
        {
            byte[] outputb = Convert.FromBase64String(base64Str);
            MemoryStream stream = new MemoryStream();
            //将保存在流中的数据写入内存流
            stream.Write(outputb, 0, outputb.Length);
            Image image = Image.FromStream(stream);
            image.Save(fileName);
        }
        public static void updateXMLPic(String xmlFile, String oldFileName, String newFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNodeList parts = xmlDoc.GetElementsByTagName("pkg:part");
            foreach(XmlNode part in parts)
            {
                XmlElement element = (XmlElement)part;
                if (element.GetAttribute("pkg:name") == "/word/media/" + oldFileName)
                {
                    XmlNode data = element.FirstChild;
                    data.InnerText = convertToBase64(newFileName);
                }
            }
            xmlDoc.Save(xmlFile);
        }
    }
}
