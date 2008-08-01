using System;


namespace Yahoo.Yui.Compressor
{
    public class JavaScriptToken
    {
        public int TokenType { get; private set; }
        public string Value { get; private set; }

        public JavaScriptToken(int type, 
            string value)
        {
            this.TokenType = type;
            this.Value = value;
        }
    }
}