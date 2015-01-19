// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
