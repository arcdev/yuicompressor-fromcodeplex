using System;
using System.Collections.Specialized;
using EcmaScript.NET;

namespace Yahoo.Yui.Compressor
{
    public class CustomErrorReporter : ErrorReporter
    {
        private readonly bool _isVerboseLogging;
        public StringCollection ErrorMessages { get; private set; }

        public CustomErrorReporter(bool isVerboseLogging)
        {
            _isVerboseLogging = isVerboseLogging;
            ErrorMessages = new StringCollection();
        }

        public virtual void Warning(string message, 
            string sourceName, 
            int line, 
            string lineSource, 
            int lineOffset)
        {
            if (_isVerboseLogging)
            {
                string text;
                if (line != -1)
                {
                    text = string.Format("[WARNING] {0} ******** Source: {1}. Line: {2}. LineOffset: {4}. LineSource: \"{3}\"",
                                  message,
                                  sourceName,
                                  line,
                                  lineSource == null ? "" : lineSource.Trim(),
                                  lineOffset);
                }
                else
                {
                    text = string.Format("[WARNING] {0}", message);
                }
                Console.WriteLine(text);
                ErrorMessages.Add(text);
            }
        }

        public virtual void Error (string message, 
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            EcmaScriptException exception;
            exception = new EcmaScriptRuntimeException(message,
                                                       sourceName,
                                                       line,
                                                       lineSource == null ? "" : lineSource.Trim(),
                                                       lineOffset);
            exception.Source = "EcmaScript";
            throw exception;
        }

        public virtual EcmaScriptRuntimeException RuntimeError(string message,
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            EcmaScriptException exception = new EcmaScriptRuntimeException(string.Format("EcmaScriptRuntimeException :: {0}", message),
                              sourceName,
                              line,
                              lineSource == null ? "" : lineSource.Trim(),
                              lineOffset);
            exception.Source = "EcmaScriptRuntime";
            throw exception;
        }
    }
}