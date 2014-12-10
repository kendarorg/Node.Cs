using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.CommandHandlers;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class NodeRootModuleTest : TestBase<NodeRootModule>
	{
		private static readonly string[] _commands =
		{
			"run", 
			"exit", 
			"echo", 
			"help"
		};

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
		}

		[TestMethod]
		public void PreInitialize_ShouldRegisterRequiredCommands()
		{
			//Setup
			var registeredCommands = new List<CommandDescriptor>();
			SetupTarget();
			var commandsHandler = MockOf<IUiCommandsHandler>();
			commandsHandler.Setup(a => a.RegisterCommand(It.IsAny<CommandDescriptor>()))
				.Callback((CommandDescriptor cd) => registeredCommands.Add(cd));

			//Act
			Target.PreInitialize();

			//Verify
			Assert.AreEqual(4, registeredCommands.Count);

			RunSeries(
				(item) =>
				{
					var position = SetsAssert.Contains<string, CommandDescriptor>(registeredCommands, item, (a) => a.CommandId);
					var selected = registeredCommands[position];
					Assert.IsNotNull(selected);
					Assert.IsNotNull(selected.CalledFunction);
					Assert.IsFalse(string.IsNullOrWhiteSpace(selected.ShortHelp));
				},
				_commands);

		}
	}
}