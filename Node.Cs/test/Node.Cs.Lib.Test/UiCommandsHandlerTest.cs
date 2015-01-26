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


using System;
using Kendar.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Mocks;
using Node.Cs.Test;
using Castle.MicroKernel.Registration;
using Node.Cs.Utils;

namespace Node.Cs
{
	[TestClass]
	public class UiCommandsHandlerTest : TestBase<UiCommandsHandler, IUiCommandsHandler>
	{
		private UiCommandsHandlerTestMock _handler;
		private MockNodeConsole _mockConsole;

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			_mockConsole = new MockNodeConsole();

			Container.Register(
											Component.For<INodeConsole>().Instance(_mockConsole));

			Container.Register(
											Component.For<ICommandParser>().
											ImplementedBy<BasicCommandParser>());

			_handler = new UiCommandsHandlerTestMock();
		}

		[TestMethod]
		public void RegisterCommand_ShouldBeAbleToRegisterAndUnregisterCommands()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");

			//Act
			Target.RegisterCommand(cd);

			//Verify
			Assert.IsTrue(Target.ContainsCommand("test", new Type[0]));
		}

		[TestMethod]
		public void UnRegisterCommand_ShouldBeAbleToUnregisterCommands()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.UnregisterCommand("test", new Type[0]);

			//Verify
			Assert.IsFalse(Target.ContainsCommand("test", new Type[0]));
		}

		[TestMethod]
		public void UnRegisterCommand_ShouldBeAbleToUnregisterNonExistingCommand()
		{
			//Setup
			SetupTarget();

			//Act
			Assert.IsFalse(Target.ContainsCommand("test", new Type[0]));
			Target.UnregisterCommand("test", new Type[0]);

			//Verify
			Assert.IsFalse(Target.ContainsCommand("test", new Type[0]));
		}


		[TestMethod]
		public void RegisterCommand_ShouldBlockDuplicateCommands()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);
			DuplicateCommandException result;

			//Act
			ExceptionAssert.Throws(() => Target.RegisterCommand(cd), out result);

			//Verify
			Assert.IsTrue(result.Message.Contains("test"));
		}

		[TestMethod]
		public void RegisterCommand_ShouldNotBlockDuplicateCommandsWithDifferentParameters()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, string>(_handler.CommantWithOverload), "test help");
			Target.RegisterCommand(cd);
			var cds = new CommandDescriptor("test", new Action<INodeExecutionContext, int>(_handler.CommantWithOverload), "test help");

			//Act
			Target.RegisterCommand(cds);

			//Verify
			Assert.IsTrue(Target.ContainsCommand("test", new[] { typeof(string) }));
			Assert.IsTrue(Target.ContainsCommand("test", new[] { typeof(int) }));
		}

		[TestMethod]
		public void Run_ShouldThrowOnMissingCommandId()
		{
			//Setup
			SetupTarget();
			MissingCommandException result;

			//Act
			ExceptionAssert.Throws(() => Target.Run("test"), out result);

			//Verify
			Assert.IsTrue(result.Message.Contains("test"));
		}

		[TestMethod]
		public void Run_ShouldExecuteSimpleCommands()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTest", _handler.IsAny<INodeExecutionContext>());
		}

		[TestMethod]
		public void Run_ShouldShowHelpWhenNoMatchingParamIsShown()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test wrongParam");

			//Verify
			SetsAssert.Contains(_mockConsole.Data, string.Format("Node.Cs Help for command '{0}':\r\n", "test"));
			SetsAssert.Contains(_mockConsole.Data, "test help\r\n");
		}

		[TestMethod]
		public void Run_ShouldShowListOfCommandsNameWhenNoMatchingCommandIsFound()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "the help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("prova", new Action<INodeExecutionContext>(_handler.DoTest), "the other help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("help", new Action<INodeExecutionContext, string>(Target.Help), "helping help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("notpresent wrongParam");

			//Verify
			SetsAssert.Contains(_mockConsole.Data, "Node.Cs AvailableCommands:\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* test\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* prova\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* help\r\n");
		}



		[TestMethod]
		public void Run_Help_ShouldShowListOfCommandsNameWhenNoMatchingCommandIsFound()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "the help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("prova", new Action<INodeExecutionContext>(_handler.DoTest), "the other help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("help", new Action<INodeExecutionContext, string>(Target.Help), "helping help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("help");

			//Verify
			SetsAssert.Contains(_mockConsole.Data, "Node.Cs AvailableCommands:\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* test\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* prova\r\n");
			SetsAssert.Contains(_mockConsole.Data, "* help\r\n");
		}

		[TestMethod]
		public void Run_Help_Command_ShouldShowHelpWhenNoMatchingParamIsShown()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("help", new Action<INodeExecutionContext, string>(Target.Help), "helping help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("help test");

			//Verify
			SetsAssert.Contains(_mockConsole.Data, string.Format("Node.Cs Help for command '{0}':\r\n", "test"));
			SetsAssert.Contains(_mockConsole.Data, "test help\r\n");
			CollectionAssert.DoesNotContain(_mockConsole.Data, "helping help\r\n");
		}

		[TestMethod]
		public void Run_Command_ShouldHandleMissingBooleanParametersAddingDefault()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, bool>(_handler.DoTestBoolean), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTestBoolean", _handler.IsAny<INodeExecutionContext>(), false);
		}

		[TestMethod]
		public void Run_Command_ShouldHandleParametersWithWrongValues_ShowingHelp()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, bool>(_handler.DoTestBoolean), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test fuffa");

			//Verify
			Assert.AreEqual(0, _handler.Run.Count);

			SetsAssert.Contains(_mockConsole.Data, string.Format("Node.Cs Help for command '{0}':\r\n", "test"));
		}

		[TestMethod]
		public void Run_NotInteractiveNotExistingCommand_ShouldThrowWithoutShowingHelp()
		{
			//Setup
			const bool interactive = false;
			SetupTarget();
			MissingCommandException result;

			//Act
			ExceptionAssert.Throws(() => Target.Run("test", interactive), out result);

			//Verify
			Assert.IsTrue(result.Message.Contains("test"));
			Assert.AreEqual("", string.Join("", _mockConsole.Data));
		}

		[TestMethod]
		public void Run_Command_ShouldHandleBooleanParameters()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, bool>(_handler.DoTestBoolean), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test true");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTestBoolean", _handler.IsAny<INodeExecutionContext>(), true);
		}

		[TestMethod]
		public void Run_Command_ShouldHandleMissingNumericParametersAddingDefault()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, int>(_handler.DoTestInt), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTestInt", _handler.IsAny<INodeExecutionContext>(), 3);
		}

		[TestMethod]
		public void Run_Command_ShouldHandleNumericParameters()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext, int>(_handler.DoTestInt), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test 33");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTestInt", _handler.IsAny<INodeExecutionContext>(), 33);
		}

		[TestMethod]
		public void Run_Command_ShouldThrowTheCorrectException()
		{
			//Setup
			var expected = new InsufficientExecutionStackException("test");
			InsufficientExecutionStackException result;
			SetupTarget();
			var cd = new CommandDescriptor("test", new Action<INodeExecutionContext>(ctx =>
			{
				throw expected;
			}), "test help");
			Target.RegisterCommand(cd);

			//Act
			ExceptionAssert.Throws(() => Target.Run("test"), out result);

			//Verify
			Assert.AreSame(expected, result);
		}

		[TestMethod]
		public void RegisterCommand_ShouldBeAbleToRegisterAndUnregisterCommands_WithMultipleParam()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test example", new Action<INodeExecutionContext>(_handler.DoTest), "test help");

			//Act
			Target.RegisterCommand(cd);

			//Verify
			Assert.IsTrue(Target.ContainsCommand("test example", new Type[0]));
		}

		[TestMethod]
		public void Run_ShouldExecuteSimpleCommands_WithMultiParams_ChoosingTheLongestAlternative()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test do Stuff", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("test do", new Action<INodeExecutionContext, int>(_handler.DoTestInt), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test do stuff");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTest", _handler.IsAny<INodeExecutionContext>());
		}


		[TestMethod]
		public void Run_ShouldExecuteSimpleCommands_WithMultiParams_ChoosingTheLongestAlternativeDespiteOrder()
		{
			//Setup
			SetupTarget();
			var cd = new CommandDescriptor("test do", new Action<INodeExecutionContext, int>(_handler.DoTestInt), "test help");
			Target.RegisterCommand(cd);
			cd = new CommandDescriptor("test do Stuff", new Action<INodeExecutionContext>(_handler.DoTest), "test help");
			Target.RegisterCommand(cd);

			//Act
			Target.Run("test do stuff");

			//Verify
			Assert.AreEqual(1, _handler.Run.Count);
			_handler.Verify("DoTest", _handler.IsAny<INodeExecutionContext>());
		}



	}
}