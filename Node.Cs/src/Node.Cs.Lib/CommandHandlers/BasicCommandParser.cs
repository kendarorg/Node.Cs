// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================



using System.Collections.Generic;
using System.Globalization;

namespace Node.Cs.CommandHandlers
{
	public class BasicCommandParser : ICommandParser
	{
		private static bool IsSeparator(char ch)
		{
			var tmp = ch.ToString(CultureInfo.InvariantCulture);
			return tmp.Trim().Length == 0;
		}

		private static bool IsStringDelimiter(char prevCh, char ch)
		{
			return (ch == '\"' || ch == '\'') && prevCh != '\\';
		}

		public ParsedCommand Parse(string item)
		{


			var preParsedBlocks = Tokenize(item);

			return BuildParsedCommand(preParsedBlocks);
		}

		private static ParsedCommand BuildParsedCommand(List<string> preParsedBlocks)
		{
			var result = new ParsedCommand();
			for (int index = 0; index < preParsedBlocks.Count; index++)
			{
				var preParsed = preParsedBlocks[index];
				if (index == 0)
				{
					result.Command = preParsed;
				}
				else
				{
					result.Parameters.Add(preParsed);
				}
			}
			return result;
		}

		/// <summary>
		/// Tokenize the whole block. I have not found a way to reduce the cyclomatic complexity
		/// below this point keeping the function readable
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private static List<string> Tokenize(string item)
		{
			var preParsedBlocks = new List<string>();
			bool inString = false;
			char stringDelimiter = '\0';
			string lastItem = string.Empty;
			for (int index = 0; index < item.Length; index++)
			{
				var ch = item[index];
				if (!inString)
				{
					if (IsSeparator(ch))
					{
						if (index > 1 && !IsSeparator(item[index - 1]))
						{
							if (lastItem.Length > 0)
							{
								preParsedBlocks.Add(lastItem);
							}
							lastItem = string.Empty;
						}
						continue;
					}
					if (
							(index > 1 && IsStringDelimiter(item[index - 1], ch)) ||
							(index == 0 && IsStringDelimiter(' ', ch)))
					{
						stringDelimiter = ch;
						lastItem = ch.ToString(CultureInfo.InvariantCulture);
						inString = true;
					}
					else
					{
						lastItem += ch;
					}
				}
				else if (((index > 1 && IsStringDelimiter(item[index - 1], ch)) ||
								(index == 0 && IsStringDelimiter(' ', ch)))
								&& ch == stringDelimiter)
				{
					lastItem += ch;
					lastItem = lastItem.Substring(1, lastItem.Length - 2);
					inString = false;
				}
				else
				{
					lastItem += ch;
				}
			}
			if (lastItem.Length > 0)
			{
				preParsedBlocks.Add(lastItem);
			}
			return preParsedBlocks;
		}
	}
}
