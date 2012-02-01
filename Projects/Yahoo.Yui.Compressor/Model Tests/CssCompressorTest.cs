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
        public void A_Stylesheet_With_Empty_Content_Only_Returns_An_Empty_Result()
        {
            // Arrange.
            string source = @"body
                              {
                              }";

            // Act & Assert
            CompressAndCompare(source, string.Empty);
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
        [Description("PK to look at")]
        public void Compressing_Css_With_No_Closing_Comment_Symbol_Returns_Something()
        {
            // What is CP3723?  Test was originally called CompressBadCssCP3723ReturnsCompressedCss

            // Arrange.
            string source = @".moreactions_applyfilter_reset
                            {
                            text-align: right;
                            }

                            /* end of moreactions_filter";
            string expected = @".moreactions_applyfilter_reset{text-align:right}/*___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_0___";

            // Act & Assert
            CompressAndCompare(source, expected);
            // Original Assert was just that *something* was returned (ie result length > 0
            // I have set the expected result to be what is actually returned currently - is this correct?
        }

        [TestMethod]
        public void BackgroundPositionTest()
        {
            // Arrange
            const string source = @"a {background-position: 0 0 0 0;}
                                    b {BACKGROUND-POSITION: 0 0;}";
            const string expected = @"a{background-position:0 0}b{background-position:0 0}";

            // Act
            var actual = CssCompressor.Compress(source, -1, CssCompressionType.StockYuiCompressor, true);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Background_None_Will_Be_Replaced_With_Background_0()
        {
            // Arrange
            const string source = @"a {
                                        border: none;
                                    }
                                    s {border-top: none;}";
            const string expected = @"a{border:0}s{border-top:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Border_None_Will_Be_Replaced_With_Border_0()
        {
            // Arrange
            const string source = @"a {
                                        BACKGROUND: none;
                                    }
                                    b {BACKGROUND:none}";
            const string expected = @"a{background:0}b{background:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Box_Model_Hack_Css_Is_Compressed_Correctly()
        {
            // Box Model Hack: http://tantek.com/CSS/Examples/boxmodelhack.html

            // Arrange
            const string source = @"#elem { 
                                         width: 100px; 
                                         voice-family: ""\""}\""""; 
                                         voice-family:inherit;
                                         width: 200px;
                                        }
                                        html>body #elem {
                                         width: 200px;
                                        }";
            const string expected = @"#elem{width:100px;voice-family:""\""}\"""";voice-family:inherit;width:200px}html>body #elem{width:200px}";

            // Act & Assert
            CompressAndCompare(source, expected); 
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
        public void Color_Styles_Have_Rgb_Values_Replaced_With_Hex_Values()
        {
            // Arrange
            const string source = @".color {
                                      me: rgb(123, 123, 123);
                                      background: none repeat scroll 0 0 rgb(255, 0,0);
                                      alpha: rgba(1, 2, 3, 4);
                                    }";
            const string expected = @".color{me:#7b7b7b;background:none repeat scroll 0 0 #f00;alpha:rgba(1,2,3,4)}";
            
            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Color_Styles_Have_Unquoted_Hex_Values_Compressed_To_Shorter_Equivalents()
        {
            // Arrange
            const string source = @".color {
                                      impressed: #ffeedd;
                                      filter: chroma(color=""#FFFFFF"");
                                    }";
            const string expected = @".color{impressed:#fed;filter:chroma(color=""#FFFFFF"")}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [Description("Hack for IE7")]
        public void Empty_Comments_After_A_Child_Selector_Are_Preserved()
        {
            // Arrange
            const string source = @"html >/**/ body p {
                                        color: blue; 
                                    }
                                    ";
            const string expected = @"html>/**/body p{color:blue}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [DeploymentItem(@"Cascading Style Sheet Files\concat-charset.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\concat-charset.css.min", "Cascading Style Sheet Files")]
        public void ConcatCharsetCssTest()
        {
            CompareTwoFiles(@"Cascading Style Sheet Files\concat-charset.css", @"Cascading Style Sheet Files\concat-charset.css.min", CompressorType.CascadingStyleSheet);
        }

        [TestMethod]
        public void Decimal_Values_Are_Preserved_With_Leading_Zeroes_Removed()
        {
            // Arrange
            const string source = @"::selection { 
                                      margin: 0.6px 0.333pt 1.2em 8.8cm;
                                   }";
            const string expected = @"::selection{margin:.6px .333pt 1.2em 8.8cm}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [Description("PK to Look at")]
        public void A_Comment_With_Dollar_Header_Is_Preserved_But_Only_It_Seemes_Because_The_Preserve_Comment_Exclaimation_Exists()
        {
            // What is the significant of the $Header bit?

            // Arrange
            const string source = @"/*!
                                    $Header: /temp/dirname/filename.css 3 2/02/08 3:37p JSmith $
                                    */

                                    foo {
                                        bar: baz
                                    }";
            const string expected = @"/*!
                                    $Header: /temp/dirname/filename.css 3 2/02/08 3:37p JSmith $
                                    */foo{bar:baz}";

            // Act & Assert
            CompressAndCompare(source, expected);
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
        [DeploymentItem(@"Cascading Style Sheet Files\star-underscore-hacks.css", "Cascading Style Sheet Files")]
        [DeploymentItem(@"Cascading Style Sheet Files\star-underscore-hacks.css.min", "Cascading Style Sheet Files")]
        public void Star_And_Underscore_Hacks_Are_Preserved()
        {
            // Arrange
            const string source = @"#elementarr {
                                      width: 1px;
                                      *width: 3pt;
                                      _width: 2em;
                                    }";
            const string expected = @"#elementarr{width:1px;*width:3pt;_width:2em}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [Description("PK to Look at")]
        public void Some_Commments_Are_Preserved_Empty_But_Im_Not_Sure_If_This_Is_Correct_Or_Not()
        {
            // Arrange
            const string source = @"/* te "" st */
                                    a{a:1}
                                    /* quite "" quote ' \' \"" */
                                    /* ie mac \*/
                                    c {c : 3}
                                    /* end hiding */";
            const string expected = @"a{a:1}/*\*/c{c:3}/**/";

            // Act & Assert
            CompressAndCompare(source, expected);            
        }

        [TestMethod]
        [Description("PK to look at")]
        public void Comments_Marked_To_Be_Preserved_Are_Retained_In_The_Output()
        {
            // Not sure wy there is all the extra stuff ie mac, end hiding etc & why the end hiding /**/
            // are preserved but not the words "end hiding"?  Wouldn't expect either to be preserved....

            // Arrange
            const string source = @"/* te "" st */
                                    a{a:1}
                                    /*!""preserve"" me*/
                                    /* quite "" quote ' \' \"" */
                                    /* ie mac \*/
                                    c {c : 3}
                                    /* end hiding */";
            const string expected = @"a{a:1}/*!""preserve"" me*//*\*/c{c:3}/**/";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [Description("PK to look at")]
        public void Comments_Marked_To_Be_Preserved_Are_Retained_In_The_Output2()
        {
            // From what was originally "special-comments.css"
            // Does this test add anything that the test above (which also checks preserved comments) doesn't do?
            // It is clearer re: preserved comments.

            // Arrange
            const string source = @"/*!************88****
                                     Preserving comments
                                        as they are
                                     ********************
                                     Keep the initial !
                                     *******************/
                                    #yo {
                                        ma: ""ma"";
                                    }
                                    /*!
                                    I said
                                    pre-
                                    serve! */";

            const string expected = @"/*!************88****
                                     Preserving comments
                                        as they are
                                     ********************
                                     Keep the initial !
                                     *******************/#yo{ma:""ma""}/*!
                                    I said
                                    pre-
                                    serve! */";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Comments_In_A_Content_Value_Are_Retained_In_The_Output()
        {
            // Arrange
            const string source = @"a{content: ""/* comment in content*/""}";
            const string expected = @"a{content:""/* comment in content*/""}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Webkit_And_Moz_Transform_Origins_Have_Single_0_Replaced_With_Two_0s()
        {
            // Arrange
            const string source = @"a {-webkit-transform-origin: 0;}
                                    b {-webkit-transform-origin: 0 0;}
                                    c {-MOZ-TRANSFORM-ORIGIN: 0 }
                                    d {-MOZ-TRANSFORM-ORIGIN: 0 0;}";
            const string expected = @"a{-webkit-transform-origin:0 0}b{-webkit-transform-origin:0 0}c{-moz-transform-origin:0 0}d{-moz-transform-origin:0 0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        public void Zeroes_Have_The_Measurement_Type_Removed()
        {
            // Arrange
            const string source = @"a { 
                                      margin: 0px 0pt 0em 0%;
                                      _padding-top: 0ex;
                                      background-position: 0 0;
                                      padding: 0in 0cm 0mm 0pc
                                    }";
            const string expected = @"a{margin:0;_padding-top:0;background-position:0 0;padding:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
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

        private void CompressAndCompare(string source, string expected)
        {
            // Act
            var actual = CssCompressor.Compress(source, -1, CssCompressionType.StockYuiCompressor, true);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}