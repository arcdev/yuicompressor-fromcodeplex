using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using EcmaScript.NET;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Yahoo.Yui.Compressor.MsBuildTask
{
    public class CompressorTask : Task, ICompressorTask
    {
        private CssCompressionType _cssCompressionType;
        private JavaScriptCompressionType _javaScriptCompressionType;
        private bool _deleteCssFiles;
        private bool _deleteJavaScriptFiles;
        private bool _disableOptimizations;
        private bool _doNotErrorWhenNoFilesAreProvided;
        private Encoding _encoding;
        private bool _isEvalIgnored;
        private int _lineBreakPosition;
        private LoggingType _loggingType;
        private bool _obfuscateJavaScript;
        private bool _preserveAllSemicolons;
        private bool _preseveCssComments;
        private CultureInfo _threadCulture;

        public CompressorTask()
        {
            JavaScriptFiles = new ITaskItem[0];
            CssFiles = new ITaskItem[0];
        }

        public ITaskItem[] CssFiles { get; set; }
        public ITaskItem[] JavaScriptFiles { get; set; }        

        #region ICompressorTask Members

        public string JavaScriptCompressionType { get; set; }
        public string CssCompressionType { get; set; }
        public string DeleteCssFiles { get; set; }
        public string CssOutputFile { get; set; }
        public string ObfuscateJavaScript { get; set; }
        public string PreserveAllSemicolons { get; set; }
        public string DisableOptimizations { get; set; }
        public string LineBreakPosition { get; set; }
        public string EncodingType { get; set; }
        public string DeleteJavaScriptFiles { get; set; }
        public string JavaScriptOutputFile { get; set; }
        public string LoggingType { get; set; }
        public string ThreadCulture { get; set; }
        public string IsEvalIgnored { get; set; }
        public string DoNotErrorWhenNoFilesAreProvided { get; set; }
        public string PreserveCssComments { get; set; }

        #endregion

        private static bool ParseSillyTrueFalseValue(string value)
        {
            bool result;


            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            switch (value.ToLowerInvariant())
            {
                case "yes":
                case "y":
                case "yep":
                case "yeah":
                case "true":
                case "fosho":
                case "fo sho":
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        private void InitialiseBuildSettings()
        {
            #region Required Elements

            if (string.IsNullOrEmpty(CssCompressionType))
            {
                LogMessage("No Css Compression type defined. Defaulting to 'YuiStockCompression'.");
                CssCompressionType = "YUIStockCompression";
            }

            _cssCompressionType = GetCssCompressionTypeFrom(CssCompressionType);

            if (string.IsNullOrEmpty(JavaScriptCompressionType))
            {
                LogMessage("No JavaScript Compression type defined. Defaulting to 'StockYuiCompressor'.");
                JavaScriptCompressionType = Compressor.JavaScriptCompressionType.YuiStockCompression.ToString();
            }

            _javaScriptCompressionType = GetJavaScriptCompressionTypeFrom(JavaScriptCompressionType);

            if (string.IsNullOrEmpty(LoggingType))
            {
                Log.LogWarning("No logging argument defined. Defaulting to 'ALittleBit'.");
                LoggingType = "ALittleBit";
            }

            switch (LoggingType.ToLowerInvariant())
            {
                case "none":
                    _loggingType = MsBuildTask.LoggingType.None;
                    break;
                case "hardcorebringiton":
                    _loggingType = MsBuildTask.LoggingType.HardcoreBringItOn;
                    break;
                default:
                    _loggingType = MsBuildTask.LoggingType.ALittleBit;
                    break;
            }

            #endregion

            #region Optional Elements

            // Optional property.
            _deleteCssFiles = !string.IsNullOrEmpty(DeleteCssFiles) && ParseSillyTrueFalseValue(DeleteCssFiles);

            // Optional property.
            _deleteJavaScriptFiles = !string.IsNullOrEmpty(DeleteJavaScriptFiles) &&
                                     ParseSillyTrueFalseValue(DeleteJavaScriptFiles);

            // Optional Property.
            _obfuscateJavaScript = !string.IsNullOrEmpty(ObfuscateJavaScript) &&
                                   ParseSillyTrueFalseValue(ObfuscateJavaScript);

            // Optional Property.
            _preserveAllSemicolons = !string.IsNullOrEmpty(PreserveAllSemicolons) &&
                                     ParseSillyTrueFalseValue(PreserveAllSemicolons);

            // Optional Property.
            _disableOptimizations = !string.IsNullOrEmpty(DisableOptimizations) &&
                                    ParseSillyTrueFalseValue(DisableOptimizations);

            // Optional Property.
            int tempLineBreakPosition;
            if (!string.IsNullOrEmpty(LineBreakPosition) &&
                int.TryParse(LineBreakPosition, out tempLineBreakPosition))
            {
                _lineBreakPosition = tempLineBreakPosition;
            }
            else
            {
                _lineBreakPosition = -1;
            }

            // Optional Property.
            if (!string.IsNullOrEmpty(EncodingType))
            {
                switch (EncodingType.ToLowerInvariant())
                {
                    case "ascii":
                        _encoding = Encoding.ASCII;
                        break;
                    case "bigendianunicode":
                        _encoding = Encoding.BigEndianUnicode;
                        break;
                    case "unicode":
                        _encoding = Encoding.Unicode;
                        break;
                    case "utf32":
                    case "utf-32":
                        _encoding = Encoding.UTF32;
                        break;
                    case "utf7":
                    case "utf-7":
                        _encoding = Encoding.UTF7;
                        break;
                    case "":
                    case "utf8":
                    case "utf-8":
                        _encoding = Encoding.UTF8;
                        break;
                    default:
                        _encoding = Encoding.Default;
                        break;
                }
            }
            else
            {
                _encoding = Encoding.Default;
            }

            // Optional Property.
            if (!string.IsNullOrEmpty(ThreadCulture))
            {
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
                                _threadCulture = CultureInfo.InvariantCulture;
                                break;
                            }
                        default:
                            {
                                _threadCulture = CultureInfo.CreateSpecificCulture(ThreadCulture);
                                break;
                            }
                    }
                }
                catch
                {
                    LogMessage("Failed to read in a legitimate culture value. As such, this property will *not* be set.");
                }
            }
            else
            {
                _threadCulture = CultureInfo.CreateSpecificCulture("en-gb");
            }

            // Optional property.
            _isEvalIgnored = !string.IsNullOrEmpty(IsEvalIgnored) &&
                             ParseSillyTrueFalseValue(IsEvalIgnored);


            // Optional Property.
            _doNotErrorWhenNoFilesAreProvided = !string.IsNullOrEmpty(DoNotErrorWhenNoFilesAreProvided) &&
                                                ParseSillyTrueFalseValue(DoNotErrorWhenNoFilesAreProvided);

            // Optional Property.
            _preseveCssComments = !string.IsNullOrEmpty(PreserveCssComments) &&
                                  ParseSillyTrueFalseValue(PreserveCssComments);

            #endregion
        }

        private CssCompressionType GetCssCompressionTypeFrom(string cssCompressionType)
        {
            switch (cssCompressionType.ToLowerInvariant())
            {
                case "none":
                    return Compressor.CssCompressionType.None;
                case "michaelashsregexenhancements":
                    return Compressor.CssCompressionType.MichaelAshRegexEnhancements;
                case "havemycakeandeatit":
                case "bestofbothworlds":
                case "hybrid":
                    return Compressor.CssCompressionType.Hybrid;
                default:
                    return Compressor.CssCompressionType.StockYuiCompressor;
            }
        }


        private JavaScriptCompressionType GetJavaScriptCompressionTypeFrom(string javaScriptCompressionType)
        {
            switch (javaScriptCompressionType.ToLowerInvariant())
            {
                case "none":
                    return Compressor.JavaScriptCompressionType.None;
                case "yuistockcompression":
                    return Compressor.JavaScriptCompressionType.YuiStockCompression;
                default:
                    Log.LogError("Unrecognised JavaScriptCompressionType: {0}", JavaScriptCompressionType);
                    return Compressor.JavaScriptCompressionType.None;
            }
        }

        private void LogMessage(string message,
                                bool isIndented = false)
        {
            if (_loggingType == MsBuildTask.LoggingType.ALittleBit ||
                _loggingType == MsBuildTask.LoggingType.HardcoreBringItOn)
            {
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                             "{0}{1}",
                                             isIndented ? "    " : string.Empty,
                                             message));
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private StringBuilder CompressFiles(ActionType actionType)
        {
            string actionDescription;
            ITaskItem[] fileList = null;
            int totalOriginalContentLength = 0;
            string compressedContent = null;
            StringBuilder finalContent = null;

            const string yep = "Yep!";
            const string nope = "Nope :(";

            switch (actionType)
            {
                case ActionType.Css:
                    actionDescription = "css";
                    break;
                case ActionType.JavaScript:
                    actionDescription = "JavaScript";
                    break;
                default:
                    actionDescription = "Unknown action type";
                    break;
            }

            LogMessage(string.Format(CultureInfo.InvariantCulture,
                                     "# Found one or more {0} file arguments. Now parsing ...",
                                     actionDescription));

            // Get the list of files to compress.
            if (actionType == ActionType.Css)
            {
                fileList = CssFiles;
            }
            else if (actionType == ActionType.JavaScript)
            {
                // First, lets display what javascript specific arguments have been specified.
                LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Obfuscate Javascript: {0}",
                                         _obfuscateJavaScript ? yep : nope));
                LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Preserve semi colons: {0}",
                                         _preserveAllSemicolons ? yep : nope));
                LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Disable optimizations: {0}",
                                         _disableOptimizations ? "Yeah :(" : "Hell No!"));
                LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Line break position: {0}",
                                         _lineBreakPosition <= -1 ? "None" : LineBreakPosition));
                LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Thread Culture: {0}",
                                         _threadCulture == null ? "Not defined" : _threadCulture.DisplayName));

                fileList = JavaScriptFiles;
            }

            if (fileList != null)
            {
                LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "# {0} {1} file{2} requested.",
                                         fileList.Length,
                                         actionDescription,
                                         fileList.Length.ToPluralString()));

                // Now compress each file.
                foreach (ITaskItem file in fileList)
                {
                    string message = "=> " + file.ItemSpec;

                    // Load up the file.
                    try
                    {
                        string originalContent = File.ReadAllText(file.ItemSpec);
                        totalOriginalContentLength += originalContent.Length;

                        if (string.IsNullOrEmpty(originalContent))
                        {
                            LogMessage(message, true);
                            Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                       "There is no data in the file [{0}]. Please check that this is the file you want to compress. Lolz :)",
                                                       file));
                        }

                        if (actionType == ActionType.Css)
                        {
                            CssCompressionType compressionType = _cssCompressionType;
                            string overrideType = file.GetMetadata("CompressionType");
                            if (!string.IsNullOrEmpty(overrideType))
                            {
                                compressionType = GetCssCompressionTypeFrom(overrideType);
                                if (compressionType != _cssCompressionType)
                                {
                                    message += string.Format(" (CompressionType override: {0})", compressionType.ToString());
                                }
                            }
                            LogMessage(message, true);
                            compressedContent = CssCompressor.Compress(originalContent, 0, compressionType,
                                                                       !_preseveCssComments);
                        }
                        else if (actionType == ActionType.JavaScript)
                        {
                            JavaScriptCompressionType compressionType = _javaScriptCompressionType;
                            string overrideType = file.GetMetadata("CompressionType");
                            if (!string.IsNullOrEmpty(overrideType))
                            {
                                compressionType = GetJavaScriptCompressionTypeFrom(overrideType);
                                if (compressionType != _javaScriptCompressionType)
                                {
                                    message += string.Format(" (CompressionType override: {0})",
                                                             compressionType.ToString());
                                }
                            }
                            LogMessage(message, true);
                            compressedContent = JavaScriptCompressor.Compress(originalContent,
                                                                              _loggingType ==
                                                                              MsBuildTask.LoggingType.HardcoreBringItOn,
                                                                              _obfuscateJavaScript,
                                                                              _preserveAllSemicolons,
                                                                              _disableOptimizations,
                                                                              _lineBreakPosition,
                                                                              _encoding,
                                                                              _threadCulture,
                                                                              _isEvalIgnored,
                                                                              compressionType);
                        }

                        if (!string.IsNullOrEmpty(compressedContent))
                        {
                            if (finalContent == null)
                            {
                                finalContent = new StringBuilder();
                            }
                            finalContent.Append(compressedContent);
                        }
                    }
                    catch (EcmaScriptException ecmaScriptException)
                    {
                        Log.LogError(string.Format(CultureInfo.InvariantCulture, "An error occurred in parsing the Javascript file [{0}].", file));
                        if (ecmaScriptException.LineNumber == -1)
                        {
                            Log.LogError("[ERROR] {0} ********", ecmaScriptException.Message);
                        }
                        else
                        {
                            Log.LogError("[ERROR] {0} ******** Line: {2}. LineOffset: {3}. LineSource: \"{4}\"",
                                         ecmaScriptException.Message,
                                         string.IsNullOrEmpty(ecmaScriptException.SourceName) ? string.Empty : "Source: {1}. " + ecmaScriptException.SourceName,
                                         ecmaScriptException.LineNumber,
                                         ecmaScriptException.ColumnNumber,
                                         ecmaScriptException.LineSource);
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception is FileNotFoundException)
                        {
                            Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                       "ERROR reading file or path [{0}].", file));
                        }
                        else
                        {
                            // FFS :( Something bad happened.
                            Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                       "Failed to read/parse data in file [{0}].",
                                                       file));
                        }
                        Log.LogErrorFromException(exception, false);
                    }

                    // Try and remove this file, if the user requests to do this.
                    try
                    {
                        if ((actionType == ActionType.Css &&
                             _deleteCssFiles) ||
                            (actionType == ActionType.JavaScript &&
                             _deleteJavaScriptFiles))
                        {
                            File.Delete(file.ItemSpec);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                   "Failed to delete the path/file [{0}]. It's possible the file is locked?",
                                                   file));
                        Log.LogErrorFromException(exception,
                                                  false);
                    }
                }

                LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "Finished compressing all {0} file{1}.",
                                         fileList.Length,
                                         fileList.Length.ToPluralString()),
                           true);

                int finalContentLength = finalContent == null ? 0 : finalContent.ToString().Length;

                LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "Total original {0} file size: {1}. After compression: {2}. Compressed down to {3}% of original size.",
                                         actionDescription,
                                         totalOriginalContentLength,
                                         finalContentLength,
                                         100 -
                                         (totalOriginalContentLength - (float) finalContentLength)/
                                         totalOriginalContentLength*100));

                if (actionType == ActionType.Css)
                {
                    LogMessage(string.Format(CultureInfo.InvariantCulture,
                                             "Css Compression Type: {0}.",
                                             _cssCompressionType == Compressor.CssCompressionType.StockYuiCompressor
                                                 ? "Stock YUI compression"
                                                 : _cssCompressionType ==
                                                   Compressor.CssCompressionType.MichaelAshRegexEnhancements
                                                       ? "Micahel Ash's Regex Enhancement compression"
                                                       : "Hybrid compresssion (the best compression out of all compression types)"));
                }
            }

            return finalContent;
        }


        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool SaveCompressedText(StringBuilder compressedText, ActionType actionType)
        {
            // Note: compressedText CAN be null or empty, so no check.

            string destinationFileName = actionType == ActionType.Css ? CssOutputFile : JavaScriptOutputFile;

            try
            {
                File.WriteAllText(destinationFileName, compressedText == null ? string.Empty : compressedText.ToString(),
                                  _encoding);
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture, "Compressed content saved to file [{0}].{1}",
                                             destinationFileName, Environment.NewLine));
            }
            catch (Exception exception)
            {
                // Most likely cause of this exception would be that the user failed to provide the correct path/file
                // or the file is read only, unable to be written, etc.. 
                Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                           "Failed to save the compressed text into the output file [{0}]. Please check the path/file name and make sure the file isn't magically locked, read-only, etc..",
                                           destinationFileName));
                Log.LogErrorFromException(exception, false);

                return false;
            }

            return true;
        }

        public override bool Execute()
        {
            StringBuilder compressedText;

            InitialiseBuildSettings();

            // Check to make sure we have the bare minimum arguments supplied to the task.
            if (CssFiles.Length == 0 &&
                JavaScriptFiles.Length == 0)
            {
                if (_doNotErrorWhenNoFilesAreProvided)
                {
                    // No files were provided. Fine .. BUT the user optionally asked that, if there are no files found .. then don't error .. but just quit gracefully.
                    // Sure! We can do that, too :)
                    return true;
                }

                Log.LogError("At least one css or javascript file is required to be compressed / minified.");
                return false;
            }

            if (CssFiles.Length > 0 &&
                (string.IsNullOrEmpty(CssOutputFile)))
            {
                Log.LogError("The css outfile is required if one or more css input files have been defined.");
                return false;
            }

            if (JavaScriptFiles.Length > 0 &&
                (string.IsNullOrEmpty(JavaScriptOutputFile)))
            {
                Log.LogError(
                    "The javascript outfile is required if one or more javascript input files have been defined.");
                return false;
            }

            Log.LogMessage("Starting Css/Javascript compression...");

            // Determine and log the Assembly version.
            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] fileVersionAttributes = assembly.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false);
            string assemblyFileVersion = fileVersionAttributes.Length > 0
                                             ? ((AssemblyFileVersionAttribute) fileVersionAttributes[0]).Version
                                             : "Unknown File Version";

            object[] assemblyTitleAttributes = assembly.GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
            string assemblyTitle = assemblyTitleAttributes.Length > 0
                                       ? ((AssemblyTitleAttribute) assemblyTitleAttributes[0]).Title
                                       : "Unknown Title";

            Log.LogMessage(string.Format("Using version {0} of {1}.", assemblyFileVersion, assemblyTitle));

            // What is the current thread culture?
            Log.LogMessage(string.Format(
                "Current thread culture / UI culture (before modifying, if requested): {0}/{1}",
                Thread.CurrentThread.CurrentCulture.EnglishName, Thread.CurrentThread.CurrentUICulture.EnglishName));

            Log.LogMessage(string.Empty); // This, in effect, is a new line.

            DateTime startTime = DateTime.Now;

            if (CssFiles.Length > 0)
            {
                compressedText = CompressFiles(ActionType.Css);

                // Save this css to the output file, if we have some result text.
                if (!SaveCompressedText(compressedText,
                                        ActionType.Css))
                {
                    return false;
                }
            }

            if (JavaScriptFiles.Length > 0)
            {
                compressedText = CompressFiles(ActionType.JavaScript);

                // Save this JavaScript to the output file, if we have some result text.
                if (!SaveCompressedText(compressedText,
                                        ActionType.JavaScript))
                {
                    return false;
                }
            }

            Log.LogMessage("Finished Css/Javascript compression.");
            // What is the current thread culture?
            Log.LogMessage(string.Format(
                "Reverted back to thread culture / UI culture: {0}/{1}",
                Thread.CurrentThread.CurrentCulture.EnglishName, Thread.CurrentThread.CurrentUICulture.EnglishName));
            Log.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "Total time to execute task: {0}",
                                         (DateTime.Now - startTime)));
            Log.LogMessage("8< ---------------------------------  ( o Y o )  --------------------------------- >8");
            Log.LogMessage(string.Empty); // This, in effect, is a new line.

            return true;
        }
    }
}