using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Test;
using System.IO;

namespace Node.Cs
{
	[TestClass]
	public class BasicNodeCommandsTest : TestBase<BasicNodeCommands, IBasicNodeCommands>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

		#region CS scripts

		[TestMethod]
		public void Echo_ShouldWriteNewLineToConsole()
		{
			//Setup
			SetupTarget();

			//Act
			Target.Echo(null, "message");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("message"), Times.Once);
		}


		[TestMethod]
		public void Run_ShouldThrowANotFoundExceptionIfTheFileDoesNotExists()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			ExceptionAssert.Throws<FileNotFoundException>(() => Target.Run(context, "BasicNodeCommandsTest\\notExisting.cs"));
		}

		[TestMethod]
		public void Run_ShouldLoadCsFileCompileAndRun()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			Target.Run(context, "BasicNodeCommandsTests\\test.cs");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs"), Times.Once);
		}

		[TestMethod]
		public void Run_ShouldLoadCsFileCompileAndRunDespiteDirectorySeparatorCharAndCasing()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			Target.Run(context, "BasicNodeCommandsTests/tEst.cs");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs"), Times.Once);
		}

		[TestMethod]
		public void Run_ShouldLoadCsFileCompileAndRunSpecifyinigFunction()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			Target.Run(context, "BasicNodeCommandsTests\\test.cs", "do");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs::Do"), Times.Once);
		}

		[TestMethod]
		public void Run_ShouldThrowANotFoundExceptionIfTheFunctionDoesNotExists()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			ExceptionAssert.Throws<MissingFunctionException>(() => Target.Run(context, "BasicNodeCommandsTests\\test.cs", "notExisting"));
		}

		[TestMethod]
		public void Run_ShouldReloadFileWhenFileIsChanged()
		{
			//Setup
			const string modPath = "Run_ShouldReloadFileWhenFileIsChanged.cs";

			SetupTarget();
			var context = Object<INodeExecutionContext>();

			var srcPath = Path.Combine(context.CurrentDirectory.Data, "BasicNodeCommandsTests\\test.cs");
			var destPath = Path.Combine(context.CurrentDirectory.Data, modPath);
			var src = File.ReadAllText(srcPath);
			File.WriteAllText(destPath, src);

			//Act
			Target.Run(context, modPath, "do");

			src = src.Replace("Executing Test.cs::Do", "Executing modified Test.cs::Do");
			File.WriteAllText(destPath, src);

			Target.Run(context, modPath, "do");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs::Do"), Times.Once);
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing modified Test.cs::Do"), Times.Once);
		}

		[TestMethod]
		public void Run_ShouldThrowACompilationExceptionWhenNamespaceIsDeclared()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			ExceptionAssert.Throws<CompilationException>(() => Target.Run(context, "BasicNodeCommandsTests\\TestWrongNamespace.cs", "Execute"));
		}


		[TestMethod]
		public void Run_ShouldRunTwiceIfInvokedTwice()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();

			//Act
			Target.Run(context, "BasicNodeCommandsTests\\test.cs", "do");
			Target.Run(context, "BasicNodeCommandsTests\\test.cs", "do");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs::Do"), Times.Exactly(2));
		}

		[TestMethod]
		public void Run_ShouldThrowACompilationExceptionWhenProblemIsFound()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();
			CompilationException resultException = null;

			//Act
			ExceptionAssert.Throws(
				() => Target.Run(context, "BasicNodeCommandsTests\\TestWithCompilationErrors.cs", "Execute"),
				out resultException);

			//Verify
			Assert.IsTrue(resultException.Message.Contains(": 6"));
			Assert.IsTrue(resultException.Message.Contains(": 14"));
			Assert.IsTrue(resultException.Message.Contains(": 19"));
			Assert.IsTrue(resultException.CompiledSource.Contains("this is an error"));
		}



		[TestMethod]
		public void Run_ShouldThrowANotSupportExceptionWhenTheFileExtensionDoesNotMatch()
		{
			//Setup
			SetupTarget();
			var context = Object<INodeExecutionContext>();
			NotSupportedException resultException = null;

			//Act
			ExceptionAssert.Throws(
				() => Target.Run(context, "BasicNodeCommandsTests\\FileWithWrong.extension", "Execute"),
				out resultException);
		}
		#endregion

		#region NCS scripts

		[TestMethod]
		public void Run_ScriptWithEcho_ShouldEcho()
		{
			//Setup
			SetupTarget();
			SetupRunAndEchoCommands();

			var context = Object<INodeExecutionContext>();
			const string path = "Run_ScriptWithEcho_ShouldEcho.ncs";
			var destPath = Path.Combine(context.CurrentDirectory.Data, path);
			File.WriteAllText(destPath, "echo message");

			//Act
			Target.Run(context, path);

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("message"), Times.Once);
		}


		[TestMethod]
		public void Run_ScriptWithRun_ShouldRunTheScript()
		{
			//Setup
			SetupTarget();
			SetupRunAndEchoCommands();

			var context = Object<INodeExecutionContext>();
			const string path = "Run_ScriptWithRun_ShouldRunTheScript.ncs";
			var destPath = Path.Combine(context.CurrentDirectory.Data, path);
			File.WriteAllText(destPath, "run BasicNodeCommandsTests/Test.cs");

			//Act
			Target.Run(context, path);

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs"), Times.Once);
		}


		#endregion

		#region Exit
		[TestMethod]
		public void Exit_ShouldExitWithErrorCodeZero()
		{
			Assert.Inconclusive("Should add the command line parameters.");
			/*
			//Setup
			var asmUri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
			var asmPath = asmUri.Path;
			var baseDir = Path.GetDirectoryName(asmPath);
			Process.Start(Path.Combine(baseDir, "Node.Cs.Exe"));
			SetupTarget();

			//Act
			Target.Echo(null, "message");

			//Verify
			MockOf<INodeConsole>().Verify(a => a.WriteLine("message"), Times.Once);*/
		}
		#endregion

		#region LoadDll

		[TestMethod]
		public void LoadDll_ShouldBeAbleToFindDll()
		{
			//Setup
			SetupTarget();

			var context = Object<INodeExecutionContext>();
			const string path = "BasicDllFor.Test.dll";
			const string name = "BasicDllFor.Test";

			//Act
			Target.LoadDll(context, path);

			//Verify
			var dll = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName.Contains(name));
			Assert.IsNotNull(dll);
		}


		[TestMethod]
		public void LoadDll_ShouldThrowDllNotFoundException()
		{
			//Setup
			SetupTarget();

			var context = Object<INodeExecutionContext>();
			const string path = "NonExisting.dll";

			//Act
			ExceptionAssert.Throws<DllNotFoundException>(() => Target.LoadDll(context, path));
		}
		#endregion

		#region Utility methods

		private void SetupRunAndEchoCommands()
		{
			Container.Register(Component.For<IUiCommandsHandler>().ImplementedBy<UiCommandsHandler>());
			Container.Register(Component.For<ICommandParser>().ImplementedBy<BasicCommandParser>());

			Container.Resolve<IUiCommandsHandler>().RegisterCommand(
				new CommandDescriptor(
					"echo", new Action<INodeExecutionContext, string>(Target.Echo), ""));
			Container.Resolve<IUiCommandsHandler>().RegisterCommand(
				new CommandDescriptor(
					"run", new Action<INodeExecutionContext, string, string>(Target.Run), ""));
		}

		#endregion
	}
}