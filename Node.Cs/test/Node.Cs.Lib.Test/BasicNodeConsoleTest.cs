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


using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Test;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Node.Cs
{
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Used only for testing"), TestClass]
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