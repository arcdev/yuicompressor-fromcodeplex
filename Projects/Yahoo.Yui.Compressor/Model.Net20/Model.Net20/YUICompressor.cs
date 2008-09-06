using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahoo.Yui.Compressor
{
    public static class YUICompressor
    {
        #region Private Extension Methods

        private static string RemoveCommentBlocks(string input)
        {
            var startIndex = 0;
            var iemac = false;

            startIndex = input.IndexOf(@"/*",
                                       startIndex,
                                       StringComparison.OrdinalIgnoreCase);
            while (startIndex >= 0)
            {
                var preserve = input.Length > startIndex + 2 &&
                               input[startIndex + 2] == '!';

                var endIndex = input.IndexOf(@"*/",
                                             startIndex + 2,
                                             StringComparison.OrdinalIgnoreCase);

                if(endIndex < 0)
                {
                    if(!preserve)
                    {
                        Ext.RemoveRange(input, startIndex, input.Length);
                    }
                }
                else if(endIndex >= startIndex + 2)
                {
                    if(input[endIndex - 1] == '\\')
                    {
                        startIndex = endIndex + 2;
                        iemac = true;
                    }
                    else if(iemac)
                    {
                        startIndex = endIndex + 2;
                        iemac = false;
                    }
                    else if(!preserve)
                    {
                        input = Ext.RemoveRange(input, startIndex, endIndex + 2);
                    }
                    else
                    {
                        startIndex = endIndex + 2;
                    }
                }

                startIndex = input.IndexOf(@"/*",
                                           startIndex,
                                           StringComparison.OrdinalIgnoreCase);
            }

            return input;
        }

        private static string ShortenRgbColors(string css)
        {
            var stringBuilder = new StringBuilder();
            var pattern = new Regex("rgb\\s*\\(\\s*([0-9,\\s]+)\\s*\\)");
            var match = pattern.Match(css);

            var index = 0;
            while (match.Success)
            {
                var colors = match.Groups[1].Value.Split(',');
                var hexcolor = new StringBuilder("#");
                
                foreach (var color in colors)
                {
                    int value;
                    if(!Int32.TryParse(color,
                                       out value))
                    {
                        value = 0;
                    }

                    if(value < 16)
                    {
                        hexcolor.Append("0");
                    }

                    hexcolor.Append(Ext.ToHexString(value));
                }

                index = Ext.AppendReplacement(match, stringBuilder,
                                              css,
                                              hexcolor.ToString(),
                                              index);
                match = match.NextMatch();
            }

            Ext.AppendTail(stringBuilder, css, index);
            return stringBuilder.ToString();
        }

        private static string ShortenHexColors(string css)
        {
            var stringBuilder = new StringBuilder();
            var pattern =
                new Regex(
                    "([^\"'=\\s])(\\s*)#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])");
            var match = pattern.Match(css);

            var index = 0;
            while (match.Success)
            {
                if(Ext.EqualsIgnoreCase(match.Groups[3].Value, match.Groups[4].Value) &&
                   Ext.EqualsIgnoreCase(match.Groups[5].Value, match.Groups[6].Value) &&
                   Ext.EqualsIgnoreCase(match.Groups[7].Value, match.Groups[8].Value))
                {
                    var replacement = String.Concat(match.Groups[1].Value,
                                                    match.Groups[2].Value,
                                                    "#", match.Groups[3].Value,
                                                    match.Groups[5].Value,
                                                    match.Groups[7].Value);
                    index = Ext.AppendReplacement(match, stringBuilder,
                                                  css,
                                                  replacement,
                                                  index);
                }
                else
                {
                    index = Ext.AppendReplacement(match, stringBuilder,
                                                  css,
                                                  match.Value,
                                                  index);
                }

                match = match.NextMatch();
            }

            Ext.AppendTail(stringBuilder, css, index);
            return stringBuilder.ToString();
        }

        private static string RemovePrecedingSpaces(string css)
        {
            var stringBuilder = new StringBuilder();
            var pattern = new Regex("(^|\\})(([^\\{:])+:)+([^\\{]*\\{)");
            var match = pattern.Match(css);

            var index = 0;
            while (match.Success)
            {
                var s = match.Value;
                s = Ext.RegexReplace(s, ":", "___PSEUDOCLASSCOLON___");

                index = Ext.AppendReplacement(match, stringBuilder,
                                              css,
                                              s,
                                              index);
                match = match.NextMatch();
            }

            Ext.AppendTail(stringBuilder, css, index);

            var result = stringBuilder.ToString();
            result = Ext.RegexReplace(result, "\\s+([!{};:>+\\(\\)\\],])", "$1");
            result = Ext.RegexReplace(result, "___PSEUDOCLASSCOLON___", ":");

            return result;
        }

        private static string BreakLines(string css,
                                         int columnWidth)
        {
            var i = 0;
            var start = 0;

            var stringBuilder = new StringBuilder(css);
            while (i < stringBuilder.Length)
            {
                var c = stringBuilder[i++];
                if(c == '}' &&
                   i - start > columnWidth)
                {
                    stringBuilder.Insert(i, '\n');
                    start = i;
                }
            }

            return stringBuilder.ToString();
        }

        #endregion

        public static string Compress(string css,
                                      int columnWidth)
        {
            if(string.IsNullOrEmpty(css))
            {
                throw new ArgumentNullException("css");
            }

            // Safety check the other arguments.
            if(columnWidth < 0)
            {
                columnWidth = 0;
            }

            // Now compress the css!
            css = RemoveCommentBlocks(css);
            css = Ext.RegexReplace(css, "\\s+", " ");
            css = Ext.RegexReplace(css, "\"\\\\\"}\\\\\"\"", "___PSEUDOCLASSBMH___");
            css = RemovePrecedingSpaces(css);
            css = Ext.RegexReplace(css, "([!{}:;>+\\(\\[,])\\s+", "$1");
            css = Ext.RegexReplace(css, "([^;\\}])}", "$1;}");
            css = Ext.RegexReplace(css, "([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");
            css = Ext.RegexReplace(css, ":0 0 0 0;", ":0;");
            css = Ext.RegexReplace(css, ":0 0 0;", ":0;");
            css = Ext.RegexReplace(css, ":0 0;", ":0;");
            css = Ext.RegexReplace(css, "background-position:0;", "background-position:0 0;");
            css = Ext.RegexReplace(css, "(:|\\s)0+\\.(\\d+)", "$1.$2");
            css = ShortenRgbColors(css);
            css = ShortenHexColors(css);
            css = Ext.RegexReplace(css, "[^\\}]+\\{;\\}", "");

            if(columnWidth > 0)
            {
                css = BreakLines(css, columnWidth);
            }

            css = Ext.RegexReplace(css, "___PSEUDOCLASSBMH___", "\"\\\\\"}\\\\\"\"");

            // Replace multiple semi-colons in a row by a single one
            // See SF bug #1980989
            css = css.Replace(";;+", ";");
            css = css.Trim();

            return css;
        }
    }
}