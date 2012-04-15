using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Yahoo.Yui.Compressor.Build.MsBuild;
using Yahoo.Yui.Compressor.Tests.TestHelpers;

namespace Yahoo.Yui.Compressor.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class JavaScriptCompressorTaskTests
    {
        [Test]
        public void When_The_CompressionType_Is_None_The_Input_Files_Are_Concatenated_Unchanged()
        {
            // Arange
            var compressor = CreateCompressorTask();
            compressor.CompressionType = "None";
            compressor.SourceFiles = new ITaskItem[]
                {
                    new TaskItem(@"Javascript Files\SampleJavaScript1.js"),
                    new TaskItem(@"Javascript Files\SampleJavaScript2.js")
                };
            compressor.OutputFile = "noCompression.js";

            // Act
            compressor.Execute();

            // Assert
            var actual = File.ReadAllText("noCompression.js");
            var sb = new StringBuilder();
            foreach (var file in compressor.SourceFiles)
            {
                sb.Append(File.ReadAllText(file.ItemSpec));
            }
            Assert.That(actual, Is.EqualTo(sb.ToString()));
        }

        [Test]
        public void When_The_JavaScriptCompressionType_Is_Not_Specified_The_Input_Files_Are_Compressed()
        {
            // Arrange
            var compressor = CreateCompressorTask();
            compressor.SourceFiles = new ITaskItem[]
                {
                    new TaskItem(@"Javascript Files\SampleJavaScript1.js"),
                    new TaskItem(@"Javascript Files\SampleJavaScript2.js")
                };
            compressor.OutputFile = "compressed.js";

            // Act
            compressor.Execute();

            // Assert
            var actual = File.ReadAllText("compressed.js");
            var sb = new StringBuilder();
            foreach (var file in compressor.SourceFiles)
            {
                sb.Append(File.ReadAllText(file.ItemSpec));
            }
            Assert.That(actual.Length, Is.LessThan(sb.Length));
        }

        [Test]
        public void When_The_Compressions_Type_Is_Overridden_On_An_Individual_Item_It_Takes_Precedence_Over_The_Task_Compression_Type()
        {
            // Arrange
            var compressor = CreateCompressorTask();
            compressor.CompressionType = CompressionType.None.ToString();

            compressor.SourceFiles = new ITaskItem[]
                {
                    new TaskItem(@"Javascript Files\SampleJavaScript1.js"),
                    new TaskItem(@"Javascript Files\SampleJavaScript2.js")
                };
            compressor.SourceFiles[0].SetMetadata("CompressionType", CompressionType.Standard.ToString());
            compressor.OutputFile = "semicompressed.js";

            // Act
            compressor.Execute();

            // Assert
            var actual = File.ReadAllText("semicompressed.js");
            var sb = new StringBuilder();
            foreach (var file in compressor.SourceFiles)
            {
                sb.Append(File.ReadAllText(file.ItemSpec));
            }
            Assert.That(actual.Length, Is.LessThan(sb.Length));
        }

        [Test]
        public void An_Error_Is_Logged_If_The_CompressionType_Is_Not_Recognised()
        {
            // Arrange
            var compressor = CreateCompressorTask();
            compressor.CompressionType = "invalid";

            // Act
            var result = compressor.Execute();

            // Assert
            Assert.IsFalse(result);
            Assert.That(BuildEngineExtensions.ContainsError(compressor.BuildEngine, "Compression Type: invalid is invalid"));
        }

        [Test]
        public void An_Invalid_File_Or_Path_Results_In_An_Could_Not_File_File_Exception_Being_Logged()
        {
            var compressor = GetJavascriptCompressorFor(@"DoesNotExist");

            compressor.Execute();
            Assert.That(BuildEngineExtensions.ContainsError(compressor.BuildEngine, ("ERROR reading file or path")));
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9719")]
        public void A_Reserved_Word_As_A_Property_In_The_Source_Should_Throw_An_Error_With_The_Correct_Error_Exposed()
        {
            var compressor = GetJavascriptCompressorFor("Issue9719");

            compressor.Execute();
            Assert.That(BuildEngineExtensions.ContainsError(compressor.BuildEngine, ("invalid property id")));
        }

        private static CompressorTask GetJavascriptCompressorFor(string fileName)
        {
            var compressorTask = CreateCompressorTask();
            if (!string.IsNullOrEmpty(fileName))
            {
                compressorTask.SourceFiles = new ITaskItem[] { new TaskItem(@"Javascript Files\" + fileName + ".js") };
                compressorTask.OutputFile = fileName + "min.js";
            }
            return compressorTask;
        }

        private static JavaScriptCompressorTask CreateCompressorTask()
        {
            return new JavaScriptCompressorTask
                {
                    BuildEngine = new BuildEngineStub(),
                    DeleteSourceFiles = false,
                    CompressionType = "Standard",
                    DisableOptimizations = false,
                    EncodingType = "Default",
                    LineBreakPosition = -1,
                    LoggingType = "None",
                };
        }
    }
    // ReSharper restore InconsistentNaming
}