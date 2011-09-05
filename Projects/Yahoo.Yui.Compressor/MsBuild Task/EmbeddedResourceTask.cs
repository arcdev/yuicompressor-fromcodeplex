using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Yahoo.Yui.Compressor.MsBuildTask
{
    public class EmbeddedResourcesTask : Task
    {
        public ITaskItem[] DestinationFiles { get; set; }
        public ITaskItem[] SourceFiles { get; set; }

        public override bool Execute()
        {
            try
            {
                Console.WriteLine("Cleaning Resource files: ");
                if (SourceFiles == null ||
                    SourceFiles.Length == 0)
                {
                    Log.LogWarning("No SourceFiles provided", new object[0]);
                    return true;
                }

                if (DestinationFiles == null ||
                    DestinationFiles.Length != SourceFiles.Length)
                {
                    Log.LogError("There must be as many DestinationFiles as SourceFiles", new object[0]);
                    return false;
                }

                for (int i = 0; i < SourceFiles.Length; i++)
                {
                    ITaskItem item = SourceFiles[i];
                    ITaskItem item2 = DestinationFiles[i];
                    if (!File.Exists(item.ItemSpec))
                    {
                        Log.LogError("Could not find source file '" + item.ItemSpec + "'", new object[0]);
                        return false;
                    }

                    string str;
                    using (FileStream stream = File.OpenRead(item.ItemSpec))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string ext = Path.GetExtension(item.ItemSpec);
                            str = reader.ReadToEnd();
                            switch (ext)
                            {
                                case ".css":
                                    {
                                        str = CssCompressor.Compress(str, 0, CssCompressionType.StockYuiCompressor, true);
                                        break;
                                    }
                                case ".js":
                                    {
                                        str = JavaScriptCompressor.Compress(str, false, true, false, false, 0);
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("invalid file type: {0}.  Must be a CSS or JavaSCript file.",
                                                          ext);
                                        break;
                                    }
                            }

                            ConsoleColor color = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\t " + item.ItemSpec.Replace("obj\\Release\\", string.Empty));
                            Console.ForegroundColor = color;
                        }
                    }

                    if (File.Exists(item2.ItemSpec))
                    {
                        File.Delete(item2.ItemSpec);
                    }

                    using (StreamWriter writer = File.CreateText(item2.ItemSpec))
                    {
                        writer.Write(str);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Log.LogErrorFromException(exception);
                return false;
            }
        }
    }
}