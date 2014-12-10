namespace Node.Cs.CommandHandlers
{
	public interface ICommandParser
	{
		ParsedCommand Parse(string item);
	}
}