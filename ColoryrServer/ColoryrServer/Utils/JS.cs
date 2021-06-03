using System;
using System.IO;
using System.Text;

namespace ColoryrServer.Utils
{
    public sealed class JavaScriptMinifier
    {
        private const int EOF = -1;

        private readonly StringBuilder _JsBuilder;
        private readonly TextReader _JsReader;
        private int _TheA = Convert.ToInt32('\n');
        private int _TheB;
        private int _TheLookahead = EOF;

        /// <summary>
        /// 初始化 <see cref="JavaScriptMinifier"/> 类的新实例。
        /// </summary>
        /// <param name="jsReader">包含要压缩的 JavaScript 代码的 <see cref="TextReader"/>。</param>
        private JavaScriptMinifier(TextReader jsReader)
        {
            if (jsReader == null)
                throw new ArgumentNullException("jsReader");

            _JsReader = jsReader;
            _JsBuilder = new StringBuilder();
        }

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="js">包含要压缩的 JavaScript 代码的 <see cref="StringBuilder"/>。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(StringBuilder js)
        {
            return Minify(new StringReader(js.ToString()));
        }

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="jsCode">要压缩的 JavaScript 代码。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(string jsCode) { return Minify(new StringReader(jsCode)); }

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="jsReader">包含要压缩的 JavaScript 代码的 <see cref="TextReader"/>。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(TextReader jsReader)
        {
            JavaScriptMinifier jsmin = new JavaScriptMinifier(jsReader);

            jsmin._Jsmin();

            return jsmin._JsBuilder;
        }

        private void _Jsmin()
        {
            _Action(3);

            while (_TheA != EOF)
            {
                switch ((Char)_TheA)
                {
                    case ' ':
                        if (_IsAlphanum(_TheB))
                            _Action(1);
                        else
                            _Action(2);

                        break;
                    case '\n':
                        switch ((Char)_TheB)
                        {
                            case '{':
                            case '[':
                            case '(':
                            case '+':
                            case '-':
                                _Action(1);

                                break;
                            case ' ':
                                _Action(3);

                                break;
                            default:
                                if (_IsAlphanum(_TheB))
                                    _Action(1);
                                else
                                    _Action(2);

                                break;
                        }

                        break;
                    default:
                        switch ((Char)_TheB)
                        {
                            case ' ':
                                if (_IsAlphanum(_TheA))
                                {
                                    _Action(1);

                                    break;
                                }

                                _Action(3);

                                break;
                            case '\n':
                                switch ((Char)_TheA)
                                {
                                    case '}':
                                    case ']':
                                    case ')':
                                    case '+':
                                    case '-':
                                    case '"':
                                    case '\'':
                                        _Action(1);

                                        break;
                                    default:
                                        if (_IsAlphanum(_TheA))
                                            _Action(1);
                                        else
                                            _Action(3);

                                        break;
                                }

                                break;
                            default:
                                _Action(1);

                                break;
                        }

                        break;
                }
            }
        }

        private void _Action(int d)
        {
            if (d <= 1)
                _Put(_TheA);
            if (d <= 2)
            {
                _TheA = _TheB;

                if (_TheA == '\'' || _TheA == '"')
                {
                    for (; ; )
                    {
                        _Put(_TheA);
                        _TheA = _Get();

                        if (_TheA == _TheB)
                            break;
                        if (_TheA <= '\n')
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}", _TheA));
                        if (_TheA != '\\')
                            continue;

                        _Put(_TheA);
                        _TheA = _Get();
                    }
                }
            }

            if (d > 3)
                return;

            _TheB = _Next();

            if (_TheB != '/' ||
                (_TheA != '('
                && _TheA != ','
                && _TheA != '='
                && _TheA != '['
                && _TheA != '!'
                && _TheA != ':'
                && _TheA != '&'
                && _TheA != '|'
                && _TheA != '?'
                && _TheA != '{'
                && _TheA != '}'
                && _TheA != ';'
                && _TheA != '\n'))
                return;

            _Put(_TheA);
            _Put(_TheB);

            for (; ; )
            {
                _TheA = _Get();

                if (_TheA == '/')
                    break;

                if (_TheA == '\\')
                {
                    _Put(_TheA);
                    _TheA = _Get();
                }
                else if (_TheA <= '\n')
                    throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.", _TheA));

                _Put(_TheA);
            }

            _TheB = _Next();
        }

        private int _Next()
        {
            int c = _Get();
            const int s = '*';

            if (c == '/')
            {
                switch (_Peek())
                {
                    case '/':
                        for (; ; )
                        {
                            c = _Get();

                            if (c <= '\n')
                                return c;
                        }
                    case '*':
                        _Get();

                        for (; ; )
                        {
                            switch (_Get())
                            {
                                case s:
                                    if (_Peek() == '/')
                                    {
                                        _Get();

                                        return Convert.ToInt32(' ');
                                    }

                                    break;
                                case EOF:
                                    throw new Exception("Error: JSMIN Unterminated comment.");
                            }
                        }
                    default:
                        return c;
                }
            }

            return c;
        }

        private int _Peek()
        {
            _TheLookahead = _Get();

            return _TheLookahead;
        }

        private int _Get()
        {
            int c = _TheLookahead;
            _TheLookahead = EOF;

            if (c == EOF)
                c = _JsReader.Read();

            return c >= ' ' || c == '\n' || c == EOF ? c : (c == '\r' ? '\n' : ' ');
        }

        private void _Put(int c) { _JsBuilder.Append((char)c); }

        private static bool _IsAlphanum(int c) { return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' || c > 126; }
    }
}
