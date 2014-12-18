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
using Node.Cs.Exceptions;
using Node.Cs.Mocks;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class ModulesCollectionTest : TestBase<ModulesCollection, IModulesCollection>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

		[TestMethod]
		public void Register_ShouldAddTheModuleAndGetByName()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			
			//Act
			Target.Register(module);
			var result = Target.GetModule(module.Name);

			//Verify
			Assert.IsNotNull(result);
			Assert.AreSame(module,result);
		}

		[TestMethod]
		public void Register_ShouldBlockDuplicateNamesWithDifferentVersion()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			var duplicate = new FakeModuleFirstDuplicate();

			//Act
			Target.Register(module);
			ExceptionAssert.Throws<DuplicateModuleException>(() => Target.Register(duplicate));
		}

		[TestMethod]
		public void Register_ShouldIgnoreDuplicateNamesWithSameVersionAndTakeFirst()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			var duplicate = new FakeModuleFirst();

			//Act
			Target.Register(module);
			Target.Register(duplicate);

			//Verify
			var result = Target.GetModule(module.Name);
			Assert.AreSame(module, result);
		}


		[TestMethod]
		public void Register_ShouldTakeMultipleItems()
		{
			//Setup
			SetupTarget();
			var module1 = new FakeModuleFirst();
			var module2 = new FakeModuleSecond();

			//Act
			Target.Register(module1,module2);

			//Verify
			var result = Target.GetModule(module1.Name);
			Assert.AreSame(module1, result);
			result = Target.GetModule(module2.Name);
			Assert.AreSame(module2, result);
		}

		[TestMethod]
		public void GetModule_ShouldThrowWhenNotFindingModule()
		{
			//Setup
			SetupTarget();

			//Act
			ExceptionAssert.Throws<MissingModuleException>(() => Target.GetModule("test"));
		}


		[TestMethod]
		public void GetModuleT_ShouldGetModuleByType()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			Target.Register(module);

			//Act
			var result = Target.GetModule<FakeModuleFirst>();

			//Verify
			Assert.IsNotNull(result);
			Assert.AreSame(module, result);
		}


		[TestMethod]
		public void GetModuleT_ShouldGetModuleByTypeAndName()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			Target.Register(module);

			//Act
			var result = Target.GetModule<FakeModuleFirst>(module.Name);

			//Verify
			Assert.IsNotNull(result);
			Assert.AreSame(module, result);
		}


		[TestMethod]
		public void GetModuleT_ShouldThrowIfNotMatchingType()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			
			//Act
			ExceptionAssert.Throws<MissingModuleException>(() => Target.GetModule<FakeModuleFirst>(module.Name));
		}


		[TestMethod]
		public void GetModuleT_ShouldThrowIfMatchingNameButNotType()
		{
			//Setup
			SetupTarget();
			var module = new FakeModuleFirst();
			Target.Register(module);

			//Act
			ExceptionAssert.Throws<ModuleTypeException>(() => Target.GetModule<FakeModuleSecond>(module.Name));
		}
	}
}
