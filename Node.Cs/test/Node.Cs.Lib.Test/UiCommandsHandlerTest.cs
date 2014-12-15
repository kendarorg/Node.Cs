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


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Mocks;
using Node.Cs.Test;
using Castle.MicroKernel.Registration;

namespace Node.Cs
{
	[TestClass]
	public class UiCommandsHandlerTest : TestBase<UiCommandsHandler, IUiCommandsHandler>
	{
		private Mock<IUiCommandsHandlerTestMock> _handlerMock;
		private IUiCommandsHandlerTestMock _handler;
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

			_handlerMock = MockOf<IUiCommandsHandlerTestMock>();
			_handler = Container.Resolve<IUiCommandsHandlerTestMock>();
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
			DuplicateCommandException result;

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
			_handlerMock.Verify(a => a.DoTest(It.IsAny<INodeExecutionContext>()), Times.Once);
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

	}
}