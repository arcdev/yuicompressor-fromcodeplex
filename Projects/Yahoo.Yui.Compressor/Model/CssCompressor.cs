using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Yahoo.Yui.Compressor
{
    public static class CssCompressor
    {
        public static string Compress(string css)
        {
            return CssCompressor.Compress(css,
                0,
                CssCompressionType.StockYUICompressor);
        }

        public static string Compress(string css,
            int columnWidth,
            CssCompressionType cssCompressionType)
        {
            string compressedCss = null;


            switch (cssCompressionType)
            {
                case CssCompressionType.StockYUICompressor: compressedCss = YUICompressor.Compress(css,
                    columnWidth);
                    break;
                case CssCompressionType.MichaelAshsRegexEnhancements: compressedCss = MichaelAshsRegexCompressor.Compress(css,
                    columnWidth);
                    break;
                case CssCompressionType.Hybrid :
                    // We need to try both types. We get the keep size.
                    string yuiCompressedCss = YUICompressor.Compress(css,
                        columnWidth);
                    string michaelAshsRegexEnhancementsCompressedCss = MichaelAshsRegexCompressor.Compress(css,
                        columnWidth);
                    compressedCss = yuiCompressedCss.Length < michaelAshsRegexEnhancementsCompressedCss.Length ? yuiCompressedCss : michaelAshsRegexEnhancementsCompressedCss;
                    break;
                default: throw new InvalidOperationException("Unhandled CssCompressionType found when trying to determine which compression method to use.");
            }

            return compressedCss;
        }
    }
}