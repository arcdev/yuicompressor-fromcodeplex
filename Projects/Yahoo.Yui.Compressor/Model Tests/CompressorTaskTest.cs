using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Yahoo.Yui.Compressor.MsBuildTask;

namespace Yahoo.Yui.Compressor.Tests
{
    // ReSharper disable InconsistentNaming

    public abstract class CompressorTaskTest
    {
        [TestFixture]
        public class When_Compressing_Javascript_Files : CompressorTaskTest
        {
            [Test]
            public void True_Is_Returned_When_DoNotErrorWhenNoFilesAreProvided_Is_True_And_No_Javascript_Files_Are_Provided()
            {
                // Arrange
                var compressorTask = GetJavascriptCompressorFor("");
                compressorTask.DoNotErrorWhenNoFilesAreProvided = "true";

                // Act
                var worked = compressorTask.Execute();

                // Assert
                Assert.That(worked, Is.True, "Did not Work");
            }

            [Test]
            public void When_The_JavaScriptCompressionType_Is_None_The_Input_Files_Are_Concatenated_Unchanged()
            {
                // Arange
                var compressor = CreateCompressorTask();
                compressor.JavaScriptCompressionType = "None";
                compressor.JavaScriptFiles = new ITaskItem[]
                                          {
                                              new TaskItem(@"Javascript Files\SampleJavaScript1.js"),
                                              new TaskItem(@"Javascript Files\SampleJavaScript2.js")
                                          };
                compressor.JavaScriptOutputFile = "noCompression.js";

                // Act
                compressor.Execute();

                // Assert
                var actual = File.ReadAllText("noCompression.js");
                var sb = new StringBuilder();
                foreach (var file in compressor.JavaScriptFiles)
                {
                    sb.AppendLine(File.ReadAllText(file.ItemSpec));
                }
                Assert.That(actual, Is.EqualTo(sb.ToString()));
            }

            [Test]
            public void When_The_JavaScriptCompressionType_Is_Not_Specified_The_Input_Files_Are_Compressed()
            {
                // Arrange
                var compressor = CreateCompressorTask();
                compressor.JavaScriptFiles = new ITaskItem[]
                                          {
                                              new TaskItem(@"Javascript Files\SampleJavaScript1.js"),
                                              new TaskItem(@"Javascript Files\SampleJavaScript2.js")
                                          };
                compressor.JavaScriptOutputFile = "compressed.js";

                // Act
                compressor.Execute();

                // Assert
                var actual = File.ReadAllText("compressed.js");
                var sb = new StringBuilder();
                foreach (var file in compressor.JavaScriptFiles)
                {
                    sb.AppendLine(File.ReadAllText(file.ItemSpec));
                }
                Assert.That(actual.Length, Is.LessThan(sb.Length));
            }

            [Test]
            public void An_Error_Is_Logged_If_The_JavascriptCompressionType_Is_Not_Recognised()
            {
                // Arrange
                var compressor = CreateCompressorTask();
                compressor.JavaScriptCompressionType = "invalid";

                // Act
                var success = compressor.Execute();

                // Assert
                Assert.That(success, Is.False, "Worked");
                Assert.That(((BuildEngineStub)compressor.BuildEngine).Errors.Contains("Unrecognised JavaScriptCompressionType: invalid"));
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

        [TestFixture]
        public class When_Compressing_Css_Files : CompressorTaskTest
        {
            [Test]
            public void True_Is_Returned_When_DoNotErrorWhenNoFilesAreProvided_Is_True_And_No_Css_Files_Are_Provided()
            {
                // Arrange
                var compressorTask = GetCssCompressorFor("");
                compressorTask.DoNotErrorWhenNoFilesAreProvided = "true";

                // Act
                var worked = compressorTask.Execute();

                // Assert
                Assert.That(worked, Is.True, "Did not Work");
            }

            [Test]
            [Description("http://yuicompressor.codeplex.com/workitem/9527")]
            public void PreserveCssComments_True_Preserves_Comments()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "true";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.That(worked, Is.True, "Task Didn't work");
                Assert.That(compressedCss, Is.StringContaining("/* comment */"), compressedCss);
            }

            [Test]
            [Description("http://yuicompressor.codeplex.com/workitem/9527")]
            public void PreserveCssComments_False_Removes_Comments()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.That(worked, Is.True, "Task Didn't work");
                Assert.That(compressedCss, Is.Not.StringContaining("/* comment */"), compressedCss);
            }

            /// <summary>
            /// There is a specific IE7 hack to preserve these, so just test it works ;)
            /// </summary>
            [Test]
            public void PreserveCssComments_False_Does_Not_Remove_Empty_Comments_After_Child_Selectors()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.That(worked, Is.True, "Task Didn't work");
                Assert.That(compressedCss, Is.StringContaining("/**/"), compressedCss);
            }

            [Test]
            public void PreserveCssComments_False_Does_Not_Remove_Comments_Which_Should_Be_Preserved()
            {
                // Arrange.
                var compressorTask = GetCssCompressorFor("bug9527");
                compressorTask.PreserveCssComments = "false";

                // Act.
                var worked = compressorTask.Execute();
                var compressedCss = File.ReadAllText(compressorTask.CssOutputFile);

                // Assert.
                Assert.That(worked, Is.True, "Task Didn't work");
                Assert.That(compressedCss, Is.StringContaining("/*! preserved comment */"), compressedCss);
            }

            [Test]
            public void When_The_CssCompressionType_Is_None_The_Input_Files_Are_Concatenated_Unchanged()
            {
                // Arrange
                var compressor = CreateCompressorTask();
                compressor.CssCompressionType = "None";
                compressor.CssFiles = new ITaskItem[]
                                          {
                                              new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css"),
                                              new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css")
                                          };
                compressor.CssOutputFile = "noCompression.css";

                // Act
                compressor.Execute();

                // Assert
                var actual = File.ReadAllText("noCompression.css");
                var sb = new StringBuilder();
                foreach (var file in compressor.CssFiles)
                {
                    sb.AppendLine(File.ReadAllText(file.ItemSpec));
                }
                Assert.That(actual, Is.EqualTo(sb.ToString()));
            }

            [Test]
            public void When_The_CssCompressionType_Is_Not_Specified_The_Input_Files_Are_Compressed()
            {
                // Arrange
                var compressor = CreateCompressorTask();
                compressor.CssFiles = new ITaskItem[]
                                          {
                                              new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css"),
                                              new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css")
                                          };
                compressor.CssOutputFile = "compressed.css";

                // Act
                compressor.Execute();

                // assert
                var actual = File.ReadAllText("compressed.css");
                var sb = new StringBuilder();
                foreach (var file in compressor.CssFiles)
                {
                    sb.AppendLine(File.ReadAllText(file.ItemSpec));
                }
                Assert.That(actual.Length, Is.LessThan(sb.Length));
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

    // ReSharper restore InconsistentNaming
}