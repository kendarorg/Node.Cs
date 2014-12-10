
using Castle.Windsor;

namespace Node.Cs
{
	class Program
	{
		static void Main(string[] args)
		{
			var container = new WindsorContainer();
			var entryPoint = new NodeCsEntryPoint(args, container);
			entryPoint.Run();
		}
	}
}
