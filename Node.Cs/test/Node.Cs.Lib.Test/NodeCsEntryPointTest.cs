using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Test;

namespace Node.Cs.Lib.Test
{
	[TestClass]
	public class NodeCsEntryPointTest : TestBase
	{
		[TestMethod]
		public void Run_ShouldInitializeTheUiCommandHandler()
		{
			var mock = this.MockOf<IUiCommandsHandler>();
			

			var target = new NodeCsEntryPoint(new string[] {}, Container);
			target.Run();
		}
	}
}
