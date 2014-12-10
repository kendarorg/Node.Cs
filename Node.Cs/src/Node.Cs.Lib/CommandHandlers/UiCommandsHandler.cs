
namespace Node.Cs.CommandHandlers
{
	public class UiCommandsHandler : IUiCommandsHandler
	{
		private readonly ICommandParser _commandParser;

		public UiCommandsHandler(ICommandParser commandParser)
		{
			_commandParser = commandParser;
		}

		public void RegisterCommand(CommandDescriptor cd)
		{
			throw new System.NotImplementedException();
		}

		public void Run(string command)
		{
			throw new System.NotImplementedException();
		}

		public void Help(NodeExecutionContext context, string command = null, string subcommand = null)
		{
			throw new System.NotImplementedException();
		}
	}
}
