using System.Reflection;

namespace Yahoo.Yui.Compressor
{
    using System;

    public abstract class Compressor : ICompressor
    {
        public CompressionType CompressionType { get; set; }
        public int LineBreakPosition { get; set; }

        static Compressor()
        {
            AssemblyResolver.Initialise();
        }

        protected Compressor()
        {
            CompressionType = CompressionType.Standard;
            this.LineBreakPosition = -1;
        }

        public string Compress(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }

            if (CompressionType == CompressionType.None)
            {
                return source;
            }

            return DoCompress(source);
        }

        protected abstract string DoCompress(string source);
    }
}
