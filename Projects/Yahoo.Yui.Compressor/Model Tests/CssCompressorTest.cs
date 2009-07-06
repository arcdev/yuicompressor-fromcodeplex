using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class CssCompressorTest
    {
        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CompressYUIStockWithNoColumnWidthTest()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            string compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CompressYUIStockWithColumnWidthSpecifiedTest()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            string compressedCss = CssCompressor.Compress(css,
                                                          73,
                                                          CssCompressionType.StockYuiCompressor);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet2.css")]
        public void CompressYUIStockWithNoCssContent()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet2.css");

            // Now compress the css - result should be empty.
            string compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(string.IsNullOrEmpty(compressedCss));
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CompressMichaelAshsRegexWithNoColumnWidthTest()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            string compressedCss = CssCompressor.Compress(css,
                                                          0,
                                                          CssCompressionType.MichaelAshRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CompressMichaelAshsRegexWithColumnWidthSpecifiedTest()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            string compressedCss = CssCompressor.Compress(css,
                                                          73,
                                                          CssCompressionType.MichaelAshRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet-MissingClosingCommentSymbol-CP3723.css")]
        public void CompressYUIStockAgainstBadCss_CP3723Test()
        {
            // First load up some Css.
            string css = File.ReadAllText("SampleStylesheet-MissingClosingCommentSymbol-CP3723.css");

            // Now compress the css.
            string compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);
        } 
    }
}