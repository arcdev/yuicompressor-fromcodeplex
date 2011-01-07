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
    }
}