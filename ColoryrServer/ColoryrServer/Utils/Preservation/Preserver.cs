using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class Preserver
	{
		private readonly string _key;
		protected readonly List<string> _blocks;
		public virtual bool Enabled { get; set; } = true;
		public Regex Pattern { get; set; }

		public Preserver(Regex pattern)
		{
			Pattern = pattern;
			_blocks = new List<string>();
			_key = GetType().Name + Guid.NewGuid().ToString().Replace("-", "");
		}

		public IReadOnlyList<string> Blocks => _blocks.AsReadOnly();

		public virtual void Process(HtmlCompressor compressor)
		{
			if (compressor.Settings.GenerateStatistics)
			{
				foreach (var block in _blocks)
				{
					compressor.Statistics.SetPreservedSize(compressor.Statistics.GetPreservedSize() + block.Length);
				}
			}
		}

		public virtual string Preserve(string html)
		{
			if (!Enabled)
			{
				return html;
			}
			var matcher = Pattern.Matches(html);
			var index = 0;
			var sb = new StringBuilder();
			var lastValue = 0;

			foreach (Match match in matcher)
			{
				if (!AssertMatch(match))
				{
					continue;
				}
				lastValue = PreserveMatch(html, match, sb, lastValue, ref index);
			}

			//matcher.appendTail(sb);
			sb.Append(html.Substring(lastValue));

			html = sb.ToString();
			return html;
		}

		public virtual int PreserveMatch(string html, Match match, StringBuilder sb, int lastValue, ref int index)
		{
			var block = GenerateBlock(match);
			return PreserveMatch(html, match, sb, lastValue, ref index, block);
		}

		public virtual int PreserveMatch(string html, Match match, StringBuilder sb, int lastValue, ref int index, string block)
		{
			_blocks.Add(block);

			sb.Append(html.Substring(lastValue, match.Index - lastValue));
			//matcher.appendReplacement(sb, string.Format(tempLineBreakBlock, index++));
			sb.Append(GenerateReplacecment(match, ref index));

			lastValue = match.Index + match.Length;
			return lastValue;
		}

		private string GenerateReplacecment(Match match, ref int index)
		{
			var tempBlock = GetTempBlock(index++);
			if (ExpandReplacement)
			{
				tempBlock = "$1" + tempBlock + "$3";
			}
			return match.Result(tempBlock);
		}

		public bool ExpandReplacement { get; set; } = false;

		protected virtual string GetTempBlock(int i)
		{
			return $"%%%~COMPRESS~{_key}~{i}~%%%";
		}

		protected virtual string GenerateBlock(Match match)
		{
			return match.Groups[BlockIndex].Value;
		}

		public int BlockIndex { get; set; } = 0;

		protected virtual bool AssertMatch(Match match)
		{
			return true;
		}

		public string Restore(string html)
		{
			//if (!Enabled)
			//{
			//	return html;
			//}
			var matcher = new Regex($@"%%%~COMPRESS~{_key}~(\d+?)~%%%").Matches(html);
			var sb = new StringBuilder();
			var lastValue = 0;
			foreach (Match match in matcher)
			{
				var i = int.Parse(match.Groups[1].Value);
				if (_blocks.Count > i)
				{
					sb.Append(html.Substring(lastValue, match.Index - lastValue));
					//matcher.appendReplacement(sb, lineBreakBlocks[i]);
					sb.Append(match.Result(_blocks[i]));

					lastValue = match.Index + match.Length;
				}
			}
			_blocks.Clear();

			//matcher.appendTail(sb);
			sb.Append(html.Substring(lastValue));

			html = sb.ToString();
			return html;
		}
	}
}