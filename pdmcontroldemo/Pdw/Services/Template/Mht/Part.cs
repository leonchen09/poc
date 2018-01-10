
using System.Collections.Generic;

namespace Pdw.Services.Template.Mht
{
    class Part
    {
        private const char MarkReplace = (char)27;
        private const char MarkRemove = (char)13;
        private const string LinkedChar = "=";
        private const string Redundant = "�";
        private const string RedundantSpace = "> <";
        private const string StartParagrap = "<p";
        private const string EndParagrap = "</p>";

        #region properties
        public int Index { get; set; }
        public List<string> Lines { get; set; }
        public PartType Type { get; set; }
        public PartContent Content { get; set; }
        #endregion

        #region constructor
        public Part()
        {
            Index = 0;
            Lines = new List<string>();
            Content = new PartContent();
            Type = PartType.None;
        }

        public Part(PartType type)
            : this()
        {
            Type = type;
        }

        public Part(int index)
            : this()
        {
            Index = index;
        }

        public Part(PartType type, int index)
            : this(type)
        {
            Index = index;
        }
        #endregion

        #region methods
        public void RemoveLine(int lineIndex)
        {
            if (lineIndex >= 0 && lineIndex < Lines.Count)
            {
                Lines[lineIndex] = MarkRemove.ToString();
            }
        }

        public List<PartBookmark> GetBookmarks()
        {
            List<PartBookmark> bookmarks = new List<PartBookmark>();
            Dictionary<string, bool> checks = new Dictionary<string, bool>();

            if (Lines == null)
                return bookmarks;

            for (int lineIndex = 0; lineIndex < Lines.Count; lineIndex++)
            {
                string line = Lines[lineIndex];
                if (string.IsNullOrEmpty(line))
                    continue;

                bool hasBookmark = true;
                int startIndexOfBm = 0;
                while (hasBookmark)
                {                    
                    int startIndex = lineIndex;
                    int endIndex = lineIndex;
                    string endLine = line;
                    if (!line.Substring(startIndexOfBm).Contains(Core.MarkupConstant.PrefixBookmark) && line.EndsWith(LinkedChar))
                    {
                        string nextLine = Lines[startIndex + 1];
                        if (!nextLine.Contains(Core.MarkupConstant.PrefixBookmark))
                        {
                            endIndex++;
                            line = line.Remove(line.Length - 1) + nextLine;
                            endLine = nextLine;
                        }
                    }

                    if (line.Substring(startIndexOfBm).Contains(Core.MarkupConstant.PrefixBookmark))
                    {
                        PartBookmark bookmark = new PartBookmark();
                        bookmark.StartLineIndex = startIndex;
                        bookmark.StartCharIndexOfStartLine = PartBookmark.FindString(line, Core.MarkupConstant.PrefixBookmark, startIndexOfBm);

                        string content = line;
                        while (!content.Substring(startIndexOfBm).Contains(Core.MarkupConstant.SuffixEndBookmark) && content.EndsWith(LinkedChar))
                        {
                            endIndex++;
                            endLine = Lines[endIndex];
                            content = content.Remove(content.Length - 1) + endLine;
                        }
                        if (content.Substring(startIndexOfBm).Contains(Core.MarkupConstant.SuffixEndBookmark))
                        {
                            bookmark.EndLineIndex = endIndex;
                            bookmark.EndCharIndexOfEndLine = PartBookmark.FindString(content, Core.MarkupConstant.SuffixEndBookmark,
                                bookmark.StartCharIndexOfStartLine) + Core.MarkupConstant.SuffixEndBookmark.Length;
                            bookmark.EndCharIndexOfEndLine = bookmark.EndCharIndexOfEndLine - (content.Length - endLine.Length);
                            startIndexOfBm = bookmark.EndCharIndexOfEndLine;
                            bookmark.Content = content;

                            if (!string.IsNullOrEmpty(bookmark.Key) && !string.IsNullOrEmpty(bookmark.BizName))
                            {
                                if (!checks.ContainsKey(bookmark.Key))
                                {
                                    bookmarks.Add(bookmark);
                                    checks.Add(bookmark.Key, true);
                                }
                            }
                        }

                        if (startIndex == endIndex)
                        {
                            if (line.Length <= startIndexOfBm)
                                hasBookmark = false;
                        }
                        else
                            hasBookmark = false;
                    }
                    else
                        hasBookmark = false;
                }
            }

            return bookmarks;
        }

