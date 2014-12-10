
using System;
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

		private static bool IsLetter(char ch)
		{
			return !IsSeparator(ch) && !IsStringDelimiter(' ', ch);
		}

		public ParsedCommand Parse(string item)
		{
			var preParsedBlocks = new List<string>();
			bool inString = false;
			item = item.Trim();
			int currentItem = -1;
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
							preParsedBlocks.Add(lastItem);
							lastItem = string.Empty;
						}
						continue;
					}
					else if (
						(index > 1 && IsStringDelimiter(item[index - 1], ch)) ||
						(index == 0 && IsStringDelimiter(' ', ch)))
					{
						
					}
					else
					{
						lastItem += ch;
					}
				}

			}
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
	}
}
