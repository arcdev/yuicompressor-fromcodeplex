using System;
using System.Globalization;

namespace Yahoo.Yui.Compressor.MsBuildTask
{
    using EcmaScript.NET;

    public class JavaScriptCompressorTask : CompressorTask
    {
        private readonly IJavaScriptCompressor compressor;

        private CultureInfo threadCulture;

        public bool ObfuscateJavaScript { get; set; }

        public bool PreserveAllSemicolons { get; set; }

        public bool DisableOptimizations { get; set; }

        public string ThreadCulture { get; set; }

        public bool IsEvalIgnored { get; set; }

        public JavaScriptCompressorTask() : this(new JavaScriptCompressor())
        {
        }

        public JavaScriptCompressorTask(IJavaScriptCompressor compressor) : base(compressor)
        {
            this.compressor = compressor;
            ObfuscateJavaScript = true;
        }

        protected override void ParseBuildParameters()
        {
            base.ParseBuildParameters();
            ParseThreadCulture();
        }

        protected override string Compress(Microsoft.Build.Framework.ITaskItem file, string originalContent)
        {
            compressor.ErrorReporter = new CustomErrorReporter(loggingType);
            compressor.DisableOptimizations = DisableOptimizations;
            compressor.IgnoreEval = IsEvalIgnored;
            compressor.ObfuscateJavascript = ObfuscateJavaScript;
            compressor.PreserveAllSemicolons = PreserveAllSemicolons;
            compressor.ThreadCulture = threadCulture;
            compressor.Encoding = encoding;
            try
            {
                return base.Compress(file, originalContent);
            }
            catch (EcmaScriptException ecmaScriptException)
            {
                Log.LogError(
                    string.Format(
                        CultureInfo.InvariantCulture, "An error occurred in parsing the Javascript file [{0}].", file));
                if (ecmaScriptException.LineNumber == -1)
                {
                    Log.LogError("[ERROR] {0} ********", ecmaScriptException.Message);
                }
                else
                {
                    Log.LogError(
                        "[ERROR] {0} ******** Line: {2}. LineOffset: {3}. LineSource: \"{4}\"",
                        ecmaScriptException.Message,
                        string.IsNullOrEmpty(ecmaScriptException.SourceName)
                            ? string.Empty
                            : "Source: {1}. " + ecmaScriptException.SourceName,
                        ecmaScriptException.LineNumber,
                        ecmaScriptException.ColumnNumber,
                        ecmaScriptException.LineSource);
                }
                return string.Empty;
            }
        }

        protected override void LogTaskParameters()
        {
            base.LogTaskParameters();
            LogBoolean("Obfuscate Javascript", ObfuscateJavaScript);
            LogBoolean("Preserve semi colons", PreserveAllSemicolons);
            LogBoolean("Disable optimizations", DisableOptimizations);
            LogBoolean("Is Eval Ignored", IsEvalIgnored);
            LogMessage(
                "Line break position: "
                + (LineBreakPosition <= -1 ? "None" : LineBreakPosition.ToString(CultureInfo.InvariantCulture)));
            LogMessage("Thread Culture: " + threadCulture.DisplayName);
        }

        private void ParseThreadCulture()
        {
            if (string.IsNullOrEmpty(ThreadCulture))
            {
                threadCulture = CultureInfo.InvariantCulture;
                return;
            }

            try
            {
                switch (ThreadCulture.ToLowerInvariant())
                {
                    case "iv":
                    case "ivl":
                    case "invariantculture":
                    case "invariant culture":
                    case "invariant language":
                    case "invariant language (invariant country)":
                        {
                            threadCulture = CultureInfo.InvariantCulture;
                            break;
                        }
                    default:
                        {
                            threadCulture = CultureInfo.CreateSpecificCulture(ThreadCulture);
                            break;
                        }
                }
            }
            catch
            {
                throw new ArgumentException("Thread Culture: " + ThreadCulture + " is invalid.", "ThreadCulture");
            }
        }

        private void LogBoolean(string name, bool value)
        {
            LogMessage(name + ": " + (value ? "Yes" : "No"));
        }
    }
}