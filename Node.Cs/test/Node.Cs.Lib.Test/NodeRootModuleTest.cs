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
using Moq;
using Node.Cs.CommandHandlers;
using Node.Cs.Modules;
using Node.Cs.Test;

namespace Node.Cs
{
	[TestClass]
	public class NodeRootModuleTest : TestBase<NodeRootModule>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
		}

		private static readonly string[] _commands =
		{
			"run", 
			"exit", 
			"echo", 
			"help",
			"dll load",
			"nuget load"
		};


		[TestMethod]
		public void VersionAndNameShouldMatch()
		{
			SetupTarget();
			var res = new NodeRootModule(null, null);
			Assert.AreEqual(res.Version, Target.Version);
			Assert.AreEqual(res.Name, Target.Name);
		}

		[TestMethod]
		public void PreInitialize_ShouldRegisterRequiredCommands()
		{
			RunSeries(
				item =>
				{
					//Setup
					CommandDescriptor registeredCommand = null;
					SetupTarget();
					var commandsHandler = MockOf<IUiCommandsHandler>();
					commandsHandler.Setup(a => a.RegisterCommand(It.IsAny<CommandDescriptor>()))
						.Callback((CommandDescriptor cd) => registeredCommand = cd);

					//Act
					Target.Initialize();

					//Verify
					Assert.IsNotNull(registeredCommand);
					Assert.IsNotNull(registeredCommand.CalledFunction);
					Assert.IsFalse(string.IsNullOrWhiteSpace(registeredCommand.ShortHelp));
				},
				_commands);

		}
	}
}