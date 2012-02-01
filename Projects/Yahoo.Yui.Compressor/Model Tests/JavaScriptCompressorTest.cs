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
        public void A_New_Line_Appended_In_The_Source_Is_Retained_In_The_Output()
        {
            // Arrange.
            const string source = @"fred += '\n'; ";
            const string expected = @"fred+=""\n"";";

            // Act & Assert
            CompressAndCompare(source, expected);
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
        public void CompressRegExWithUnicodeTest()
        {
            // Arrange.
            const string source = @"// Helper function used by the dimensions and offset modules
                                    function num(elem, prop) {
                                       return elem[0] && parseInt(jQuery.curCSS(elem[0], prop, true), 10) || 0;
                                   } 
                            
                                   var chars = jQuery.browser.safari && parseInt(jQuery.browser.version) < 417 ? 
                                               ""(?:[\\w*_-]|\\\\.)"" : ""(?:[\\w\u0128-\uFFFF*_-]|\\\\.)"",
                                   quickChild = new RegExp(""^>\\s*("" + chars + ""+)""),
                                   quickID = new RegExp(""^("" + chars + ""+)(#)("" + chars + ""+)""),
                                   quickClass = new RegExp(""^([#.]?)("" + chars + ""*)"");";
            
            // Act.
            var compressedJavascript = JavaScriptCompressor.Compress(source, true, true, false, false, -1);
            var compressedJavascriptNoObfuscation = JavaScriptCompressor.Compress(source, true, false, false, false, -1);

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
        [Description("http://yuicompressor.codeplex.com/discussions/46679")]
        public void Compressing_An_ExtJs_Definition_Works_As_Expected()
        {
            // Arrange
            const string source = @"controls.SearchCombo = Ext.extend(Ext.form.ComboBox, {
                                        forceSelection: true,
                                        loadingText: 'Searching...',
                                        minChars: 3,
                                        mode: 'remote',
                                        msgTarget: 'side',
                                        queryDelay: 300,
                                        queryParam: 'q',
                                        selectOnFocus: true,
                                        typeAhead: false
                                    }); ";

            const string expected = @"controls.SearchCombo=Ext.extend(Ext.form.ComboBox,{forceSelection:true,loadingText:""Searching..."",minChars:3,mode:""remote"",msgTarget:""side"",queryDelay:300,queryParam:""q"",selectOnFocus:true,typeAhead:false});";

            // Act & Assert
            CompressAndCompare(source, expected);
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
        public void If_CultureInfo_Is_Supplied_Then_The_Original_Thread_Culture_Is_Restored_After_Compression()
        {
            // Arrange
            // Save existing culture
            var currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            var currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;

            var expectedCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            try
            {
                // Change the culture to something specific
                Thread.CurrentThread.CurrentCulture = expectedCulture;
                Thread.CurrentThread.CurrentUICulture = expectedCulture;

                // Act
                // Pass in some other culture
                JavaScriptCompressor.Compress("var stuff = {foo:0.9, faa:3};", false, true, false, false, -1, Encoding.Default, CultureInfo.CreateSpecificCulture("it-IT"), false);

                // Assert
                // Check the culture is thee sam
                Assert.AreEqual(Thread.CurrentThread.CurrentCulture, expectedCulture, "Test CurrentCulture");
                Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, expectedCulture, "Test CurrentUICulture");
            }
            finally
            {
                // Restore original culture coming into the tests
                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadUiCulture;
                
                // heck the culture is now restored
                Assert.AreEqual(currentThreadCulture, Thread.CurrentThread.CurrentCulture, "Original CurrentCulture");
                Assert.AreEqual(currentThreadUiCulture, Thread.CurrentThread.CurrentUICulture, "Original CurrentUICulture");
            }

        }

        [TestMethod]
        [Description("http://yuicompressor.codeplex.com/discussions/243522")]
        public void If_CultureInfo_Is_Supplied_Then_The_Output_Respects_It_Irrespective_Of_The_Current_Thread_Culture()
        {
            // Arrange
            var currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            var currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string  expected = "var stuff={foo:0.9,faa:3};";

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = JavaScriptCompressor.Compress(source, false, false, false, false, 200, Encoding.UTF8, CultureInfo.InvariantCulture);

                // Assert.
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadUiCulture;
            }
        }

        [TestMethod]
        public void If_CultureInfo_Is_Not_Supplied_Then_The_Output_Respects_The_Current_Thread_Culture()
        {
            // Arrange
            var currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            var currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string expected = "var stuff={foo:0,9,faa:3};";

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = JavaScriptCompressor.Compress(source, false, false, false, false, 200);

                // Assert.
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadUiCulture;
            }
        }

        [TestMethod]
        public void The_Output_Is_Obfuscated_When_IsObfuscateJavascript_Is_True()
        {
            // Arrange
            const string source =
                @"(function() {
                    var w = window;
                    w.hello = function(a, abc) {
                    ""a:nomunge"";
                    w.alert(""Hello, "" + a);
                };
            })();";

            const string expected = @"(function(){var a=window;a.hello=function(a,b){a.alert(""Hello, ""+a)}})();";

            // Act
            var actual = JavaScriptCompressor.Compress(source, true, true, false, false, -1);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void The_Output_Is_Not_Obfuscated_When_IsObfuscateJavascript_Is_False()
        {
            // Arrange
            const string source =
                @"(function() {
                    var w = window;
                    w.hello = function(a, abc) {
                    ""a:nomunge"";
                    w.alert(""Hello, "" + a);
                };
            })();";

            const string expected = @"(function(){var w=window;w.hello=function(a,abc){w.alert(""Hello, ""+a)}})();";

            // Act
            var actual = JavaScriptCompressor.Compress(source, true, false, false, false, -1);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Concatenated_Strings_Are_Combined()
        {
            // Arrange
            const string source = @"function test(){
                                        var a = ""a"" +
                                        ""b"" +
                                        ""c"";
                                    }";
            const string expected = @"function test(){var a=""abc""};";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [DeploymentItem(@"Javascript Files\_syntax_error.js", "Javascript Files")]
        [DeploymentItem(@"Javascript Files\_syntax_error.js.min", "Javascript Files")]
        public void SyntaxErrorJsTest()
        {
            // Because the Java code uses a Hashtable to determine what variables names can be obfuscated, we can't do an exact file compare. But we can
            // do a file LENGTH compare .. which might be a bit closer to fair Assert test.
            CompareTwoFiles(@"Javascript Files\_syntax_error.js", @"Javascript Files\_syntax_error.js.min",
                            CompressorType.JavaScript, ComparingTwoFileTypes.FileLength);
        }

        [TestMethod]
        public void Bug8092_Should_Be_Fixed()
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
            var compressor = new JavaScriptCompressor(source) { CompressionType = JavaScriptCompressionType.None };

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

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [TestMethod]
        [Description("Item 9856 / http://yuicompressor.codeplex.com/discussions/279118")]
        public void Decimals_Will_Be_Reasonably_Accurate()
        {
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just confirming some of the more accurate values

            // Arrange
            const string source = @"var serverResolutions = [ 
                                        156543.03390625,
                                        9783.939619140625, 
                                        611.4962261962891,
                                        0.07464553542435169
                                   ];";
            const string expected = @"var serverResolutions=[156543.03390625,9783.939619140625,611.4962261962891,0.07464553542435169];";

            // Act
            CompressAndCompare(source, expected);
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

            // Arrange
            const string source = @"var serverResolutions = [ 
                                      152.87405654907226,
                                      0.14929107084870338
                                  ];";
            var compressor = new JavaScriptCompressor(source);
            
            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.AreNotEqual("var serverResolutions=[152.87405654907226,0.14929107084870338];", actual);
        }

        [TestMethod]
        public void Errors_Will_Include_Line_Numbers()
        {
            // Arrange
            const string source = @"var terminated = 'some string';
                                    var unterminated = 'some other;";
            var compressor = new JavaScriptCompressor(source);   

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
            const string source = @"function foo(bar, bar) {}";
            var compressor = new JavaScriptCompressor(source);

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
            const string source = @"var foo = 'bar';
                                    var foo = 'bar';";
            var compressor = new JavaScriptCompressor(source);

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

        private void CompressAndCompare(string source, string expected)
        {
            // Act
            var actual = JavaScriptCompressor.Compress(source, false, false, false, false, -1);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}