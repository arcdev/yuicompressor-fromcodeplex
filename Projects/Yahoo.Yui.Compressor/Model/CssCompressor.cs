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
                case CssCompressionType.MichaelAshRegexEnhancements:
                    compressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth, removeComments);
                    break;
                case CssCompressionType.Hybrid:
                    // We need to try both types. We get the keep size.
                    string yuiCompressedCss = YUICompressor.Compress(css, columnWidth, removeComments);
                    string michaelAshsRegexEnhancementsCompressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth, removeComments);
                    compressedCss = yuiCompressedCss.Length < michaelAshsRegexEnhancementsCompressedCss.Length
                                        ? yuiCompressedCss
                                        : michaelAshsRegexEnhancementsCompressedCss;
                    break;
                default:
                    throw new InvalidOperationException("Unhandled CssCompressionType \"" + cssCompressionType + "\" found when trying to determine which compression method to use.");
            }
            return compressedCss;
        }
    }
}