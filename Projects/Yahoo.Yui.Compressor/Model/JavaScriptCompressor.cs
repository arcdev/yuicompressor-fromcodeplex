using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using EcmaScript.NET;
using EcmaScript.NET.Collections;



namespace Yahoo.Yui.Compressor
{
    public class JavaScriptCompressor
    {
        #region Fields

        private ErrorReporter _logger;

        private bool _munge;
        private bool _verbose;

        private static int BUILDING_SYMBOL_TREE = 1;
        private static int CHECKING_SYMBOL_TREE = 2;

        private int _mode;
        private int _offset;
        private int _braceNesting;
        private ArrayList _tokens;
        private Stack _scopes = new Stack();
        private ScriptOrFunctionScope _globalScope = new ScriptOrFunctionScope(-1, null);
        private Hashtable _indexedScopes = new Hashtable();

        private static HashSet<string> _builtin;
        private static object _synLock = new object();

        private static Regex SIMPLE_IDENTIFIER_NAME_PATTERN = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$",
            RegexOptions.Compiled);

        #endregion

        #region Properties

        internal static List<string> Ones;
        internal static List<string> Twos;
        internal static List<string> Threes;
        private static Hashtable Literals { get; set; }
        private static HashSet<string> Reserved { get; set; }

        #endregion

        #region Constructors

        public JavaScriptCompressor(string javaScript) : this(javaScript,
            true)
        {
        }

        public JavaScriptCompressor(string javaScript,
            bool isVerboseLogging)
        {
            MemoryStream memoryStream;
            CustomErrorReporter customErrorReporter;


            if (string.IsNullOrEmpty(javaScript))
            {
                throw new ArgumentNullException("javaScript");
            }

            JavaScriptCompressor.Initialise();

            memoryStream  = new MemoryStream(System.Text.Encoding.Default.GetBytes(javaScript));

            customErrorReporter = new CustomErrorReporter(isVerboseLogging);
            this._logger = customErrorReporter;
            this._tokens = JavaScriptCompressor.Parse(new StreamReader(memoryStream),
                customErrorReporter);
        }

        #endregion

        #region Methods

        #region Private Methods

