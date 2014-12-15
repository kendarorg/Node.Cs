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


using System.Reflection;
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
				Tuple("do multiple parameters","do",new []{"multiple","parameters"}
		)
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
				Tuple("do \"string with double commas\" other","do",new []{"\"string with double commas\"","other"}),
				Tuple("do \"has single'inside\" other","do",new []{"\"has single'inside\"","other"}),
				Tuple("do \"string with \\\" stuff inside\" other","do",new []{"\"string with \\\" stuff inside\"","other"})
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
				Tuple("do 'string with double commas' other","do",new []{"'string with double commas'","other"}),
				Tuple("do 'has single\"inside' other","do",new []{"'has single\"inside'","other"}),
				Tuple("do 'string with \\' stuff inside' other","do",new []{"'string with \\' stuff inside'","other"})
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