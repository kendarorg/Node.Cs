
using Castle.Windsor;
using GenericHelpers;

namespace Node.Cs
{
	class Program
	{
		static void Main(string[] args)
		{
			const string helpMessage = "Node.cs -execute file.cs -service serviceName";
			string[] allowMultiple = {"execute"};
			var clp = new CommandLineParser(args, helpMessage, null, ';', allowMultiple);
			var container = new WindsorContainer();
			var entryPoint = new NodeCsEntryPoint(clp, container);
			entryPoint.Run();
		}
	}
}
