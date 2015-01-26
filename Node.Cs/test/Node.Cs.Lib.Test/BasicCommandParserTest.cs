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


using Kendar.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class BasicCommandParserTest : TestBase<BasicCommandParser, ICommandParser>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			InitializeMock<IUiCommandsHandler>();
		}

		[TestMethod]
		public void Parse_ShouldCorrectlyWork_WithStandardPatterns()
		{
			var commands = new[]
			{
				Tuple("do","do",new string[0]),
				Tuple(" do\r\n","do",new string[0]),
				Tuple("\tdo\f","do",new string[0]),
				Tuple("do test.cs","do",new []{"test.cs"}),
				Tuple("do multiple parameters","do",new []{"multiple","parameters"})
			};
			RunSeries(
				(item1, item2, item3) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.Parse(item1);

					//Verify
					Assert.AreEqual(item2, result.Command);
					SetsAssert.AreEqual(item3, result.Parameters);
				},
				commands);
		}

		[TestMethod]
		public void Parse_ShouldCorrectlyWork_WithStringPatternsAndDoubleCommas()
		{
			var commands = new[]
			{
				Tuple("do \"string with double commas\" other","do",new []{"string with double commas","other"}),
				Tuple("do \"has single'inside\" other","do",new []{"has single'inside","other"}),
				Tuple("do \"string with \\\" stuff inside\" other","do",new []{"string with \\\" stuff inside","other"})
			};
			RunSeries(
				(item1, item2, item3) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.Parse(item1);

					//Verify
					Assert.AreEqual(item2, result.Command);
					SetsAssert.AreEqual(item3, result.Parameters);
				},
				commands);
		}


		[TestMethod]
		public void Parse_ShouldCorrectlyWork_WithStringPatternsAndSingleCommas()
		{
			var commands = new[]
			{
				Tuple("do 'string with double commas' other","do",new []{"string with double commas","other"}),
				Tuple("do 'has single\"inside' other","do",new []{"has single\"inside","other"}),
				Tuple("do 'string with \\' stuff inside' other","do",new []{"string with \\' stuff inside","other"})
			};
			RunSeries(
				(item1, item2, item3) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.Parse(item1);

					//Verify
					Assert.AreEqual(item2, result.Command);
					SetsAssert.AreEqual(item3, result.Parameters);
				},
				commands);
		}
	}
}