        public void FindStartParagrap(int startLine, int startChar, out int paragrapLineIndex, out int paragrapCharIndex)
        {
            paragrapLineIndex = startLine;
            paragrapCharIndex = startChar;

            int maxLength = 100;
            int count = 0;
            while (count < maxLength)
            {
                string line = Lines[paragrapLineIndex];
                if (!string.IsNullOrEmpty(line) && line.Length > paragrapCharIndex)
                {
                    string subLine = line.Substring(paragrapCharIndex);
                    if (subLine.StartsWith(StartParagrap))
                        return;
                    if (subLine.StartsWith(Core.MarkupConstant.SuffixEndBookmark))
                    {
                        paragrapCharIndex = paragrapCharIndex + Core.MarkupConstant.SuffixEndBookmark.Length;
                        return;
                    }
                }
                if (paragrapCharIndex == 0)
                {
                    paragrapLineIndex--;
                    paragrapCharIndex = Lines[paragrapLineIndex].Length - 1;
                }
                else
                    paragrapCharIndex--;
                count++;
            }

            paragrapLineIndex = startLine;
            paragrapCharIndex = startChar;
        }

        public void FindEndParagrap(int endLine, int endChar, out int paragrapLineIndex, out int paragrapCharIndex)
        {
            paragrapLineIndex = endLine;
            paragrapCharIndex = endChar;

            int maxLength = 100;
            int count = 0;
            while (count < maxLength)
            {
                string line = Lines[paragrapLineIndex];
                if (!string.IsNullOrEmpty(line) && line.Length > paragrapCharIndex)
                {
                    string subLine = line.Substring(paragrapCharIndex);
                    if (subLine.EndsWith(EndParagrap))
                    {
                        paragrapCharIndex = paragrapCharIndex + EndParagrap.Length;
                        return;
                    }
                    if (subLine.EndsWith(Core.MarkupConstant.PrefixBookmark))
                    {
                        paragrapCharIndex = paragrapCharIndex - Core.MarkupConstant.PrefixBookmark.Length;
                        return;
                    }
                }
                if (paragrapCharIndex == line.Length)
                {
                    paragrapLineIndex++;
                    paragrapCharIndex = 0;
                }
                else
                    paragrapCharIndex++;
                count++;
            }

            paragrapLineIndex = endLine;
            paragrapCharIndex = endChar;
        }
        #endregion

        #region overwrite methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            int count = Lines.Count - 1;
            int lineIndex = 0;
            foreach (string line in Lines)
            {
                if (line != MarkRemove.ToString())
                {
                    string content = string.IsNullOrEmpty(line) ? "" : line.Replace(Redundant, "").Replace(RedundantSpace, "");
                    content = content.Replace(MarkReplace.ToString(), "");
                    if (lineIndex == count)
                        builder.Append(content);
                    else
                        builder.AppendLine(content);
                }

                lineIndex++;
            }

            return builder.ToString();
        }

        public string RemoveString(string value, int start, int count)
        {
            string temp = string.Empty;
            for (int index = 0; index < value.Length; index++)
            {
                char ch = (index >= start && index < start + count) ? MarkReplace : value[index];
                temp = temp + ch;
            }

            return temp;
        }

        public string RemoveString(string value, int start)
        {
            string temp = string.Empty;
            for (int index = 0; index < value.Length; index++)
            {
                char ch = (index >= start) ? MarkReplace : value[index];
                temp = temp + ch;
            }

            return temp;
        }
        #endregion
    }
}
