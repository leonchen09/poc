using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.Core;

namespace Pdw.Services.Template.Pdm
{
    public class PartBookmark : Base.BaseBookmark
    {
        private const string XslFormat_Select = "<xsl:value-of select=\"{0}\" />";
        private const string XslFormat_StartForeach = "<xsl:for-each match=\"{0}\">";
        private const string XslFormat_EndForeach = "</xsl:for-each>";
        private const string XslFormat_StartIf = "<xsl:if test=\"{0}\">";
        private const string XslFormat_EndIf = "</xsl:if>";
        private const string XslFormat_Image = "<img><xsl:attribute name=\"scr\"><xsl:value-of select=\"concat('data:image/png;base64,', {0})\" /></xsl:attribute></img>";

        public bool IsControlBookmark
        {
            get
            {
                return Type == BookmarkType.StartForeach || Type == BookmarkType.EndForeach ||
                    Type == BookmarkType.StartIf || Type == BookmarkType.EndIf;
            }
        }

        public PartBookmark(string name, InternalBookmark ibookmark)
        {
            InternalBookmarkItem bookmark = ibookmark.GetInternalBookmarkItem(name);

            if (bookmark != null)
            {
                BizName = bookmark.OrginalBizName;

                Key = name; 
            }
            else
            {
                PdeDataTagInfo pdeTag = ibookmark.GetPdeDataTagInfo(name);

                if (pdeTag != null)
                {
                    BizName = pdeTag.BookmarkName;

                    Key = name; 
                }
            }

            InitializeBookmark();
        }

        private void InitializeBookmark()
        {
            Type = Core.BookmarkType.None;
            XslString = string.Empty; ;

            if (!string.IsNullOrEmpty(Key))
            {
                if (Key.EndsWith(Core.MarkupConstant.MarkupStartForeach))
                {
                    Type = BookmarkType.StartForeach;

                    XslString = XslFormat_StartForeach;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupEndForeach))
                {
                    Type = BookmarkType.EndForeach;

                    XslString = XslFormat_EndForeach;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupStartIf))
                {
                    Type = BookmarkType.StartIf;

                    XslString = XslFormat_StartIf;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupEndIf))
                {
                    Type = BookmarkType.EndIf;

                    XslString = XslFormat_EndIf;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupSelect))
                {
                    Type = BookmarkType.Select;

                    XslString = XslFormat_Select;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupImage))
                {
                    Type = BookmarkType.Image;

                    XslString = XslFormat_Image;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupComment))
                {
                    Type = BookmarkType.Comment;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupPdeTag))
                {
                    Type = BookmarkType.PdeTag;

                    XslString = XslFormat_Select;
                }
                else if (Key.EndsWith(Core.MarkupConstant.MarkupPdeChart))
                {
                    Type = BookmarkType.PdeChart;

                    XslString = XslFormat_Image;
                }
            }             
        }
    }
}
