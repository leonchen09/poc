
using System.Collections.Generic;

namespace Pdw.Services.Template.Pdx
{
    class TextParser
    {
        #region properties
        public string[] Lines { get; private set; }
        public List<PartBookmark> Bookmarks { get; private set; }
        #endregion

        #region constructor
        public TextParser(string content)
        {
            ParseLines(content);
            ParseBookmark();
        }
        #endregion

        #region public methods
        public void ReplaceXsl(PartBookmark bookmark)
        {
            if (bookmark == null)
                return;
            
            int lineIndex = bookmark.LineIndex;
            string line = Lines[lineIndex];
            line = line.Replace(bookmark.Content, bookmark.XslString);
            Lines[lineIndex] = line;
        }

        public void AppendHeader(string content)
        {
            if (Lines == null || Lines.Length < 1)
                return;

            int lineIndex = 0;
            AppendContent(lineIndex, content, true);
        }

        public void AppendFooter(string content)
        {
            if (Lines == null || Lines.Length < 1)
                return;

            int lineIndex = Lines.Length - 1;
            AppendContent(lineIndex, content, false);
        }

        private void AppendContent(int lineIndex, string content, bool isBefore)
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (Lines == null || Lines.Length <= lineIndex)
                return;

            string line = Lines[lineIndex];
            if (string.IsNullOrEmpty(line))
                line = content;
            else
                line = isBefore ? line.Insert(0, content) : line + content;
            Lines[lineIndex] = line;
        }
        #endregion

        #region overwrite method
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            int count = Lines.Length - 1;
            int lineIndex = 0;
            if (Lines != null)
            {
                foreach (string line in Lines)
                {
                    if (lineIndex == count)
                        builder.Append(line);
                    else
                        builder.AppendLine(line);
                    lineIndex++;
                }
            }

            return builder.ToString();
        }
        #endregion

        #region helper methods
        private void ParseLines(string content)
        {
            Lines = null;
            if (string.IsNullOrEmpty(content))
                return;
            Lines = content.Split(new char[] { (char)13 });
        }

        private void ParseBookmark()
        {
            Dictionary<string, bool> checks = new Dictionary<string, bool>();
            Bookmarks = new List<PartBookmark>();

            if (Lines == null)
                return;

            for (int lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
            {
                string line = Lines[lineIndex];

                if (string.IsNullOrEmpty(line))
                    continue;

                int prefixLength = 0;
                while (line.Contains(Core.MarkupConstant.PrefixBookmark) && line.Contains(Core.MarkupConstant.SuffixEndBookmark))
                {
                    PartBookmark bookmark = new PartBookmark();
                    bookmark.LineIndex = lineIndex;

                    int startIndex = Mht.PartBookmark.FindString(line, Core.MarkupConstant.PrefixBookmark);
                    int endIndex = Mht.PartBookmark.FindString(line, Core.MarkupConstant.SuffixEndBookmark, startIndex) +
                        Core.MarkupConstant.SuffixEndBookmark.Length;
                    bookmark.StartCharIndex = startIndex + prefixLength;
                    bookmark.EndCharIndex = endIndex + prefixLength;
                    bookmark.Content = line.Substring(startIndex, endIndex - startIndex);

                    if (!string.IsNullOrEmpty(bookmark.Key) && !string.IsNullOrEmpty(bookmark.BizName))
                    {
                        if (!checks.ContainsKey(bookmark.Key))
                        {
                            Bookmarks.Add(bookmark);
                            checks.Add(bookmark.Key, true);
                        }
                    }

                    prefixLength = prefixLength + endIndex;
                    line = line.Substring(endIndex);
                }
            }
        }
        #endregion
    }
}
