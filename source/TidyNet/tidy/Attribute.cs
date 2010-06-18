using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// HTML attribute
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
	internal class Attribute
	{
		public Attribute(string name, bool nowrap, HtmlVersion versions, IAttrCheck attrCheck)
		{
			_name = name;
			_nowrap = nowrap;
			_literal = false;
			_versions = versions;
			_attrCheck = attrCheck;
		}
		
		public Attribute(string name, HtmlVersion versions, IAttrCheck attrCheck)
		{
			_name = name;
			_nowrap = false;
			_literal = false;
			_versions = versions;
			_attrCheck = attrCheck;
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public bool Nowrap
		{
			get
			{
				return _nowrap;
			}
			set
			{
				_nowrap = value;
			}
		}

		public bool Literal
		{
			get
			{
				return _literal;
			}
			set
			{
				_literal = value;
			}
		}

		public HtmlVersion Versions
		{
			get
			{
				return _versions;
			}
			set
			{
				_versions = value;
			}
		}

		public IAttrCheck AttrCheck
		{
			get
			{
				return _attrCheck;
			}
			set
			{
				_attrCheck = value;
			}
		}

		private string _name;
		private bool _nowrap;
		private bool _literal;
		private HtmlVersion _versions;
		private IAttrCheck _attrCheck;
	}
}