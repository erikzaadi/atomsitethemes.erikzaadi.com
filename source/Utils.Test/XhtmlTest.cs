using MarkupSanitizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaceTests
{
	[TestClass]
	public class XhtmlTest
	{
		[TestMethod]
		public void Special_Characters_Will_Not_Save_In_Editor()
		{
			string html = "<p>Test&uuml;&nbsp;&rsquo;&lsquo;&rdquo;&ldquo;</p>";
			SanitizedMarkup sanitized=  MarkupSanitizer.Sanitizer.SanitizeMarkup(html);
			string result = sanitized.MarkupText;
			Assert.AreEqual("<p>Test&#252;&#160;&#8217;&#8216;&#8221;&#8220;</p>", result);
		}
	}
}
