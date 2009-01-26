using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Yahoo.Yui.Compressor;


namespace Yahoo.Yui.Compressor.MsBuild
{
    public class CompressorTask : Task
    {
        #region Fields

        private CssCompressionType _cssCompressionType;
        private LoggingType _loggingType;
        private bool _deleteCssFiles;
        private bool _deleteJavaScriptFiles;
        private bool _obfuscateJavaScript;
        private bool _preserveAllSemicolons;
        private bool _disableOptimizations;
        private int _lineBreakPosition;
        private Encoding _encoding;

        #endregion

        #region Properties

        public string CssCompressionType { get; set; }
        public string CssFiles { get; set; }
        public string DeleteCssFiles { get; set; }
        public string CssOutputFile { get; set; }
        public string JavaScriptFiles { get; set; }
        public string ObfuscateJavaScript { get; set; }
        public string PreserveAllSemicolons { get; set; }
        public string DisableOptimizations { get; set; }
        public string LineBreakPosition { get; set; }
        public string EncodingType { get; set; }
        public string DeleteJavaScriptFiles { get; set; }
        public string JavaScriptOutputFile { get; set; }
        public string LoggingType { get; set; }
        
        #endregion

        #region Methods

        private static bool ParseSillyTrueFalseValue(string value)
        {
            bool result;


            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            switch (value)
            {
                case "YES":
                case "Y":
                case "YEP":
                case "YEAH":
                case "TRUE":
                case "FOSHO":
                case "FO SHO":
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

            if (string.IsNullOrEmpty(this.CssCompressionType))
            {
                this.LogMessage("No Compression type defined. Defaulting to 'YuiStockCompression'.");
                this.CssCompressionType = "YUIStockCompression";
            }

            switch (this.CssCompressionType)
            {
                case "MichaelAshsRegexEnhancements": this._cssCompressionType = Yahoo.Yui.Compressor.CssCompressionType.MichaelAshRegexEnhancements;
                    break;
                case "HaveMyCakeAndEatIt":
                case "BestOfBothWorlds":
                case "Hybrid": this._cssCompressionType = Yahoo.Yui.Compressor.CssCompressionType.Hybrid;
                    break;
                default: this._cssCompressionType = Yahoo.Yui.Compressor.CssCompressionType.StockYuiCompressor;
                    break;
            }

            if (string.IsNullOrEmpty(this.LoggingType))
            {
                Log.LogWarning("No logging argument defined. Defaulting to 'ALittleBit'.");
                this.LoggingType = "ALittleBit";
            }

            switch (this.LoggingType)
            {
                case "None": this._loggingType = MsBuild.LoggingType.None;
                    break;
                case "HardcoreBringItOn": this._loggingType = MsBuild.LoggingType.HardcoreBringItOn;
                    break;
                default: this._loggingType = MsBuild.LoggingType.ALittleBit;
                    break;
            }

            #endregion

            #region Optional Elements

            // Optional property.
            if (!string.IsNullOrEmpty(this.DeleteCssFiles))
            {
                this._deleteCssFiles = CompressorTask.ParseSillyTrueFalseValue(this.DeleteCssFiles.ToUpperInvariant());
            }
            else
            {
                this._deleteCssFiles = false;
            }

            // Optional property.
            if (!string.IsNullOrEmpty(this.DeleteJavaScriptFiles))
            {
                this._deleteJavaScriptFiles = CompressorTask.ParseSillyTrueFalseValue(this.DeleteJavaScriptFiles.ToUpperInvariant());
            }
            else
            {
                this._deleteJavaScriptFiles = false;
            }

            // Optional Property.
            if (!string.IsNullOrEmpty(this.ObfuscateJavaScript))
            {
                this._obfuscateJavaScript = CompressorTask.ParseSillyTrueFalseValue(this.ObfuscateJavaScript.ToUpperInvariant());
            }
            else
            {
                this._obfuscateJavaScript = false;
            }

            // Optional Property.
            if (!string.IsNullOrEmpty(this.PreserveAllSemicolons))
            {
                this._preserveAllSemicolons = CompressorTask.ParseSillyTrueFalseValue(this.PreserveAllSemicolons.ToUpperInvariant());
            }
            else
            {
                this._preserveAllSemicolons = false;
            }

            // Optional Property.
            if (!string.IsNullOrEmpty(this.DisableOptimizations))
            {
                this._disableOptimizations = CompressorTask.ParseSillyTrueFalseValue(this.DisableOptimizations.ToUpperInvariant());
            }
            else
            {
                this._disableOptimizations = false;
            }

            // Optional Property.
            int tempLineBreakPosition;
            if (!string.IsNullOrEmpty(this.LineBreakPosition) &&
                int.TryParse(this.LineBreakPosition, out tempLineBreakPosition))
            {
                this._lineBreakPosition = tempLineBreakPosition;
            }
            else
            {
                this._lineBreakPosition = -1;
            }

            // Optional Property.
            switch(this.EncodingType)
            {
                case "ASCII":
                    this._encoding = Encoding.ASCII;
                    break;
                case "BigEndianUnicode":
                    this._encoding = Encoding.BigEndianUnicode;
                    break;
                case "Unicode":
                    this._encoding = Encoding.Unicode;
                    break;
                case "UTF32":
                    this._encoding = Encoding.UTF32;
                    break;
                case "UTF7":
                    this._encoding = Encoding.UTF7;
                    break;
                case "":
                    this._encoding = Encoding.UTF8;
                    break;
                default:
                    this._encoding = Encoding.Default;
                    break;
            }

            #endregion
        }

        private static IList<string> ParseFiles(string filesToParse)
        {
            IList<string> fileList;


            if (string.IsNullOrEmpty(filesToParse))
            {
                throw new ArgumentNullException("filesToParse");
            }

            fileList = filesToParse.Split(new string[] { " ", ",", ";" },
                StringSplitOptions.RemoveEmptyEntries).ToList();

            return fileList == null ||
                fileList.Count <= 0 ? null : fileList;
        }

        private void LogMessage(string message)
        {
            this.LogMessage(message,
                false);
        }

        private void LogMessage(string message,
            bool isIndented)
        {
            if (this._loggingType == MsBuild.LoggingType.ALittleBit ||
                this._loggingType == MsBuild.LoggingType.HardcoreBringItOn)
            {
                this.Log.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                    "{0}{1}",
                    isIndented ? "    " : string.Empty,
                    message));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private StringBuilder CompressFiles(ActionType actionType)
        {
            string actionDescription;
            IList<string> fileList = null;
            string originalContent;
            int totalOriginalContentLength = 0;
            string compressedContent = null;
            StringBuilder finalContent = null;
            int finalContentLength;
            const string yep = "Yep!";
            const string nope = "Nope :(";


            switch(actionType)
            {
                case ActionType.Css: actionDescription = "css"; break;
                case ActionType.JavaScript: actionDescription = "JavaScript"; break;
                default : actionDescription = "Unknown action type"; break;
            }

            this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                "# Found one or more {0} file arguments. Now parsing ...",
                actionDescription));

