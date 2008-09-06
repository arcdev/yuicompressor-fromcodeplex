using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahoo.Yui.Compressor
{
    public static class Ext
    {
        public static int AppendReplacement(Capture capture,
            StringBuilder value,
            string input,
            string replacement,
            int index)
        {
            string preceding = input.Substring(index,
                capture.Index - index);

            value.Append(preceding);
            value.Append(replacement);

            return capture.Index + capture.Length;
        }

        public static void AppendTail(StringBuilder value,
            string input,
            int index)
        {
            value.Append(input.Substring(index));
        }

        public static string RegexReplace(string input,
            string pattern,
            string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string RegexReplace(string input,
            string pattern,
            string replacement,
            RegexOptions options)
        {
            return Regex.Replace(input,
                pattern,
                replacement,
                options);
        }

        public static string Fill(string format,
            params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture,
                format,
                args);
        }

        public static string RemoveRange(string input,
            int startIndex,
            int endIndex)
        {
            return input.Remove(startIndex,
                endIndex - startIndex);
        }

        public static bool EqualsIgnoreCase(string left,
            string right)
        {
            return String.Compare(left,
                right,
                StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string ToHexString(int value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string input = value.ToString(CultureInfo.InvariantCulture);
            
            foreach (char digit in input)
            {
                stringBuilder.AppendFormat("{0:x2}", Convert.ToUInt32(digit));
            }

            return stringBuilder.ToString();
        }

        public static string ToPluralString(int value)
        {
            return value == 1 ? string.Empty : "s";
        }

        public static bool IsNullOrEmpty<T>(IList<T> value)
        {
            return value == null || value.Count <= 0 ? true : false;
        }

        public static IList<T> ToListIfNotNullOrEmpty<T>(IList<T> value)
        {
            return IsNullOrEmpty(value) ? null : value;
        }
    }
}