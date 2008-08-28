﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahoo.Yui.Compressor;


namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class JavaScriptCompressorTest
    {
        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript1.js")]
        [DeploymentItem("bin\\SampleJavaScript2.js")]
        [DeploymentItem("bin\\SampleJavaScript3.js")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressTest()
        {
            string javascript;
            string compressedJavascript;


            // First load up some simple javascript.
            javascript = File.ReadAllText("SampleJavaScript1.js");

            // Now compress the small javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Now lets try the big mother fraker.
            javascript = File.ReadAllText("SampleJavaScript2.js");

            // Now compress the small javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // And now some weird \n test.
            javascript = File.ReadAllText("SampleJavaScript3.js");

            // Now compress the small javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript);
            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            javascript = "for(var x in _2f[i]){};";
            compressedJavascript = JavaScriptCompressor.Compress(javascript);

            // Expected failure.
            JavaScriptCompressor.Compress(null);
        }

        [TestMethod]
        [DeploymentItem("bin\\SampleJavaScript4.js")]
        public void CompressWithObfuscationTest()
        {
            string javascript;
            string compressedJavascript;
            int notObfuscatedLength;


            // Now lets try the big mother fraker.
            javascript = File.ReadAllText("SampleJavaScript4.js");

            // Now compress the small javascript without obfuscating.
            compressedJavascript = JavaScriptCompressor.Compress(javascript,
                true,
                false,
                true,
                false);

            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            notObfuscatedLength = compressedJavascript.Length;

            // Now obfuscate that same javascript.
            compressedJavascript = JavaScriptCompressor.Compress(javascript,
                true,
                true,
                true,
                false);

            Assert.IsTrue(!string.IsNullOrEmpty(compressedJavascript));
            Assert.IsTrue(javascript.Length > compressedJavascript.Length);

            // Is the obfuscated smaller?
            Assert.IsTrue(compressedJavascript.Length < notObfuscatedLength);
        }
    }
}