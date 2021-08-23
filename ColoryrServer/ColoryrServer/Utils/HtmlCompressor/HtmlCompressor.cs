using HtmlCompression.Core.Preservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlCompression.Core
{
    // Original von: https://code.google.com/p/htmlcompressor/
    // Diese Datei von https://code.google.com/p/htmlcompressor/source/browse/trunk/src/main/java/com/googlecode/htmlcompressor/compressor/HtmlCompressor.java
    // Tipps auf http://stackoverflow.com/questions/3789472/what-is-the-c-sharp-regex-equivalent-to-javas-appendreplacement-and-appendtail
    // Java-Regex auf http://www.devarticles.com/c/a/Java/Introduction-to-the-Javautilregex-Object-Model/8/

    /**
	 * Class that compresses given HTML source by removing comments, extra spaces and 
	 * line breaks while preserving content within &lt;pre>, &lt;textarea>, &lt;script> 
	 * and &lt;style> tags. 
	 * <p>Blocks that should be additionally preserved could be marked with:
	 * <br><code>&lt;!-- {{{ -->
	 * <br>&nbsp;&nbsp;&nbsp;&nbsp;...
	 * <br>&lt;!-- }}} --></code> 
	 * <br>or any number of user defined patterns. 
	 * <p>Content inside &lt;script> or &lt;style> tags could be optionally compressed using 
	 * <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a> or <a href="http://code.google.com/closure/compiler/">Google Closure Compiler</a>
	 * libraries.
	 * 
	 * @author <a href="mailto:serg472@gmail.com">Sergiy Kovalchuk</a>
	 */

    public sealed class HtmlCompressor :
      ICompressor
    {
        public HtmlCompressorSettings Settings { get; set; }
        public List<Preserver> Preservers { get; set; } = new List<Preserver>();
        //public static readonly string JS_COMPRESSOR_YUI = "yui";
        //public static readonly string JS_COMPRESSOR_CLOSURE = "closure";

        private readonly Preserver PrePreserver = new PreTagPreserver();

        private readonly Preserver TextAreaPreserver = new TextAreaPreserver();

        private readonly Preserver ScriptPreserver;

        private readonly Preserver StylePreserver = new StylePreserver();

        private readonly SkipPreserver SkipPreserver = new SkipPreserver();

        private readonly Preserver CondCommentPreserver;
        private readonly Preserver InlineEventsPreserver = new InlineEventsPreserver();
        private readonly Preserver LineBreakPreserver;

        public HtmlCompressor() : this(new HtmlCompressorSettings())
        {

        }

        public HtmlCompressor(HtmlCompressorSettings settings)
        {
            Settings = settings;
            CondCommentPreserver = new CondCommentPreserver(this);
            ScriptPreserver = new ScriptPreserver(SkipPreserver);
            LineBreakPreserver = new LineBreakPreserver(Settings);
            Preservers.AddRange(new[]
            {
                PrePreserver,
                TextAreaPreserver,
                ScriptPreserver,
                StylePreserver,
                SkipPreserver,
                CondCommentPreserver,
                InlineEventsPreserver,
                LineBreakPreserver
            });
        }

        /**
		 * Predefined pattern that matches <code>&lt;?php ... ?></code> tags. 
		 * Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
		 */

        public static readonly Regex PhpTagPattern = new Regex("<\\?php.*?\\?>",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /**
		 * Predefined pattern that matches <code>&lt;% ... %></code> tags. 
		 * Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
		 */
        public static readonly Regex ServerScriptTagPattern = new Regex("<%.*?%>", RegexOptions.Singleline);

        /**
		 * Predefined pattern that matches <code>&lt;--# ... --></code> tags. 
		 * Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
		 */
        public static readonly Regex ServerSideIncludePattern = new Regex("<!--\\s*#.*?-->", RegexOptions.Singleline);

        private static readonly string _blockTagsMin = "html,head,body,br,p";
        /**
		 * Predefined list of tags that are very likely to be block-level. 
		 * Could be passed to {@link #setRemoveSurroundingSpaces(string) setRemoveSurroundingSpaces} method.
		 */
        public static readonly IReadOnlyList<string> BlockTagsMin = _blockTagsMin.Split(',').ToList();

        private static readonly string _blockTagsMax = "h1,h2,h3,h4,h5,h6,blockquote,center,dl,fieldset,form,frame,frameset,hr,noframes,ol,table,tbody,tr,td,th,tfoot,thead,ul";
        /**
		 * Predefined list of tags that are block-level by default, excluding <code>&lt;div></code> and <code>&lt;li></code> tags. 
		 * Table tags are also included.
		 * Could be passed to {@link #setRemoveSurroundingSpaces(string) setRemoveSurroundingSpaces} method.
		 */
        public static readonly IReadOnlyList<string> BlockTagsMax = BlockTagsMin.Concat(_blockTagsMax.Split(',')).ToList();

        ////YUICompressor settings
        //private bool yuiJsNoMunge = false;
        //private bool yuiJsPreserveAllSemiColons = false;
        //private bool yuiJsDisableOptimizations = false;
        //private int yuiJsLineBreak = -1;
        //private int yuiCssLineBreak = -1;

        ////error reporter implementation for YUI compressor
        //private ErrorReporter yuiErrorReporter = null;

        //compiled regex patterns
        private static readonly Regex EmptyPattern = new Regex("\\s");

        private static readonly Regex CommentPattern = new Regex("<!---->|<!--[^\\[].*?-->",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex IntertagPatternTagTag = new Regex(">\\s+<",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex IntertagPatternTagCustom = new Regex(">\\s+%%%~",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex IntertagPatternCustomTag = new Regex("~%%%\\s+<",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex IntertagPatternCustomCustom = new Regex("~%%%\\s+%%%~",
          RegexOptions.Singleline |
          RegexOptions.IgnoreCase);

        private static readonly Regex MultispacePattern = new Regex("\\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex TagEndSpacePattern = new Regex("(<(?:[^>]+?))(?:\\s+?)(/?>)",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex TagLastUnquotedValuePattern = new Regex("=\\s*[a-z0-9-_]+$", RegexOptions.IgnoreCase);

        private static readonly Regex TagQuotePattern = new Regex("\\s*=\\s*([\"'])([a-z0-9-_]+?)\\1(/?)(?=[^<]*?>)",
          RegexOptions.IgnoreCase);

        private static readonly Regex TagPropertyPattern = new Regex("(\\s\\w+)\\s*=\\s*(?=[^<]*?>)", RegexOptions.IgnoreCase);

        private static readonly Regex DoctypePattern = new Regex("<!DOCTYPE[^>]*>",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex JsTypeAttrPattern =
          new Regex("(<script[^>]*)type\\s*=\\s*([\"']*)(?:text|application)/javascript\\2([^>]*>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex JsLangAttrPattern =
          new Regex("(<script[^>]*)language\\s*=\\s*([\"']*)javascript\\2([^>]*>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex StyleTypeAttrPattern =
          new Regex("(<style[^>]*)type\\s*=\\s*([\"']*)text/style\\2([^>]*>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex LinkTypeAttrPattern =
          new Regex("(<link[^>]*)type\\s*=\\s*([\"']*)text/(?:css|plain)\\2([^>]*>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex LinkRelAttrPattern =
          new Regex("<link(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?stylesheet\\1(?:[^>]*)>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex FormMethodAttrPattern = new Regex("(<form[^>]*)method\\s*=\\s*([\"']*)get\\2([^>]*>)",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex InputTypeAttrPattern = new Regex("(<input[^>]*)type\\s*=\\s*([\"']*)text\\2([^>]*>)",
          RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex BooleanAttrPattern =
          new Regex("(<\\w+[^>]*)(checked|selected|disabled|readonly)\\s*=\\s*([\"']*)\\w*\\3([^>]*>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex HttpProtocolPattern =
          new Regex("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])http:(//[^>]+?>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex HttpsProtocolPattern =
          new Regex("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])https:(//[^>]+?>)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex RelExternalPattern =
          new Regex("<(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?external\\1(?:[^>]*)>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex EventPattern1 =
          new Regex("(\\son[a-z]+\\s*=\\s*\")([^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*)(\")", RegexOptions.IgnoreCase);

        //unmasked: \son[a-z]+\s*=\s*"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*"

        private static readonly Preserver EventPattern2Preserver = new EventPattern2Preserver();

        //private static readonly Regex SurroundingSpacesMinPattern =
        //  new Regex("\\s*(</?(?:" + BlockTagsMin.Replace(",", "|") + ")(?:>|[\\s/][^>]*>))\\s*",
        //    RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //private static readonly Regex SurroundingSpacesMaxPattern =
        //  new Regex("\\s*(</?(?:" + BlockTagsMax.Replace(",", "|") + ")(?:>|[\\s/][^>]*>))\\s*",
        //    RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex SurroundingSpacesAllPattern = new Regex("\\s*(<[^>]+>)\\s*",
          RegexOptions.Singleline |
          RegexOptions.IgnoreCase);

        //statistics

        //javascript and css compressor implementations
        public HtmlCompressorStatistics Statistics { get; private set; }

        /**
		 * The main method that compresses given HTML source and returns compressed
		 * result.
		 * 
		 * @param html HTML content to compress
		 * @return compressed content.
		 */

        public string Compress(string html)
        {
            if (!Settings.Enabled || string.IsNullOrEmpty(html))
            {
                return html;
            }

            //calculate uncompressed statistics
            InitStatistics(html);

            html = PreserveBlocks(html);

            html = ProcessHtml(html);

            ProcessPreservedBlocks();

            html = ReturnBlocks(html);

            //calculate compressed statistics
            EndStatistics(html);

            return html;
        }

        private void ProcessPreservedBlocks()
        {
            foreach (var preserver in Preservers)
            {
                preserver.Process(this);
            }
        }

        private void InitStatistics(string html)
        {
            //create stats
            if (Settings.GenerateStatistics)
            {
                Statistics = new HtmlCompressorStatistics();
                Statistics.SetTime(DateTime.Now.Ticks);
                Statistics.GetOriginalMetrics().SetFilesize(html.Length);

                //calculate number of empty chars
                var matcher = EmptyPattern.Matches(html);
                Statistics.GetOriginalMetrics().SetEmptyChars(Statistics.GetOriginalMetrics().GetEmptyChars() + matcher.Count);
            }
            else
            {
                Statistics = null;
            }
        }

        private void EndStatistics(string html)
        {
            //calculate compression time
            if (Settings.GenerateStatistics)
            {
                Statistics.SetTime(DateTime.Now.Ticks - Statistics.GetTime());
                Statistics.GetCompressedMetrics().SetFilesize(html.Length);

                //calculate number of empty chars
                var matcher = EmptyPattern.Matches(html);
                Statistics.GetCompressedMetrics().SetEmptyChars(Statistics.GetCompressedMetrics().GetEmptyChars() + matcher.Count);
            }
        }

        private string PreserveBlocks(string html)
        {
            foreach (var preserver in Preservers)
            {
                html = preserver.Preserve(html);
            }
            return html;
        }

        private string ReturnBlocks(string html)
        {
            var arr = Preservers.ToArray().Reverse();
            foreach (var preserver in arr)
            {
                html = preserver.Restore(html);
            }
            return html;
        }

        private string ProcessHtml(string html)
        {
            //remove comments
            html = RemoveComments(html);

            //simplify doctype
            html = SimpleDoctype(html);

            //remove script attributes
            html = RemoveScriptAttributes(html);

            //remove style attributes
            html = RemoveStyleAttributes(html);

            //remove link attributes
            html = RemoveLinkAttributes(html);

            //remove form attributes
            html = RemoveFormAttributes(html);

            //remove input attributes
            html = RemoveInputAttributes(html);

            //simplify bool attributes
            html = SimpleBooleanAttributes(html);

            //remove http from attributes
            html = RemoveHttpProtocol(html);

            //remove https from attributes
            html = RemoveHttpsProtocol(html);

            //remove inter-tag spaces
            html = RemoveIntertagSpaces(html);

            //remove multi whitespace characters
            html = RemoveMultiSpaces(html);

            //remove spaces around equals sign and ending spaces
            html = RemoveSpacesInsideTags(html);

            //remove quotes from tag attributes
            html = RemoveQuotesInsideTags(html);

            //remove surrounding spaces
            html = RemoveSurroundingSpaces(html);

            return html.Trim();
        }

        private string RemoveSurroundingSpaces(string html)
        {
            //remove spaces around provided tags
            if (Settings.SurroundingSpaces != SurroundingSpaces.Keep)
            {
                Regex pattern;
                if (Settings.SurroundingSpaces == SurroundingSpaces.RemoveForAllTags)
                {
                    pattern = SurroundingSpacesAllPattern;
                }
                else if (Settings.SurroundingSpaces == SurroundingSpaces.UseRemoveSurroundingSpacesForTags)
                {
                    if (Settings.RemoveSurroundingSpacesForTags != null && Settings.RemoveSurroundingSpacesForTags.Any())
                    {
                        pattern =
                            new Regex($"\\s*(</?(?:{string.Join("|", Settings.RemoveSurroundingSpacesForTags)})(?:>|[\\s/][^>]*>))\\s*",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        return html;
                    }
                }
                else
                {
                    throw new NotImplementedException($"Unknown surrounding spaces setting: {Settings.SurroundingSpaces}");
                }

                var matcher = pattern.Matches(html);
                var sb = new StringBuilder();
                var lastValue = 0;

                foreach (Match match in matcher)
                {
                    sb.Append(html.Substring(lastValue, match.Index - lastValue));
                    //matcher.appendReplacement(sb, "$1");
                    sb.Append(match.Result("$1"));

                    lastValue = match.Index + match.Length;
                }

                //matcher.appendTail(sb);
                sb.Append(html.Substring(lastValue));

                html = sb.ToString();
            }
            return html;
        }

        private string RemoveQuotesInsideTags(string html)
        {
            //remove quotes from tag attributes
            if (Settings.RemoveQuotes)
            {
                var matcher = TagQuotePattern.Matches(html);
                var sb = new StringBuilder();
                var lastValue = 0;

                foreach (Match match in matcher)
                {
                    //if quoted attribute is followed by "/" add extra space
                    if (match.Groups[3].Value.Trim().Length == 0)
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "=$2");
                        sb.Append(match.Result("=$2"));

                        lastValue = match.Index + match.Length;
                    }
                    else
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "=$2 $3");
                        sb.Append(match.Result("=$2 $3"));

                        lastValue = match.Index + match.Length;
                    }
                }

                //matcher.appendTail(sb);
                sb.Append(html.Substring(lastValue));

                html = sb.ToString();
            }
            return html;
        }

        private string RemoveSpacesInsideTags(string html)
        {
            //remove spaces around equals sign inside tags
            html = TagPropertyPattern.Replace(html, "$1=");

            //remove ending spaces inside tags
            //html = tagEndSpacePattern.Matches(html).Replace("$1$2");
            var matcher = TagEndSpacePattern.Matches(html);
            var sb = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matcher)
            {
                //keep space if attribute value is unquoted before trailing slash
                if (match.Groups[2].Value.StartsWith("/") && TagLastUnquotedValuePattern.IsMatch(match.Groups[1].Value))
                {
                    sb.Append(html.Substring(lastValue, match.Index - lastValue));
                    //matcher.appendReplacement(sb, "$1 $2");
                    sb.Append(match.Result("$1 $2"));

                    lastValue = match.Index + match.Length;
                }
                else
                {
                    sb.Append(html.Substring(lastValue, match.Index - lastValue));
                    //matcher.appendReplacement(sb, "$1$2");
                    sb.Append(match.Result("$1$2"));

                    lastValue = match.Index + match.Length;
                }
            }

            //matcher.appendTail(sb);
            sb.Append(html.Substring(lastValue));

            html = sb.ToString();

            return html;
        }

        private string RemoveMultiSpaces(string html)
        {
            //collapse multiple spaces
            if (Settings.RemoveMultiSpaces)
            {
                html = MultispacePattern.Replace(html, " ");
            }
            return html;
        }

        private string RemoveIntertagSpaces(string html)
        {
            //remove inter-tag spaces
            if (Settings.RemoveIntertagSpaces)
            {
                html = IntertagPatternTagTag.Replace(html, "><");
                html = IntertagPatternTagCustom.Replace(html, ">%%%~");
                html = IntertagPatternCustomTag.Replace(html, "~%%%<");
                html = IntertagPatternCustomCustom.Replace(html, "~%%%%%%~");
            }
            return html;
        }

        private string RemoveComments(string html)
        {
            //remove comments
            if (Settings.RemoveComments)
            {
                html = CommentPattern.Replace(html, "");
            }
            return html;
        }

        private string SimpleDoctype(string html)
        {
            //simplify doctype
            if (Settings.SimpleDoctype)
            {
                html = DoctypePattern.Replace(html, "<!DOCTYPE html>");
            }
            return html;
        }

        private string RemoveScriptAttributes(string html)
        {
            if (Settings.RemoveScriptAttributes)
            {
                //remove type from script tags
                html = JsTypeAttrPattern.Replace(html, "$1$3");

                //remove language from script tags
                html = JsLangAttrPattern.Replace(html, "$1$3");
            }
            return html;
        }

        private string RemoveStyleAttributes(string html)
        {
            //remove type from style tags
            if (Settings.RemoveStyleAttributes)
            {
                html = StyleTypeAttrPattern.Replace(html, "$1$3");
            }
            return html;
        }

        private string RemoveLinkAttributes(string html)
        {
            //remove type from link tags with rel=stylesheet
            if (Settings.RemoveLinkAttributes)
            {
                var matcher = LinkTypeAttrPattern.Matches(html);
                var sb = new StringBuilder();
                var lastValue = 0;

                foreach (Match match in matcher)
                {
                    //if rel=stylesheet
                    if (Matches(LinkRelAttrPattern, match.Groups[0].Value))
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$1$3");
                        sb.Append(match.Result("$1$3"));

                        lastValue = match.Index + match.Length;
                    }
                    else
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$0");
                        sb.Append(match.Result("$0"));

                        lastValue = match.Index + match.Length;
                    }
                }

                //matcher.appendTail(sb);
                sb.Append(html.Substring(lastValue));

                html = sb.ToString();
            }
            return html;
        }

        private string RemoveFormAttributes(string html)
        {
            //remove method from form tags
            if (Settings.RemoveFormAttributes)
            {
                html = FormMethodAttrPattern.Replace(html, "$1$3");
            }
            return html;
        }

        private string RemoveInputAttributes(string html)
        {
            //remove type from input tags
            if (Settings.RemoveInputAttributes)
            {
                html = InputTypeAttrPattern.Replace(html, "$1$3");
            }
            return html;
        }

        private string SimpleBooleanAttributes(string html)
        {
            //simplify bool attributes
            if (Settings.SimpleBooleanAttributes)
            {
                html = BooleanAttrPattern.Replace(html, "$1$2$4");
            }
            return html;
        }

        private string RemoveHttpProtocol(string html)
        {
            //remove http protocol from tag attributes
            if (Settings.RemoveHttpProtocol)
            {
                var matcher = HttpProtocolPattern.Matches(html);
                var sb = new StringBuilder();
                var lastValue = 0;

                foreach (Match match in matcher)
                {
                    //if rel!=external
                    if (!Matches(RelExternalPattern, match.Groups[0].Value))
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$1$2");
                        sb.Append(match.Result("$1$2"));

                        lastValue = match.Index + match.Length;
                    }
                    else
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$0");
                        sb.Append(match.Result("$0"));

                        lastValue = match.Index + match.Length;
                    }
                }

                //matcher.appendTail(sb);
                sb.Append(html.Substring(lastValue));

                html = sb.ToString();
            }
            return html;
        }

        private string RemoveHttpsProtocol(string html)
        {
            //remove https protocol from tag attributes
            if (Settings.RemoveHttpsProtocol)
            {
                var matcher = HttpsProtocolPattern.Matches(html);
                var sb = new StringBuilder();
                var lastValue = 0;

                foreach (Match match in matcher)
                {
                    //if rel!=external
                    if (!Matches(RelExternalPattern, match.Groups[0].Value))
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$1$2");
                        sb.Append(match.Result("$1$2"));

                        lastValue = match.Index + match.Length;
                    }
                    else
                    {
                        sb.Append(html.Substring(lastValue, match.Index - lastValue));
                        //matcher.appendReplacement(sb, "$0");
                        sb.Append(match.Result("$0"));

                        lastValue = match.Index + match.Length;
                    }
                }

                //matcher.appendTail(sb);
                sb.Append(html.Substring(lastValue));

                html = sb.ToString();
            }
            return html;
        }

        private static bool Matches(Regex regex, string value)
        {
            // http://stackoverflow.com/questions/4450045/difference-between-matches-and-find-in-java-regex

            var cloneRegex = new Regex(@"^" + regex + @"$", regex.Options);
            return cloneRegex.IsMatch(value);
        }

        public HtmlCompressor CreateClone()
        {
            var clone = new HtmlCompressor(Settings);
            return clone;
        }
    }
}