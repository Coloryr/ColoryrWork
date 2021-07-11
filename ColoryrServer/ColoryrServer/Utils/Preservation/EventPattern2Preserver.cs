using System.Text.RegularExpressions;

namespace HtmlCompression.Core.Preservation
{
	public class EventPattern2Preserver : Match2Preserver
	{
		public EventPattern2Preserver() : base(new Regex("(\\son[a-z]+\\s*=\\s*')([^'\\\\\\r\\n]*(?:\\\\.[^'\\\\\\r\\n]*)*)(')", RegexOptions.IgnoreCase))
		{
			ExpandReplacement = true;
		}
	}
}