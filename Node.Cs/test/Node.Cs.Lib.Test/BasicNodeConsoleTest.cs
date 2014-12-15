using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class BasicNodeConsoleTest
	{
		private TextWriter _realOut;
		private ConsoleRedirector _redirector;
		private TextReader _realIn;
		public BasicNodeConsole Target { get; set; }

		[TestInitialize]
		public void TestInitialize()
		{
			_realOut = Console.Out;
			_realIn = Console.In;
			_redirector = new ConsoleRedirector();
			Console.SetOut(_redirector);
			Target = new BasicNodeConsole();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			Console.SetOut(_realOut);
			Console.SetIn(_realIn);
		}

		[TestMethod]
		public void ConsoleWrite_ShouldWrite()
		{
			Target.Write("Pre");
			Target.WriteLine("Test-{0}", "Value");
			CollectionAssert.Contains(_redirector.Data, "PreTest-Value\r\n");
		}


		[TestMethod]
		public void ConsoleWriteLine_ShouldWrite()
		{
			Target.WriteLine("Pre");
			Target.WriteLine("Test-{0}", "Value");
			CollectionAssert.Contains(_redirector.Data, "Pre\r\n");
			CollectionAssert.Contains(_redirector.Data, "Test-Value\r\n");
		}


		[TestMethod]
		public void ConsoleReadLine_ShouldRead()
		{
			const string expected = "valueToRead";
			using (var sr = new StringReader(expected))
			{
				Console.SetIn(sr);

				var result = Target.ReadLine();

				Assert.AreEqual(expected, result);
			}
		}


	}
}