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
using Node.Cs.Exceptions;
using Node.Cs.Mocks;
using Node.Cs.Modules;
using Node.Cs.Test;
using Node.Cs.Utils;

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
