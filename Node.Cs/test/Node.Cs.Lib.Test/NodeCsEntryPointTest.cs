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


using GenericHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class NodeCsEntryPointTest : TestBase<NodeCsEntryPointForTest>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			InitializeMock<IUiCommandsHandler>();
		}

		[TestMethod]
		public void Run_AsService_ShouldThrowNotImplemented()
		{
			//Setup
			SetupTarget(new NodeCsEntryPointForTest(new CommandLineParser(new string[0], ""), Container, NodeCsEntryPointForTest.Once));

			//Act
			ExceptionAssert.Throws<NotImplementedException>(()=>Target.Run(true));
		}

		[TestMethod]
		public void Run_ShouldAskForUserInput()
		{
			//Setup
			var console = MockOf<INodeConsole>();
			SetupTarget(new NodeCsEntryPointForTest(new CommandLineParser(new string[0], ""), Container, NodeCsEntryPointForTest.Once));

			//Act
			Target.Run();

			//Verify
			console.Verify(a => a.ReadLine(), Times.Once);
		}

		[TestMethod]
		public void Constructor_ShouldInitializeExecutionContext()
		{
			//Setup
			var clp = new CommandLineParser(new[]
			{
				"-par1",
				"-par2"
			}, "");
			SetupTarget(new NodeCsEntryPointForTest(clp, Container, NodeCsEntryPointForTest.Once));

			//Act
			Target.Run();

			//Verify
			var context = Container.Resolve<INodeExecutionContext>();

			Assert.IsNotNull(context);

			Assert.IsTrue(clp.Has("par1"));
			Assert.IsTrue(clp.Has("par2"));

			Assert.AreEqual(Path.GetFileName(context.NodeCsExecutablePath), "node.cs.lib.dll", true);
		}

		[TestMethod]
		public void Run_ShouldRunCommands()
		{
			//Setup
			const string command = "do stuff";
			var console = MockOf<INodeConsole>();
			console.Setup(c => c.ReadLine()).Returns(command);
			SetupTarget(new NodeCsEntryPointForTest(new CommandLineParser(new string[0], ""), Container, NodeCsEntryPointForTest.Once));

			//Act
			Target.Run();

			//Verify
			var commandsHandler = MockOf<IUiCommandsHandler>();
			commandsHandler.Verify(a => a.Run(It.IsAny<string>()), Times.Once);
			commandsHandler.Verify(a => a.Run(command), Times.Once);
		}


		[TestMethod]
		public void Run_CommandsWithExceptions_ShouldNotInterruptExecutionButLog()
		{
			//Setup
			const string command = "do stuff";
			const string exceptionMessage = "exception message";
			var commandHandler = MockOf<IUiCommandsHandler>();
			var console = MockOf<INodeConsole>();
			var obtainedLines = new List<string>();

			var expected = new Exception(exceptionMessage);

			commandHandler.Setup(a => a.Run(command)).Throws(expected);
			console.Setup(c => c.ReadLine()).Returns(command);
			console.Setup(c => c.WriteLine(It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback<string, object[]>((s, p) => obtainedLines.Add(string.Format(s, p)));

			var target = new NodeCsEntryPointForTest(new CommandLineParser(new string[0], ""), Container, NodeCsEntryPointForTest.Once);

			//Act
			target.Run();

			//Verify
			console.Verify(a => a.WriteLine(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
			Assert.AreEqual(1, obtainedLines.Count);
			var line = obtainedLines.First();
			Assert.IsTrue(line.Contains(exceptionMessage));
			Assert.IsTrue(line.Contains(command));
		}

		[Ignore]
		[TestMethod]
		public void Run_ShouldBeAbleToRunAsService()
		{
			//Setup
			const bool runAsService = true;
			InitializeMock<INodeConsole>();
			SetupTarget(new NodeCsEntryPointForTest(new CommandLineParser(new string[0], ""), Container, NodeCsEntryPointForTest.Once));

			//Act
			Target.Run(runAsService);

			//Verify
			throw new NotImplementedException();
		}
	}
}
