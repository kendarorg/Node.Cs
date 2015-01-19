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
using Node.Cs.CommandHandlers;
using Node.Cs.Utils;

namespace Node.Cs.Modules
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
					"dll load", new Action<INodeExecutionContext, string>(_nodeCommands.LoadDll), "loadDll [path]"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"nuuget load", new Action<INodeExecutionContext, string, string, bool>(_nodeCommands.LoadNuget), "loadnuget [package] (version) (boolUsePreRelease)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"exit", new Action<INodeExecutionContext, int>(_nodeCommands.Exit), "exit (errorCode)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"help", new Action<INodeExecutionContext, string>(_commandsHandler.Help), "help (command)"));
		}
	}
}
