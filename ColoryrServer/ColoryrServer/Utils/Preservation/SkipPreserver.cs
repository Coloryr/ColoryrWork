using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
    /// <summary>
    /// Preserve <!-- {{{ ---><!-- }}} ---> skip blocks
    /// </summary>
    public class SkipPreserver : Preserver
    {
        public SkipPreserver() : base(new Regex("<!--\\s*\\{\\{\\{\\s*-->(.*?)<!--\\s*\\}\\}\\}\\s*-->",
          RegexOptions.Singleline | RegexOptions.IgnoreCase))
        {
            ExpandReplacement = false;
            BlockIndex = 1;
        }

        protected override bool AssertMatch(Match match)
        {
            return match.Groups[1].Value.Trim().Length > 0;
        }
    }
}