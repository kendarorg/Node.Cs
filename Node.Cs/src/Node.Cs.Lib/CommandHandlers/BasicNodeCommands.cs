
using System;
using Node.Cs.Consoles;

namespace Node.Cs.CommandHandlers
{
	public class BasicNodeCommands:IBasicNodeCommands
	{
		private readonly INodeConsole _console;

		public BasicNodeCommands(INodeConsole console)
		{
			_console = console;
		}

		/// <summary>
		/// Runs a .cs file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		public void Run(NodeExecutionContext context, string path)
		{
			throw new NotImplementedException();
		}

		public void Echo(NodeExecutionContext context, string message)
		{
			_console.WriteLine(message);
		}

		public void Exit(NodeExecutionContext context, int errorCode)
		{
			Environment.Exit(errorCode);
		}
	}
}
