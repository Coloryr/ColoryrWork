namespace HtmlCompression.Core
{
	public sealed class HtmlMetrics
	{
		private int _emptyChars;
		private int _filesize;
		private int _inlineEventSize;
		private int _inlineScriptSize;
		private int _inlineStyleSize;

		/**
		 * Returns total filesize of a document
		 * 
		 * @return total filesize of a document, in bytes
		 */

		public int GetFilesize()
		{
			return _filesize;
		}

		/**
		 * @param filesize the filesize to set
		 */

		public void SetFilesize(int filesize)
		{
			_filesize = filesize;
		}

		/**
		 * Returns number of empty characters (spaces, tabs, end of lines) in a document
		 * 
		 * @return number of empty characters in a document
		 */

		public int GetEmptyChars()
		{
			return _emptyChars;
		}

		/**
		 * @param emptyChars the emptyChars to set
		 */

		public void SetEmptyChars(int emptyChars)
		{
			_emptyChars = emptyChars;
		}

		/**
		 * Returns total size of inline <code>&lt;script></code> tags
		 * 
		 * @return total size of inline <code>&lt;script></code> tags, in bytes
		 */

		public int GetInlineScriptSize()
		{
			return _inlineScriptSize;
		}

		/**
		 * @param inlineScriptSize the inlineScriptSize to set
		 */

		public void SetInlineScriptSize(int inlineScriptSize)
		{
			_inlineScriptSize = inlineScriptSize;
		}

		/**
		 * Returns total size of inline <code>&lt;style></code> tags
		 * 
		 * @return total size of inline <code>&lt;style></code> tags, in bytes
		 */

		public int GetInlineStyleSize()
		{
			return _inlineStyleSize;
		}

		/**
		 * @param inlineStyleSize the inlineStyleSize to set
		 */

		public void SetInlineStyleSize(int inlineStyleSize)
		{
			_inlineStyleSize = inlineStyleSize;
		}

		/**
		 * Returns total size of inline event handlers (<code>onclick</code>, etc)
		 * 
		 * @return total size of inline event handlers, in bytes
		 */

		public int GetInlineEventSize()
		{
			return _inlineEventSize;
		}

		/**
		 * @param inlineEventSize the inlineEventSize to set
		 */

		public void SetInlineEventSize(int inlineEventSize)
		{
			_inlineEventSize = inlineEventSize;
		}

		public override string ToString()
		{
			return string.Format(
				"Filesize={0}, Empty Chars={1}, Script Size={2}, Style Size={3}, Event Handler Size={4}",
				_filesize, _emptyChars, _inlineScriptSize, _inlineStyleSize, _inlineEventSize);
		}
	}
}