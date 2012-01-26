using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestClass]
    public class TestBase
    {
        protected void CompareTwoFiles(string sourceFilePath, string minifiedDestinationPath, CompressorType compressorType)
        {
            CompareTwoFiles(sourceFilePath, minifiedDestinationPath, compressorType, ComparingTwoFileTypes.Content);
        }

        protected void CompareTwoFiles(string sourceFilePath, string minifiedDestinationPath, CompressorType compressorType, ComparingTwoFileTypes comparingTwoFileTypes)
        {
            CompareTwoFiles(sourceFilePath, minifiedDestinationPath, compressorType, comparingTwoFileTypes, CssCompressionType.StockYuiCompressor);
        }

        protected void CompareTwoFiles(string sourceFilePath, string minifiedDestinationPath, CompressorType compressorType, ComparingTwoFileTypes comparingTwoFileTypes, CssCompressionType compressionType)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                throw new ArgumentNullException("sourceFilePath");
            }

            if (string.IsNullOrEmpty(minifiedDestinationPath))
            {
                throw new ArgumentNullException("minifiedDestinationPath");
            }

            if (compressorType == CompressorType.Unknown)
            {
                throw new ArgumentException("Invalid compressorType selected. Must be either CSS or JS.");
            }

            // Arrange.

            // Act.
            // Load up both files into strings.
            var sourceFileContext = File.ReadAllText(sourceFilePath, Encoding.UTF8);
            var minifiedDestinationContent = File.ReadAllText(minifiedDestinationPath);
            Assert.IsNotNull(sourceFileContext);

            var result = compressorType == CompressorType.JavaScript
                             ? JavaScriptCompressor.Compress(sourceFileContext, true, true, false, false, -1)
                             : CssCompressor.Compress(sourceFileContext, -1, compressionType, true);

            // Assert.
            Assert.IsNotNull(minifiedDestinationContent);
            Assert.IsNotNull(result);

            switch (comparingTwoFileTypes)
            {
                case ComparingTwoFileTypes.Content:
                    Assert.AreEqual(minifiedDestinationContent, result);
                    break;
                case ComparingTwoFileTypes.FileLength:
                    Assert.AreEqual(minifiedDestinationContent.Length, result.Length);
                    break;
            }
        }
    }

    public enum ComparingTwoFileTypes
    {
        Unknown,
        Content,
        FileLength
    }
}