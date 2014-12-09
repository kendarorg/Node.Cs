using System;

namespace Node.Cs
{
	public class CommandDescriptor
	{
		public CommandDescriptor(string commandId, Delegate calledFunction, string shortHelp)
		{
			CommandId = commandId;
			CalledFunction = calledFunction;
			ShortHelp = shortHelp;
		}
		public string CommandId { get; private set; }
		public Delegate CalledFunction { get; private set; }
		public string ShortHelp { get; private set; }
	}
}