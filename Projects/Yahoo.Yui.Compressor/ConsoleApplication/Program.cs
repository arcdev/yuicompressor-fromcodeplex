using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;

namespace ConsoleApplication
{
    public class Program
    {

        static void Main(string[] args)
        {
            CompressorService compressorService = new CompressorService();
  
            foreach(string arg in args)
            {
                var argData = arg.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (argData.Length > 0)
                {
                    // Now iterate through each arg (eg. ObfuscateJavaScript=true)
                    foreach (string data in argData)
                    {
                        // Check to see if we have two parts to each data arg.
                        var dataParts = data.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                        if (dataParts.Length == 2)
                        {
                            switch (data[0])
                            {
                                case "CssFiles":

                                    break;
                                case "DeleteCssFiles":
                                    compressorService.DeleteCssFiles = data[1];
                                    break;
                                case "CssOutputFile":
                                    compressorService.CssOutputFile = data[1];
                                    break;
                                case "CssCompressionType":
                                    compressorService.CssCompressionType = data[1];
                                    break;
                                case "JavaScriptFiles":

                                    break;
                                case "ObfuscateJavaScript":
                                    compressorService.ObfuscateJavaScript = data[1];
                                    break;
                                case "PreserveAllSemicolons":
                                    compressorService.PreserveAllSemicolons = data[1];
                                    break;
                                case "DisableOptimizations":
                                    compressorService.DisableOptimizations = data[1];
                                    break;
                                case "EncodingType":
                                    compressorService.EncodingType = data[1];
                                    break;
                                case "DeleteJavaScriptFiles":
                                    compressorService.DeleteJavaScriptFiles = data[1];
                                    break;
                                case "LineBreakPosition":
                                    compressorService.LineBreakPosition = data[1];
                                    break;
                                case "JavaScriptOutputFile":
                                    compressorService.JavaScriptOutputFile = data[1];
                                    break;
                                case "LoggingType":
                                    compressorService.LoggingType = data[1];
                                    break;
                                case "ThreadCulture":
                                    compressorService.ThreadCulture = data[1];
                                    break;
                                case "IsEvalIgnored":
                                    compressorService.IsEvalIgnored = data[1];
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
