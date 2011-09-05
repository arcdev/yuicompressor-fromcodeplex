using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahoo.Yui.Compressor.MsBuildTask;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class CompressorTaskTest
    {
        [TestMethod]
        public void NoJavascriptFilesProvidedWithDoNotErrorWhenNoFilesAreProvidedSetToTrueReturnsAnEmptyString()
        {
            // Arrange.
            var compressorTask = new CompressorTask
                                     {
                                         CssOutputFile = "output.css",
                                         DeleteCssFiles = "false",
                                         DeleteJavaScriptFiles = "false",
                                         CssCompressionType = "YuiStockCompression",
                                         DisableOptimizations = "false",
                                         DoNotErrorWhenNoFilesAreProvided = "true",
                                         EncodingType = "Default",
                                         IsEvalIgnored = "false",
                                         JavaScriptOutputFile = "js_out.js",
                                         LineBreakPosition = "-1",
                                         LoggingType = "HardcoreBringItOn",
                                         ObfuscateJavaScript = "true",
                                         PreserveAllSemicolons = "false"
                                     };

            // Act.
            compressorTask.Execute();

            // Assert.
        }
    }
}