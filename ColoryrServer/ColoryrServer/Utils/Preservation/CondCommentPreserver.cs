using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class CondCommentPreserver : Preserver
	{
		private readonly HtmlCompressor _compressor;
		private HtmlCompressor _clone;

		public CondCommentPreserver(HtmlCompressor compressor) : base(new Regex("(<!(?:--)?\\[[^\\]]+?]>)(.*?)(<!\\[[^\\]]+]-->)",
			RegexOptions.Singleline | RegexOptions.IgnoreCase))
		{
			_compressor = compressor;
			ExpandReplacement = false;
			BlockIndex = 1;
		}

		public override string Preserve(string html)
		{
			_clone = _compressor.CreateClone();
			return base.Preserve(html);
		}

		protected override bool AssertMatch(Match match)
		{
			return match.Groups[2].Value.Trim().Length > 0;
		}

		protected override string GenerateBlock(Match match)
		{
			return match.Groups[1].Value + _clone.Compress(match.Groups[2].Value) + match.Groups[3].Value;
		}
	}
}