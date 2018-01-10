using System;
using System.IO;
using System.Collections.Generic;

namespace Pdwx
{
    public static class Utilities
    {
        /// <summary>
        /// create new file with content
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteFile(string filePath, string content, bool append = false, bool isMakeNewLine = true)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, append))
            {
                if (isMakeNewLine)
                    writer.WriteLine(content);
                else
                    writer.Write(content);

                writer.Close();
            }
        }

        /// <summary>
        /// create new file with content
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteFile(string filePath, List<string> contents)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                foreach (string content in contents)
                    writer.WriteLine(content);
                writer.Close();
            }
        }

        public static string ReadFile(string filePath)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] ReadFileContent(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[32768];
                int readed = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        readed = stream.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, readed);
                    }
                    while (readed > 0);
                    return ms.ToArray();
                }
            }
        }

        public static T Deserialize<T>(string xml)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

                using (System.IO.StringReader reader = new System.IO.StringReader(xml))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static string Serialize(object objValue)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(objValue.GetType());

            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                serializer.Serialize(writer, objValue);

                return writer.ToString();
            }
        }

        public static bool IsNullOrEmptyWithoutEnterLine(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            return string.IsNullOrEmpty(value.Replace('\n', ' ').Trim());
        }

        public static bool SaveXmlReader(System.Xml.XmlReader xmlReader, string outputFile)
        {
            try
            {
                System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                xDoc.Load(xmlReader);

                xDoc.Save(outputFile);
            }
            catch { }

            return false;
        }

        public static string GetXmlReaderContent(System.Xml.XmlReader xmlReader)
        {
            string content = string.Empty;
            try
            {
                System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                xDoc.Load(xmlReader);

                content = xDoc.OuterXml;
            }
            catch { }

            return content;
        }

        /// <summary>
        /// check and delete a file
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            bool isUsing = true;
            System.DateTime end = System.DateTime.Now.AddSeconds(5);

            while (isUsing)
            {
                if (System.DateTime.Now > end)
                    break;
                try
                {
                    using (System.IO.StreamReader tryReader = new System.IO.StreamReader(filePath))
                    {
                        tryReader.ReadToEnd();
                        tryReader.Close();
                    }
                    isUsing = false;
                }
                catch
                {
                    isUsing = true;
                }
            }

            if (!isUsing)
                System.IO.File.Delete(filePath);
        }

        public static System.Xml.XmlReader GetXmlReader(string filePath)
        {
            // return System.Xml.XmlReader.Create(new System.IO.StringReader(ReadFile(filePath)));
            using (System.IO.StreamReader reader = new StreamReader(filePath))
            {
                return System.Xml.XmlReader.Create(reader);
            }
        }
    }
}
