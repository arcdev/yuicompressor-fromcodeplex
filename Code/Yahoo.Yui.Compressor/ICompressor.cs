namespace Yahoo.Yui.Compressor
{
    public interface ICompressor
    {
        CompressionType CompressionType { get; set; }
        int LineBreakPosition { get; set; }

        string Compress(string source);
    }
}