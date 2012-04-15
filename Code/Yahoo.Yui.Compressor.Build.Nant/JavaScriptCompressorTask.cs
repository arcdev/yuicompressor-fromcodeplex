using System;
using System.Globalization;

using NAnt.Core.Attributes;

using EcmaScript.NET;

namespace Yahoo.Yui.Compressor.Build.Nant
{
    [TaskName("javaScriptCompressor")]
    public class JavaScriptCompressorTask : CompressorTask
    {
        private readonly IJavaScriptCompressor compressor;

        private CultureInfo threadCulture;

        [TaskAttribute("obfuscateJavaScript")]
        public bool ObfuscateJavaScript { get; set; }

        [TaskAttribute("preserveAllSemicolons")]
        public bool PreserveAllSemicolons { get; set; }

        [TaskAttribute("disableOptimizations")]
        public bool DisableOptimizations { get; set; }

        [TaskAttribute("threadCulture")]
        public string ThreadCulture { get; set; }

        [TaskAttribute("isEvalIgnored")]
        public bool IsEvalIgnored { get; set; }

        public JavaScriptCompressorTask() : this(new JavaScriptCompressor())
        {
        }

        public JavaScriptCompressorTask(IJavaScriptCompressor compressor) : base(compressor)
        {
            this.compressor = compressor;
            this.ObfuscateJavaScript = true;
            this.TaskEngine.LogAdditionalTaskParameters = this.LogTaskParameters;
        }

        protected override void SetBuildParameters()
        {
            base.SetBuildParameters();
            this.ParseThreadCulture();
            this.compressor.DisableOptimizations = this.DisableOptimizations;
            this.compressor.IgnoreEval = this.IsEvalIgnored;
            this.compressor.ObfuscateJavascript = this.ObfuscateJavaScript;
            this.compressor.PreserveAllSemicolons = this.PreserveAllSemicolons;
            this.compressor.ThreadCulture = this.threadCulture;
            this.compressor.Encoding = this.TaskEngine.Encoding;
            this.compressor.ErrorReporter = new CustomErrorReporter(this.TaskEngine.LogType);
        }

        protected override void ExecuteTask()
        {
            try
            {
                base.ExecuteTask();
            }
            catch (EcmaScriptException ecmaScriptException)
            {
                this.TaskEngine.Log.LogError("An error occurred in parsing the Javascript file.");
                if (ecmaScriptException.LineNumber == -1)
                {
                    this.TaskEngine.Log.LogError("[ERROR] {0} ********", ecmaScriptException.Message);
                }
                else
                {
                    this.TaskEngine.Log.LogError(
                        "[ERROR] {0} ******** Line: {2}. LineOffset: {3}. LineSource: \"{4}\"",
                        ecmaScriptException.Message,
                        string.IsNullOrEmpty(ecmaScriptException.SourceName)
                            ? string.Empty
                            : "Source: {1}. " + ecmaScriptException.SourceName,
                        ecmaScriptException.LineNumber,
                        ecmaScriptException.ColumnNumber,
                        ecmaScriptException.LineSource);
                }
            }
        }

        private void LogTaskParameters()
        {
            this.TaskEngine.Log.LogBoolean("Obfuscate Javascript", this.ObfuscateJavaScript);
            this.TaskEngine.Log.LogBoolean("Preserve semi colons", this.PreserveAllSemicolons);
            this.TaskEngine.Log.LogBoolean("Disable optimizations", this.DisableOptimizations);
            this.TaskEngine.Log.LogBoolean("Is Eval Ignored", this.IsEvalIgnored);
            this.TaskEngine.Log.LogMessage(
                "Line break position: "
                + (this.LineBreakPosition <= -1 ? "None" : this.LineBreakPosition.ToString(CultureInfo.InvariantCulture)));
            this.TaskEngine.Log.LogMessage("Thread Culture: " + this.threadCulture.DisplayName);
        }

        private void ParseThreadCulture()
        {
            if (string.IsNullOrEmpty(this.ThreadCulture))
            {
                this.threadCulture = CultureInfo.InvariantCulture;
                return;
            }

            try
            {
                switch (this.ThreadCulture.ToLowerInvariant())
                {
                    case "iv":
                    case "ivl":
                    case "invariantculture":
                    case "invariant culture":
                    case "invariant language":
                    case "invariant language (invariant country)":
                        {
                            this.threadCulture = CultureInfo.InvariantCulture;
                            break;
                        }
                    default:
                        {
                            this.threadCulture = CultureInfo.CreateSpecificCulture(this.ThreadCulture);
                            break;
                        }
                }
            }
            catch
            {
                throw new ArgumentException("Thread Culture: " + this.ThreadCulture + " is invalid.", "ThreadCulture");
            }
        }
    }
}