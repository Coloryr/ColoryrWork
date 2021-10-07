using System.Text;
using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
    public class ScriptPreserver : Preserver
    {
        private readonly SkipPreserver _skipPreserver;

        private static readonly Regex TypeAttrPattern = new Regex("type\\s*=\\s*([\\\"']*)(.+?)\\1",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        internal static readonly Regex CdataPattern = new Regex("\\s*<!\\[CDATA\\[(.*?)\\]\\]>\\s*",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public ScriptPreserver(SkipPreserver skipPreserver) : base(new Regex("(<script[^>]*?>)(.*?)(</script>)",
          RegexOptions.Singleline | RegexOptions.IgnoreCase))
        {
            _skipPreserver = skipPreserver;
            BlockIndex = 2;
            ExpandReplacement = true;
        }

        public override void Process(HtmlCompressor compressor)
        {
            if (compressor.Settings.GenerateStatistics)
            {
                foreach (var block in _blocks)
                {
                    compressor.Statistics.GetOriginalMetrics()
                      .SetInlineScriptSize(compressor.Statistics.GetOriginalMetrics().GetInlineScriptSize() + block.Length);
                }
            }

            if (compressor.Settings.CompressJavaScript)
            {
                for (var i = 0; i < _blocks.Count; i++)
                {
                    _blocks[i] = CompressJavaScript(compressor, _blocks[i]);
                }
            }
            else if (compressor.Settings.GenerateStatistics)
            {
                foreach (var block in _blocks)
                {
                    compressor.Statistics.SetPreservedSize(compressor.Statistics.GetPreservedSize() + block.Length);
                }
            }

            if (compressor.Settings.GenerateStatistics)
            {
                foreach (var block in _blocks)
                {
                    compressor.Statistics.GetCompressedMetrics()
                      .SetInlineScriptSize(compressor.Statistics.GetCompressedMetrics().GetInlineScriptSize() + block.Length);
                }
            }
        }

        public override int PreserveMatch(string html, Match match, StringBuilder sb, int lastValue, ref int index)
        {
            //check type
            var type = "";
            var typeMatcher = TypeAttrPattern.Match(match.Groups[1].Value);
            if (typeMatcher.Success)
            {
                type = typeMatcher.Groups[2].Value.ToLowerInvariant();
            }

            if (type.Length == 0 || type.Equals("text/javascript") || type.Equals("application/javascript"))
            {
                //javascript block, preserve and compress with js compressor
                _blocks.Add(match.Groups[2].Value);

                sb.Append(html.Substring(lastValue, match.Index - lastValue));
                //matcher.appendReplacement(sb, "$1" + string.Format(tempScriptBlock, index++) + "$3");
                sb.Append(match.Result("$1" + GetTempBlock(index++) + "$3"));

                lastValue = match.Index + match.Length;
            }
            else if (type.Equals("text/x-jquery-tmpl"))
            {
                //jquery template, ignore so it gets compressed with the rest of html
            }
            else
            {
                //some custom script, preserve it inside "skip blocks" so it won't be compressed with js compressor 
                lastValue = _skipPreserver.PreserveMatch(html, match, sb, lastValue, ref index, match.Groups[2].Value);
            }
            return lastValue;
        }

        private static readonly Regex ScriptCdataPattern = new Regex("/\\*\\s*<!\\[CDATA\\[\\*/(.*?)/\\*\\]\\]>\\s*\\*/",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private string CompressJavaScript(HtmlCompressor compressor, string source)
        {
            //set default javascript compressor
            if (compressor.Settings.JavaScriptCompressor == null)
            {
                return source;
                //YuiJavaScriptCompressor yuiJsCompressor = new YuiJavaScriptCompressor();
                //yuiJsCompressor.setNoMunge(yuiJsNoMunge);
                //yuiJsCompressor.setPreserveAllSemiColons(yuiJsPreserveAllSemiColons);
                //yuiJsCompressor.setDisableOptimizations(yuiJsDisableOptimizations);
                //yuiJsCompressor.setLineBreak(yuiJsLineBreak);

                //if (yuiErrorReporter != null)
                //{
                //    yuiJsCompressor.setErrorReporter(yuiErrorReporter);
                //}

                //javaScriptCompressor = yuiJsCompressor;
            }

            //detect CDATA wrapper
            var scriptCdataWrapper = false;
            var cdataWrapper = false;
            var matcher = ScriptCdataPattern.Match(source);
            if (matcher.Success)
            {
                scriptCdataWrapper = true;
                source = matcher.Groups[1].Value;
            }
            else if (CdataPattern.Match(source).Success)
            {
                cdataWrapper = true;
                source = matcher.Groups[1].Value;
            }

            var result = compressor.Settings.JavaScriptCompressor.Compress(source);

            if (scriptCdataWrapper)
            {
                result = $"/*<![CDATA[*/{result}/*]]>*/";
            }
            else if (cdataWrapper)
            {
                result = $"<![CDATA[{result}]]>";
            }

            return result;
        }
    }
}