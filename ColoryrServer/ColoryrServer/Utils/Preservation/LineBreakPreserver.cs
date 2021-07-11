using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class LineBreakPreserver : Preserver
	{
		private readonly HtmlCompressorSettings _settings;

		public override bool Enabled
		{
			get { return _settings.PreserveLineBreaks; }
			set { _settings.PreserveLineBreaks = value; }
		}

		public LineBreakPreserver(HtmlCompressorSettings settings) : base(new Regex("(?:[ \t]*(\\r?\\n)[ \t]*)+"))
		{
			_settings = settings;
			ExpandReplacement = false;
			BlockIndex = 1;
		}
	}
}