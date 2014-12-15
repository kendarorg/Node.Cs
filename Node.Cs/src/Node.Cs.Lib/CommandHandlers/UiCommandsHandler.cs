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


using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var lps = GetParametersFromDelegate(l.CalledFunction);
            var rps = GetParametersFromDelegate(r.CalledFunction);
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
            var commandPars = GetParametersFromDelegate(command.CalledFunction);
            if (commandPars.Length != paramTypes.Length) return false;
            for (int i = 0; i < commandPars.Length; i++)
            {
                var cp = commandPars[i].ParameterType;
                var pt = paramTypes[i];
                if (cp != pt) return false;
            }
            return true;
        }

        private ParameterInfo[] GetParametersFromDelegate(Delegate del)
        {
            var result = new List<ParameterInfo>();
            var method = del.GetType().GetMethod("Invoke");
            for (int index = 1; index < method.GetParameters().Length; index++)
            {
                var par = method.GetParameters()[index];
                result.Add(par);
            }
            return result.ToArray();
        }

        public void Run(string command)
        {
            var parsedCommand = _commandParser.Parse(command);
            if (!_commands.ContainsKey(parsedCommand.Command))
            {
                if (ContainsCommand("help", new[] { typeof(string) }))
                {
                    ShowHelpForCommand("help");
                    return;
                }
                throw new MissingCommandException("Command '{0}' not registered.", parsedCommand.Command);
            }
            var possibleCommands = _commands[parsedCommand.Command];
            for (int i = 0; i < possibleCommands.Count; i++)
            {
                var possibleCommand = possibleCommands[i];
                var realParams = SetupParams(possibleCommand, parsedCommand.Parameters);
                if (realParams != null)
                {
                    possibleCommand.CalledFunction.DynamicInvoke(realParams.ToArray());
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
                for (int i = 0; possibleCommands != null && i < possibleCommands.Count; i++)
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
            var parmInfos = GetParametersFromDelegate(possibleCommand.CalledFunction);
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
                }
                //if (!paramInfo.IsOptional) return null;
                converted.Add(paramInfo.DefaultValue == DBNull.Value ? UniversalTypeConverter.Convert(null, paramInfo.ParameterType) : paramInfo.DefaultValue);
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
