using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace Yahoo.Yui.Compressor.Tests
{
    public class BuildEngineStub : IBuildEngine
    {
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public bool ContinueOnError
        {
            get { return true; }
        }

        public int LineNumberOfTaskNode
        {
            get { throw new NotImplementedException(); }
        }

        public int ColumnNumberOfTaskNode
        {
            get { throw new NotImplementedException(); }
        }

        public string ProjectFileOfTaskNode
        {
            get { return string.Empty; }
        }
    }
}
