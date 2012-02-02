using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class JavaScriptCompressorTest 
    {
        [Test]
        public void CompressSampleJavaScript1ReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript1.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void CompressSampleJavaScript2ReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript2.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void A_New_Line_Appended_In_The_Source_Is_Retained_In_The_Output()
        {
            // Arrange.
            const string source = @"fred += '\n'; ";
            const string expected = @"fred+=""\n"";";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void CompressJQuery126VSDocReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.2.6-vsdoc.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
            Assert.That(actual.Length, Is.EqualTo(61591), "Exact Length");
        }

        [Test]
        public void CompressJQuery131JavascriptReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void CompressWithObfuscationTest()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript4.js");

            // Act.
            var actualCompressedNotObfuscatedJavascript = JavaScriptCompressor.Compress(source, true, false, false,
                                                                                     false, -1);
            var actualCompressedObfuscatedJavascript = JavaScriptCompressor.Compress(source, true, true, false, false,
                                                                                  -1);

            // Assert.
            Assert.That(actualCompressedNotObfuscatedJavascript, Is.Not.Null.Or.Empty, "Not Obfuscated Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actualCompressedNotObfuscatedJavascript.Length), "Not Obfuscated Not Greater");
            Assert.That(actualCompressedObfuscatedJavascript, Is.Not.Null.Or.Empty, "Obfuscated Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actualCompressedObfuscatedJavascript.Length), "Obfuscated Not Greater");

            // Is the obfuscated smaller?
            Assert.That(actualCompressedObfuscatedJavascript.Length, Is.LessThan(actualCompressedNotObfuscatedJavascript.Length), "Obfuscated not smaller than Not Obfuscated");
        }

        [Test]
        public void CompressNestedIdentifiersTest()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript5.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source, true, true, false, false, -1);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
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
            Assert.That(compressedJavascript, Is.Not.StringContaining(@"}get var"));
            Assert.That(compressedJavascript, Is.StringContaining(@"\w\u0128"));
            Assert.That(compressedJavascriptNoObfuscation, Is.StringContaining(@"\w\u0128"));
        }

        [Test]
        public void CompressJQuery131WithNoMungeReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source, false, false, false, false, -1);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
            Assert.That(actual.Length, Is.EqualTo(71146), "Exact Length");
        }

        [Test]
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

        [Test]
        public void CompressJavascriptIgnoreEvalReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source, false, true, false, false, -1,
                                                                        Encoding.Default, null, true);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(actual, Is.Not.StringContaining("number"), "Turning on ignoreEval should compress functions that call eval");
        }

        [Test]
        public void CompressJavascriptRespectEval()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");

            // Act.
            var actual = JavaScriptCompressor.Compress(source, false, true, false, false, -1,
                                                                        Encoding.Default, null, false);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(actual, Is.StringContaining("number"), "Functions that call eval should not be compressed when ignoreEval is false");
        }

        [Test]
        public void If_CultureInfo_Is_Supplied_Then_The_Original_Thread_Culture_Is_Restored_After_Compression()
        {
            // Arrange
            // Save existing culture
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;

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
                Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(expectedCulture), "Test CurrentCulture");
                Assert.That(Thread.CurrentThread.CurrentUICulture, Is.EqualTo(expectedCulture), "Test CurrentUICulture");
            }
            finally
            {
                // Restore original culture coming into the tests
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
                
                // heck the culture is now restored
                Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(originalThreadCulture) , "Original CurrentCulture");
                Assert.That(Thread.CurrentThread.CurrentUICulture, Is.EqualTo(originalThreadUICulture), "Original CurrentUICulture");
            }

        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/discussions/243522")]
        public void If_CultureInfo_Is_Supplied_Then_The_Output_Respects_It_Irrespective_Of_The_Current_Thread_Culture()
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string expected = "var stuff={foo:0.9,faa:3};";

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = JavaScriptCompressor.Compress(source, false, false, false, false, 200, Encoding.UTF8, CultureInfo.InvariantCulture);

                // Assert.
                Assert.That(actual, Is.EqualTo(expected));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
        }

        [Test]
        public void If_CultureInfo_Is_Not_Supplied_Then_The_Output_Respects_The_Current_Thread_Culture()
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string expected = "var stuff={foo:0,9,faa:3};";

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = JavaScriptCompressor.Compress(source, false, false, false, false, 200);

                // Assert.
                Assert.That(actual, Is.EqualTo(expected));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
        }

        [Test]
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
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

        [Test]
        public void SyntaxErrorJsTest()
        {
            // Arrange
            var source = File.ReadAllText(@"Javascript Files\_syntax_error.js", Encoding.UTF8);
            var expected = File.ReadAllText(@"Javascript Files\_syntax_error.js.min");

            // Act
            var actual = JavaScriptCompressor.Compress(source);

            // Assert
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null Or Empty");
            // Because the Java code uses a Hashtable to determine what variables names can be obfuscated, we can't do an exact file compare. But we can
            // do a file LENGTH compare .. which might be a bit closer to fair Assert test.
            Assert.That(actual.Length, Is.EqualTo(expected.Length), "Length mismatch");
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/8092")]
        public void Bug8092_Should_Be_Fixed()
        {
            // Arrange
            var source = string.Format("var anObject = {{{0}property: \"value\",{0}propertyTwo: \"value2\"{0}}};{0}{0}alert('single quoted string ' + anObject.property + ' end string');{0}// Outputs: single quoted string value end string", Environment.NewLine);
            const string expected = "var anObject={property:\"value\",propertyTwo:\"value2\"};alert(\"single quoted string \"+anObject.property+\" end string\");";

            // Act
            var actual = JavaScriptCompressor.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void When_The_CompressionType_Is_None_The_Input_Is_Returned_Unchanged()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            var compressor = new JavaScriptCompressor(source) { CompressionType = JavaScriptCompressionType.None };

            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.That(actual, Is.EqualTo(source));
        }

        [Test]
        public void The_Input_Will_Be_Compressed_By_Default()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            const string expected = @"function foo(){return""bar""};";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9856")]
        public void Decimals_Will_Be_Reasonably_Accurate()
        {
            // Also see http://yuicompressor.codeplex.com/discussions/279118
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

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9856")]
        public void Decimals_Will_Not_Be_Entirely_Accurate_Until_We_Implement_A_Proper_Solution()
        {
            // Also see http://yuicompressor.codeplex.com/discussions/279118
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just checking that some inaccuracies still exist & can be removed once we have a proper solution
            // If this test fails, it means accuracy is now sorted!

            // Arrange
            const string source = @"var serverResolutions = [ 
                                      152.87405654907226,
                                      0.14929107084870338
                                  ];";
            var compressor = new JavaScriptCompressor(source);
            
            // Act
            var actual = compressor.Compress();

            // Assert
            Assert.That(actual, Is.Not.EqualTo("var serverResolutions=[152.87405654907226,0.14929107084870338];"));
        }

        [Test]
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
                Assert.That(iox.Message, Is.StringContaining("Line: 2"));
            }
        }

        [Test]
        public void Warnings_Will_Include_Line_Numbers_Where_Available()
        {
            // Arrange
            const string source = @"function foo(bar, bar) {}";
            var compressor = new JavaScriptCompressor(source);

            // Act
            compressor.Compress();

            // Assert
            var reporter = (CustomErrorReporter) compressor.ErrorReporter;
            Assert.That(reporter.ErrorMessages.Count, Is.Not.EqualTo(0), "No Messages");

            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("[WARNING] Duplicate parameter name \"bar\""))
                {
                    Assert.That(errorMessage, Is.StringContaining("Line: 1"), "\"Line: 1\" not found in: " + errorMessage);
                    return;
                }
            }
            Assert.Fail("Message not found");
        }

        [Test]
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
            Assert.That(reporter.ErrorMessages.Count, Is.Not.EqualTo(0), "No Messages");
            
            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("The variable foo has already been declared in the same scope"))
                {
                    Assert.That(errorMessage, Is.Not.StringContaining("Line:"), "\"Line:\" found in: "+ errorMessage);
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    // ReSharper restore InconsistentNaming
}