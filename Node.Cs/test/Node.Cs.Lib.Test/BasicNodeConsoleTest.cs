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