            // Get the list of files to compress.
            if (actionType == ActionType.Css)
            {
                fileList = CompressorTask.ParseFiles(this.CssFiles);
            }
            else if (actionType == ActionType.JavaScript)
            {
                // First, lets display what javascript specific arguments have been specified.
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Obfuscate Javascript: {0}",
                    this._obfuscateJavaScript ? yep : nope));
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Preserve semi colons: {0}",
                    this._preserveAllSemicolons ? yep : nope));
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Disable optimizations: {0}",
                    this._disableOptimizations ? "Yeah :(" : "Hell No!"));
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, "    ** Line break position: {0}",
                    this._lineBreakPosition <= -1 ? "None" : LineBreakPosition));

                fileList = CompressorTask.ParseFiles(this.JavaScriptFiles);
            }

            if (fileList != null)
            {
                this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                    "# {0} {1} file{2} requested.",
                    fileList.Count,
                    actionDescription,
                    fileList.Count.ToPluralString()));

                foreach (string file in fileList)
                {
                    this.LogMessage("=> " + file,
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
                            compressedContent = CssCompressor.Compress(originalContent,
                                0,
                                this._cssCompressionType);
                        }
                        else if (actionType == ActionType.JavaScript)
                        {
                            compressedContent = JavaScriptCompressor.Compress(originalContent,
                                this._loggingType == MsBuild.LoggingType.HardcoreBringItOn,
                                this._obfuscateJavaScript,
                                this._preserveAllSemicolons,
                                this._disableOptimizations,
                                this._lineBreakPosition,
                                this._encoding);
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
                            this._deleteCssFiles) ||
                            (actionType == ActionType.JavaScript &&
                            this._deleteJavaScriptFiles))
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

                this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                    "Finished compressing all {0} file{1}.",
                    fileList.Count,
                    fileList.Count.ToPluralString()),
                    true);

                finalContentLength = finalContent == null ? 0 : finalContent.ToString().Length;

                this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                    "Total original {0} file size: {1}. After compression: {2}. Compressed down to {3}% of original size.",
                    actionDescription,
                    totalOriginalContentLength,
                    finalContentLength,
                    100 - ((float)totalOriginalContentLength - (float)finalContentLength) / (float)totalOriginalContentLength * 100));

                if (actionType == ActionType.Css)
                {
                    this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                        "Css Compression Type: {0}.",
                        this._cssCompressionType == Yahoo.Yui.Compressor.CssCompressionType.StockYuiCompressor ? "Stock YUI compression" :
                            this._cssCompressionType == Yahoo.Yui.Compressor.CssCompressionType.MichaelAshRegexEnhancements ? "Micahel Ash's Regex Enhancement compression" :
                            "Hybrid compresssion (the best compression out of all compression types)"));
                }

            }

            return finalContent;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool SaveCompressedText(StringBuilder compressedText,
            ActionType actionType)
        {
            string destinationFileName;

            // Note: compressedText CAN be null or empty, so no check.
            //if (string.IsNullOrEmpty(compressedText))
            //{
            //    compressedText == string.Empty;
            //}

            destinationFileName = actionType == ActionType.Css ? this.CssOutputFile : this.JavaScriptOutputFile;

            try
            {
                File.WriteAllText(destinationFileName,
                    compressedText == null ? string.Empty : compressedText.ToString());
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture,
                    "Compressed content saved to file [{0}].{1}",
                    destinationFileName,
                    System.Environment.NewLine));
            }
            catch (Exception exception)
            {
                // Most likely cause of this exception would be that the user failed to provide the correct path/file
                // or the file is read only, unable to be written, etc.. 
                Log.LogError(string.Format(CultureInfo.InvariantCulture, 
                    "Failed to save the compressed text into the output file [{0}]. Please check the path/file name and make sure the file isn't magically locked, read-only, etc..",
                    destinationFileName));
                Log.LogErrorFromException(exception,
                    false);

                return false;
            }

            return true;
        }

        public override bool Execute()
        {
            StringBuilder compressedText;
            

            // Check to make sure we have the bare minimum arguments supplied to the task.
            if (string.IsNullOrEmpty(this.CssFiles) &&
                string.IsNullOrEmpty(this.JavaScriptFiles))
            {
                Log.LogError("At least one css or javascript file is required to be compressed / minified.");
                return false;
            }
            else if (!string.IsNullOrEmpty(this.CssFiles) &&
                (string.IsNullOrEmpty(this.CssOutputFile)))
            {
                Log.LogError("The css outfile is required if one or more css input files have been defined.");
                return false;
            }
            else if (!string.IsNullOrEmpty(this.JavaScriptFiles) &&
                (string.IsNullOrEmpty(this.JavaScriptOutputFile)))
            {
                Log.LogError("The javascript outfile is required if one or more javascript input files have been defined.");
                return false;
            }

            this.InitialiseBuildSettings();

            Log.LogMessage("Starting Css/Javascript compression...");
            Log.LogMessage(System.Environment.NewLine);
            DateTime startTime = DateTime.Now;

            if (!string.IsNullOrEmpty(this.CssFiles))
            {
                compressedText = this.CompressFiles(ActionType.Css);
                
                // Save this css to the output file, if we have some result text.
                if (!this.SaveCompressedText(compressedText,
                    ActionType.Css))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(this.JavaScriptFiles))
            {
                compressedText = this.CompressFiles(ActionType.JavaScript);
                
                // Save this JavaScript to the output file, if we have some result text.
                if (!this.SaveCompressedText(compressedText,
                    ActionType.JavaScript))
                {
                    return false;
                }
            }

            Log.LogMessage("Finished Css/Javascript compression.");
            Log.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                "Total time to execute task: {0}",
                (DateTime.Now - startTime).ToString()));

            return true;
        }

        #endregion
    }
}