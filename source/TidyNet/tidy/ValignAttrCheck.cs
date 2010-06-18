using System;

namespace TidyNet
{
	/// <summary>
	/// Check attribute values
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
	/// See Tidy.cs for the copyright notice.
	/// Derived from <a href="http://www.w3.org/People/Raggett/tidy">
	/// HTML Tidy Release 4 Aug 2000</a>
	/// 
	/// </summary>
	/// <author>Dave Raggett &lt;dsr@w3.org&gt;</author>
	/// <author>Andy Quick &lt;ac.quick@sympatico.ca&gt; (translation to Java)</author>
	/// <author>Seth Yates &lt;seth_yates@hotmail.com&gt; (translation to C#)</author>
	/// <version>1.0, 1999/05/22</version>
	/// <version>1.0.1, 1999/05/29</version>
	/// <version>1.1, 1999/06/18 Java Bean</version>
	/// <version>1.2, 1999/07/10 Tidy Release 7 Jul 1999</version>
	/// <version>1.3, 1999/07/30 Tidy Release 26 Jul 1999</version>
	/// <version>1.4, 1999/09/04 DOM support</version>
	/// <version>1.5, 1999/10/23 Tidy Release 27 Sep 1999</version>
	/// <version>1.6, 1999/11/01 Tidy Release 22 Oct 1999</version>
	/// <version>1.7, 1999/12/06 Tidy Release 30 Nov 1999</version>
	/// <version>1.8, 2000/01/22 Tidy Release 13 Jan 2000</version>
	/// <version>1.9, 2000/06/03 Tidy Release 30 Apr 2000</version>
	/// <version>1.10, 2000/07/22 Tidy Release 8 Jul 2000</version>
	/// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
	internal class ValignAttrCheck : IAttrCheck
	{
		public virtual void Check(Lexer lexer, Node node, AttVal attval)
		{
			string val = attval.Val;

			if (val == null)
			{
				Report.AttrError(lexer, node, attval.Attribute, Report.MISSING_ATTR_VALUE);
			}
			else if (String.Compare(val, "top") == 0 || String.Compare(val, "middle") == 0 || String.Compare(val, "bottom") == 0 || String.Compare(val, "baseline") == 0)
			{
				/* all is fine */
			}
			else if (String.Compare(val, "left") == 0 || String.Compare(val, "right") == 0)
			{
				if (!(node.Tag != null && ((node.Tag.Model & ContentModel.Img) != 0)))
				{
					Report.AttrError(lexer, node, val, Report.BAD_ATTRIBUTE_VALUE);
				}
			}
			else if (String.Compare(val, "texttop") == 0 || String.Compare(val, "absmiddle") == 0 || String.Compare(val, "absbottom") == 0 || String.Compare(val, "textbottom") == 0)
			{
				lexer.versions &= HtmlVersion.Proprietary;
				Report.AttrError(lexer, node, val, Report.PROPRIETARY_ATTR_VALUE);
			}
			else
			{
				Report.AttrError(lexer, node, val, Report.BAD_ATTRIBUTE_VALUE);
			}
		}
	}
}
