using Node.Cs;
using Node.Cs.Consoles;


// ReSharper disable once CheckNamespace
public class Test
{
	private readonly INodeConsole _console;
	private readonly INodeExecutionContext _context;

	public Test(INodeConsole console, INodeExecutionContext context)
	{
		_console = console;
		_context = context;
	}

	public void Execute()
	{
		_console.WriteLine("Executing Test.cs");
	}

	public void Do()
	{
		_console.WriteLine("Executing Test.cs::Do");
	}
}
