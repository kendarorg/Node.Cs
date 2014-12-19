using System;
using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Nugets;
using Node.Cs.Test;
using Node.Cs.Utils;

namespace Node.Cs
{
	[TestClass]
	public class NugetVersionVerifierTest : TestBase<NugetVersionVerifier, INugetVersionVerifier>
	{
		/*
		 * Should support
1.0  = x => 1.0
(,1.0]  = x <= 1.0
(,1.0)  = x < 1.0
[1.0] = x == 1.0
(1.0,) =  x>1.0
(1.0,2.0) = 1.0 < x < 2.0
[1.0,2.0] = 1.0 <= x <= 2.0
empty = latest version.
(1.0) = invalid*/

		//http://www.nuget.org/api/v2/Packages()?$orderby=LastUpdated desc&$select=Id,Tags,Title,Description,ProjectUrl,GalleryDetailsUrl,Version,LastUpdated
		///v2/Packages()?$filter=(Id eq
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

		[TestMethod]
		public void BuildODataQuery_ShouldThrowExceptionOnInvalidVersions()
		{
			const string testPackage = "TestPackage";
			var commands = new[]
			{
				"[1.0)",
				"(1.0]",
				"(1.0)",
				"(1.0,2.0",
				"[1.0,2.0",
				"1.0,2.0)",
				"1.0,2.0]",
				"1.0,2.0,3.0"
			};
			RunSeries(
				item1 =>
				{
					//Setup
					SetupTarget();

					//Act
					ExceptionAssert.Throws<Exception>(() => Target.BuildODataQuery(testPackage, item1));
				},
				commands);
		}

		[TestMethod]
		public void BuildODataQuery_ShouldHandleTheVariousCombination()
		{
			const string testPackage = "TestPackage";
			var commands = new[]
			{
				Tuple("1.0","(Id eq 'TestPackage' and Version ge '1.0')"),
				Tuple("(,1.0]","(Id eq 'TestPackage' and Version le '1.0')"),
				Tuple("(,1.0)","(Id eq 'TestPackage' and Version lt '1.0')"),
				Tuple("(1.0,)","(Id eq 'TestPackage' and Version gt '1.0')"),
				Tuple("[1.0,)","(Id eq 'TestPackage' and Version ge '1.0')"),
				Tuple("[1.0]","(Id eq 'TestPackage' and Version eq '1.0')"),
				Tuple("(1.0,2.0)","(Id eq 'TestPackage' and Version gt '1.0' and Version lt '2.0')"),
				Tuple("[1.0,2.0]","(Id eq 'TestPackage' and Version ge '1.0' and Version le '2.0')"),
				Tuple("(1.0,2.0]","(Id eq 'TestPackage' and Version gt '1.0' and Version le '2.0')"),
				Tuple("[1.0,2.0)","(Id eq 'TestPackage' and Version ge '1.0' and Version lt '2.0')"),
				Tuple("","(Id eq 'TestPackage')")
			};
			RunSeries(
				(item1, item2) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.BuildODataQuery(testPackage, item1);

					//Verify
					Assert.AreEqual(item2, result, "Input: " + item1);
				},
				commands);
		}



		[TestMethod]
		public void Compare_ShouldHandleTheVariousCombination_AndViceVersa()
		{
			var commands = new[]
			{
				Tuple("1.0.0-alpha","1.0.0-alpha.1",-1),
				Tuple("2.0","1.0",1),
				Tuple("1.0","1.0",0),
				Tuple("1.0","2.0",-1),
				Tuple("1.0.0-alpha.1","1.0.0-alpha.beta",-1),
				Tuple("1.0.0-alpha.beta","1.0.0-beta",-1),
				Tuple("1.0.0-beta","1.0.0-beta.2",-1),
				Tuple("1.0.0-beta.2","1.0.0-beta.11",-1),
				Tuple("1.0.0-beta.11","1.0.0-rc.1",-1),
				Tuple("1.0.0-rc.1","1.0.0",-1),
			};
			RunSeries(
				(item1, item2, expected) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.Compare(item1, item2);

					//Verify
					Assert.AreEqual(expected, result, string.Format("Input: '{0}'-'{1}'.", item1, item2));

					if (result != 0)
					{
						var newExpected = expected == 1 ? -1 : 1;

						//Act
						var newResult = Target.Compare(item2, item1);

						//Verify
						Assert.AreEqual(newExpected, newResult, string.Format("Input: '{0}'-'{1}'.", item2, item1));
					}
				},
				commands);
		}
	}
}