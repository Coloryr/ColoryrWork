using HtmlCompression.Core;
using Yahoo.Yui.Compressor;

namespace ColoryrServer.Utils
{
    internal class Css : HtmlCompression.Core.ICompressor
    {
        public string Compress(string source)
        {
            CssCompressor css = new CssCompressor();
            return css.Compress(source);
        }
    }
    internal class JS : HtmlCompression.Core.ICompressor
    {
        public string Compress(string source)
        {
            if (source == "")
                return source;
            JavaScriptCompressor css = new JavaScriptCompressor();
            return css.Compress(source);
        }
    }
    internal class CodeCompress
    {
        private static readonly HtmlCompressor Compress = new HtmlCompressor(new HtmlCompressorSettings
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
