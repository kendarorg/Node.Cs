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


using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Node.Cs.Utils;
using TB.ComponentModel;

namespace Node.Cs.CommandHandlers
{
	public class UiCommandsHandler : IUiCommandsHandler
	{
		private readonly ICommandParser _commandParser;
		private readonly INodeExecutionContext _context;
		private readonly Dictionary<string, List<CommandDescriptor>> _commands;
		private readonly INodeConsole _console;

		public UiCommandsHandler(ICommandParser commandParser, INodeExecutionContext context, INodeConsole console)
		{
			_commandParser = commandParser;
			_context = context;
			_commands = new Dictionary<string, List<CommandDescriptor>>(StringComparer.OrdinalIgnoreCase);
			_console = console;
		}

		public bool ContainsCommand(string name, params Type[] paramTypes)
		{
			if (!_commands.ContainsKey(name))
			{
				return false;
			}
			var possibleCommands = _commands[name];
			// ReSharper disable once LoopCanBeConvertedToQuery
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < possibleCommands.Count; i++)
			{
				var command = possibleCommands[i];
				if (HasSameParameters(command, paramTypes))
				{
					return true;
				}
			}
			return false;
		}


		public void RegisterCommand(CommandDescriptor cd)
		{
			if (!_commands.ContainsKey(cd.CommandId))
			{
				_commands.Add(cd.CommandId, new List<CommandDescriptor>());
			}
			var possibleCommands = _commands[cd.CommandId];
			// ReSharper disable once LoopCanBeConvertedToQuery
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < possibleCommands.Count; i++)
			{
				var command = possibleCommands[i];
				if (HasSameParameters(command, cd))
				{
					throw new DuplicateCommandException("Duplicate command '{0}' with same parameters.", cd.CommandId);
				}
			}
			possibleCommands.Add(cd);
		}

		private bool HasSameParameters(CommandDescriptor l, CommandDescriptor r)
		{
			var lps = GetParametersFromDelegate(l.CalledFunction.Method);
			var rps = GetParametersFromDelegate(r.CalledFunction.Method);
			if (lps.Length != rps.Length) return false;
			for (int i = 0; i < lps.Length; i++)
			{
				var cp = lps[i];
				var pt = rps[i];
				if (cp != pt) return false;
			}
			return true;
		}

		public void UnregisterCommand(string name, params Type[] paramTypes)
		{
			if (!_commands.ContainsKey(name))
			{
				return;
			}
			var possibleCommands = _commands[name];
			var toRemove = -1;
			for (int i = 0; i < possibleCommands.Count; i++)
			{
				var command = possibleCommands[i];
				if (HasSameParameters(command, paramTypes))
				{
					toRemove = i;
					break;
				}
			}
			if (toRemove >= 0)
			{
				possibleCommands.RemoveAt(toRemove);
			}
		}


		private bool HasSameParameters(CommandDescriptor command, Type[] paramTypes)
		{
			var commandPars = GetParametersFromDelegate(command.CalledFunction.Method);
			if (commandPars.Length != paramTypes.Length) return false;
			for (int i = 0; i < commandPars.Length; i++)
			{
				var cp = commandPars[i].ParameterType;
				var pt = paramTypes[i];
				if (cp != pt) return false;
			}
			return true;
		}

		private ParameterInfo[] GetParametersFromDelegate(MethodInfo method)
		{
			var result = new List<ParameterInfo>();
			//var method = del.GetType().GetMethod("Invoke");
			for (int index = 1; index < method.GetParameters().Length; index++)
			{
				var par = method.GetParameters()[index];
				result.Add(par);
			}
			return result.ToArray();
		}

		public void Run(string command, bool interactive = true)
		{
			var parsedCommand = _commandParser.Parse(command);
			for (int i = parsedCommand.Parameters.Count(); i > 0; i--)
			{
				var currentParams = parsedCommand.Parameters.Take(i);
				var resultCommands = string.Join(" ", currentParams).ToLowerInvariant();
				if (currentParams.Count() > 0)
				{
					resultCommands = " " + resultCommands;
				}
				resultCommands = parsedCommand.Command + resultCommands;
				if (_commands.ContainsKey(resultCommands))
				{
					var newCommand = new ParsedCommand();
					newCommand.Command = resultCommands;
					newCommand.Parameters.AddRange(parsedCommand.Parameters.Skip(i));
					parsedCommand = newCommand;
					break;
				}
			}

			if (!_commands.ContainsKey(parsedCommand.Command))
			{
				if (ContainsCommand("help", new[] { typeof(string) }) && interactive)
				{
					ShowHelpForCommand("help");
					return;
				}
				throw new MissingCommandException("Command '{0}' not registered.", parsedCommand.Command);
			}

			var possibleCommands = _commands[parsedCommand.Command];
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < possibleCommands.Count; i++)
			{
				var possibleCommand = possibleCommands[i];
				var realParams = SetupParams(possibleCommand, parsedCommand.Parameters);
				if (realParams != null)
				{
					try
					{
						possibleCommand.CalledFunction.DynamicInvoke(realParams.ToArray());
					}
					catch (Exception ex)
					{
						throw ex.InnerException;
					}
					return;
				}
			}
			ShowHelpForCommand(parsedCommand.Command, possibleCommands);
		}

		private void ShowHelpForCommand(string commandId, List<CommandDescriptor> possibleCommands = null)
		{
			if (possibleCommands != null)
			{
				_console.WriteLine("Node.Cs Help for command '{0}':", commandId);
				// ReSharper disable once LoopCanBeConvertedToQuery
				// ReSharper disable once ForCanBeConvertedToForeach
				for (int i = 0; i < possibleCommands.Count; i++)
				{
					var possibleCommand = possibleCommands[i];
					_console.WriteLine("");
					_console.WriteLine(possibleCommand.LongHelp);
				}
			}
			else
			{
				_console.WriteLine("Node.Cs AvailableCommands:");
				_console.WriteLine("");
				var commands = _commands.Keys.ToArray();
				// ReSharper disable once LoopCanBeConvertedToQuery
				// ReSharper disable once ForCanBeConvertedToForeach
				for (int i = 0; i < commands.Length; i++)
				{
					var possibleCommand = commands[i];
					_console.WriteLine("* {0}", possibleCommand);
				}

			}
			_console.WriteLine("");
		}

		private List<object> SetupParams(CommandDescriptor possibleCommand, List<string> parameters)
		{
			var converted = new List<object>();
			var parmInfos = GetParametersFromDelegate(possibleCommand.CalledFunction.Method);
			converted.Add(_context);
			if (parmInfos.Length < parameters.Count) return null;
			for (int i = 0; i < parmInfos.Length; i++)
			{
				var paramInfo = parmInfos[i];

				if (parameters.Count > i)
				{
					var data = parameters[i];
					if (UniversalTypeConverter.CanConvert(data, paramInfo.ParameterType))
					{
						object result;
						if (UniversalTypeConverter.TryConvert(data, paramInfo.ParameterType, out result))
						{
							converted.Add(result);
							continue;
						}
					}
					else
					{
						return null;
					}
				}
				var paramSpec = paramInfo.LoadSpecifications();
				if (paramSpec.HasDefault)
				{
					converted.Add(paramSpec.DefaultValue);
				}
				else
				{
					return null;
				}
			}
			return converted;
		}

		public void Help(INodeExecutionContext context, string command = null)
		{
			var parsedCommands = string.IsNullOrWhiteSpace(command) ? null : _commands[command];
			ShowHelpForCommand(command, parsedCommands);
		}
	}
}
