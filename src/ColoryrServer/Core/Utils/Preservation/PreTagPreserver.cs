using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation;

public class PreTagPreserver : Match2Preserver
{
    public PreTagPreserver() : base(new Regex("(<pre[^>]*?>)(.*?)(</pre>)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase))
    {
        ExpandReplacement = true;
    }
}
