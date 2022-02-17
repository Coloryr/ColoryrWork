using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation;

public class Match2Preserver : Preserver
{
    public Match2Preserver(Regex pattern) : base(pattern)
    {
    }
    protected override bool AssertMatch(Match match)
    {
        return match.Groups[2].Value.Trim().Length > 0;
    }

    protected override string GenerateBlock(Match match)
    {
        return match.Groups[2].Value;
    }
}
