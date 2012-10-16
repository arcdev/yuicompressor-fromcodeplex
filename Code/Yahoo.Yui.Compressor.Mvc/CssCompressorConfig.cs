namespace Yahoo.Yui.Compressor.Mvc
{
    public class CssCompressorConfig : CompressorConfig
    {
        public CssCompressorConfig()
        {
            RemoveComments = true;
        }

        public bool RemoveComments { get; set; }
    }
}