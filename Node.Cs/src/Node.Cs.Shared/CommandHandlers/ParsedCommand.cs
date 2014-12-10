using System.Collections.Generic;

namespace Node.Cs.CommandHandlers
{
	public class ParsedCommand
	{
		public ParsedCommand()
		{
			Parameters = new List<string>();
		}
		public string Command { get; set; }
		public List<string> Parameters { get; private set; }
	}
}