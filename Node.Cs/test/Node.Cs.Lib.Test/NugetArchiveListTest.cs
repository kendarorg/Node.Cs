// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using Castle.MicroKernel.Registration;
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