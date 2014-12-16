using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs
{
    [TestClass]
    public class AssemblySeekerTest : TestBase<AssemblySeeker, IAssemblySeeker>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
            base.TestInitialize();
            InitializeMock<INodeConsole>();
            Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

        [TestMethod]
        public void FindAssembly_ShouldFindAbsoultePaths()
        {
            SetupTarget();
            Assert.Inconclusive("FindAssembly_ShouldFindAbsoultePaths");
        }


        [TestMethod]
        public void FindAssembly_ShouldFindThroughDllNameIfRootedAndNotFound()
        {
            SetupTarget();
            Assert.Inconclusive("FindAssembly_ShouldFindThroughDllNameIfRootedAndNotFound");
        }

        [TestMethod]
        public void FindAssembly_ShouldReturnNullIfSearchingForFileNotDll()
        {
            SetupTarget();
            Assert.Inconclusive("FindAssembly_ShouldReturnNullIfSearchingForFileNotDll");
        }

        [TestMethod]
        public void FindAssembly_IfExtensionIsMissingShouldBeAdded()
        {
            SetupTarget();
            Assert.Inconclusive("FindAssembly_IfExtensionIsMissingShouldBeAdded");
        }
    }
}
