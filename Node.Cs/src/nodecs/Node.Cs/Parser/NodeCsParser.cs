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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoroutinesLib.Shared;
using GenericHelpers;
using NodeCs.Shared;

namespace NodeCs.Parser
{
	internal class NodeCsParser
	{


		private readonly CommandLineParser _clp;
		private readonly string _configPath;
		private  Exception _lastException;

		public NodeCsParser(CommandLineParser clp, string configPath, ICoroutinesManager coroutinesManager)
		{
			_clp = clp;
			_configPath = configPath;
		}

		public bool Execute(string result)
		{
			var tokens = Tokenize(result);
			var first = tokens.FirstOrDefault();
			if (first == null)
			{
				Shared.NodeRoot.Write();
			}
			else if (first.Type == TokenType.Command && first.Value == "exit")
			{
				return false;
			}
			else if (first.Type == TokenType.Command && first.Value == "lasterror")
			{
				if (_lastException == null)
				{
					Shared.NodeRoot.CWriteLine("No previous errors.");
				}
				else
				{
					Shared.NodeRoot.CWriteLine(_lastException.Message);
					Shared.NodeRoot.CWriteLine(_lastException.ToString());
				}
				return true;
			}
			else if (first.Type == TokenType.Command)
			{
				var functionIndex = (first.Value + (tokens.Count - 1)).ToLowerInvariant();
				if (!Commands.Functions.ContainsKey(functionIndex))
				{
					Shared.NodeRoot.CWriteLine(string.Format("Command not found. Type 'help {0}' to get the parameters.", first.Value));
					return true;
				}
				var cmd = Commands.Functions[functionIndex];
				if (tokens.Count != cmd.ParametersCount + 1)
				{
					Shared.NodeRoot.CWriteLine(string.Format("Command '{0}' requires '{1}' parameters.", first.Value, cmd.ParametersCount));
				}
				else
				{
					try
					{
						ExecuteCommand(cmd, tokens);
					}
					catch (Exception ex)
					{
						Shared.NodeRoot.CWriteLine(string.Format("Error executing: {0}", result));
						Shared.NodeRoot.CWriteLine(ex.ToString());
					}
				}
			}
			else
			{
				Shared.NodeRoot.CWriteLine(string.Format("Invalid command: {0}", result));
			}
			return true;
		}

		private List<NodeCsToken> Tokenize(string result)
		{
			var tokens = new List<NodeCsToken>();
			var tmp = string.Empty;
			for (int index = 0; index < result.Length; index++)
			{
				var c = result[index];
				if (IsStringStart(c))
				{
					tmp = string.Empty;
					index++;
					var end = FindEndOfString(c, index, result);
					tokens.Add(new NodeCsToken(result.Substring(index, end - index), TokenType.String));
					index = end + 1;
				}
				else if (IsSeparator(c))
				{
					if (tmp.Length > 0)
					{
						tokens.Add(BuildCommand(tmp));
					}
					tmp = string.Empty;
				}
				else
				{
					tmp += c;
				}
			}
			if (tmp.Length > 0)
			{
				tokens.Add(BuildCommand(tmp));
			}
			return tokens;
		}

		private NodeCsToken BuildCommand(string tokenString)
		{
			tokenString = tokenString.ToLowerInvariant().Trim();
			if (Commands.Functions.Any(c => c.Key.StartsWith(tokenString)))
			{
				return new NodeCsToken(tokenString, TokenType.Command);
			}
			double result;
			if (double.TryParse(tokenString, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return new NodeCsToken(tokenString, TokenType.Numeric);
			}
			return new NodeCsToken(tokenString, TokenType.String);
		}

		private bool IsSeparator(char c)
		{
			return c == ' ' || c == '\t';
		}

		private int FindEndOfString(char startChar, int index, string result)
		{
			char prevChar = startChar;
			for (; index < result.Length; index++)
			{
				var current = result[index];
				if (IsStringEnd(startChar, prevChar, current))
				{
					return index;
				}
			}
			throw new Exception("Missing end of string");
		}
		private bool IsStringEnd(char start, char prev, char current)
		{
			return current == start && prev != '\\';
		}
		private bool IsStringStart(char c)
		{
			return c == '\"' || c == '\'';
		}

		private void ExecuteCommand(CommandDefinition cmd, List<NodeCsToken> tokens)
		{
			try
			{
				var type = cmd.Action.GetType();
				if (type == typeof(Action))
				{
					((Action)cmd.Action)();
				}
				else if (type == typeof(Action<string>))
				{
					((Action<string>)cmd.Action)(tokens[1].Value);
				}
				else if (type == typeof(Action<string, string>))
				{
					((Action<string, string>)cmd.Action)(tokens[1].Value, tokens[2].Value);
				}
				else if (type == typeof(Action<string, string,string>))
				{
					((Action<string, string, string>)cmd.Action)(tokens[1].Value, tokens[2].Value, tokens[3].Value);
				}
				else
				{
					throw new Exception("Unsupported command");
				}
			}
			catch (Exception ex)
			{
				_lastException = ex;
				Shared.NodeRoot.CWriteLine("Error executing command:");
				Shared.NodeRoot.CWriteLine(ex.Message);
			}
		}
	}
}
