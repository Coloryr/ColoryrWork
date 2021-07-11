namespace HtmlCompression.Core
{
	public sealed class HtmlCompressorStatistics
	{
		private HtmlMetrics _originalMetrics = new HtmlMetrics();
		private HtmlMetrics _compressedMetrics = new HtmlMetrics();
		private long _time;
		private int _preservedSize;

		/**
		 * Returns metrics of an uncompressed document
		 * 
		 * @return metrics of an uncompressed document
		 * @see HtmlMetrics
		 */
		public HtmlMetrics GetOriginalMetrics()
		{
			return _originalMetrics;
		}

		/**
		 * @param originalMetrics the originalMetrics to set
		 */
		public void SetOriginalMetrics(HtmlMetrics originalMetrics)
		{
			_originalMetrics = originalMetrics;
		}

		/// <summary>
		/// <see cref="HtmlMetrics"/>
		/// </summary>
		/// <value>Metrics of a compressed document</value>
		public HtmlMetrics CompressedMetrics { get; set; }

		/**
		 * Returns 
		 * 
		 * @return metrics of a compressed document
		 * @see HtmlMetrics
		 */
		public HtmlMetrics GetCompressedMetrics()
		{
			return _compressedMetrics;
		}

		/**
		 * @param compressedMetrics the compressedMetrics to set
		 */
		public void SetCompressedMetrics(HtmlMetrics compressedMetrics)
		{
			_compressedMetrics = compressedMetrics;
		}

		/**
		 * Returns total compression time. 
		 * 
		 * <p>Please note that compression performance varies very significantly depending on whether it was 
		 * a cold run or not (specifics of Java VM), so for accurate real world results it is recommended 
		 * to take measurements accordingly.   
		 * 
		 * @return the compression time, in milliseconds 
		 *      
		 */
		public long GetTime()
		{
			return _time;
		}

		/**
		 * @param time the time to set
		 */
		public void SetTime(long time)
		{
			_time = time;
		}

		/**
		 * Returns total size of blocks that were skipped by the compressor 
		 * (for example content inside <code>&lt;pre></code> tags or inside   
		 * <code>&lt;script></code> tags with disabled javascript compression)
		 * 
		 * @return the total size of blocks that were skipped by the compressor, in bytes
		 */
		public int GetPreservedSize()
		{
			return _preservedSize;
		}

		/**
		 * @param preservedSize the preservedSize to set
		 */
		public void SetPreservedSize(int preservedSize)
		{
			_preservedSize = preservedSize;
		}

		public override string ToString()
		{
			return $"Time={_time}, Preserved={_preservedSize}, Original={_originalMetrics}, Compressed={_compressedMetrics}";
		}
	}
}