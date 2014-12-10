namespace Node.Cs.CommandHandlers
{
	public interface IBasicNodeCommands
	{
		void Run(NodeExecutionContext context, string path);
		void Echo(NodeExecutionContext context, string message);
		void Exit(NodeExecutionContext context, int errorCode);
	}
}