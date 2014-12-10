using Castle.Windsor;
using GenericHelpers;

namespace Node.Cs
{
	public class NodeCsEntryPointForTest : NodeCsEntryPoint
	{
		public const int Once = 1;
		public int CyclesToRun { get; set; }

		public NodeCsEntryPointForTest(CommandLineParser args, WindsorContainer container, int cyclesToRun) : 
			base(args, container)
		{
			CyclesToRun = cyclesToRun;
		}

		protected override bool ShouldContinueRunning()
		{
			if (CyclesToRun > 0)
			{
				CyclesToRun--;
				return true;
			}
			return false;
		}
	}
}