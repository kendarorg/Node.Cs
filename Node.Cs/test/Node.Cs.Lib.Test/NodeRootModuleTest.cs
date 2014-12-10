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
			RunSeries(
				(item) =>
				{
					//Setup
					CommandDescriptor registeredCommand = null;
					SetupTarget();
					var commandsHandler = MockOf<IUiCommandsHandler>();
					commandsHandler.Setup(a => a.RegisterCommand(It.IsAny<CommandDescriptor>()))
						.Callback((CommandDescriptor cd) => registeredCommand = cd);

					//Act
					Target.PreInitialize();

					//Verify
					Assert.IsNotNull(registeredCommand);
					Assert.IsNotNull(registeredCommand.CalledFunction);
					Assert.IsFalse(string.IsNullOrWhiteSpace(registeredCommand.ShortHelp));
				},
				_commands);

		}
	}
}