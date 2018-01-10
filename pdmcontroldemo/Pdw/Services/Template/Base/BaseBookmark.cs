
namespace Pdw.Services.Template.Base
{
    public class BaseBookmark
    {
        public string BizName { get; set; }
        public string Key { get; protected set; }
        public Core.BookmarkType Type { get; protected set; }

        public string XslString { get; set; }
        public bool IsDelete
        {
            get { return Type == Core.BookmarkType.None; }
        }

        public void MarkDelete()
        {
            Type = Core.BookmarkType.None;
        }
    }
}
