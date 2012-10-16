namespace Yahoo.Yui.Compressor.Mvc
{
    public abstract class CompressorConfig
    {
        protected CompressorConfig()
        {
            CompressionType = CompressionType.Standard;
            LineBreakPosition = -1;
        }

        public CompressionType CompressionType { get; set; }
        public int LineBreakPosition { get; set; }
    }
}