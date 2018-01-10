using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using mshtml;
using Pdw.FormControls.Design;

namespace Pdw.FormControls.Extension
{
    public static class Extensions
    {
        public static bool Is(this string source, string compareTo, bool ignoreCase = true)
        {
            return string.Compare(source, compareTo, ignoreCase) == 0;
        }

        internal static string ToString(this Font font, string format)
        {
            string style = null;

            if (!string.IsNullOrEmpty(format) && format.Is("W"))
            {
                StringBuilder styleBuilder = new StringBuilder();

                string unit = font.Unit == GraphicsUnit.Point ? "pt" : "px";

                styleBuilder.AppendFormat("font-family:{0};", font.FontFamily.Name);
                styleBuilder.AppendFormat("font-size:{0}{1};", font.Size, unit);

                if (font.Italic)
                {
                    styleBuilder.Append("font-style:italic;");
                }

                if (font.Bold)
                {
                    styleBuilder.Append("font-weight:bold;");
                }
            }
            else
            {
                style = font.ToString();
            }
           
            return style;
        }

        internal static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        internal static string ToString(this Color source, string format)
        {
            string toReturn = string.Empty;

            if (!string.IsNullOrEmpty(format) && format.Is("RGB"))
            {
                toReturn = string.Format("rgb({0},{1},{2})", source.R, source.G, source.B);
            }
            else
            {
                toReturn = source.ToString();
            }

            return toReturn;
        }
    }
}
