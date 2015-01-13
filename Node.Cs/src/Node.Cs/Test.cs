using Node.Cs.Consoles;


	class Test
	{
		private readonly INodeConsole _console;

		public Test(INodeConsole console)
		{
			_console = console;
		}

		public void Execute()
		{
			_console.WriteLine("Test called");
		}
	}
