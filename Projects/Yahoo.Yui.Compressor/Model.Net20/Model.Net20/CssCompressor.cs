using System;

namespace Yahoo.Yui.Compressor
{
    public static class CssCompressor
    {
        public static string Compress(string css)
        {
            return Compress(css, 0, CssCompressionType.StockYuiCompressor, true);
        }


        public static string Compress(string css, int columnWidth, CssCompressionType cssCompressionType,
                                      bool removeComments)
        {
            string compressedCss;


            switch (cssCompressionType)
            {
                case CssCompressionType.StockYuiCompressor:
                    compressedCss = YUICompressor.Compress(css, columnWidth, removeComments);
                    break;
                case CssCompressionType.MichaelAshRegexEnhancements:
                    compressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth, removeComments);
                    break;
                case CssCompressionType.Hybrid:
                    // We need to try both types. We get the keep size.
                    string yuiCompressedCss = YUICompressor.Compress(css, columnWidth, removeComments);
                    string michaelAshsRegexEnhancementsCompressedCss = MichaelAshRegexCompressor.Compress(css,
                                                                                                          columnWidth,
                                                                                                          removeComments);
                    compressedCss = yuiCompressedCss.Length < michaelAshsRegexEnhancementsCompressedCss.Length
                                        ? yuiCompressedCss
                                        : michaelAshsRegexEnhancementsCompressedCss;
                    break;
                default:
                    throw new InvalidOperationException(
                        "Unhandled CssCompressionType found when trying to determine which compression method to use.");
            }

            return compressedCss;
        }
    }
}