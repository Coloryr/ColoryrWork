using HtmlCompression.Core;
using Yahoo.Yui.Compressor;

namespace ColoryrServer.Utils
{
    internal class CodeCompress
    {
        private static readonly HtmlCompressor html = new();
        private static readonly CssCompressor css = new();
        private static readonly JavaScriptCompressor js = new();

        public static string JS(string code)
        {
            return js.Compress(code);
        }
        public static string CSS(string code)
        {
            return css.Compress(code);
        }
        public static string HTML(string code)
        {
            return html.Compress(code);
        }
    }
}
