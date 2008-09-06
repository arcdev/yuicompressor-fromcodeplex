using System;
using EcmaScript.NET;

namespace Yahoo.Yui.Compressor
{
    public class CustomErrorReporter : ErrorReporter
    {
        private readonly bool _isVerboseLogging;

        public CustomErrorReporter(bool isVerboseLogging)
        {
            _isVerboseLogging = isVerboseLogging;
        }

        public virtual void Warning(string message, 
            string sourceName, 
            int line, 
            string lineSource, 
            int lineOffset)
        {
            if (_isVerboseLogging)
            {
                Console.WriteLine("[WARNING] {0}{1}", message, Environment.NewLine);
            }
        }

        public virtual void Error (string message, 
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            throw new InvalidOperationException(message + Environment.NewLine);
        }

        public virtual EcmaScriptRuntimeException RuntimeError(string message,
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            throw new InvalidOperationException(message + Environment.NewLine);
        }
    }
}