using System;
using EcmaScript.NET;


namespace Yahoo.Yui.Compressor
{
    public class CustomErrorReporter : ErrorReporter
    {
        private bool _isVerboseLogging;

        public CustomErrorReporter(bool isVerboseLogging)
        {
            this._isVerboseLogging = isVerboseLogging;
        }

        public virtual void Warning(string message, 
            string sourceName, 
            int line, 
            string lineSource, 
            int lineOffset)
        {
            if (this._isVerboseLogging)
            {
                Console.WriteLine("[WARNING] " + message + Environment.NewLine);
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