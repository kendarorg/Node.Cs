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


using System;
using Node.Cs.CommandHandlers;

namespace Node.Cs
{
	public class NodeRootModule : INodeModule
	{
		private const string NAME = "Node.Cs.RootModule";
		private static readonly Version _version = new Version("2.0.0.0");

		private readonly IUiCommandsHandler _commandsHandler;
		private readonly IBasicNodeCommands _nodeCommands;

		public NodeRootModule(IUiCommandsHandler commandsHandler, IBasicNodeCommands basicNodeCommands)
		{
			_commandsHandler = commandsHandler;
			_nodeCommands = basicNodeCommands;
		}

		public string Name
		{
			get { return NAME; }
		}
		public Version Version
		{
			get { return _version; }
		}

		public void Initialize()
		{
			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"run", new Action<INodeExecutionContext, string, string>(_nodeCommands.Run), "run [c# file] (method) (command)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"echo", new Action<INodeExecutionContext, string>(_nodeCommands.Echo), "echo [Message]"));


			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"loadDll", new Action<INodeExecutionContext, string>(_nodeCommands.LoadDll), "loadDll [path]"));


			_commandsHandler.RegisterCommand(
					new CommandDescriptor(
							"loadnuget", new Action<INodeExecutionContext, string, string, bool>(_nodeCommands.LoadNuget), "loadnuget [package] (version) (boolUsePreRelease)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"exit", new Action<INodeExecutionContext, int>(_nodeCommands.Exit), "exit (errorCode)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"help", new Action<INodeExecutionContext, string>(_commandsHandler.Help), "help (command)"));
		}
	}
}
