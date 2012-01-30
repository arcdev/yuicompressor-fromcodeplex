using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class JavaScriptCompressorTest : TestBase
    {
        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript1.js", "Javascript Files")]
        public void CompressSampleJavaScript1ReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript1.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript2.js", "Javascript Files")]
        public void CompressSampleJavaScript2ReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript2.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript3.js", "Javascript Files")]
        public void CompressSampleJavaScript3ReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript3.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\jquery-1.2.6-vsdoc.js", "Javascript Files")]
        public void CompressJQuery126VSDocReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\jquery-1.2.6-vsdoc.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
            Assert.AreEqual(61591, compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\jquery-1.3.1.js", "Javascript Files")]
        public void CompressJQuery131JavascriptReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript4.js", "Javascript Files")]
        public void CompressWithObfuscationTest()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript4.js");

            // Act.
            string compressedNotObfuscatedJavascript = JavaScriptCompressor.Compress(javascript, true, false, false,
                                                                                     false, -1);
            string compressedObfuscatedJavascript = JavaScriptCompressor.Compress(javascript, true, true, false, false,
                                                                                  -1);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedNotObfuscatedJavascript));
            Assert.IsTrue(javascript.Length > compressedNotObfuscatedJavascript.Length);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedObfuscatedJavascript));
            Assert.IsTrue(javascript.Length > compressedObfuscatedJavascript.Length);

            // Is the obfuscated smaller?
            Assert.IsTrue(compressedObfuscatedJavascript.Length < compressedNotObfuscatedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript5.js", "Javascript Files")]
        public void CompressNestedIdentifiersTest()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript5.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, true, true, false, false, -1);

            // Assert.
            Assert.IsTrue(compressedJavascript.Length < javascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript6.js", "Javascript Files")]
        public void CompressRegExWithUnicodeTest()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript6.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, true, true, false, false, -1);
            string compressedJavascriptNoObfuscation = JavaScriptCompressor.Compress(javascript, true, false, false,
                                                                                     false, -1);

            // Assert.
            Assert.IsFalse(compressedJavascript.Contains(@"}get var"));
            Assert.IsTrue(compressedJavascript.Contains(@"\w\u0128"));
            Assert.IsTrue(compressedJavascriptNoObfuscation.Contains(@"\w\u0128"));
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\jquery-1.3.1.js", "Javascript Files")]
        public void CompressJQuery131WithNoMungeReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, false, false, false, false, -1);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
            Assert.AreEqual(71146, compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript-CP46679.js", "Javascript Files")]
        public void CompressJavascriptCodePlex46679ReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript-CP46679.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, false, true, false, false, -1);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript-ignoreEval.js", "Javascript Files")]
        public void CompressJavascriptIgnoreEvalReturnsCompressedJavascript()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, false, true, false, false, -1,
                                                                        Encoding.Default, null, true);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsFalse(compressedJavascript.Contains("number"),
                           "Turning on ignoreEval should compress functions that call eval");
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript-ignoreEval.js", "Javascript Files")]
        public void CompressJavascriptRespectEval()
        {
            // Arrange.
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript, false, true, false, false, -1,
                                                                        Encoding.Default, null, false);

            // Assert.
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(compressedJavascript.Contains("number"),
                          "Functions that call eval should not be compressed when ignoreEval is false");
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\SampleJavaScript-ignoreEval.js", "Javascript Files")]
        public void CompressFull_DontChangeThreadCulture()
        {
            // Arrange.
            var currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            var currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;
            string javascript = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");

                // Act.
                JavaScriptCompressor.Compress(javascript, false, true, false, false, -1, Encoding.Default, null, false);

                // Assert.
                Assert.AreEqual(Thread.CurrentThread.CurrentCulture, CultureInfo.CreateSpecificCulture("fr-FR"));
                Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, CultureInfo.CreateSpecificCulture("fr-FR"));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadUiCulture;
            }

            // More Asserts.
            // Assert.
            Assert.AreEqual(currentThreadCulture, Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual(currentThreadUiCulture, Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\_munge.js", "Javascript Files")]
        [DeploymentItem(@"Javascript Files\_munge.js.min", "Javascript Files")]
        public void MungeJsTest()
        {
            CompareTwoFiles(@"Javascript Files\_munge.js", @"Javascript Files\_munge.js.min", CompressorType.JavaScript);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\_string_combo.js", "Javascript Files")]
        [DeploymentItem(@"Javascript Files\_string_combo.js.min", "Javascript Files")]
        public void StringComboJsTest()
        {
            CompareTwoFiles(@"Javascript Files\_string_combo.js", @"Javascript Files\_string_combo.js.min",
                            CompressorType.JavaScript);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\_syntax_error.js", "Javascript Files")]
        [DeploymentItem(@"Javascript Files\_syntax_error.js.min", "Javascript Files")]
        public void SyntaxErrorJsTest()
        {
            // Because the Java code uses a Hashtable to determine what variables names can be obfuscated, we can't do an exact file comapre. But we can
            // do a file LENGTH compare .. which might be a bit closer to fair Assert test.
            CompareTwoFiles(@"Javascript Files\_syntax_error.js", @"Javascript Files\_syntax_error.js.min",
                            CompressorType.JavaScript, ComparingTwoFileTypes.FileLength);
        }

        [TestMethod]
        public void Bug8092JavascriptTest()
        {
            // Arrange.
            string javascript = string.Format("var anObject = {{{0}property: \"value\",{0}propertyTwo: \"value2\"{0}}};{0}{0}alert('single quoted string ' + anObject.property + ' end string');{0}// Outputs: single quoted string value end string", Environment.NewLine);

            // Act.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Assert.
            Assert.AreEqual("var anObject={property:\"value\",propertyTwo:\"value2\"};alert(\"single quoted string \"+anObject.property+\" end string\");",
                compressedJavascript);
        }

        [TestMethod]
        public void When_The_CompressionType_Is_None_The_Input_Is_Returned_Unchanged()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            JavaScriptCompressor compressor = new JavaScriptCompressor(source);
            compressor.CompressionType = JavaScriptCompressionType.None;

            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.AreEqual(source, actual);
        }

        [TestMethod]
        public void The_Input_Will_Be_Compressed_By_Default()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            const string expected = @"function foo(){return""bar""};";
            JavaScriptCompressor compressor = new JavaScriptCompressor(source);

            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Item 9856")]
        public void Decimals_Will_Be_Reasonably_Accurate()
        {
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just confirming some of the more accurate values
            // See this thread for more: http://yuicompressor.codeplex.com/discussions/279118

            // Arrange
            var js = @"var serverResolutions = [ 
                            156543.03390625,
                            9783.939619140625, 
                            611.4962261962891,
                            0.07464553542435169
                       ];";
            JavaScriptCompressor compressor = new JavaScriptCompressor(js);

            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.AreEqual("var serverResolutions=[156543.03390625,9783.939619140625,611.4962261962891,0.07464553542435169];", actual);
        }

        [TestMethod]
        [Description("Item 9856")]
        public void Decimals_Will_Not_Be_Entirely_Accurate_Until_We_Implement_A_Proper_Solution()
        {
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just checking that some inaccuracies still exist & can be removed once we have a proper solution
            // If this test fails, it means accuracy is now sorted!
            // See this thread for more: http://yuicompressor.codeplex.com/discussions/279118


            var js = @"var serverResolutions = [ 
                            152.87405654907226,
                            0.14929107084870338
                       ];";
            JavaScriptCompressor compressor = new JavaScriptCompressor(js);

            var actual = compressor.Compress();

            Assert.AreNotEqual("var serverResolutions=[152.87405654907226,0.14929107084870338];", actual);
        }

        [TestMethod]
        public void Errors_Will_Include_Line_Numbers()
        {
            // Arrange
            var js = @"var terminated = 'some string';
                       var unterminated = 'some other;";
            JavaScriptCompressor compressor = new JavaScriptCompressor(js);   

            // Act
            try
            {
                compressor.Compress();
                Assert.Fail("Succeeded");
            }
            catch (InvalidOperationException iox)
            {
                // Assert
                Assert.IsTrue(iox.Message.Contains("Line: 2"));
            }
        }

        [TestMethod]
        public void Warnings_Will_Include_Line_Numbers_Where_Available()
        {
            // Arrange
            const string js = @"function foo(bar, bar) {}";
            var compressor = new JavaScriptCompressor(js);

            // Act
            compressor.Compress();

            // Assert
            var reporter = (CustomErrorReporter) compressor.ErrorReporter;
            Assert.AreNotEqual(0, reporter.ErrorMessages.Count, "No Messages");

            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("[WARNING] Duplicate parameter name \"bar\""))
                {
                    Assert.IsTrue(errorMessage.Contains("Line: 1"), "\"Line: 1\" not found in: " + errorMessage);
                    return;
                }
            }
            Assert.Fail("Message not found");
        }

        [TestMethod]
        public void Warnings_Will_Not_Include_Line_Numbers_Where_Not_Available()
        {
            // Arrange
            const string js = @"var foo = 'bar';
                                var foo = 'bar';";
            var compressor = new JavaScriptCompressor(js);

            // Act
            compressor.Compress();

            // Assert
            var reporter = (CustomErrorReporter) compressor.ErrorReporter;
            Assert.AreNotEqual(0, reporter.ErrorMessages.Count, "No Messages");
            
            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("The variable foo has already been declared in the same scope"))
                {
                    Assert.IsFalse(errorMessage.Contains("Line:"), "\"Line:\" found in: "+ errorMessage);
                    return;
                }
            }
            Assert.Fail("Message not found");
        }
    }
}