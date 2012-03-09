using System.Text;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests.TestHelpers
{
    public static class BuildEngineExtensions
    {
        public static bool ContainsError(IBuildEngine engine, string error)
        {
            var buildEngine = engine as BuildEngineStub;
            if (buildEngine == null)
            {
                Assert.Fail("Not a BuildEngineStub, cannot test with this");
            }
            if (buildEngine.Errors != null && buildEngine.Errors.Count > 0)
            {
                foreach (var anError in buildEngine.Errors)
                {
                    if (anError.StartsWith(error))
                    {
                        return true;
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(error + " not found.  Actual errors: ");
            foreach (var anError in buildEngine.Errors)
            {
                sb.AppendLine(anError);
            }
            Assert.Fail(sb.ToString());

            // ReSharper disable HeuristicUnreachableCode
            return false;
            // ReSharper restore HeuristicUnreachableCode
        }
    }
}
