
namespace Pdw.Services.Template.Mht
{
    class PartBookmark:Base.BaseBookmark
    {
        public int StartLineIndex { get; set; }
        public int StartCharIndexOfStartLine { get; set; }

        public int EndLineIndex { get; set; }
        public int EndCharIndexOfEndLine { get; set; }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                UpdateOtherInfo();
            }
        }

        public string BookmarkString { get; private set; }

        public PartBookmark()
        {
            XslString = "";
        }

        #region helper
        private void UpdateOtherInfo()
        {
            Type = Core.BookmarkType.None;
            Key = string.Empty;
            BizName = string.Empty;

            if (!string.IsNullOrEmpty(_content) && _content.Contains(Core.MarkupConstant.PrefixBookmark) &&
                _content.Contains(Core.MarkupConstant.SuffixEndBookmark) &&
                _content.Contains(Core.MarkupConstant.SplitCharBookmark))
            {
                // find bookmark string
                int startIndex = FindString(_content, Core.MarkupConstant.PrefixBookmark, StartCharIndexOfStartLine);
                int endIndex = FindString(_content, Core.MarkupConstant.SuffixEndBookmark, startIndex) +
                    Core.MarkupConstant.SuffixEndBookmark.Length;
                BookmarkString = _content.Substring(startIndex, endIndex - startIndex);

                // find key, biz name and type
                int splitIndex = BookmarkString.IndexOf(Core.MarkupConstant.SplitCharBookmark);
                Key = BookmarkString.Substring(Core.MarkupConstant.PrefixBookmark.Length, 
                    splitIndex - Core.MarkupConstant.PrefixBookmark.Length);
                Pdw.Core.InternalBookmarkItem iBm =
                    WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkItem(Key);
                if (iBm == null)
                {
                    BizName = BookmarkString.Substring(splitIndex + 1);
                    BizName = BizName.Replace(Core.MarkupConstant.SpaceInBookmark, Core.MarkupConstant.Space);
                }
                else
                    BizName = Core.MarkupUtilities.GetOriginalBizName(iBm.Key, iBm.BizName);
                if (BizName.EndsWith(Core.MarkupConstant.SuffixEndBookmark))
                    BizName = BizName.Remove(BizName.Length - Core.MarkupConstant.SuffixEndBookmark.Length);
                if (Key.EndsWith(Core.MarkupConstant.MarkupStartForeach))
                    Type = Core.BookmarkType.StartForeach;
                else if (Key.EndsWith(Core.MarkupConstant.MarkupEndForeach))
                    Type = Core.BookmarkType.EndForeach;
                else if (Key.EndsWith(Core.MarkupConstant.MarkupStartIf))
                    Type = Core.BookmarkType.StartIf;
                else if (Key.EndsWith(Core.MarkupConstant.MarkupEndIf))
                    Type = Core.BookmarkType.EndIf;
                else if (Key.EndsWith(Core.MarkupConstant.MarkupSelect))
                    Type = Core.BookmarkType.Select;
                else if (Key.EndsWith(Core.MarkupConstant.MarkupImage))
                    Type = Core.BookmarkType.Image;
            }
        }

        public static int FindString(string content, string template, int startIndex = 0)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(template))
                return -1;

            for (int charIndex = startIndex; charIndex <= content.Length - template.Length; charIndex++)
            {
                string temp = content.Substring(charIndex, template.Length);
                if (template == temp)
                {
                    if (charIndex > 0 && content[charIndex] == ' ')
                        charIndex--;
                    return charIndex;
                }
            }
            return -1;
        }
        #endregion
    }
}
