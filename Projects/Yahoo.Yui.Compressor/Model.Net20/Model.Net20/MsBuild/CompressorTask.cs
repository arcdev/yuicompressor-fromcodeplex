using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Yahoo.Yui.Compressor.MsBuild
{
    public class CompressorTask : Task
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

            switch (CssCompressionType.ToLowerInvariant())
            {
                case "none":
                    _cssCompressionType = Compressor.CssCompressionType.None;
                    break;
                case "michaelashsregexenhancements":
                    _cssCompressionType = Compressor.CssCompressionType.MichaelAshRegexEnhancements;
                    break;
                case "havemycakeandeatit":
                case "bestofbothworlds":
                case "hybrid":
                    _cssCompressionType = Compressor.CssCompressionType.Hybrid;
                    break;
                default:
                    _cssCompressionType = Compressor.CssCompressionType.StockYuiCompressor;
                    break;
            }

            if (string.IsNullOrEmpty(JavaScriptCompressionType))
            {
                LogMessage("No JavaScript Compression type defined. Defaulting to 'YuiStockCompression'.");
                JavaScriptCompressionType = Compressor.JavaScriptCompressionType.YuiStockCompression.ToString();
            }

            switch (JavaScriptCompressionType.ToLowerInvariant())
            {
                case "none":
                    _javaScriptCompressionType = Compressor.JavaScriptCompressionType.None;
                    break;
                case  "yuistockcompressor":
                    _javaScriptCompressionType = Compressor.JavaScriptCompressionType.YuiStockCompression;
                    break;
                default:
                    Log.LogError("Unrecognised JavaScriptCompressionType: {0}", JavaScriptCompressionType);
                    break;
            }

            if (string.IsNullOrEmpty(LoggingType))
            {
                Log.LogWarning("No logging argument defined. Defaulting to 'ALittleBit'.");
                LoggingType = "ALittleBit";
            }

            switch (LoggingType.ToLowerInvariant())
            {
                case "none":
                    _loggingType = MsBuild.LoggingType.None;
                    break;
                case "hardcorebringiton":
                    _loggingType = MsBuild.LoggingType.HardcoreBringItOn;
                    break;
                default:
                    _loggingType = MsBuild.LoggingType.ALittleBit;
                    break;
            }

            #endregion

            #region Optional Elements

            // Optional property.
            _deleteCssFiles = !string.IsNullOrEmpty(DeleteCssFiles) &&
                              ParseSillyTrueFalseValue(DeleteCssFiles.ToUpperInvariant());

            // Optional property.
            _deleteJavaScriptFiles = !string.IsNullOrEmpty(DeleteJavaScriptFiles) &&
                                     ParseSillyTrueFalseValue(DeleteJavaScriptFiles.ToUpperInvariant());

            // Optional Property.
            _obfuscateJavaScript = !string.IsNullOrEmpty(ObfuscateJavaScript) &&
                                   ParseSillyTrueFalseValue(ObfuscateJavaScript.ToUpperInvariant());

            // Optional Property.
            _preserveAllSemicolons = !string.IsNullOrEmpty(PreserveAllSemicolons) &&
                                     ParseSillyTrueFalseValue(PreserveAllSemicolons.ToUpperInvariant());

            // Optional Property.
            _disableOptimizations = !string.IsNullOrEmpty(DisableOptimizations) &&
                                    ParseSillyTrueFalseValue(DisableOptimizations.ToUpperInvariant());

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
                    _threadCulture = CultureInfo.CreateSpecificCulture(ThreadCulture);
                }
                catch
                {
                    LogMessage("Failed to read in a legitimate culture value. As such, this property will *not* be set.");
                }
            }

            // Optional property.
            _isEvalIgnored = !string.IsNullOrEmpty(IsEvalIgnored) &&
                             ParseSillyTrueFalseValue(IsEvalIgnored.ToUpperInvariant());

            // Optional Property.
            _doNotErrorWhenNoFilesAreProvided = !string.IsNullOrEmpty(DoNotErrorWhenNoFilesAreProvided) &&
                                                ParseSillyTrueFalseValue(DoNotErrorWhenNoFilesAreProvided);

            // Optional Property.
            _preseveCssComments = !string.IsNullOrEmpty(PreserveCssComments) &&
                                  ParseSillyTrueFalseValue(PreserveCssComments);

            #endregion
        }

        private void LogMessage(string message)
        {
            LogMessage(message,
                       false);
        }

        private void LogMessage(string message,
                                bool isIndented)
        {
            if (_loggingType == MsBuild.LoggingType.ALittleBit ||
                _loggingType == MsBuild.LoggingType.HardcoreBringItOn)
            {
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                             "{0}{1}",
                                             isIndented ? "    " : string.Empty,
                                             message));
            }
        }

        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private StringBuilder CompressFiles(ActionType actionType)
        {
            string actionDescription;
            IList<string> fileList = null;
            string originalContent;
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
                foreach (ITaskItem item in CssFiles)
                {
                    if (fileList == null)
                    {
                        fileList = new List<string>();
                    }
                    fileList.Add(item.ItemSpec);
                }
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

                foreach (ITaskItem item in JavaScriptFiles)
                {
                    if (fileList == null)
                    {
                        fileList = new List<string>();
                    }

                    fileList.Add(item.ItemSpec);
                }
            }

            if (fileList != null)
            {
                LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "# {0} {1} file requested.",
                                         fileList.Count,
                                         actionDescription));

                foreach (string file in fileList)
                {
                    LogMessage("=> " + file,
                               true);
                }

                // Now compress each file.
                foreach (string file in fileList)
                {
                    // Load up the file.
                    try
                    {
                        originalContent = File.ReadAllText(file);
                        totalOriginalContentLength += originalContent.Length;

                        if (string.IsNullOrEmpty(originalContent))
                        {
                            Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                       "There is no data in the file [{0}]. Please check that this is the file you want to compress. Lolz :)",
                                                       file));
                        }

                        if (actionType == ActionType.Css)
                        {
                            compressedContent = CssCompressor.Compress(originalContent, 0, _cssCompressionType,
                                                                       !_preseveCssComments);
                        }
                        else if (actionType == ActionType.JavaScript)
                        {
                            compressedContent = JavaScriptCompressor.Compress(originalContent,
                                                                              _loggingType ==
                                                                              MsBuild.LoggingType.HardcoreBringItOn,
                                                                              _obfuscateJavaScript,
                                                                              _preserveAllSemicolons,
                                                                              _disableOptimizations,
                                                                              _lineBreakPosition,
                                                                              _encoding,
                                                                              _threadCulture,
                                                                              _isEvalIgnored,
                                                                              _javaScriptCompressionType);
                        }

                        if (!string.IsNullOrEmpty(compressedContent))
                        {
                            if (finalContent == null)
                            {
                                finalContent = new StringBuilder();
                            }
                            finalContent.Append(compressedContent);
                            if ((actionType == ActionType.Css && _cssCompressionType == Compressor.CssCompressionType.None) ||
                                (actionType == ActionType.JavaScript && _javaScriptCompressionType == Compressor.JavaScriptCompressionType.None))
                            {
                                finalContent.AppendLine();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        // FFS :( Something bad happened.
                        // The most likely scenario is that the user provided some incorrect path information.
                        Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                                   "Failed to read in the data for the path/file [{0}]. The most common cause for this is because the path is incorrect or the file name is incorrect ... so please check your path and file names. Until you fix this up, I can't continue ... sowwy.",
                                                   file));
                        Log.LogErrorFromException(exception,
                                                  false);
                    }

                    // Try and remove this file, if the user requests to do this.
                    try
                    {
                        if ((actionType == ActionType.Css &&
                             _deleteCssFiles) ||
                            (actionType == ActionType.JavaScript &&
                             _deleteJavaScriptFiles))
                        {
                            File.Delete(file);
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
                                         "Finished compressing all {0} file.",
                                         fileList.Count),
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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
            Log.LogMessage(Environment.NewLine);
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
            Log.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                         "Total time to execute task: {0}",
                                         (DateTime.Now - startTime)));
            Log.LogMessage("8< ---------------------------------  ( o Y o )  --------------------------------- >8");
            Log.LogMessage(string.Empty); // This, in effect, is a new line.

            return true;
        }
    }
}