using System;
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
		public void Parse_ShouldCreateTheCorrectResult()
		{
			var commands = new[]
			{
				Tuple("do","do",""),
				Tuple(" do\r\n","do",""),
				Tuple("\tdo\f","do",""),
				/*Tuple("do test.cs","do","test.cs"),
				Tuple("do multiple parameters","do","multiple#parameters"),
				Tuple("do \"string with double commas\" other","do","\"string with double commas\"#other"),
				Tuple("do \"string with \\\" stuff inside\" other","do","\"string with \\\" stuff inside\"#other"),*/
			};
			RunSeries(
				(item1, item2, item3) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.Parse(item1);

					//Verify
					var pars = string.Join("#", result.Parameters);
					Assert.AreEqual(item2, result.Command);
					Assert.AreEqual(item3, pars);
				},
				commands);
		}
	}
}