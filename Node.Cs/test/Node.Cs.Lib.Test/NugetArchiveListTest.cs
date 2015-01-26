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
using Node.Cs.Nugets;
using Node.Cs.Test;
using Node.Cs.Utils;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs
{
	[TestClass]
	public class NugetArchiveListTest : TestBase<NugetArchiveList, INugetArchiveList>
	{
		private string _path;

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();

			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
			Container.Register(Component.For<IAssemblySeeker>().ImplementedBy<AssemblySeeker>());
			CleanupPackagesList();
		}

		[TestMethod]
		public void Add_ShouldCreateTheFileIfNotExists()
		{
			//Setup
			SetupTarget();

			//Act
			Target.Add("test.module", ("1.0.0.0"), new List<string>(),new List<NugetPackageDependency>());

			//Verify
			Assert.IsTrue(File.Exists(_path));
		}


		[TestMethod]
		public void Get_NonExistingModuleWhenSomeIsPresentReturnNull()
		{
			//Setup
			SetupTarget();
			Target.Add("test.module", ("1.0.0.0"), new List<string>(), new List<NugetPackageDependency>());

			//Act
			var result = Target.Get("aa");

			//Verify
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Remove_ShouldCreateTheFileIfNotExists()
		{
			//Setup
			SetupTarget();

			//Act
			Target.Remove("test.module");

			//Verify
			Assert.IsTrue(File.Exists(_path));
		}

		[TestMethod]
		public void Check_ShouldCreateTheFileIfNotExists()
		{
			//Setup
			SetupTarget();

			//Act
			Target.Check("test.module", ("1.0.0.0"));

			//Verify
			Assert.IsTrue(File.Exists(_path));
		}

		private void CleanupPackagesList()
		{
			var context = Container.Resolve<INodeExecutionContext>();
			_path = Path.Combine(context.NodeCsPackagesDirectory, "packages.json");
			if (File.Exists(_path))
			{
				File.Delete(_path);
			}
		}

		[TestMethod]
		public void Check_ShouldReturnFalseWhenNoPackageExists()
		{
			//Setup
			SetupTarget();

			//Act
			var result = Target.Check("test.module", ("1.0.0.0"));

			//Verify
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void Add_ShouldLetCheckReturnTrue()
		{
			//Setup
			SetupTarget();
			Target.Add("test.module", ("1.0.0.0"), new List<string>(), new List<NugetPackageDependency>());

			//Act
			var result = Target.Check("test.module", ("1.0.0.0"));

			//Verify
			Assert.IsTrue(result);
		}



		[TestMethod]
		public void Add_ShouldLetGetReturnThePackageData()
		{
			//Setup
			SetupTarget();
			Target.Add("test.module", ("1.0.0.0"), new List<string> { "a.dll", "b.dll" }, new List<NugetPackageDependency>());

			//Act
			var result = Target.Get("test.module", "1.0.0.0");

			//Verify
			Assert.IsNotNull(result);
			Assert.AreEqual(result.Id, "test.module");
			Assert.AreEqual(result.Version, "1.0.0.0");
			CollectionAssert.Contains(result.Dlls, "a.dll");
			CollectionAssert.Contains(result.Dlls, "b.dll");
		}

		[TestMethod]
		public void Add_ShouldMakeCheckReturnTrueWithoutVersion()
		{
			Assert.Inconclusive("Add_ShouldMakeCheckReturnTrueWithoutVersion");
			////Setup
			//SetupTarget();
			//Target.Add("test.module", ("1.0.0.0"));

			////Act
			//var result = Target.Check("test.module");

			////Verify
			//Assert.IsTrue(result);
		}

		[TestMethod]
		public void Add_ShouldConsiderDependencies()
		{
			Assert.Inconclusive("Add_ShouldConsiderDependencies");
		}

		[TestMethod]
		public void Add_ShouldThrowWithMultipleVersions()
		{
			Assert.Inconclusive("Add_ShouldThrowWithMultipleVersions");
			////Setup
			//SetupTarget();
			//Target.Add("test.module", ("1.0.0.0"));

			////Act
			//var result = Target.Check("test.module");

			////Verify
			//Assert.IsTrue(result);
		}
	}
}