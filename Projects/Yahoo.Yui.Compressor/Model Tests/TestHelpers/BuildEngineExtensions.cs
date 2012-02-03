using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests.TestHelpers
{
    public static class BuildEngineExtensions
    {
        public static bool ContainsError(this IBuildEngine engine, string error)
        {
            var buildEngine = engine as BuildEngineStub;
            if (buildEngine == null)
            {
                Assert.Fail("Not a BuildEngineStub, cannot test with this");
            }
            if (buildEngine.Errors.FirstOrDefault(e => e.StartsWith(error)) != null)
            {
                return true;
            }
            var sb = new StringBuilder();
            sb.AppendLine(error + " not found.  Actual errors: ");
            foreach (var err in buildEngine.Errors)
            {
                sb.AppendLine(err);
            }
            Assert.Fail(sb.ToString());
            // ReSharper disable HeuristicUnreachableCode
            return false;
            // ReSharper restore HeuristicUnreachableCode
        }
    }
}
