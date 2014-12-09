using System;
using Node.Cs.CommandHandlers;

namespace Node.Cs
{
	public class NodeRootModule : INodeModule
	{
		public NodeRootModule(IUiCommandsHandler commandsHandler)
		{
			_commandsHandler = commandsHandler;
		}
		public IUiCommandsHandler _commandsHandler;

		public void PreInitialize()
		{
			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"run", new Action<NodeExecutionContext, string>(BasicNodeCommands.Run), "run [c# file]"));


			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"echo", new Action<NodeExecutionContext, string>(BasicNodeCommands.Echo), "echo [Message]"));

			_commandsHandler.RegisterCommand(
				new CommandDescriptor(
					"help", new Action<NodeExecutionContext, string,string>(_commandsHandler.Help), "help [command] (subcommand)"));
		}
	}
}
