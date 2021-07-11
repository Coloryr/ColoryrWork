using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class StylePreserver : Match2Preserver
	{
		public StylePreserver() : base(new Regex("(<style[^>]*?>)(.*?)(</style>)",
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
					  .SetInlineStyleSize(compressor.Statistics.GetOriginalMetrics().GetInlineStyleSize() + block.Length);
				}
			}

			if (compressor.Settings.CompressCss)
			{
				for (var i = 0; i < _blocks.Count; i++)
				{
					_blocks[i] = CompressCssStyles(compressor, _blocks[i]);
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
					  .SetInlineStyleSize(compressor.Statistics.GetCompressedMetrics().GetInlineStyleSize() + block.Length);
				}
			}
		}
		private string CompressCssStyles(HtmlCompressor compressor, string source)
		{
			//set default css compressor
			if (compressor.Settings.CssCompressor == null)
			{
				return source;
				//YuiCssCompressor yuiCssCompressor = new YuiCssCompressor();
				//yuiCssCompressor.setLineBreak(yuiCssLineBreak);

				//cssCompressor = yuiCssCompressor;
			}

			//detect CDATA wrapper
			var cdataWrapper = false;
			var matcher = ScriptPreserver.CdataPattern.Match(source);
			if (matcher.Success)
			{
				cdataWrapper = true;
				source = matcher.Groups[1].Value;
			}

			var result = compressor.Settings.CssCompressor.Compress(source);

			if (cdataWrapper)
			{
				result = $"<![CDATA[{result}]]>";
			}

			return result;
		}
	}
}