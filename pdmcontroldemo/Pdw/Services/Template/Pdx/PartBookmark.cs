
namespace Pdw.Services.Template.Pdx
{
    class PartBookmark:Base.BaseBookmark
    {
        public int LineIndex { get; set; }
        public int StartCharIndex { get; set; }
        public int EndCharIndex { get; set; }

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

        public PartBookmark()
        {
            XslString = "";
        }

        private void UpdateOtherInfo()
        {
            Type = Core.BookmarkType.None;
            Key = string.Empty;
            BizName = string.Empty;

            if (!string.IsNullOrEmpty(_content) && _content.Contains(Core.MarkupConstant.PrefixBookmark) &&
                _content.Contains(Core.MarkupConstant.SuffixEndBookmark) &&
                _content.Contains(Core.MarkupConstant.SplitCharBookmark))
            {                
                // find key, biz name and type
                int splitIndex = _content.IndexOf(Core.MarkupConstant.SplitCharBookmark);
                Key = _content.Substring(Core.MarkupConstant.PrefixBookmark.Length,
                    splitIndex - Core.MarkupConstant.PrefixBookmark.Length);
                Pdw.Core.InternalBookmarkItem iBm =
                    WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkItem(Key);
                if (iBm == null)
                {
                    BizName = _content.Substring(splitIndex + 1);
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
    }
}
