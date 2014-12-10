using System;
using Node.Cs.CommandHandlers;

namespace Node.Cs
{
	public class NodeRootModule : INodeModule
	{
		private readonly IUiCommandsHandler _commandsHandler;
		private readonly IBasicNodeCommands _nodeCommands;

		public NodeRootModule(IUiCommandsHandler commandsHandler,IBasicNodeCommands basicNodeCommands)
		{
			_commandsHandler = commandsHandler;
			_nodeCommands = basicNodeCommands;
		}

		public void PreInitialize()
		{
			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"run", new Action<NodeExecutionContext, string>(_nodeCommands.Run), "run [c# file]"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"echo", new Action<NodeExecutionContext, string>(_nodeCommands.Echo), "echo [Message]"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"exit", new Action<NodeExecutionContext,int>(_nodeCommands.Exit), "exit (errorCode)"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"help", new Action<NodeExecutionContext, string,string>(_commandsHandler.Help), "help [command] (subcommand)"));
		}
	}
}
