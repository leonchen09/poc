
namespace Pdw.Services.Template.Mht
{
    class PartContent
    {
        public const string SplitChar = ":";
        public const string ContentLocation = "Content-Location";
        public const string ContentTransferEncoding = "Content-Transfer-Encoding";
        public const string ContentType = "Content-Type";

        /// <summary>
        /// ex: Content-Location: file:///C:/6F68D2E5/fba47852-ddad-4235-a183-6c737065034e_files/image001.jpg
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// ex: Content-Transfer-Encoding: base64
        /// </summary>
        public string TransferEncoding { get; set; }

        /// <summary>
        /// ex: Content-Type: image/jpeg
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetContentValue(string content)
        {
            if (!string.IsNullOrEmpty(content) && content.Contains(SplitChar))
            {
                int index = content.IndexOf(SplitChar);
                return content.Substring(index + 1).Trim();
            }

            return content;
        }
    }
}
