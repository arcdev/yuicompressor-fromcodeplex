using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Yahoo.Yui.Compressor.MsBuild;

namespace Yahoo.Yui.Compressor
{
    public interface ICompressorTask
    {
        string CssCompressionType { get; set; }
        string DeleteCssFiles { get; set; }
        string CssOutputFile { get; set; }
        string ObfuscateJavaScript { get; set; }
        string PreserveAllSemicolons { get; set; }
        string DisableOptimizations { get; set; }
        string LineBreakPosition { get; set; }
        string EncodingType { get; set; }
        string DeleteJavaScriptFiles { get; set; }
        string JavaScriptOutputFile { get; set; }
        string LoggingType { get; set; }
        string ThreadCulture { get; set; }
        string IsEvalIgnored { get; set; }
    }
}