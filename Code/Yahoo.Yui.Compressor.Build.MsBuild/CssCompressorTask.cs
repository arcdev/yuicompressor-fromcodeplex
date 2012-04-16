namespace Yahoo.Yui.Compressor.Build.MsBuild
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

        public override bool Execute()
        {
            compressor.RemoveComments = !PreserveComments;
            return base.Execute();
        }
    }
}