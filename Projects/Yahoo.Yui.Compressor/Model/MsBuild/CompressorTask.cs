using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Yahoo.Yui.Compressor;


namespace Yahoo.Yui.Compressor.MSBuild
{
    public class CompressorTask : Task
    {
        #region Properties

        public string CssFiles { get; set; }
        public string CssOutputFile { get; set; }
        public string JavaScriptFiles { get; set; }
        public string JavaScriptFileToReplace { get; set; }
        public bool VerboseLogging { get; set; }
        public bool DeleteFiles { get; set; }

        #endregion

        #region Methods

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
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            if (this.VerboseLogging)
            {
                this.Log.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                    "{0}{1}",
                    isIndented ? "    " : string.Empty,
                    message));
            }
        }

        private int DeleteListedFiles(string filesToDelete)
        {
            IList<string> files;
            int counter = 0;


            if (string.IsNullOrEmpty(filesToDelete))
            {
                throw new ArgumentNullException("filesToDelete");
            }

            files = CompressorTask.ParseFiles(filesToDelete);
            if (!files.IsNullOrEmpty())
            {
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        counter++;
                    }
                    catch
                    {
                        Log.LogError(string.Format(CultureInfo.InvariantCulture,
                            "Failed to delete the file [{0}]. Please make sure this file exists and it's not locked, etc.",
                            file));
                        throw;
                    }
                }
            }

            return counter;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private StringBuilder CompressCss()
        {
            IList<string> cssFiles;
            string originalCss;
            int totalOriginalCssLength = 0;
            string compressedCss;
            StringBuilder finalCss = null;


            this.LogMessage("# Found one or more css file arguments. Now parsing ...");

            // Get the list of css files to compress.
            cssFiles = CompressorTask.ParseFiles(this.CssFiles);

            if (cssFiles != null)
            {
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                    "# {0} css file{1} requested.",
                    cssFiles.Count,
                    cssFiles.Count.ToPluralString()));
                foreach (string file in cssFiles)
                {
                    this.LogMessage("=> " + file,
                        true);
                }

                // Now compress each css file.
                foreach (string file in cssFiles)
                {
                    // Load up the file.
                    try
                    {
                        originalCss = File.ReadAllText(file);
                        totalOriginalCssLength += originalCss.Length;

                        if (string.IsNullOrEmpty(originalCss))
                        {
                            Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                "There is no data in the file [{0}]. Please check that this is the file you want to compress. Lolz :)",
                                file));
                        }

                        compressedCss = CssCompressor.Compress(originalCss);
                        if (finalCss == null)
                        {
                            finalCss = new StringBuilder();
                        }

                        finalCss.Append(compressedCss);
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
                }

                this.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                    "Finished compressing all {0} file{1}.",
                    cssFiles.Count,
                    cssFiles.Count.ToPluralString()),
                    true);
                this.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                    "Total original css file size: {0}. After compression: {1}. Compressed down to {2}% of original size.",
                    totalOriginalCssLength,
                    finalCss.ToString().Length,
                    100 - ((float)totalOriginalCssLength - (float)finalCss.ToString().Length) / (float)totalOriginalCssLength * 100));

            }

            return finalCss;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool SaveCompressedCss(string compressedCss)
        {
            if (string.IsNullOrEmpty(compressedCss))
            {
                throw new ArgumentNullException("compressedCss");
            }

            try
            {
                File.WriteAllText(this.CssOutputFile,
                    compressedCss);
            }
            catch (Exception exception)
            {
                // Most likely cause of this exception would be that the user failed to provide the correct path/file
                // or the file is read only, unable to be written, etc.. 
                Log.LogError(string.Format(CultureInfo.InvariantCulture, 
                    "Failed to save the compressed css into the output file [{0}]. Please check the path/file name and make sure the file isn't magically locked, read-only, etc..",
                    this.CssOutputFile));
                Log.LogErrorFromException(exception,
                    false);

                return false;
            }

            return true;
        }

        public override bool Execute()
        {
            DateTime startTime;
            StringBuilder compressedCss;
            bool saveResult;
            int counter;


            // Check to make sure we have the bare minimum arguments supplied to the task.
            if (string.IsNullOrEmpty(this.CssFiles) &&
                string.IsNullOrEmpty(this.JavaScriptFiles))
            {
                Log.LogError("At least one css or javascript file is required to be compressed / minified.");
                return false;
            }
            else if (string.IsNullOrEmpty(this.CssOutputFile))
            {
                Log.LogError("At least one css or javascript output file is required.");
                return false;
            }

            this.LogMessage("Starting Css/Javascript compression...");
            startTime = DateTime.Now;

            if (!string.IsNullOrEmpty(this.CssFiles))
            {
                compressedCss = this.CompressCss();
                if (compressedCss == null)
                {
                    // Assumption: an error has occured and it has already been logged.
                    return false;
                }

                // Save this css to the output file.
                saveResult = this.SaveCompressedCss(compressedCss.ToString());
                if (!saveResult)
                {
                    return false;
                }
                else if (this.DeleteFiles)
                {
                    this.LogMessage("Request to delete files ...");

                    // Remove the files.
                    if (!string.IsNullOrEmpty(this.CssFiles))
                    {
                        counter = this.DeleteListedFiles(this.CssFiles);
                        this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                "=> Finished deleting {0} Css files.",
                                counter),
                            true);
                    }

                    if (!string.IsNullOrEmpty(this.JavaScriptFiles))
                    {
                        counter = this.DeleteListedFiles(this.JavaScriptFiles);
                        this.LogMessage(string.Format(CultureInfo.InvariantCulture,
                                "=> Finished deleting {0} Javascript files.",
                                counter),
                            true);
                    }
                }
            }

            this.LogMessage("Finished Css/Javascript compression.");
            this.LogMessage(string.Format(CultureInfo.InvariantCulture, 
                "Total time to execute task: {0}",
                (DateTime.Now - startTime).ToString()));

            return true;
        }

        #endregion
    }
}