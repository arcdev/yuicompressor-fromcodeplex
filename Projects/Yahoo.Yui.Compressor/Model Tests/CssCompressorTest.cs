using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahoo.Yui.Compressor;


namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class CssCompressorTest
    {
        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressYUIStockWithNoColumnWidthTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressYUIStockWithColumnWidthSpecifiedTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
                73,
                CssCompressionType.StockYUICompressor);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressMichaelAshsRegexWithNoColumnWidthTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
                0,
                CssCompressionType.MichaelAshsRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressMichaelAshsRegexWithColumnWidthSpecifiedTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
                73,
                CssCompressionType.MichaelAshsRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }
    }
}