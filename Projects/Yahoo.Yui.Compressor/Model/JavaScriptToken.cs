﻿namespace Yahoo.Yui.Compressor
{
    public class JavaScriptToken
    {
        public int TokenType { get; private set; }
        public string Value { get; private set; }

        public JavaScriptToken(int type, 
            string value)
        {
            TokenType = type;
            Value = value;
        }
    }
}