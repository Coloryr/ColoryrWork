using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class InlineEventsPreserver : Match2Preserver
	{
		public InlineEventsPreserver() : base(new Regex("(<!(?:--)?\\[[^\\]]+?]>)(.*?)(<!\\[[^\\]]+]-->)",
			RegexOptions.Singleline | RegexOptions.IgnoreCase))
		{
			ExpandReplacement = true;
		}

		public override void Process(HtmlCompressor compressor)
		{
			if (compressor.Settings.GenerateStatistics)
			{
				foreach (var block in _blocks)
				{
					compressor.Statistics.GetOriginalMetrics()
					  .SetInlineEventSize(compressor.Statistics.GetOriginalMetrics().GetInlineEventSize() + block.Length);
				}
			}

			if (compressor.Settings.RemoveJavaScriptProtocol)
			{
				for (var i = 0; i < _blocks.Count; i++)
				{
					_blocks[i] = RemoveJavaScriptProtocol(compressor, _blocks[i]);
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
					  .SetInlineEventSize(compressor.Statistics.GetCompressedMetrics().GetInlineEventSize() + block.Length);
				}
			}
		}

		private static readonly Regex EventJsProtocolPattern = new Regex("^javascript:\\s*(.+)",
		  RegexOptions.Singleline | RegexOptions.IgnoreCase);

		private string RemoveJavaScriptProtocol(HtmlCompressor compressor, string source)
		{
			//remove javascript: from inline events
			var result = EventJsProtocolPattern.Replace(source, @"$1", 1);
			//var matcher = eventJsProtocolPattern.Match(source);
			//if (matcher.Success)
			//{
			//    result = matcher.replaceFirst("$1");
			//}

			if (compressor.Settings.GenerateStatistics)
			{
				compressor.Statistics.SetPreservedSize(
					compressor.Statistics.GetPreservedSize() + result.Length);
			}

			return result;
		}
	}
}