        private static HashSet<string> InitialiseBuiltIn()
        {
            HashSet<string> builtin;


            if (JavaScriptCompressor._builtin == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor._builtin == null)
                    {
                        builtin = new HashSet<string>();
                        builtin.Add("NaN");
                        builtin.Add("top");

                        JavaScriptCompressor._builtin = builtin;
                    }
                }
            }

            return JavaScriptCompressor._builtin;
        }

        private static List<string> InitialiseOnesList()
        {
            List<string> onesList;


            if (JavaScriptCompressor.Ones == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor.Ones == null)
                    {
                        onesList = new List<string>();
                        for (char c = 'A'; c <= 'Z'; c++)
                        {
                            onesList.Add(Convert.ToString(c, CultureInfo.InvariantCulture));
                        }

                        for (char c = 'a'; c <= 'z'; c++)
                        {
                            onesList.Add(Convert.ToString(c, CultureInfo.InvariantCulture));
                        }

                        JavaScriptCompressor.Ones = onesList;
                    }
                }
            }

            return JavaScriptCompressor.Ones;
        }

        private static List<string> InitialiseTwosList()
        {
            List<string> twosList;


            if (JavaScriptCompressor.Twos == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor.Twos == null)
                    {
                        twosList = new List<string>();

                        for (int i = 0; i < JavaScriptCompressor.Ones.Count; i++)
                        {
                            string one = JavaScriptCompressor.Ones[i];

                            for (char c = 'A'; c <= 'Z'; c++)
                            {
                                twosList.Add(one + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }

                            for (char c = 'a'; c <= 'z'; c++)
                            {
                                twosList.Add(one + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }

                            for (char c = '0'; c <= '9'; c++)
                            {
                                twosList.Add(one + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }
                        }

                        // Remove two-letter JavaScript reserved words and built-in globals...
                        twosList.Remove("as");
                        twosList.Remove("is");
                        twosList.Remove("do");
                        twosList.Remove("if");
                        twosList.Remove("in");

                        foreach (string word in JavaScriptCompressor._builtin)
                        {
                            twosList.Remove(word);
                        }

                        JavaScriptCompressor.Twos = twosList;
                    }
                }
            }

            return JavaScriptCompressor.Twos;
        }

        private static List<string> InitialiseThreesList()
        {
            List<string> threesList;


            if (JavaScriptCompressor.Threes == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor.Threes == null)
                    {
                        threesList = new List<string>();

                        for (int i = 0; i < JavaScriptCompressor.Twos.Count; i++)
                        {
                            string two = JavaScriptCompressor.Twos[i];

                            for (char c = 'A'; c <= 'Z'; c++)
                            {
                                threesList.Add(two + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }

                            for (char c = 'a'; c <= 'z'; c++)
                            {
                                threesList.Add(two + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }

                            for (char c = '0'; c <= '9'; c++)
                            {
                                threesList.Add(two + Convert.ToString(c, CultureInfo.InvariantCulture));
                            }
                        }

                        // Remove three-letter JavaScript reserved words and built-in globals...
                        threesList.Remove("for");
                        threesList.Remove("int");
                        threesList.Remove("new");
                        threesList.Remove("try");
                        threesList.Remove("use");
                        threesList.Remove("var");

                        foreach (string word in JavaScriptCompressor._builtin)
                        {
                            threesList.Remove(word);
                        }

                        JavaScriptCompressor.Threes = threesList;
                    }
                }
            }

            return JavaScriptCompressor.Threes;
        }

        private static Hashtable InitialiseLiterals()
        {
            Hashtable literals;


            if (JavaScriptCompressor.Literals == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor.Literals == null)
                    {
                        literals = new Hashtable();

                        literals.Add(Token.GET, "get ");
                        literals.Add(Token.SET, "set ");
                        literals.Add(Token.TRUE, "true");
                        literals.Add(Token.FALSE, "false");
                        literals.Add(Token.NULL, "null");
                        literals.Add(Token.THIS, "this");
                        literals.Add(Token.FUNCTION, "function ");
                        literals.Add(Token.COMMA, ",");
                        literals.Add(Token.LC, "{");
                        literals.Add(Token.RC, "}");
                        literals.Add(Token.LP, "(");
                        literals.Add(Token.RP, ")");
                        literals.Add(Token.LB, "[");
                        literals.Add(Token.RB, "]");
                        literals.Add(Token.DOT, ".");
                        literals.Add(Token.NEW, "new ");
                        literals.Add(Token.DELPROP, "delete ");
                        literals.Add(Token.IF, "if");
                        literals.Add(Token.ELSE, "else");
                        literals.Add(Token.FOR, "for");
                        literals.Add(Token.IN, "in ");
                        literals.Add(Token.WITH, "with");
                        literals.Add(Token.WHILE, "while");
                        literals.Add(Token.DO, "do");
                        literals.Add(Token.TRY, "try");
                        literals.Add(Token.CATCH, "catch");
                        literals.Add(Token.FINALLY, "finally");
                        literals.Add(Token.THROW, "throw ");
                        literals.Add(Token.SWITCH, "switch");
                        literals.Add(Token.BREAK, "break ");
                        literals.Add(Token.CONTINUE, "continue ");
                        literals.Add(Token.CASE, "case ");
                        literals.Add(Token.DEFAULT, "default");
                        literals.Add(Token.RETURN, "return ");
                        literals.Add(Token.VAR, "var ");
                        literals.Add(Token.SEMI, ";");
                        literals.Add(Token.ASSIGN, "=");
                        literals.Add(Token.ASSIGN_ADD, "+=");
                        literals.Add(Token.ASSIGN_SUB, "-=");
                        literals.Add(Token.ASSIGN_MUL, "*=");
                        literals.Add(Token.ASSIGN_DIV, "/=");
                        literals.Add(Token.ASSIGN_MOD, "%=");
                        literals.Add(Token.ASSIGN_BITOR, "|=");
                        literals.Add(Token.ASSIGN_BITXOR, "^=");
                        literals.Add(Token.ASSIGN_BITAND, "&=");
                        literals.Add(Token.ASSIGN_LSH, "<<=");
                        literals.Add(Token.ASSIGN_RSH, ">>=");
                        literals.Add(Token.ASSIGN_URSH, ">>>=");
                        literals.Add(Token.HOOK, "?");
                        literals.Add(Token.OBJECTLIT, ":");
                        literals.Add(Token.COLON, ":");
                        literals.Add(Token.OR, "||");
                        literals.Add(Token.AND, "&&");
                        literals.Add(Token.BITOR, "|");
                        literals.Add(Token.BITXOR, "^");
                        literals.Add(Token.BITAND, "&");
                        literals.Add(Token.SHEQ, "===");
                        literals.Add(Token.SHNE, "!==");
                        literals.Add(Token.EQ, "==");
                        literals.Add(Token.NE, "!=");
                        literals.Add(Token.LE, "<=");
                        literals.Add(Token.LT, "<");
                        literals.Add(Token.GE, ">=");
                        literals.Add(Token.GT, ">");
                        literals.Add(Token.INSTANCEOF, " instanceof ");
                        literals.Add(Token.LSH, "<<");
                        literals.Add(Token.RSH, ">>");
                        literals.Add(Token.URSH, ">>>");
                        literals.Add(Token.TYPEOF, "typeof ");
                        literals.Add(Token.VOID, "void ");
                        literals.Add(Token.CONST, "const ");
                        literals.Add(Token.NOT, "!");
                        literals.Add(Token.BITNOT, "~");
                        literals.Add(Token.POS, "+");
                        literals.Add(Token.NEG, "-");
                        literals.Add(Token.INC, "++");
                        literals.Add(Token.DEC, "--");
                        literals.Add(Token.ADD, "+");
                        literals.Add(Token.SUB, "-");
                        literals.Add(Token.MUL, "*");
                        literals.Add(Token.DIV, "/");
                        literals.Add(Token.MOD, "%");
                        literals.Add(Token.COLONCOLON, "::");
                        literals.Add(Token.DOTDOT, "..");
                        literals.Add(Token.DOTQUERY, ".(");
                        literals.Add(Token.XMLATTR, "@");

                        JavaScriptCompressor.Literals = literals;
                    }
                }
            }

            return JavaScriptCompressor.Literals;
        }

        private static HashSet<string> InitialiseReserved()
        {
            HashSet<string> reserved;


            if (JavaScriptCompressor.Reserved == null)
            {
                lock (JavaScriptCompressor._synLock)
                {
                    if (JavaScriptCompressor.Reserved == null)
                    {
                        reserved = new HashSet<string>();

                        // See http://developer.mozilla.org/en/docs/Core_JavaScript_1.5_Reference:Reserved_Words

                        // JavaScript 1.5 reserved words
                        reserved.Add("break");
                        reserved.Add("case");
                        reserved.Add("catch");
                        reserved.Add("continue");
                        reserved.Add("default");
                        reserved.Add("delete");
                        reserved.Add("do");
                        reserved.Add("else");
                        reserved.Add("finally");
                        reserved.Add("for");
                        reserved.Add("function");
                        reserved.Add("if");
                        reserved.Add("in");
                        reserved.Add("instanceof");
                        reserved.Add("new");
                        reserved.Add("return");
                        reserved.Add("switch");
                        reserved.Add("this");
                        reserved.Add("throw");
                        reserved.Add("try");
                        reserved.Add("typeof");
                        reserved.Add("var");
                        reserved.Add("void");
                        reserved.Add("while");
                        reserved.Add("with");
                        // Words reserved for future use
                        reserved.Add("abstract");
                        reserved.Add("boolean");
                        reserved.Add("byte");
                        reserved.Add("char");
                        reserved.Add("class");
                        reserved.Add("const");
                        reserved.Add("debugger");
                        reserved.Add("double");
                        reserved.Add("enum");
                        reserved.Add("export");
                        reserved.Add("extends");
                        reserved.Add("final");
                        reserved.Add("float");
                        reserved.Add("goto");
                        reserved.Add("implements");
                        reserved.Add("import");
                        reserved.Add("int");
                        reserved.Add("interface");
                        reserved.Add("long");
                        reserved.Add("native");
                        reserved.Add("package");
                        reserved.Add("private");
                        reserved.Add("protected");
                        reserved.Add("public");
                        reserved.Add("short");
                        reserved.Add("static");
                        reserved.Add("super");
                        reserved.Add("synchronized");
                        reserved.Add("throws");
                        reserved.Add("transient");
                        reserved.Add("volatile");
                        // These are not reserved, but should be taken into account
                        // in isValidIdentifier (See jslint source code)
                        reserved.Add("arguments");
                        reserved.Add("eval");
                        reserved.Add("true");
                        reserved.Add("false");
                        reserved.Add("Infinity");
                        reserved.Add("NaN");
                        reserved.Add("null");
                        reserved.Add("undefined");

                        JavaScriptCompressor.Reserved = reserved;
                    }
                }
            }

            return JavaScriptCompressor.Reserved;
        }

        private static void Initialise()
        {
            JavaScriptCompressor.InitialiseBuiltIn();
            JavaScriptCompressor.InitialiseOnesList();
            JavaScriptCompressor.InitialiseTwosList();
            JavaScriptCompressor.InitialiseThreesList();
            JavaScriptCompressor.InitialiseLiterals();
            JavaScriptCompressor.InitialiseReserved();
        }

        private static int CountChar(string haystack, 
            char needle)
        {
            int index = 0;
            int count = 0;
            int length = haystack.Length;


            while (index < length)
            {
                char c = haystack[index++];
                if (c == needle)
                {
                    count++;
                }
            }

            return count;
        }

        private static int PrintSourceString(string source, 
            int offset, 
            StringBuilder stringBuilder)
        {
            int length = source[offset];
            ++offset;
            if ((0x8000 & length) != 0)
            {
                length = ((0x7FFF & length) << 16) | source[offset];
                ++offset;
            }
            if (stringBuilder != null)
            {
                //string word = source.Substring(offset, offset + length);
                string word = source.Substring(offset, length);
                stringBuilder.Append(word);
            }

            return offset + length;
        }

        private static int PrintSourceNumber(string source,
            int offset, 
            StringBuilder stringBuilder)
        {
            double number = 0.0;
            char type = source[offset];
            ++offset;
            if (type == 'S')
            {
                if (stringBuilder != null)
                {
                    number = source[offset];
                }
                ++offset;
            }
            else if (type == 'J' || type == 'D')
            {
                if (stringBuilder != null)
                {
                    long lbits;
                    lbits = (long)source[offset] << 48;
                    lbits |= (long)source[offset + 1] << 32;
                    lbits |= (long)source[offset + 2] << 16;
                    lbits |= (long)source[offset + 3];
                    if (type == 'J')
                    {
                        number = lbits;
                    }
                    else
                    {
                        number = BitConverter.Int64BitsToDouble(lbits);
                    }
                }
                offset += 4;
            }
            else
            {
                // Bad source
                throw new InvalidOperationException();
            }

            if (stringBuilder != null)
            {
                stringBuilder.Append(ScriptConvert.ToString(number, 10));
            }

            return offset;
        }

        private static ArrayList Parse(StreamReader stream, 
            ErrorReporter reporter)
        {
            CompilerEnvirons compilerEnvirons = new CompilerEnvirons();
            Parser parser = new Parser(compilerEnvirons, reporter);
            parser.Parse(stream, null, 1);
            string source = parser.EncodedSource;
            
            int offset = 0;
            int length = source.Length;
            ArrayList tokens = new ArrayList();
            StringBuilder stringBuilder = new StringBuilder();

            while (offset < length) {
                int tt = source[offset++];
                switch (tt) {

                    case Token.SPECIALCOMMENT:
                    case Token.NAME:
                    case Token.REGEXP:
                    case Token.STRING:
                        stringBuilder.Length = 0;
                        offset = JavaScriptCompressor.PrintSourceString(source, 
                            offset, 
                            stringBuilder);
                        tokens.Add(new JavaScriptToken(tt, stringBuilder.ToString()));
                        break;

                    case Token.NUMBER:
                        stringBuilder.Length = 0;
                        offset = JavaScriptCompressor.PrintSourceNumber(source, offset, stringBuilder);
                        tokens.Add(new JavaScriptToken(tt, stringBuilder.ToString()));
                        break;

                    default:
                        string literal = (string)JavaScriptCompressor.Literals[tt];
                        if (literal != null) {
                            tokens.Add(new JavaScriptToken(tt, literal));
                        }
                        break;
                }
            }

            return tokens;
        }

        private static void ProcessStringLiterals(ArrayList tokens, bool merge)
        {
            string tv;
            int i, length = tokens.Count;
            JavaScriptToken token, prevToken, nextToken;

            if (merge)
            {
                // Concatenate string literals that are being appended wherever
                // it is safe to do so. Note that we take care of the case:
                //     "a" + "b".toUpperCase()

                for (i = 0; i < length; i++)
                {
                    token = (JavaScriptToken)tokens[i];
                    switch (token.TokenType)
                    {
                        case Token.ADD:
                            if (i > 0 && i < length)
                            {
                                prevToken = (JavaScriptToken)tokens[i - 1];
                                nextToken = (JavaScriptToken)tokens[i + 1];
                                if (prevToken.TokenType == Token.STRING && nextToken.TokenType == Token.STRING &&
                                    (i == length - 1 || ((JavaScriptToken)tokens[i + 2]).TokenType != Token.DOT))
                                {
                                    tokens[i - 1] = new JavaScriptToken(Token.STRING,
                                            prevToken.Value + nextToken.Value);
                                    tokens.RemoveAt(i + 1);
                                    tokens.RemoveAt(i);
                                    i = i - 1;
                                    length = length - 2;
                                    break;
                                }
                            }
                            break;
                    }
                }
            }

            // Second pass...
            for (i = 0; i < length; i++)
            {
                token = (JavaScriptToken)tokens[i];
                if (token.TokenType == Token.STRING)
                {
                    tv = token.Value;

                    // Finally, add the quoting characters and escape the string. We use
                    // the quoting character that minimizes the amount of escaping to save
                    // a few additional bytes.

                    char quotechar;
                    int singleQuoteCount = JavaScriptCompressor.CountChar(tv, '\'');
                    int doubleQuoteCount = JavaScriptCompressor.CountChar(tv, '"');
                    if (doubleQuoteCount <= singleQuoteCount)
                    {
                        quotechar = '"';
                    }
                    else
                    {
                        quotechar = '\'';
                    }

                    tv = quotechar + JavaScriptCompressor.EscapeString(tv, quotechar) + quotechar;

                    // String concatenation transforms the old script scheme:
                    //     '<scr'+'ipt ...><'+'/script>'
                    // into the following:
                    //     '<script ...></script>'
                    // which breaks if this code is embedded inside an HTML document.
                    // Since this is not the right way to do this, let's fix the code by
                    // transforming all "</script" into "<\/script"

                    if (tv.IndexOf("</script", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tv = tv.Replace("<\\/script", "<\\\\/script");
                    }

                    tokens[i] = new JavaScriptToken(Token.STRING, tv);
                }
            }
        }

        // Add necessary escaping that was removed in Rhino's tokenizer.
        private static string EscapeString(string s, 
            char quotechar)
        {
            if (quotechar != '"' &&
                quotechar != '\'')
            {
                throw new ArgumentException("quotechar argument has to be a \" or a \\ character only.",
                    "quotechar");
            }
            else if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0, L = s.Length; i < L; i++) {
                int c = s[i];
                if (c == quotechar) {
                    stringBuilder.Append("\\");
                }
                stringBuilder.Append((char) c);
            }

            return stringBuilder.ToString();
        }

        /*
         * Simple check to see whether a string is a valid identifier name.
         * If a string matches this pattern, it means it IS a valid
         * identifier name. If a string doesn't match it, it does not
         * necessarily mean it is not a valid identifier name.
         */
        private static bool IsValidIdentifier(string s)
        {
            Match match = SIMPLE_IDENTIFIER_NAME_PATTERN.Match(s);
            return (match.Success && !JavaScriptCompressor.Reserved.Contains(s));
        }

        /*
        * Transforms obj["foo"] into obj.foo whenever possible, saving 3 bytes.
        */
        private static void OptimizeObjectMemberAccess(ArrayList tokens) {

            string tv;
            int i, length;
            JavaScriptToken token;


            for (i = 0, length = tokens.Count; i < length; i++) {

                if (((JavaScriptToken) tokens[i]).TokenType == Token.LB &&
                    i > 0 && i < length - 2 &&
                    ((JavaScriptToken)tokens[i - 1]).TokenType == Token.NAME &&
                    ((JavaScriptToken)tokens[i + 1]).TokenType == Token.STRING &&
                    ((JavaScriptToken)tokens[i + 2]).TokenType == Token.RB)
                {
                    token = (JavaScriptToken) tokens[i + 1];
                    tv = token.Value;
                    tv = tv.Substring(1, tv.Length - 1);
                    if (JavaScriptCompressor.IsValidIdentifier(tv))
                    {
                        tokens[i] = new JavaScriptToken(Token.DOT, ".");
                        tokens[i + 1] = new JavaScriptToken(Token.NAME, tv);
                        tokens.RemoveAt(i + 2);
                        i = i + 2;
                        length = length - 1;
                    }
                }
            }
        }

        /*
         * Transforms 'foo': ... into foo: ... whenever possible, saving 2 bytes.
        */
        private static void OptimizeObjLitMemberDecl(ArrayList tokens)
        {
            string tv;
            int i, length;
            JavaScriptToken token;


            for (i = 0, length = tokens.Count; i < length; i++)
            {
                if (((JavaScriptToken)tokens[i]).TokenType == Token.OBJECTLIT &&
                    i > 0 && ((JavaScriptToken)tokens[i - 1]).TokenType == Token.STRING)
                {
                    token = (JavaScriptToken)tokens[i - 1];
                    tv = token.Value;
                    tv = tv.Substring(1, tv.Length - 1);
                    if (JavaScriptCompressor.IsValidIdentifier(tv))
                    {
                        tokens[i - 1] = new JavaScriptToken(Token.NAME, tv);
                    }
                }
            }
        }

        private ScriptOrFunctionScope GetCurrentScope()
        {
            return (ScriptOrFunctionScope)this._scopes.Peek();
        }

        private void EnterScope(ScriptOrFunctionScope scope)
        {
            this._scopes.Push(scope);
        }

        private void LeaveCurrentScope()
        {
            this._scopes.Pop();
        }

        private JavaScriptToken ConsumeToken()
        {
            return (JavaScriptToken)this._tokens[this._offset++];
        }

        private JavaScriptToken GetToken(int delta)
        {
            return (JavaScriptToken)this._tokens[this._offset + delta];
        }

        /*
         * Returns the identifier for the specified symbol defined in
         * the specified scope or in any scope above it. Returns null
         * if this symbol does not have a corresponding identifier.
         */
        private static JavaScriptIdentifier GetIdentifier(string symbol, 
            ScriptOrFunctionScope scope)
        {
            JavaScriptIdentifier identifier;


            while (scope != null)
            {
                identifier = scope.GetIdentifier(symbol);
                if (identifier != null)
                {
                    return identifier;
                }

                scope = scope.ParentScope;
            }

            return null;
        }

        /*
         * If either 'eval' or 'with' is used in a local scope, we must make
         * sure that all containing local scopes don't get munged. Otherwise,
         * the obfuscation would potentially introduce bugs.
         */
        private void ProtectScopeFromObfuscation(ScriptOrFunctionScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            if (scope == this._globalScope)
            {
                // The global scope does not get obfuscated,
                // so we don't need to worry about it...
                return;
            }

            // Find the highest local scope containing the specified scope.
            while (scope.ParentScope != this._globalScope)
            {
                scope = scope.ParentScope;
            }

            if (scope.ParentScope != this._globalScope)
            {
                throw new InvalidOperationException();
            }

            scope.PreventMunging();
        }

        private String GetDebugString(int max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException("max");
            }

            StringBuilder result = new StringBuilder();
            int start = Math.Max(this._offset - max, 0);
            int end = Math.Min(this._offset + max, this._tokens.Count);

            for (int i = start; i < end; i++)
            {
                JavaScriptToken token = (JavaScriptToken)this._tokens[i];
                if (i == this._offset - 1)
                {
                    result.Append(" ---> ");
                }

                result.Append(token.Value);

                if (i == this._offset - 1)
                {
                    result.Append(" <--- ");
                }
            }

            return result.ToString();
        }

        private void Warn(string message, 
            bool showDebugString)
        {
            if (this._verbose)
            {
                if (showDebugString)
                {
                    message = message + "\n" + this.GetDebugString(10);
                }

                this._logger.Warning(message, null, -1, null, -1);
            }
        }

        private void ParseFunctionDeclaration()
        {

            string symbol;
            JavaScriptToken token;
            ScriptOrFunctionScope currentScope, functionScope;
            JavaScriptIdentifier identifier;


            currentScope = this.GetCurrentScope();

            token = this.ConsumeToken();

            if (token.TokenType == Token.NAME)
            {
                if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                {
                    // Get the name of the function and declare it in the current scope.
                    symbol = token.Value;
                    if (currentScope.GetIdentifier(symbol) != null)
                    {
                        this.Warn("The function " + symbol + " has already been declared in the same scope...", true);
                    }

                    currentScope.DeclareIdentifier(symbol);
                }

                token = this.ConsumeToken();
            }

            if (token.TokenType != Token.LP)
            {
                throw new InvalidOperationException();
            }

            if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
            {
                functionScope = new ScriptOrFunctionScope(this._braceNesting, currentScope);
                this._indexedScopes.Add(this._offset, functionScope);
            }
            else
            {
                functionScope = (ScriptOrFunctionScope)this._indexedScopes[this._offset];
            }

            // Parse function arguments.
            int argpos = 0;
            while ((token = this.ConsumeToken()).TokenType != Token.RP)
            {
                if (token.TokenType != Token.NAME &&
                    token.TokenType != Token.COMMA)
                {
                    throw new InvalidOperationException();
                }

                if (token.TokenType == Token.NAME &&
                    this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                {
                    symbol = token.Value;
                    identifier = functionScope.DeclareIdentifier(symbol);

                    if (symbol.Equals("$super", StringComparison.OrdinalIgnoreCase) && argpos == 0)
                    {
                        // Exception for Prototype 1.6...
                        identifier.MarkedForMunging = false;
                    }

                    argpos++;
                }
            }

            token = this.ConsumeToken();

            if (token.TokenType != Token.LC)
            {
                throw new InvalidOperationException();
            }

            this._braceNesting++;

            token = this.GetToken(0);
            if (token.TokenType == Token.STRING &&
                    this.GetToken(1).TokenType == Token.SEMI)
            {
                // This is a hint. Hints are empty statements that look like
                // "localvar1:nomunge, localvar2:nomunge"; They allow developers
                // to prevent specific symbols from getting obfuscated (some heretic
                // implementations, such as Prototype 1.6, require specific variable
                // names, such as $super for example, in order to work appropriately.
                // Note: right now, only "nomunge" is supported in the right hand side
                // of a hint. However, in the future, the right hand side may contain
                // other values.
                this.ConsumeToken();
                string hints = token.Value;

                // Remove the leading and trailing quotes...
                hints = hints.Substring(1, hints.Length - 1).Trim();

                foreach (string hint in hints.Split(','))
                {
                    int idx = hint.IndexOf(':');
                    if (idx <= 0 || idx >= hint.Length - 1)
                    {
                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            // No need to report the error twice, hence the test...
                            this.Warn("Invalid hint syntax: " + hint, true);
                        }

                        break;
                    }

                    string variableName = hint.Substring(0, idx).Trim();
                    string variableType = hint.Substring(idx + 1).Trim();
                    if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                    {
                        functionScope.AddHint(variableName, variableType);
                    }
                    else if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE)
                    {
                        identifier = functionScope.GetIdentifier(variableName);
                        if (identifier != null)
                        {
                            if (variableType.Equals("nomunge", StringComparison.OrdinalIgnoreCase))
                            {
                                identifier.MarkedForMunging = false;
                            }
                            else
                            {
                                this.Warn("Unsupported hint value: " + hint, true);
                            }
                        }
                        else
                        {
                            this.Warn("Hint refers to an unknown identifier: " + hint, true);
                        }
                    }
                }
            }

            this.ParseScope(functionScope);
        }

        private void ParseCatch()
        {
            string symbol;
            JavaScriptToken token;
            ScriptOrFunctionScope currentScope;
            JavaScriptIdentifier identifier;


            token = this.GetToken(-1);
            if (token.TokenType != Token.CATCH)
            {
                throw new InvalidOperationException();
            }

            token = this.ConsumeToken();
            if (token.TokenType != Token.LP)
            {
                throw new InvalidOperationException();
            }

            token = this.ConsumeToken();
            if (token.TokenType != Token.NAME)
            {
                throw new InvalidOperationException();
            }

            symbol = token.Value;
            currentScope = this.GetCurrentScope();

            if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
            {
                // We must declare the exception identifier in the containing function
                // scope to avoid errors related to the obfuscation process. No need to
                // display a warning if the symbol was already declared here...
                currentScope.DeclareIdentifier(symbol);
            }
            else
            {
                identifier = JavaScriptCompressor.GetIdentifier(symbol, currentScope);
                identifier.RefCount++;
            }

            token = this.ConsumeToken();
            if (token.TokenType != Token.RP)
            {
                throw new InvalidOperationException();
            }
        }

        private void ParseExpression()
        {
            // Parse the expression until we encounter a comma or a semi-colon
            // in the same brace nesting, bracket nesting and paren nesting.
            // Parse functions if any...

            string symbol;
            JavaScriptToken token;
            ScriptOrFunctionScope currentScope;
            JavaScriptIdentifier identifier;


            int expressionBraceNesting = this._braceNesting;
            int bracketNesting = 0;
            int parensNesting = 0;
            int length = this._tokens.Count;

            while (this._offset < length)
            {
                token = this.ConsumeToken();
                currentScope = this.GetCurrentScope();

                switch (token.TokenType)
                {
                    case Token.SEMI:
                    case Token.COMMA:
                        if (this._braceNesting == expressionBraceNesting &&
                            bracketNesting == 0 &&
                            parensNesting == 0)
                        {
                            return;
                        }
                        break;

                    case Token.FUNCTION:
                        this.ParseFunctionDeclaration();
                        break;

                    case Token.LC:
                        this._braceNesting++;
                        break;

                    case Token.RC:
                        this._braceNesting--;
                        if (this._braceNesting < expressionBraceNesting)
                        {
                            throw new InvalidOperationException();
                        }
                        break;

                    case Token.LB:
                        bracketNesting++;
                        break;

                    case Token.RB:
                        bracketNesting--;
                        break;

                    case Token.LP:
                        parensNesting++;
                        break;

                    case Token.RP:
                        parensNesting--;
                        break;

                    case Token.SPECIALCOMMENT:
                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            this.ProtectScopeFromObfuscation(currentScope);
                            this.Warn("Using JScript conditional comments is not recommended." + (this._munge ? " Moreover, using JScript conditional comments reduces the level of compression!" : ""), true);
                        }
                        break;

                    case Token.NAME:
                        symbol = token.Value;

                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            if (symbol.Equals("eval", StringComparison.OrdinalIgnoreCase))
                            {
                                this.ProtectScopeFromObfuscation(currentScope);
                                this.Warn("Using 'eval' is not recommended." + (this._munge ? " Moreover, using 'eval' reduces the level of compression!" : ""), true);
                            }

                        }
                        else if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE)
                        {
                            if ((this._offset < 2 ||
                                (this.GetToken(-2).TokenType != Token.DOT &&
                                this.GetToken(-2).TokenType != Token.GET &&
                                this.GetToken(-2).TokenType != Token.SET)) &&
                                this.GetToken(0).TokenType != Token.OBJECTLIT)
                            {
                                identifier = JavaScriptCompressor.GetIdentifier(symbol, currentScope);

                                if (identifier == null)
                                {
                                    if (symbol.Length <= 3 && !JavaScriptCompressor._builtin.Contains(symbol))
                                    {
                                        // Here, we found an undeclared and un-namespaced symbol that is
                                        // 3 characters or less in length. Declare it in the global scope.
                                        // We don't need to declare longer symbols since they won't cause
                                        // any conflict with other munged symbols.
                                        this._globalScope.DeclareIdentifier(symbol);
                                        this.Warn("Found an undeclared symbol: " + symbol, true);
                                    }
                                }
                                else
                                {
                                    identifier.RefCount++;
                                }
                            }
                        }

                        break;
                }
            }
        }

        private void ParseScope(ScriptOrFunctionScope scope)
        {
            string symbol;
            JavaScriptToken token;
            JavaScriptIdentifier identifier;


            int length = this._tokens.Count;

            this.EnterScope(scope);

            while (this._offset < length)
            {

                token = this.ConsumeToken();

                switch (token.TokenType)
                {
                    case Token.VAR:
                    case Token.CONST:
                        if (token.TokenType == Token.VAR)
                        {
                            if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE &&
                            scope.VarCount++ > 1)
                            {
                                this.Warn("Try to use a single 'var' statement per scope.", true);
                            }
                        }

                        // The var keyword is followed by at least one symbol name.
                        // If several symbols follow, they are comma separated.
                        //for (; ;)
                        while (true)
                        {
                            token = this.ConsumeToken();
                            if (token.TokenType != Token.NAME)
                            {
                                throw new InvalidOperationException();
                            }

                            if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                            {
                                symbol = token.Value;
                                if (scope.GetIdentifier(symbol) == null)
                                {
                                    scope.DeclareIdentifier(symbol);
                                }
                                else
                                {
                                    this.Warn("The variable " + symbol + " has already been declared in the same scope...", true);
                                }
                            }

                            token = this.GetToken(0);
                            if (token.TokenType != Token.SEMI &&
                                    token.TokenType != Token.ASSIGN &&
                                    token.TokenType != Token.COMMA &&
                                    token.TokenType != Token.IN)
                            {
                                throw new InvalidOperationException();
                            }

                            if (token.TokenType == Token.IN)
                            {
                                break;
                            }
                            else
                            {
                                this.ParseExpression();
                                token = this.GetToken(-1);
                                if (token.TokenType == Token.SEMI)
                                {
                                    break;
                                }
                            }
                        }

                        break;

                    case Token.FUNCTION:
                        this.ParseFunctionDeclaration();
                        break;

                    case Token.LC:
                        this._braceNesting++;
                        break;

                    case Token.RC:
                        this._braceNesting--;
                        if (this._braceNesting < scope.BraceNesting)
                        {
                            throw new InvalidOperationException();
                        }

                        if (this._braceNesting == scope.BraceNesting)
                        {
                            this.LeaveCurrentScope();
                            return;
                        }

                        break;

                    case Token.WITH:
                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            // Inside a 'with' block, it is impossible to figure out
                            // statically whether a symbol is a local variable or an
                            // object member. As a consequence, the only thing we can
                            // do is turn the obfuscation off for the highest scope
                            // containing the 'with' block.
                            this.ProtectScopeFromObfuscation(scope);
                            this.Warn("Using 'with' is not recommended." + (this._munge ? " Moreover, using 'with' reduces the level of compression!" : ""), true);
                        }
                        break;

                    case Token.CATCH:
                        this.ParseCatch();
                        break;

                    case Token.SPECIALCOMMENT:
                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            this.ProtectScopeFromObfuscation(scope);
                            this.Warn("Using JScript conditional comments is not recommended." + (this._munge ? " Moreover, using JScript conditional comments reduces the level of compression." : ""), true);
                        }
                        break;

                    case Token.NAME:
                        symbol = token.Value;

                        if (this._mode == JavaScriptCompressor.BUILDING_SYMBOL_TREE)
                        {
                            if (symbol.Equals("eval", StringComparison.OrdinalIgnoreCase))
                            {
                                this.ProtectScopeFromObfuscation(scope);
                                this.Warn("Using 'eval' is not recommended." + (this._munge ? " Moreover, using 'eval' reduces the level of compression!" : ""), true);

                            }
                        }
                        else if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE)
                        {
                            if ((this._offset < 2 ||
                                this.GetToken(-2).TokenType != Token.DOT) &&
                                    this.GetToken(0).TokenType != Token.OBJECTLIT)
                            {
                                identifier = JavaScriptCompressor.GetIdentifier(symbol, scope);

                                if (identifier == null)
                                {
                                    if (symbol.Length <= 3 &&
                                        !JavaScriptCompressor._builtin.Contains(symbol))
                                    {
                                        // Here, we found an undeclared and un-namespaced symbol that is
                                        // 3 characters or less in length. Declare it in the global scope.
                                        // We don't need to declare longer symbols since they won't cause
                                        // any conflict with other munged symbols.
                                        this._globalScope.DeclareIdentifier(symbol);
                                        this.Warn("Found an undeclared symbol: " + symbol, true);
                                    }
                                }
                                else
                                {
                                    identifier.RefCount++;
                                }
                            }
                        }

                        break;
                }
            }
        }

        private void BuildSymbolTree()
        {
            this._offset = 0;
            this._braceNesting  = 0;
            this._scopes.Clear();
            this._indexedScopes.Clear();
            this._indexedScopes.Add(0, this._globalScope);
            this._mode = JavaScriptCompressor.BUILDING_SYMBOL_TREE;
            this.ParseScope(this._globalScope);
        }

        private void MungeSymboltree()
        {
            if (!this._munge)
            {
                return;
            }

            // One problem with obfuscation resides in the use of undeclared
            // and un-namespaced global symbols that are 3 characters or less
            // in length. Here is an example:
            //
            //     var declaredGlobalVar;
            //
            //     function declaredGlobalFn() {
            //         var localvar;
            //         localvar = abc; // abc is an undeclared global symbol
            //     }
            //
            // In the example above, there is a slim chance that localvar may be
            // munged to 'abc', conflicting with the undeclared global symbol
            // abc, creating a potential bug. The following code detects such
            // global symbols. This must be done AFTER the entire file has been
            // parsed, and BEFORE munging the symbol tree. Note that declaring
            // extra symbols in the global scope won't hurt.
            //
            // Note: Since we go through all the tokens to do this, we also use
            // the opportunity to count how many times each identifier is used.

            this._offset = 0;
            this._braceNesting = 0;
            this._scopes.Clear();
            this._mode = JavaScriptCompressor.CHECKING_SYMBOL_TREE;
            this.ParseScope(this._globalScope);
            this._globalScope.Munge();
        }

        private StringBuilder PrintSymbolTree(int linebreakpos,
            bool preserveAllSemiColons)
        {
            this._offset = 0;
            this._braceNesting = 0;
            this._scopes.Clear();

            string symbol;
            JavaScriptToken token;
            ScriptOrFunctionScope currentScope;
            JavaScriptIdentifier identifier;

            int length = this._tokens.Count;
            StringBuilder result = new StringBuilder();

            int linestartpos = 0;

            this.EnterScope(this._globalScope);

            while (this._offset < length)
            {
                token = this.ConsumeToken();
                symbol = token.Value;
                currentScope = this.GetCurrentScope();

                switch (token.TokenType)
                {
                    case Token.NAME:
                        if (this._offset >= 2 &&
                            this.GetToken(-2).TokenType == Token.DOT ||
                                this.GetToken(0).TokenType == Token.OBJECTLIT)
                        {
                            result.Append(symbol);

                        }
                        else
                        {
                            identifier = JavaScriptCompressor.GetIdentifier(symbol, currentScope);

                            if (identifier != null)
                            {
                                if (identifier.MungedValue != null)
                                {
                                    result.Append(identifier.MungedValue);
                                }
                                else
                                {
                                    result.Append(symbol);
                                }

                                if (currentScope != this._globalScope &&
                                    identifier.RefCount == 0)
                                {
                                    this.Warn("The symbol " + symbol + " is declared but is apparently never used.\nThis code can probably be written in a more compact way.", true);
                                }
                            }
                            else
                            {
                                result.Append(symbol);
                            }
                        }
                        break;

                    case Token.REGEXP:
                    case Token.NUMBER:
                    case Token.STRING:
                        result.Append(symbol);
                        break;

                    case Token.ADD:
                    case Token.SUB:
                        result.Append((string)JavaScriptCompressor.Literals[token.TokenType]);

                        if (this._offset < length)
                        {
                            token = this.GetToken(0);
                            if (token.TokenType == Token.INC ||
                                token.TokenType == Token.DEC ||
                                token.TokenType == Token.ADD ||
                                token.TokenType == Token.DEC)
                            {
                                // Handle the case x +/- ++/-- y
                                // We must keep a white space here. Otherwise, x +++ y would be
                                // interpreted as x ++ + y by the compiler, which is a bug (due
                                // to the implicit assignment being done on the wrong variable)
                                result.Append(' ');
                            }
                            else if (token.TokenType == Token.POS &&
                                this.GetToken(-1).TokenType == Token.ADD ||
                                token.TokenType == Token.NEG &&
                                this.GetToken(-1).TokenType == Token.SUB)
                            {
                                // Handle the case x + + y and x - - y
                                result.Append(' ');
                            }
                        }
                        break;

                    case Token.FUNCTION:
                        result.Append("function");
                        token = this.ConsumeToken();

                        if (token.TokenType == Token.NAME)
                        {
                            result.Append(' ');
                            symbol = token.Value;
                            identifier = JavaScriptCompressor.GetIdentifier(symbol, currentScope);

                            if (identifier == null)
                            {
                                throw new InvalidOperationException();
                            }

                            if (identifier.MungedValue != null)
                            {
                                result.Append(identifier.MungedValue);
                            }
                            else
                            {
                                result.Append(symbol);
                            }

                            if (currentScope != this._globalScope &&
                                identifier.RefCount == 0)
                            {
                                this.Warn("The symbol " + symbol + " is declared but is apparently never used.\nThis code can probably be written in a more compact way.", true);
                            }

                            token = this.ConsumeToken();
                        }

                        if (token.TokenType != Token.LP)
                        {
                            throw new InvalidOperationException();
                        }

                        result.Append('(');
                        currentScope = (ScriptOrFunctionScope)this._indexedScopes[this._offset];
                        this.EnterScope(currentScope);

                        while ((token = this.ConsumeToken()).TokenType != Token.RP)
                        {
                            if (token.TokenType != Token.NAME &&
                                token.TokenType != Token.COMMA)
                            {
                                throw new InvalidOperationException();
                            }

                            if (token.TokenType == Token.NAME)
                            {
                                symbol = token.Value;
                                identifier = JavaScriptCompressor.GetIdentifier(symbol, currentScope);

                                if (identifier == null)
                                {
                                    throw new InvalidOperationException();
                                }

                                if (identifier.MungedValue != null)
                                {
                                    result.Append(identifier.MungedValue);
                                }
                                else
                                {
                                    result.Append(symbol);
                                }
                            }
                            else if (token.TokenType == Token.COMMA)
                            {
                                result.Append(',');
                            }
                        }

                        result.Append(')');
                        token = this.ConsumeToken();

                        if (token.TokenType != Token.LC)
                        {
                            throw new InvalidOperationException();
                        }

                        result.Append('{');
                        this._braceNesting++;
                        token = this.GetToken(0);

                        if (token.TokenType == Token.STRING &&
                            this.GetToken(1).TokenType == Token.SEMI)
                        {
                            // This is a hint. Skip it!
                            this.ConsumeToken();
                            this.ConsumeToken();
                        }
                        break;

                    case Token.RETURN:
                        result.Append("return");

                        // No space needed after 'return' when followed
                        // by '(', '[', '{', a string or a regexp.
                        if (this._offset < length)
                        {
                            token = this.GetToken(0);
                            if (token.TokenType != Token.LP &&
                                    token.TokenType != Token.LB &&
                                    token.TokenType != Token.LC &&
                                    token.TokenType != Token.STRING &&
                                    token.TokenType != Token.REGEXP)
                            {
                                result.Append(' ');
                            }
                        }
                        break;

                    case Token.CASE:
                        result.Append("case");

                        // White-space needed after 'case' when not followed by a string.
                        if (this._offset < length &&
                            this.GetToken(0).TokenType != Token.STRING)
                        {
                            result.Append(' ');
                        }
                        break;

                    case Token.THROW:
                        // White-space needed after 'throw' when not followed by a string.
                        result.Append("throw");

                        if (this._offset < length &&
                            this.GetToken(0).TokenType != Token.STRING)
                        {
                            result.Append(' ');
                        }
                        break;

                    case Token.BREAK:
                        result.Append("break");

                        if (this._offset < length &&
                            this.GetToken(0).TokenType != Token.SEMI)
                        {
                            // If 'break' is not followed by a semi-colon, it must be
                            // followed by a label, hence the need for a white space.
                            result.Append(' ');
                        }
                        break;

                    case Token.CONTINUE:
                        result.Append("continue");
                        if (this._offset < length &&
                            this.GetToken(0).TokenType != Token.SEMI)
                        {
                            // If 'continue' is not followed by a semi-colon, it must be
                            // followed by a label, hence the need for a white space.
                            result.Append(' ');
                        }
                        break;

                    case Token.LC:
                        result.Append('{');
                        this._braceNesting++;
                        break;

                    case Token.RC:
                        result.Append('}');
                        this._braceNesting--;
                        if (this._braceNesting < currentScope.BraceNesting)
                        {
                            throw new InvalidOperationException();
                        }

                        if (this._braceNesting == currentScope.BraceNesting)
                        {
                            this.LeaveCurrentScope();
                        }
                        break;

                    case Token.SEMI:
                        // No need to output a semi-colon if the next character is a right-curly...
                        if (preserveAllSemiColons ||
                            this._offset < length &&
                            this.GetToken(0).TokenType != Token.RC)
                        {
                            result.Append(';');
                        }

                        if (linebreakpos >= 0 &&
                            result.Length - linestartpos > linebreakpos)
                        {
                            // Some source control tools don't like it when files containing lines longer
                            // than, say 8000 characters, are checked in. The linebreak option is used in
                            // that case to split long lines after a specific column.
                            result.Append('\n');
                            linestartpos = result.Length;
                        }
                        break;

                    case Token.SPECIALCOMMENT:
                        if (result.Length > 0 &&
                            result[result.Length - 1] != '\n')
                        {
                            result.Append("\n");
                        }

                        result.Append("/*");
                        result.Append(symbol);
                        result.Append("*/\n");
                        break;

                    default:
                        string literal = (string)JavaScriptCompressor.Literals[token.TokenType];
                        if (literal != null)
                        {
                            result.Append(literal);
                        }
                        else
                        {
                            this.Warn("This symbol cannot be printed: " + symbol, true);
                        }
                        break;
                }
            }

            // Append a semi-colon at the end, even if unnecessary semi-colons are
            // supposed to be removed. This is especially useful when concatenating
            // several minified files (the absence of an ending semi-colon at the
            // end of one file may very likely cause a syntax error)
            if (!preserveAllSemiColons &&
                result.Length > 0)
            {
                if (result[result.Length - 1] == '\n')
                {
                    result.Append(";", result.Length - 1, 1);
                }
                else
                {
                    result.Append(';');
                }
            }

            return result;
        }

        #endregion

        #region Public Methods

        public static string Compress(string javaScript)
        {
            return JavaScriptCompressor.Compress(javaScript,
                true);
        }

        public static string Compress(string javaScript,
            bool isVerboseLogging)
        {
            JavaScriptCompressor javaScriptCompressor;


            if (string.IsNullOrEmpty(javaScript))
            {
                throw new ArgumentNullException("javaScript");
            }

            javaScriptCompressor = new JavaScriptCompressor(javaScript,
                isVerboseLogging);

            return javaScriptCompressor.Compress(80,
                false,
                true,
                true,
                false);
        }

        public string Compress(int lineBreak,
            bool munge,
            bool verbose,
            bool preserveAllSemicolons,
            bool disableOptimizations)
        {
            this._munge = munge;
            this._verbose = verbose;

            JavaScriptCompressor.ProcessStringLiterals(this._tokens, 
                !disableOptimizations);

            if (!disableOptimizations)
            {
                JavaScriptCompressor.OptimizeObjectMemberAccess(this._tokens);
                JavaScriptCompressor.OptimizeObjLitMemberDecl(this._tokens);
            }

            this.BuildSymbolTree();
            this.MungeSymboltree();
            StringBuilder stringBuilder = this.PrintSymbolTree(lineBreak, preserveAllSemicolons);

            return stringBuilder.ToString();
        }

        #endregion

        #endregion
    }
}