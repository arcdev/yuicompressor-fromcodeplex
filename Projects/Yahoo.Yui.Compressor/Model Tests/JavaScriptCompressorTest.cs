using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class JavaScriptCompressorTest
    {
        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript1.js")]
        [DeploymentItem("bin\\SampleJavaScript2.js")]
        [DeploymentItem("bin\\SampleJavaScript3.js")]
        [DeploymentItem("bin\\jquery-1.3.1.js")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressTest()
        {
            // First load up some simple javascript.
            string javascript = File.ReadAllText("SampleJavaScript1.js");

            // Now compress the small javascript.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Now lets try the big mother fraker.
            javascript = File.ReadAllText("SampleJavaScript2.js");

            // Now compress the small javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Special Ms AJAX test.
            var original = File.ReadAllText("SampleJavaScript2.js");
            var index = original.IndexOf("Sys.Serialization.JavaScriptSerializer._stringRegEx");
            var test = original.Substring(index);

            var compressor = new JavaScriptCompressor(original);

            var minified = compressor.Compress(false, true, false, false, -1);
            index = minified.IndexOf("Sys.Serialization.JavaScriptSerializer._stringRegEx");
            test = minified.Substring(index);


            // And now some weird \n test.
            javascript = File.ReadAllText("SampleJavaScript3.js");

            // Now compress the small javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Expected failure.
            JavaScriptCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript4.js")]
        public void CompressWithObfuscationTest()
        {
            // Now lets try the big mother fraker.
            string javascript = File.ReadAllText("SampleJavaScript4.js");

            // Now compress the small javascript without obfuscating.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript,
                                                                        true,
                                                                        false,
                                                                        false,
                                                                        false,
                                                                        -1);

            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            int notObfuscatedLength = compressedJavascript.Length;

            // Now obfuscate that same javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript,
                true,
                true,
                false,
                false,
                -1);

            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Is the obfuscated smaller?
            Assert.IsTrue(compressedJavascript.Length < notObfuscatedLength);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript5.js")]
        public void CompressNestedIdentifiersTest()
        {
            string javascript = File.ReadAllText("SampleJavaScript5.js");
            
            // Compress with full obfuscation
            string compressedJavascript = JavaScriptCompressor.Compress(javascript,
                true,
                true,
                false,
                false,
                -1);

            Assert.IsTrue(compressedJavascript.Length < javascript.Length);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript6.js")]
        public void CompressRegExWithUnicodeTest()
        {
            string javascript = File.ReadAllText("SampleJavaScript6.js");

            // Compress with full obfuscation
            string compressedJavascript = JavaScriptCompressor.Compress(javascript,
                true,
                true,
                false,
                false,
                -1);

            Assert.IsFalse(compressedJavascript.Contains(@"}get var"));
            Assert.IsTrue(compressedJavascript.Contains(@"\w\u0128"));

            // No obfuscation.
            string compressedJavascriptNoObfuscation = JavaScriptCompressor.Compress(javascript,
               true,
               false,
               false,
               false,
               -1);

            Assert.IsTrue(compressedJavascript.Contains(@"\w\u0128"));
        }

        [TestMethod]
        [DeploymentItem("bin\\jquery-1.2.6-vsdoc.js")]
        public void CompressFull_JQuery126VSDocTest()
        {
            // And now for the jQuery's..
            string javascript = File.ReadAllText("jquery-1.2.6-vsdoc.js");
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
            Assert.AreEqual(61591, compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem("bin\\jquery-1.3.1.js")]
        public void CompressFull_JQuery131Test()
        {
            // And now for the jQuery's..
            string javascript = File.ReadAllText("jquery-1.3.1.js");
            string compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
            Assert.AreEqual(55272, compressedJavascript.Length);
        }

        [TestMethod]
        [DeploymentItem("bin\\jquery-1.3.1.js")]
        public void CompressFull_JQuery131_NoMungeTest()
        {
            // And now for the jQuery's..
            string javascript = File.ReadAllText("jquery-1.3.1.js");

            // All defaults except obfuscation turned off.
            string compressedJavascript = JavaScriptCompressor.Compress(javascript,
                false,
                false,
                false,
                false,
                -1);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);
            Assert.AreEqual(71146, compressedJavascript.Length);
        }
        
    }
}