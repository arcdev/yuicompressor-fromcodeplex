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
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressYUIStockWithNoColumnWidthTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressYUIStockWithColumnWidthSpecifiedTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
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
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet2.css");

            // Now compress the css - result should be empty.
            compressedCss = CssCompressor.Compress(css);
            Assert.IsTrue(string.IsNullOrEmpty(compressedCss));
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressMichaelAshsRegexWithNoColumnWidthTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
                0,
                CssCompressionType.MichaelAshRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleStylesheet1.css")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressMichaelAshsRegexWithColumnWidthSpecifiedTest()
        {
            string css;
            string compressedCss;


            // First load up some Css.
            css = File.ReadAllText("SampleStylesheet1.css");

            // Now compress the css.
            compressedCss = CssCompressor.Compress(css,
                73,
                CssCompressionType.MichaelAshRegexEnhancements);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);

            // Expected failure.
            CssCompressor.Compress(null);
        }
    }
}