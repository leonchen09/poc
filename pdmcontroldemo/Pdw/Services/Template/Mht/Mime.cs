
using System.Collections.Generic;

namespace Pdw.Services.Template.Mht
{
    /// <summary>
    /// refer: http://www.faqs.org/rfcs/rfc822.html
    /// </summary>
    class Mime
    {
        public const string StartPart = "------=_NextPart_";
        
        private const string MhtHtml = ".htm";
        private const string MhtImage = ".jpg";
        private const string MhtFileList = "filelist.xml";
        private const string MhtHeaderFile = "_files/header.htm";
        
        public const char Dot = '.';
        public const char Equal = '=';
        public const string SourceImage = "src=3D\"";
        public const char EndAttribute = '\"';
        public const string StartBody = "<body lang=";
        public const string EndBody = "</body>";
        public const string EndHead = "</head>";
        public const string EndTag = ">";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mhtFile"></param>
        /// <returns></returns>
        public List<Part> Parse(string mhtFile)
        {
            List<Part> parts = new List<Part>();
            int index = 0;
            Part part = new Part(PartType.MhtHeader, index);

            using (System.IO.StreamReader reader = new System.IO.StreamReader(mhtFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line) && line.StartsWith(StartPart))
                    {
                        // add previous part
                        parts.Add(part);

                        // read current part
                        index++;
                        part = new Part(index);
                        part.Lines.Add(line);
                        UpdatePartConent(reader, ref part);
                    }
                    else
                        part.Lines.Add(line);
                }
            }

            if (part.Type != PartType.MhtHeader)
                parts.Add(part);

            return parts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="part"></param>
        private void UpdatePartConent(System.IO.StreamReader reader, ref Part part)
        {
            part.Content = new PartContent();
            string line = ReadLine(reader, ref part);
            part.Content.Location = PartContent.GetContentValue(line);
            if (!string.IsNullOrEmpty(line))
            {
                if (line.EndsWith(MhtHtml) && !line.EndsWith(MhtHeaderFile))
                    part.Type = PartType.MhtHtml;
                else if (line.EndsWith(MhtImage))
                    part.Type = PartType.MhtImage;
                else if (line.EndsWith(MhtFileList))
                    part.Type = PartType.MhtFileList;
            }

            line = ReadLine(reader, ref part);
            part.Content.TransferEncoding = PartContent.GetContentValue(line);

            line = ReadLine(reader, ref part);
            part.Content.Type = PartContent.GetContentValue(line);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private string ReadLine(System.IO.StreamReader reader, ref Part part)
        {
            if (reader != null)
            {
                string line = reader.ReadLine();

                if (part != null)
                    part.Lines.Add(line);

                return line;
            }

            return null;
        }
    }
}
