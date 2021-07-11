using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlCompression.Core
{
	public class HtmlCompressorSettings
	{
		/// <summary>
		/// Gets or sets whether JavaScript compression is enabled.
		/// </summary>
		/// <value>
		/// Enables JavaScript compression within &lt;script> tags using <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a> if set to <code>true</code>. Default is <code>false</code> for performance reasons. <p><b>Note:</b> Compressing JavaScript is not recommended if pages are compressed dynamically on-the-fly because of performance impact. You should consider putting JavaScript into a separate file and compressing it using standalone YUICompressor for example.</p>
		/// </value>
		/// <returns>
		/// Current state of JavaScript compression.@param compressJavaScript set <code>true</code> to enable JavaScript compression.Default is <code>false</code>
		/// </returns>
		public bool CompressJavaScript { get; set; }


		/// <summary>
		/// Gets or sets whether CSS compression is enabled.
		/// </summary>
		/// <value>
		/// Enables CSS compression within &lt;style> tags using <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a> if set to <code>true</code>. Default is <code>false</code> for performance reasons. <p><b>Note:</b> Compressing CSS is not recommended if pages are compressed dynamically on-the-fly because of performance impact. You should consider putting CSS into a separate file and compressing it using standalone YUICompressor for example.</p>
		/// </value>
		/// <returns>
		/// Current state of CSS compression.@param compressCss set <code>true</code> to enable CSS compression.Default is <code>false</code>
		/// </returns>
		public bool CompressCss { get; set; }


		// * Returns number of symbols per line Yahoo YUI ICompressor
		// * will use during CSS compression. 
		// * This corresponds to <code>--line-break</code> command line option.
		// *   
		// * @return <code>line-break</code> parameter value used for CSS compression.
		// * 
		// * @see <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a>
		// */
		//public int getYuiCssLineBreak()
		//{
		//    return yuiCssLineBreak;
		//}
		/// **
		/// **
		/// **
		/// **
		/// **
		/// **
		/// **
		/// **
		/// **
		/// **
		// * Tells Yahoo YUI ICompressor to break lines after the specified number of symbols 
		// * during CSS compression. This corresponds to 
		// * <code>--line-break</code> command line option. 
		// * This option has effect only if CSS compression is enabled.
		// * Default is <code>-1</code> to disable line breaks.
		// * 
		// * @param yuiCssLineBreak set number of symbols per line
		// * 
		// * @see <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a>
		// */
		//public void setYuiCssLineBreak(int yuiCssLineBreak)
		//{
		//    yuiCssLineBreak = yuiCssLineBreak;
		//}
		/// <summary>
		/// Gets or sets whether all unnecessary quotes will be removed from tag attributes. 
		/// </summary>
		/// <value>
		/// If set to <code>true</code> all unnecessary quotes will be removed from tag attributes. Default is <code>false</code>. <p><b>Note:</b> Even though quotes are removed only when it is safe to do so, it still might break strict HTML validation. Turn this option on only if a page validation is not very important or to squeeze the most out of the compression. This option has no performance impact.
		/// </value>
		/// <returns>
		/// @param removeQuotes set <code>true</code> to remove unnecessary quotes from tag attributes
		/// </returns>
		public bool RemoveQuotes { get; set; }


		/// <summary>
		/// Gets or sets whether compression is enabled.
		/// </summary>
		/// <value>
		/// If set to <code>false</code> all compression will be bypassed. Might be useful for testing purposes. Default is <code>true</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if compression is enabled.@param enabled set <code>false</code> to bypass all compression
		/// </returns>
		public bool Enabled { get; set; } = true;


		/// <summary>
		/// Gets or sets whether all HTML comments will be removed.
		/// </summary>
		/// <value>
		/// If set to <code>true</code> all HTML comments will be removed. Default is <code>true</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if all HTML comments will be removed@param removeComments set <code>true</code> to remove all HTML comments
		/// </returns>
		public bool RemoveComments { get; set; } = true;


		/// <summary>
		/// Gets or sets whether all multiple whitespace characters will be replaced with single spaces.
		/// </summary>
		/// <value>
		/// If set to <code>true</code> all multiple whitespace characters will be replaced with single spaces. Default is <code>true</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if all multiple whitespace characters will be replaced with single spaces.@param removeMultiSpaces set <code>true</code> to replace all multiple whitespace characterswill single spaces.
		/// </returns>
		public bool RemoveMultiSpaces { get; set; } = true;


		/// <summary>
		/// Gets or sets whether all inter-tag whitespace characters will be removed.
		/// </summary>
		/// <value>
		/// If set to <code>true</code> all inter-tag whitespace characters will be removed. Default is <code>false</code>. <p><b>Note:</b> It is fairly safe to turn this option on unless you rely on spaces for page formatting. Even if you do, you can always preserve required spaces with <code>&amp;nbsp;</code>. This option has no performance impact.
		/// </value>
		/// <returns>
		/// <code>true</code> if all inter-tag whitespace characters will be removed.@param removeIntertagSpaces set <code>true</code> to remove all inter-tag whitespace characters
		/// </returns>
		public bool RemoveIntertagSpaces { get; set; }


		/// <summary>
		/// Gets or sets the list of patterns defining custom preserving block rules
		/// </summary>
		/// <value>
		/// Returns a list of regular expression patterns defining custom preserving block rules
		/// </value>
		/// <remarks>
		/// Blocks that match provided patterns will be skipped during HTML compression. <p>Custom preservation rules have higher priority than default rules. Priority between custom rules are defined by their position in a list (beginning of a list has higher priority). <p>Besides custom patterns, you can use 3 predefined patterns: {@link #PHP_TAG_PATTERN PHP_TAG_PATTERN}, {@link #SERVER_SCRIPT_TAG_PATTERN SERVER_SCRIPT_TAG_PATTERN}, {@link #SERVER_SIDE_INCLUDE_PATTERN SERVER_SIDE_INCLUDE_PATTERN}.
		/// </remarks>
		/// <returns>
		/// List of <code>Regex</code> objects defining rules for preserving block rules@param preservePatterns List of <code>Regex</code> objects that will beused to skip matched blocks during compression
		/// </returns>
		public List<Regex> PreservePatterns { get; set; }


		// * Returns <code>ErrorReporter</code> used by YUI ICompressor to log error messages 
		// * during JavasSript compression 
		// * 
		// * @return <code>ErrorReporter</code> used by YUI ICompressor to log error messages 
		// * during JavasSript compression
		// * 
		// * @see <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a>
		// * @see <a href="http://www.mozilla.org/rhino/apidocs/org/mozilla/javascript/ErrorReporter.html">Error Reporter Interface</a>
		// */
		//public ErrorReporter getYuiErrorReporter()
		//{
		//    return yuiErrorReporter;
		//}
		/// **
		/// **
		// * Sets <code>ErrorReporter</code> that YUI ICompressor will use for reporting errors during 
		// * JavaScript compression. If no <code>ErrorReporter</code> was provided 
		// * {@link YuiJavaScriptCompressor.DefaultErrorReporter} will be used 
		// * which reports errors to <code>System.err</code> stream. 
		// * 
		// * @param yuiErrorReporter <code>ErrorReporter<code> that will be used by YUI ICompressor
		// * 
		// * @see YuiJavaScriptCompressor.DefaultErrorReporter
		// * @see <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a>
		// * @see <a href="http://www.mozilla.org/rhino/apidocs/org/mozilla/javascript/ErrorReporter.html">ErrorReporter Interface</a>
		// */
		//public void setYuiErrorReporter(ErrorReporter yuiErrorReporter)
		//{
		//    yuiErrorReporter = yuiErrorReporter;
		//}
		/// <summary>
		/// Gets or sets the JavaScript compressor implementation
		/// </summary>
		/// <value>
		/// Returns JavaScript compressor implementation that will be used to compress inline JavaScript in HTML. Sets JavaScript compressor implementation that will be used to compress inline JavaScript in HTML. <p>HtmlCompressor currently comes with basic implementations for <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a> (called {@link YuiJavaScriptCompressor}) and <a href="http://code.google.com/closure/compiler/">Google Closure Compiler</a> (called {@link ClosureJavaScriptCompressor}) that should be enough for most cases, but users can also create their own JavaScript compressors for custom needs. <p>If no compressor is set {@link YuiJavaScriptCompressor} will be used by default.
		/// </value>
		/// <returns>
		/// <code>ICompressor</code> implementation that will be usedto compress inline JavaScript in HTML.@param javaScriptCompressor {@link ICompressor} implementation that will be used for inline JavaScript compression
		/// </returns>
		public ICompressor JavaScriptCompressor { get; set; }


		/// <summary>
		/// Gets or sets the CSS compressor implementation
		/// </summary>
		/// <value>
		/// Returns CSS compressor implementation that will be used to compress inline CSS in HTML. Sets CSS compressor implementation that will be used to compress inline CSS in HTML. <p>HtmlCompressor currently comes with basic implementation for <a href="http://developer.yahoo.com/yui/compressor/">Yahoo YUI ICompressor</a> (called {@link YuiCssCompressor}), but users can also create their own CSS compressors for custom needs. <p>If no compressor is set {@link YuiCssCompressor} will be used by default.
		/// </value>
		/// <returns>
		/// <code>ICompressor</code> implementation that will be usedto compress inline CSS in HTML.@param cssCompressor {@link ICompressor} implementation that will be used for inline CSS compression
		/// </returns>
		public ICompressor CssCompressor { get; set; }


		/// <summary>
		/// Gets or sets whether existing DOCTYPE declaration will be replaced with simple <code><!DOCTYPE html></code> declaration.
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, existing DOCTYPE declaration will be replaced with simple <code>&lt;!DOCTYPE html></code> declaration. Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if existing DOCTYPE declaration will be replaced with simple <code><!DOCTYPE html></code> declaration.@param simpleDoctype set <code>true</code> to replace existing DOCTYPE declaration with <code>&lt;!DOCTYPE html></code>
		/// </returns>
		public bool SimpleDoctype { get; set; }


		/// <summary>
		/// Gets or sets whether unnecessary attributes wil be removed from <code>&lt;script></code> tags 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, following attributes will be removed from <code>&lt;script></code> tags: <ul> <li>type="text/javascript"</li> <li>type="application/javascript"</li> <li>language="javascript"</li> </ul> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if unnecessary attributes wil be removed from <code>&lt;script></code> tags@param removeScriptAttributes set <code>true</code> to remove unnecessary attributes from <code>&lt;script></code> tags
		/// </returns>
		public bool RemoveScriptAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether <code>type="text/style"</code> attributes will be removed from <code>&lt;style></code> tags 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>type="text/style"</code> attributes will be removed from <code>&lt;style></code> tags. Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>type="text/style"</code> attributes will be removed from <code>&lt;style></code> tags@param removeStyleAttributes set <code>true</code> to remove <code>type="text/style"</code> attributes from <code>&lt;style></code> tags
		/// </returns>
		public bool RemoveStyleAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether unnecessary attributes will be removed from <code>&lt;link></code> tags 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, following attributes will be removed from <code>&lt;link rel="stylesheet"></code> and <code>&lt;link rel="alternate stylesheet"></code> tags: <ul> <li>type="text/css"</li> <li>type="text/plain"</li> </ul> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if unnecessary attributes will be removed from <code>&lt;link></code> tags@param removeLinkAttributes set <code>true</code> to remove unnecessary attributes from <code>&lt;link></code> tags
		/// </returns>
		public bool RemoveLinkAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether <code>method="get"</code> attributes will be removed from <code>&lt;form></code> tags 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>method="get"</code> attributes will be removed from <code>&lt;form></code> tags. Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>method="get"</code> attributes will be removed from <code>&lt;form></code> tags@param removeFormAttributes set <code>true</code> to remove <code>method="get"</code> attributes from <code>&lt;form></code> tags
		/// </returns>
		public bool RemoveFormAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether <code>type="text"</code> attributes will be removed from <code>&lt;input></code> tags
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>type="text"</code> attributes will be removed from <code>&lt;input></code> tags. Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>type="text"</code> attributes will be removed from <code>&lt;input></code> tags@param removeInputAttributes set <code>true</code> to remove <code>type="text"</code> attributes from <code>&lt;input></code> tags
		/// </returns>
		public bool RemoveInputAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether bool attributes will be simplified 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, any values of following bool attributes will be removed: <ul> <li>checked</li> <li>selected</li> <li>disabled</li> <li>readonly</li> </ul> <p>For example, <code>&ltinput readonly="readonly"></code> would become <code>&ltinput readonly></code> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if bool attributes will be simplified@param simpleBooleanAttributes set <code>true</code> to simplify bool attributes
		/// </returns>
		public bool SimpleBooleanAttributes { get; set; }


		/// <summary>
		/// Gets or sets whether <code>javascript:</code> pseudo-protocol will be removed from inline event handlers.
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>javascript:</code> pseudo-protocol will be removed from inline event handlers. <p>For example, <code>&lta onclick="javascript:alert()"></code> would become <code>&lta onclick="alert()"></code> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>javascript:</code> pseudo-protocol will be removed from inline event handlers.@param removeJavaScriptProtocol set <code>true</code> to remove <code>javascript:</code> pseudo-protocol from inline event handlers.
		/// </returns>
		public bool RemoveJavaScriptProtocol { get; set; }


		/// <summary>
		/// Gets or sets whether <code>HTTP</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes.
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>HTTP</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes. URL without a protocol would make a browser use document's current protocol instead. <p>Tags marked with <code>rel="external"</code> will be skipped. <p>For example: <p><code>&lta href="http://example.com"> &ltscript src="http://google.com/js.js" rel="external"></code> <p>would become: <p><code>&lta href="//example.com"> &ltscript src="http://google.com/js.js" rel="external"></code> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>HTTP</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes.@param removeHttpProtocol set <code>true</code> to remove <code>HTTP</code> protocol from tag attributes
		/// </returns>
		public bool RemoveHttpProtocol { get; set; }


		/// <summary>
		/// Gets or sets whether <code>HTTPS</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes.
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, <code>HTTPS</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes. URL without a protocol would make a browser use document's current protocol instead. <p>Tags marked with <code>rel="external"</code> will be skipped. <p>For example: <p><code>&lta href="https://example.com"> &ltscript src="https://google.com/js.js" rel="external"></code> <p>would become: <p><code>&lta href="//example.com"> &ltscript src="https://google.com/js.js" rel="external"></code> <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if <code>HTTPS</code> protocol will be removed from <code>href</code>, <code>src</code>, <code>cite</code>, and <code>action</code> tag attributes.@param removeHttpsProtocol set <code>true</code> to remove <code>HTTP</code> protocol from tag attributes
		/// </returns>
		public bool RemoveHttpsProtocol { get; set; }


		/// <summary>
		/// Gets or sets whether HTML compression statistics is generated 
		/// </summary>
		/// <value>
		/// If set to <code>true</code>, HTML compression statistics will be generated. <p><strong>Important:</strong> Enabling statistics makes HTML compressor not thread safe. <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// <code>true</code> if HTML compression statistics is generated@param generateStatistics set <code>true</code> to generate HTML compression statistics
		/// </returns>
		public bool GenerateStatistics { get; set; }


		/// <summary>
		/// Gets or sets whether line breaks will be preserved.
		/// </summary>
		/// <value>
		/// Returns {@link HtmlCompressorStatistics} object containing statistics of the last HTML compression, if enabled. Should be called after {@link #compress(string)} If set to <code>true</code>, line breaks will be preserved. <p>Default is <code>false</code>.
		/// </value>
		/// <returns>
		/// {@link HtmlCompressorStatistics} object containing last HTML compression statistics<code>true</code> if line breaks will be preserved.@param preserveLineBreaks set <code>true</code> to preserve line breaks
		/// </returns>
		public bool PreserveLineBreaks { get; set; }

		/// <summary>
		/// Gets or sets whitespace removal setting
		/// </summary>
		/// <value>
		/// Enum specifying action to take when removing whitespace surrounding tags
		/// </value>
		/// <returns>
		/// Enum specifying action to take when removing whitespace surrounding tags
		/// </returns>
		public SurroundingSpaces SurroundingSpaces { get; set; } = SurroundingSpaces.RemoveForAllTags;

		/// <summary>
		/// Gets or sets tag list to allow reomval of whitespace around
		/// </summary>
		/// <value>
		/// A list of tags around which spaces will be removed. Enables surrounding spaces removal around provided comma separated list of tags. <p>Besides custom defined lists, you can pass one of 3 predefined lists of tags: {@link #BLOCK_TAGS_MIN BLOCK_TAGS_MIN}, {@link #BLOCK_TAGS_MAX BLOCK_TAGS_MAX}, {@link #ALL_TAGS ALL_TAGS}.
		/// </value>
		/// <returns>
		/// Returns a list of tags around which spaces will be removed.@param tagList a comma separated list of tags around which spaces will be removed
		/// </returns>
		public List<string> RemoveSurroundingSpacesForTags { get; set; } = new List<string>();
	}
}