using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahoo.Yui.Compressor.MsBuildTask;

namespace Yahoo.Yui.Compressor.Tests
{
    public abstract class CompressorTaskTest
    {
        [TestClass]
        public class When_Compressing_Javascript_Files : CompressorTaskTest
        {
            [TestMethod]
            public void True_Is_Returned_When_DoNotErrorWhenNoFilesAreProvided_Is_True_And_No_Javascript_Files_Are_Provided()
            {
                // Arrange
                var compressorTask = GetJavascriptCompressorFor("");
                compressorTask.DoNotErrorWhenNoFilesAreProvided = "true";

                // Act
                var worked = compressorTask.Execute();

                // Assert
                Assert.IsTrue(worked, "Did not Work");
            }

            private static CompressorTask GetJavascriptCompressorFor(string fileName)
            {
                var compressorTask = CreateCompressorTask();
                if (!string.IsNullOrEmpty(fileName))
                {
                    compressorTask.JavaScriptFiles = new ITaskItem[] { new TaskItem(@"Javascript Files\" + fileName + ".css") };
                    compressorTask.JavaScriptOutputFile = fileName + "min.js";
                }
                return compressorTask;
            }
        }

        [TestClass]
        public class When_Compressing_Css_Files : CompressorTaskTest
        {
            [TestMethod]
            public void True_Is_Returned_When_DoNotErrorWhenNoFilesAreProvided_Is_True_And_No_Css_Files_Are_Provided()
            {
                // Arrange
                var compressorTask = GetCssCompressorFor("");
                compressorTask.DoNotErrorWhenNoFilesAreProvided = "true";

                // Act
                var worked = compressorTask.Execute();

                // Assert
                Assert.IsTrue(worked, "Did not Work");
            }

            [TestMethod]
            [Description("Big: Item 9527 Fix")]
            [DeploymentItem(@"Cascading Style Sheet Files\bug9527.css", "Cascading Style Sheet Files")]
            public void PreserveCssComments_True_Preserves_Comments()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "true";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.IsTrue(worked, "Task Didn't work");
                Assert.IsTrue(compressedCss.Contains("/* comment */"), compressedCss);
            }

            [TestMethod]
            [DeploymentItem(@"Cascading Style Sheet Files\bug9527.css", "Cascading Style Sheet Files")]
            [Description("Big: Item 9527 Fix")]
            public void PreserveCssComments_False_Removes_Comments()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.IsTrue(worked, "Task Didn't work");
                Assert.IsFalse(compressedCss.Contains("/* comment */"), compressedCss);
            }

            /// <summary>
            /// There is a specific IE7 hack to preserve these, so just test it works ;)
            /// </summary>
            [TestMethod]
            [DeploymentItem(@"Cascading Style Sheet Files\bug9527.css", "Cascading Style Sheet Files")]
            public void PreserveCssComments_False_Does_Not_Remove_Empty_Comments_After_Child_Selectors()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.IsTrue(worked, "Task Didn't work");
                Assert.IsTrue(compressedCss.Contains("/**/"), compressedCss);
            }

            [TestMethod]
            [DeploymentItem(@"Cascading Style Sheet Files\bug9527.css", "Cascading Style Sheet Files")]
            public void PreserveCssComments_False_Does_Not_Remove_Comments_Which_Should_Be_Preserved()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.IsTrue(worked, "Task Didn't work");
                Assert.IsTrue(compressedCss.Contains("/*! preserved comment */"), compressedCss);
            }

            private static CompressorTask GetCssCompressorFor(string fileName)
            {
                var compressorTask = CreateCompressorTask();
                if (!string.IsNullOrEmpty(fileName))
                {
                    compressorTask.CssFiles = new ITaskItem[] { new TaskItem(@"Cascading Style Sheet Files\" + fileName + ".css") };
                    compressorTask.CssOutputFile = fileName + "min.css";
                }
                return compressorTask;
            }
        }

        private static CompressorTask CreateCompressorTask()
        {
            return new CompressorTask
                       {
                           BuildEngine = new BuildEngineStub(),
                           DeleteCssFiles = "false",
                           DeleteJavaScriptFiles = "false",
                           CssCompressionType = "YuiStockCompression",
                           DisableOptimizations = "false",
                           EncodingType = "Default",
                           LineBreakPosition = "-1",
                           LoggingType = "HardcoreBringItOn",
                       };
        }
    }
}