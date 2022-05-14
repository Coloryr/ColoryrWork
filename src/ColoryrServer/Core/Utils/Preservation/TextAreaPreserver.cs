using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation;

public class TextAreaPreserver : Match2Preserver
{
    public TextAreaPreserver() : base(new Regex("(<textarea[^>]*?>)(.*?)(</textarea>)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase))
    {
        ExpandReplacement = true;
    }
}
