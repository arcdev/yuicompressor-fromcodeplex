using Microsoft.Build.Framework;

namespace Yahoo.Yui.Compressor.MsBuildTask
{
    public class CssCompressorTask : CompressorTask
    {
        private readonly ICssCompressor compressor;

        public bool PreserveComments { get; set; }

        public CssCompressorTask() : this(new CssCompressor())
        {
        }

        public CssCompressorTask(ICssCompressor compressor) : base(compressor)
        {
            this.compressor = compressor;
        }

        protected override string Compress(ITaskItem content, string originalContent)
        {
            compressor.RemoveComments = !PreserveComments;
            return base.Compress(content, originalContent);
        }
    }
}