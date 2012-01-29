using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class CssCompressorTest : TestBase
    {
        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet1.css", "Cascading Style Sheet Files")]
        public void CompressCssWithNoColumnWidthSucessfullyCompressesText()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressCssWitANullFileNameThrowsAnException()
        {
            // Arrange.

            // Act.
            CssCompressor.Compress(null);

            // Assert.
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet1.css", "Cascading Style Sheet Files")]
        public void CompressCssWithASpecificColumnWidthSucessfullyCompressesText()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css, 73, CssCompressionType.StockYuiCompressor, true);
            
            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet2.css", "Cascading Style Sheet Files")]
        public void CompressCssWithNoFileContentButFileExistsReturnsAnEmptyResult()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet2.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css);

            // Assert.
            Assert.IsTrue(string.IsNullOrEmpty(compressedCss));
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet1.css", "Cascading Style Sheet Files")]
        public void CompressCssWithMichaelAshsRegexEnhancementsAndNoColumnWidthReturnsSomeCompressedCss()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css,0,CssCompressionType.MichaelAshRegexEnhancements, true);
            
            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);
        }
        
        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet1.css", "Cascading Style Sheet Files")]
        public void CompressCssWithMichaelAshsRegexEnhancementsAndaSpecificColumnWidthReturnsSomeCompressedCss()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css,73,CssCompressionType.MichaelAshRegexEnhancements, true);
            
            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
            Assert.IsTrue(css.Length > compressedCss.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\SampleStylesheet-MissingClosingCommentSymbol-CP3723.css", "Cascading Style Sheet Files")]
        public void CompressBadCssCP3723ReturnsCompressedCss()
        {
            // Arrange.
            string css = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet-MissingClosingCommentSymbol-CP3723.css");

            // Act.
            string compressedCss = CssCompressor.Compress(css);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedCss));
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\background-position.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\background-position.css.min", "Cascading Style Sheet Files")]
        public void BackgroundPositionTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\background-position.css", @"Cascading Style Sheet Files\background-position.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\border-none.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\border-none.css.min", "Cascading Style Sheet Files")]
        public void BorderNoneCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\border-none.css", @"Cascading Style Sheet Files\border-none.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\box-model-hack.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\box-model-hack.css.min", "Cascading Style Sheet Files")]
        public void BoxModelHackCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\box-model-hack.css", @"Cascading Style Sheet Files\box-model-hack.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\Bug2527974.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\Bug2527974.css.min", "Cascading Style Sheet Files")]
        public void Bug2527974CssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\Bug2527974.css", @"Cascading Style Sheet Files\Bug2527974.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2527991.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2527991.css.min", "Cascading Style Sheet Files")]
        public void Bug2527991CssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\bug2527991.css", @"Cascading Style Sheet Files\bug2527991.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2527998.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2527998.css.min", "Cascading Style Sheet Files")]
        public void Bug2527998CssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\bug2527998.css", @"Cascading Style Sheet Files\bug2527998.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2528034.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\bug2528034.css.min", "Cascading Style Sheet Files")]
        public void Bug2528034CssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\bug2528034.css", @"Cascading Style Sheet Files\bug2528034.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\charset-media.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\charset-media.css.min", "Cascading Style Sheet Files")]
        public void CharsetMediaCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\charset-media.css", @"Cascading Style Sheet Files\charset-media.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\color.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\color.css.min", "Cascading Style Sheet Files")]
        public void ColorCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\color.css", @"Cascading Style Sheet Files\color.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\comment.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\comment.css.min", "Cascading Style Sheet Files")]
        public void CommentCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\comment.css", @"Cascading Style Sheet Files\comment.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\concat-charset.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\concat-charset.css.min", "Cascading Style Sheet Files")]
        public void ConcatCharsetCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\concat-charset.css", @"Cascading Style Sheet Files\concat-charset.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\decimals.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\decimals.css.min", "Cascading Style Sheet Files")]
        public void DecimalsCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\decimals.css", @"Cascading Style Sheet Files\decimals.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\dollar-header.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\dollar-header.css.min", "Cascading Style Sheet Files")]
        public void DollarHeaderCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\dollar-header.css", @"Cascading Style Sheet Files\dollar-header.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\font-face.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\font-face.css.min", "Cascading Style Sheet Files")]
        public void FontFaceCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\font-face.css", @"Cascading Style Sheet Files\font-face.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\ie5mac.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\ie5mac.css.min", "Cascading Style Sheet Files")]
        public void Ie5MacCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\ie5mac.css", @"Cascading Style Sheet Files\ie5mac.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\media-empty-class.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-empty-class.css.min", "Cascading Style Sheet Files")]
        public void MediaEmptyClassCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\media-empty-class.css", @"Cascading Style Sheet Files\media-empty-class.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod, Ignore]
        [DeploymentItem(@"Cascading Style Sheet Files\media-empty-class.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-empty-class.css.min", "Cascading Style Sheet Files")]
        public void MediaEmptyClassCssTest_With_MichaelAshRegexEnhancements()
        {
            // Currently does not produce the same results as the regular compressor - is this correct?
            CompareTwoFiles(@"Cascading Style Sheet Files\media-empty-class.css", @"Cascading Style Sheet Files\media-empty-class.css.min", CompressorType.CascadingStyleSheet, ComparingTwoFileTypes.Content, CssCompressionType.MichaelAshRegexEnhancements);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\media-multi.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-multi.css.min", "Cascading Style Sheet Files")]
        public void MediaMultiCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\media-multi.css", @"Cascading Style Sheet Files\media-multi.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\media-multi.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-multi.css.min", "Cascading Style Sheet Files")]
        public void MediaMultiCssTest_With_MichaelAshRegexEnhancements()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\media-multi.css", @"Cascading Style Sheet Files\media-multi.css.min", CompressorType.CascadingStyleSheet, ComparingTwoFileTypes.Content, CssCompressionType.MichaelAshRegexEnhancements);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\media-test.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-test.css.min", "Cascading Style Sheet Files")]
        public void MediaTestCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\media-test.css", @"Cascading Style Sheet Files\media-test.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\media-test.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\media-test.css.min", "Cascading Style Sheet Files")]
        public void MediaTestCssTest_With_MichaelAshRegexEnhancements()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\media-test.css", @"Cascading Style Sheet Files\media-test.css.min", CompressorType.CascadingStyleSheet, ComparingTwoFileTypes.Content, CssCompressionType.MichaelAshRegexEnhancements);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\opacity-filter.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\opacity-filter.css.min", "Cascading Style Sheet Files")]
        public void OpacityFilterCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\opacity-filter.css", @"Cascading Style Sheet Files\opacity-filter.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\preserve-new-line.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\preserve-new-line.css.min", "Cascading Style Sheet Files")]
        public void PreserveNewLineCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\preserve-new-line.css", @"Cascading Style Sheet Files\preserve-new-line.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\preserve-strings.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\preserve-strings.css.min", "Cascading Style Sheet Files")]
        public void PreserveStringsCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\preserve-strings.css", @"Cascading Style Sheet Files\preserve-strings.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\pseudo-first.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\pseudo-first.css.min", "Cascading Style Sheet Files")]
        public void PseudoFirstCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\pseudo-first.css", @"Cascading Style Sheet Files\pseudo-first.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\pseudo.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\pseudo.css.min", "Cascading Style Sheet Files")]
        public void PseudoCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\pseudo.css", @"Cascading Style Sheet Files\pseudo.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\special-comments.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\special-comments.css.min", "Cascading Style Sheet Files")]
        public void SpecialCommentsCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\special-comments.css", @"Cascading Style Sheet Files\special-comments.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\star-underscore-hacks.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\star-underscore-hacks.css.min", "Cascading Style Sheet Files")]
        public void StarUnderscoreHacksCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\star-underscore-hacks.css", @"Cascading Style Sheet Files\star-underscore-hacks.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\string-in-comment.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\string-in-comment.css.min", "Cascading Style Sheet Files")]
        public void StringInCommentCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\string-in-comment.css", @"Cascading Style Sheet Files\string-in-comment.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\webkit-transform.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\webkit-transform.css.min", "Cascading Style Sheet Files")]
        public void WebkitTransformCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\webkit-transform.css", @"Cascading Style Sheet Files\webkit-transform.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\zeros.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\zeros.css.min", "Cascading Style Sheet Files")]
        public void ZerosCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\zeros.css", @"Cascading Style Sheet Files\zeros.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        public void When_The_CompressionType_Is_None_The_Input_Is_Returned_Unchanged()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "body      {  color : blue    }  table {   border    :   2 px;    }  /*  Some Comment */";
            
            // Act
            var actual = CssCompressor.Compress(source, 0, CssCompressionType.None, false);

            // Assert
            Assert.AreEqual(source, actual);
        }

        [TestMethod]
        [Description("Raised in issue 9529")]
        public void A_Background_Retains_The_Space_Between_The_Colour_And_The_Data_Ur()
        {
            const string source = @"
                ui-widget-shadow { 
                    margin: -5px 0 0 -5px; 
                    padding: 5px; 
                    background: #000000 url(""data:image/png;charset=utf-8;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAABkCAYAAAD0ZHJ6AAAAeUlEQVRoge3OMQHAIBAAsQf/nlsJDDfAkCjImplvHrZvB04EK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEqx92LQHHRpDUNwAAAABJRU5ErkJggg=="") 50% 50% repeat-x; 
                    opacity: .20;
                    filter:Alpha(Opacity=20);
                    -moz-border-radius: 5px; 
                    -khtml-border-radius: 5px;
                    -webkit-border-radius: 5px; 
                    border-radius: 5px; 
                 }";
            const string expected = @"ui-widget-shadow{margin:-5px 0 0 -5px;padding:5px;background:#000 url(""data:image/png;charset=utf-8;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAABkCAYAAAD0ZHJ6AAAAeUlEQVRoge3OMQHAIBAAsQf/nlsJDDfAkCjImplvHrZvB04EK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEqx92LQHHRpDUNwAAAABJRU5ErkJggg=="") 50% 50% repeat-x;opacity:.20;filter:Alpha(Opacity=20);-moz-border-radius:5px;-khtml-border-radius:5px;-webkit-border-radius:5px;border-radius:5px}";
            
            // Act
            var actual = CssCompressor.Compress(source);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}