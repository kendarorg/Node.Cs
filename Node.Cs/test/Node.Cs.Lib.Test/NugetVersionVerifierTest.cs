using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Nugets;
using Node.Cs.Test;
using Node.Cs.Utils;

namespace Node.Cs
{
	[TestClass]
	public class NugetVersionVerifierTest : TestBase<NugetVersionVerifier, INugetVersionVerifier>
	{
		/*
		 * Should support
1.0  = 1.0 <= x
(,1.0]  = x <= 1.0
(,1.0)  = x < 1.0
[1.0] = x == 1.0
(1.0) = invalid
(1.0,) = 1.0 < x
(1.0,2.0) = 1.0 < x < 2.0
[1.0,2.0] = 1.0 <= x <= 2.0
empty = latest version.*/

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

		[TestMethod]
		public void ShouldExists()
		{
			Assert.Inconclusive("NugetVersionVerifier should have an implementation");
		}
	}
}