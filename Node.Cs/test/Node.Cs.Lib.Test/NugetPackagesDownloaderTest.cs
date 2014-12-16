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


using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Test;
using System;
using System.IO;
using System.Linq;

namespace Node.Cs
{
	[TestClass]
	public class NugetPackagesDownloaderTest : TestBase<NugetPackagesDownloader, INugetPackagesDownloader>
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
		}

		[TestMethod]
		public void DownloadPackage_ShouldFallbackBetweenServers()
		{
			Container.Register(Component.For<IWebClient>().ImplementedBy<BaseWebClient>());
			SetupTarget();
			const string packageName = "BasicNugetPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			const int port = 15896;
			string prefix = Guid.NewGuid().ToString("N");
			var context = Object<INodeExecutionContext>();

			using (var server = new FakeHttpServer(port, prefix))
			{
				server.Start();

				var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\BasicNugetPackageFor.Test.1.0.0.nupkg"));
				
				var address = server.CreateAddress("{0}/{1}");
				var expectedCall = string.Format(address, packageName, version);
				server.Responses.Add(expectedCall, (c, s) => s.Respond(c, dllContent));
				Target.AddPackageSource(address);
				var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
					.FirstOrDefault();

				//Verify
				Assert.IsNotNull(result);
				Assert.AreEqual("BasicNugetPackageFor.Test.dll", result.Name);

				using (var ll = new AssemblyVerifier())
				{
					ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
					ll.AddSearchPath(context.TempPath);
					ll.AddSearchPath(context.CurrentDirectory.Data);
					ll.AddSearchPath(context.NodeCsExtraBinDirectory.Data);

					ll.LoadDll(result.Data);

					Assert.IsTrue(ll.DllLoaded);
					Assert.AreEqual("BasicNugetPackageFor.Test", ll.Name.Name);
					Assert.AreEqual("45", ll.FrameworkVersion);
				}
				
			}
		}

		[TestMethod]
		public void DownloadPackage_ShouldExtractTheCorrectDlls()
		{
			//Setup 
			SetupTarget();

			const string packageName = "BasicNugetPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", packageName, version);

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\BasicNugetPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
					.Returns(dllContent);

			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
					.FirstOrDefault();

			//Verify
			Assert.IsNotNull( result);

			Assert.AreEqual("BasicNugetPackageFor.Test.dll", result.Name);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory.Data);

				ll.LoadDll(result.Data);

				Assert.IsTrue(ll.DllLoaded);
				Assert.AreEqual("BasicNugetPackageFor.Test", ll.Name.Name);
				Assert.AreEqual("45", ll.FrameworkVersion);
			}
		}


		[TestMethod]
		public void DownloadPackage_ShouldBeAbleToSelectMinorFrameworkVersions()
		{
			//Setup 
			SetupTarget();

			const string packageName = "NoFrameworkPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", packageName, version);

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var dllContent =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NoFrameworkPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
				.Returns(dllContent);

			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
				.FirstOrDefault();

			//Verify
			Assert.IsNotNull(result);

			Assert.AreEqual("NoFrameworkPackageFor.Test.dll", result.Name);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory.Data);

				ll.LoadDll(result.Data);

				Assert.IsTrue(ll.DllLoaded);
				Assert.AreEqual("NoFrameworkPackageFor.Test", ll.Name.Name);
				Assert.AreEqual("40", ll.FrameworkVersion);
			}
		}

		[TestMethod]
		public void DownloadPackage_ShouldThrowExceptionIfNoPackageFound()
		{
			//Setup 
			SetupTarget();

			const string packageName = "NoFrameworkPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", packageName, version);

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			client.Setup(a => a.DownloadData(It.IsAny<string>())).Returns(new byte[0]);
			
			//Act
			ExceptionAssert.Throws<NugetDownloadException>(()=>Target.DownloadPackage("net45", packageName, version, allowPreRelease));
		}
	}
}
