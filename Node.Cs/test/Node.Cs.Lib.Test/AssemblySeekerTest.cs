// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using Castle.MicroKernel.Registration;
using Kendar.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Test;
using Node.Cs.Utils;
using System.IO;

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
		public void FindAssembly_ShouldFindWithoutPathSpecified()
		{
			SetupTarget();
			var path = Container.Resolve<INodeExecutionContext>();
			var expected = Path.Combine(path.CurrentDirectory.Data, "Node.cs.lib.test.dll");
			var result = Target.FindAssembly("Node.cs.lib.test.dll");
			Assert.AreEqual(expected, result);
			Assert.IsTrue(File.Exists(result));
		}

		[TestMethod]
		public void FindAssembly_ShouldFindAbsoultePaths()
		{
			SetupTarget();
			var path = Container.Resolve<INodeExecutionContext>();
			var expected = Path.Combine(path.CurrentDirectory.Data, "Node.cs.lib.test.dll");
			var result = Target.FindAssembly(expected);
			Assert.AreEqual(expected, result);
			Assert.IsTrue(File.Exists(result));
		}

		[TestMethod]
		public void FindAssembly_ShouldFindThroughDllNameIfRootedAndNotFound()
		{
			SetupTarget();
			var path = Container.Resolve<INodeExecutionContext>();
			var expected = Path.Combine(path.CurrentDirectory.Data, "Node.cs.lib.test.dll");
			var fake = Path.Combine("C:\\not_existing", "Node.cs.lib.test.dll");
			var result = Target.FindAssembly(fake);
			Assert.AreEqual(expected, result);
			Assert.IsTrue(File.Exists(result));
		}

		[TestMethod]
		public void FindAssembly_ShouldReturnNullIfSearchingForFileNotDll()
		{
			SetupTarget();
			const string path = "C:\\not_existing\\dll.dll";
			var result = Target.FindAssembly(path);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void FindAssembly_IfExtensionIsMissingShouldBeAdded_WithDottedDlls()
		{
			SetupTarget();
			var path = Container.Resolve<INodeExecutionContext>();
			var expected = Path.Combine(path.CurrentDirectory.Data, "Node.cs.lib.test.dll");
			var result = Target.FindAssembly("Node.cs.lib.test");
			Assert.AreEqual(expected, result);
			Assert.IsTrue(File.Exists(result));
		}

		[TestMethod]
		public void FindAssembly_IfExtensionIsMissingShouldBeAdded_WithNotDottedDlls()
		{
			SetupTarget();
			var path = Container.Resolve<INodeExecutionContext>();
			const string externalDllName = "SimpleNameDll";

			var expected = Path.Combine(path.CurrentDirectory.Data, externalDllName + ".dll");

			NodeCopyUtils.CopyDllOnTarget(externalDllName, path);


			var result = Target.FindAssembly(externalDllName);
			Assert.AreEqual(expected, result);
			Assert.IsTrue(File.Exists(result));
		}
	}
}