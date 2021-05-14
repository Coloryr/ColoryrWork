using HtmlCompression.Core;
using Yahoo.Yui.Compressor;

namespace ColoryrServer.Utils
{
    internal class Css : HtmlCompression.Core.ICompressor
    {
        private static readonly CssCompressor css = new();
        public string Compress(string source)
        {
            return css.Compress(source);
        }
    }
    internal class JS : HtmlCompression.Core.ICompressor
    {
        private static readonly JavaScriptCompressor css = new();
        public string Compress(string source)
        {
            if (source == "")
                return source;
            return css.Compress(source);
        }
    }
    internal class CodeCompress
    {
        private static readonly HtmlCompressor Compress = new(new HtmlCompressorSettings
        {
            CompressCss = true,
            CompressJavaScript = true,
            JavaScriptCompressor = new JS(),
            CssCompressor = new Css()
        });

        public static string JS(string js)
        {
            return Compress.Settings.JavaScriptCompressor.Compress(js);
        }
        public static string CSS(string css)
        {
            return Compress.Settings.CssCompressor.Compress(css);
        }
        public static string HTML(string html)
        {
            return Compress.Compress(html);
        }
    }
}
