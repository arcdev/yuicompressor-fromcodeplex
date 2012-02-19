using System;

namespace Yahoo.Yui.Compressor
{
    public static class CssCompressor
    {
        public static string Compress(string css)
        {
            return Compress(css, CssCompressionType.StockYuiCompressor);
        }

        public static string Compress(string css, CssCompressionType cssCompressionType)
        {
            return Compress(css, 0, cssCompressionType);
        }

        public static string Compress(string css, int columnWidth, CssCompressionType cssCompressionType)
        {
            return Compress(css, columnWidth, cssCompressionType, true);
        }
        public static string Compress(string css, int columnWidth, CssCompressionType cssCompressionType, bool removeComments)
        {
            string compressedCss;

            switch (cssCompressionType)
            {
                case CssCompressionType.None:
                    compressedCss = css;
                    break;
                case CssCompressionType.StockYuiCompressor:
                    compressedCss = YUICompressor.Compress(css, columnWidth, removeComments);
                    break;
                default:
                    throw new InvalidOperationException("Unhandled CssCompressionType \"" + cssCompressionType + "\" found when trying to determine which compression method to use.");
            }
            return compressedCss;
        }
    }
}