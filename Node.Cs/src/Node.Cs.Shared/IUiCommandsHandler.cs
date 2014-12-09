
namespace Node.Cs
{
	public interface IUiCommandsHandler
	{
		/// <summary>
		/// Register a command
		/// </summary>
		/// <param name="cd"></param>
		void RegisterCommand(CommandDescriptor cd);

		/// <summary>
		/// Run a command passing all parameters
		/// </summary>
		/// <param name="command"></param>
		void Run(string command);

		/// <summary>
		/// Show the help for the given command/subcommand couple
		/// </summary>
		/// <param name="context"></param>
		/// <param name="command"></param>
		/// <param name="subcommand"></param>
		void Help(NodeExecutionContext context, string command = null, string subcommand = null);
	}
}
