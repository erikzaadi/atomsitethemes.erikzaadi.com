using System;
using System.IO;

namespace TidyNet
{
	/// <summary>
	/// .NET native Input Stream Implementation
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
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
	internal class ClsStreamInImpl : StreamIn
	{
		public ClsStreamInImpl(Stream stream, CharEncoding encoding, int tabsize)
		{
			this.stream = stream;
			pushed = false;
			c = (int) '\x0000';
			tabs = 0;
			this.tabsize = tabsize;
			curline = 1;
			curcol = 1;
			this.encoding = encoding;
			state = FSM_ASCII;
			endOfStream = false;
			_reader = new StreamReader(stream, true);
		}

		public override bool IsEndOfStream
		{
			get
			{
				return _reader.Peek() == EndOfStream;
			}
		}

		public override int ReadCharFromStream()
		{
			return _reader.Read();
		}

		public override int ReadChar()
		{
			int c;
			
			if (pushed)
			{
				pushed = false;
				c = this.c;
				
				if (c == '\n')
				{
					curcol = 1;
					curline++;
					return c;
				}
				
				curcol++;
				return c;
			}
			
			lastcol = curcol;
			
			if (tabs > 0)
			{
				curcol++;
				tabs--;
				return ' ';
			}
			
			for (;;)
			{
				c = ReadCharFromStream();
				if (c < 0)
				{
					return EndOfStream;
				}
				
				if (c == '\n')
				{
					curcol = 1;
					curline++;
					break;
				}
				
				if (c == '\r')
				{
					c = ReadCharFromStream();
					if (c != '\n')
					{
						UngetChar(c);
						c = '\n';
					}
					curcol = 1;
					curline++;
					break;
				}
				
				if (c == '\t')
				{
					tabs = tabsize - ((curcol - 1) % tabsize) - 1;
					curcol++;
					c = ' ';
					break;
				}
				
				/* strip control characters, except for Esc */
				
				if (c == '\x001B')
				{
					break;
				}
				
				if (0 < c && c < 32)
				{
					continue;
				}

				/* watch out for IS02022 */
				
				if (encoding == CharEncoding.Raw || encoding == CharEncoding.ISO2022)
				{
					curcol++;
					break;
				}
				
				if (encoding == CharEncoding.MacroMan)
				{
					c = Mac2Unicode[c];
				}
				
				/* produced e.g. as a side-effect of smart quotes in Word */
				if (127 < c && c < 160)
				{
					Report.EncodingError((Lexer) Lexer, Report.WINDOWS_CHARS, c);
					
					c = Win2Unicode[c - 128];
					if (c == 0)
					{
						continue;
					}
				}
				
				curcol++;
				break;
			}
			
			return c;
		}

		public override void UngetChar(int c)
		{
			pushed = true;
			this.c = c;
			
			if (c == '\n')
			{
				--curline;
			}
			
			curcol = lastcol;
		}

		private StreamReader _reader;
		
		/* Mapping for Windows Western character set (128-159) to Unicode */
		private static int[] Win2Unicode = new int[]{0x20AC, 0x0000, 0x201A, 0x0192, 0x201E, 0x2026, 0x2020, 0x2021, 0x02C6, 0x2030, 0x0160, 0x2039, 0x0152, 0x0000, 0x017D, 0x0000, 0x0000, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x02DC, 0x2122, 0x0161, 0x203A, 0x0153, 0x0000, 0x017E, 0x0178};
		
		/*
		John Love-Jensen contributed this table for mapping MacRoman
		character set to Unicode
		*/
		
		private static int[] Mac2Unicode = new int[]{0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E, 0x000F, 0x0010, 0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 0x0018, 0x0019, 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F, 0x0020, 0x0021, 0x0022, 0x0023, 0x0024, 0x0025, 0x0026, 0x0027, 0x0028, 0x0029, 0x002A, 0x002B, 0x002C, 0x002D, 0x002E, 0x002F, 0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037, 0x0038, 0x0039, 0x003A, 0x003B, 0x003C, 0x003D, 0x003E, 0x003F, 0x0040, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x005B, 0x005C, 0x005D, 0x005E, 0x005F, 0x0060, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x007B, 0x007C, 0x007D, 0x007E, 0x007F, 0x00C4, 0x00C5, 0x00C7, 0x00C9, 0x00D1, 0x00D6, 0x00DC, 0x00E1, 0x00E0, 0x00E2, 0x00E4, 0x00E3, 0x00E5, 0x00E7, 0x00E9, 0x00E8, 0x00EA, 0x00EB, 0x00ED, 0x00EC, 0x00EE, 0x00EF, 0x00F1, 0x00F3, 0x00F2, 0x00F4, 0x00F6, 0x00F5, 0x00FA, 0x00F9, 0x00FB, 0x00FC, 0x2020, 0x00B0, 0x00A2, 0x00A3, 0x00A7, 0x2022, 0x00B6, 0x00DF, 0x00AE, 0x00A9, 0x2122, 0x00B4, 0x00A8, 0x2260, 0x00C6, 0x00D8, 0x221E, 0x00B1, 0x2264, 0x2265, 0x00A5, 0x00B5, 0x2202, 0x2211, 0x220F, 0x03C0, 0x222B, 0x00AA, 0x00BA, 0x03A9, 0x00E6, 0x00F8, 0x00BF, 0x00A1, 0x00AC, 0x221A, 0x0192, 0x2248, 0x2206, 0x00AB, 0x00BB, 0x2026, 0x00A0, 0x00C0, 0x00C3, 0x00D5, 0x0152, 0x0153, 0x2013, 0x2014, 0x201C, 0x201D, 0x2018, 0x2019, 0x00F7, 0x25CA, 0x00FF, 0x0178, 0x2044, 0x20AC, 0x2039, 0x203A, 0xFB01, 0xFB02, 0x2021, 0x00B7, 0x201A, 0x201E, 0x2030, 0x00C2, 0x00CA, 0x00C1, 0x00CB, 0x00C8, 0x00CD, 0x00CE, 0x00CF, 0x00CC, 0x00D3, 0x00D4, 0xF8FF, 0x00D2, 0x00DA, 0x00DB, 0x00D9, 0x0131, 0x02C6, 0x02DC, 0x00AF,
														0x02D8, 0x02D9, 0x02DA, 0x00B8, 0x02DD, 0x02DB, 0x02C7};		
	}
}
