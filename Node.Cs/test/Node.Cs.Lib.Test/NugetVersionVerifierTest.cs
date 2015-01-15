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

		[TestMethod]
		public void IsVersionMatching_ShouldThrowExceptionOnInvalidVersions()
		{
			const string testVersion = "1.0.0";
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
					ExceptionAssert.Throws<Exception>(() => Target.IsVersionMatching(testVersion, item1));
				},
				commands);
		}

		[TestMethod]
		public void IsVersionMatching_ShouldHandleTheVariousCombination()
		{
			var commands = new[]
			{
				Tuple("1.0","1.0",true),
				Tuple("1.0","2.0",false),
				Tuple("(,1.0]","0.1",true),
				Tuple("(,1.0]","1.0",true),
				Tuple("(,1.0]","2.0",false),
				Tuple("(,1.0)","1.0",false),
				Tuple("(,1.0)","0.9",true),
				Tuple("(1.0,)","0.9",false),
				Tuple("(1.0,)","1.0",false),
				Tuple("(1.0,)","10.0",true),
				Tuple("[1.0,)","0.9",false),
				Tuple("[1.0,)","1.0",true),
				Tuple("[1.0,)","10.0",true),
				Tuple("[1.0]","1.0",true),
				Tuple("[1.0]","0.9",false),
				Tuple("[1.0]","1.1",false),
				Tuple("(1.0,2.0)","1.0",false),
				Tuple("(1.0,2.0)","2.0",false),
				Tuple("(1.0,2.0)","1.5",true),
				Tuple("[1.0,2.0]","0.9",false),
				Tuple("[1.0,2.0]","1.0",true),
				Tuple("[1.0,2.0]","2.0",true),
				Tuple("[1.0,2.0]","2.1",false),
				Tuple("(1.0,2.0]","1.0",false),
				Tuple("(1.0,2.0]","2.0",true),
				Tuple("[1.0,2.0)","1.0",true),
				Tuple("[1.0,2.0)","2.0",false),
				Tuple("","9999",true),
				Tuple("","0.1",true),
			};
			RunSeries(
				(item1, item2, item3) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.IsVersionMatching(item2, item1);

					//Verify
					Assert.AreEqual(item3, result, "Input: " + item1);
				},
				commands);
		}

		[TestMethod]
		public void IsVersionMatching_ShouldHandleTheVariousCombination_AndViceVersa()
		{
			var commands = new[]
			{
				Tuple("1.0.0-alpha","1.0.0-alpha.1",false),
				Tuple("1.0.0-alpha","1.0.0-alpha",true),
				Tuple("2.0","1.0",false),
				Tuple("2.0.0","1.0.0",false),
				Tuple("1.0","1.0",true),
				Tuple("1.0","2.0",false),
				Tuple("1.0.0-alpha.1","1.0.0-alpha.beta",false),
				Tuple("1.0.0-alpha.beta","1.0.0-beta",false),
				Tuple("1.0.0-beta","1.0.0-beta.2",false),
				Tuple("1.0.0-beta.2","1.0.0-beta.11",false),
				Tuple("1.0.0-beta.11","1.0.0-rc.1",false),
				Tuple("1.0.0-rc.1","1.0.0",false),
				Tuple("1.0.0-rc.1","",true),
				Tuple("1.0.0","",true)
			};
			RunSeries(
				(item1, item2, expected) =>
				{
					//Setup
					SetupTarget();

					//Act
					var result = Target.IsVersionMatching(item1, item2);

					//Verify
					Assert.AreEqual(expected, result, string.Format("Input: '{0}'-'{1}'.", item1, item2));

					if (!result)
					{
						//Act
						var newResult = Target.IsVersionMatching(item2, item1);

						//Verify
						Assert.AreEqual(expected, newResult, string.Format("Input: '{0}'-'{1}'.", item2, item1));
					}
				},
				commands);
		}
	}
}