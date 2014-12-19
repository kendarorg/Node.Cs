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


using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Nugets;
using Node.Cs.Test;
using System;
using System.IO;
using System.Linq;
using Node.Cs.Utils;

namespace Node.Cs
{
	[TestClass]
	public class NugetPackagesDownloaderTest : TestBase<NugetPackagesDownloader, INugetPackagesDownloader>
	{
		private const string NUGET_ORG =
			"https://www.nuget.org/api/v2/Packages()?$orderby=LastUpdated desc&$filter={0}";

		private const string NUGET_ORG_FILE =
			"https://www.nuget.org/api/v2/packge/{0}/{1}";
		private const string EXTRA = "Packages()?$orderby=LastUpdated desc&$filter={0}";
		private string BuildNugetPath(string id,string version)
		{
			return id + "/" + version;
		}
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			InitializeMock<INodeConsole>();
			InitializeMock<INugetArchiveList>();
			var versionVerifier = MockOf<INugetVersionVerifier>();
			versionVerifier.Setup(a => a.BuildODataQuery(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string id, string version) => BuildNugetPath(id ,version));


			Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
			var path = Path.Combine(Container.Resolve<INodeExecutionContext>().NodeCsPackagesDirectory, "BasicNugetPackageFor.Test.dll");
			if (File.Exists(path))
			{
				File.Delete(path);
			}

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

				var address = server.CreateAddress();
				var expectedCall = address+ String.Format(EXTRA,BuildNugetPath(packageName, version));
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
					ll.AddSearchPath(context.NodeCsPackagesDirectory);
					ll.AddSearchPath(context.CurrentDirectory.Data);
					ll.AddSearchPath(context.NodeCsExtraBinDirectory);

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
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));
			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version);

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\BasicNugetPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
					.Returns(dllContent);

			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
					.FirstOrDefault();

			//Verify
			client.Verify(a=>a.DownloadData(addressDll));
			Assert.IsNotNull(result);

			Assert.AreEqual("BasicNugetPackageFor.Test.dll", result.Name);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.NodeCsPackagesDirectory);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory);

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
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var dllContent =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NoFrameworkPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
				.Returns(dllContent);
			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version); 

			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
				.FirstOrDefault();

			//Verify
			client.Verify(a => a.DownloadData(addressDll));
			Assert.IsNotNull(result);

			Assert.AreEqual("NoFrameworkPackageFor.Test.dll", result.Name);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.NodeCsPackagesDirectory);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory);

				ll.LoadDll(result.Data);

				Assert.IsTrue(ll.DllLoaded);
				Assert.AreEqual("NoFrameworkPackageFor.Test", ll.Name.Name);
				Assert.AreEqual("40", ll.FrameworkVersion);
			}
		}

		[TestMethod]
		public void DownloadPackage_ShouldThrowExceptionIfNoXmlFound()
		{
			/*//Setup 
			SetupTarget();

			const string packageName = "NoFrameworkPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var client = MockOf<IWebClient>();
			client.Setup(a => a.DownloadData(It.IsAny<string>())).Returns(new byte[0]);

			//Act
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			ExceptionAssert.Throws<NugetDownloadException>(() => Target.DownloadPackage("net45", packageName, version, allowPreRelease).ToArray());*/
			Assert.Inconclusive();
		}

		[TestMethod]
		public void DownloadPackage_shouldThrowExceptionWhenNotFindingTheDll()
		{
			Assert.Inconclusive();
		}


		[TestMethod]
		public void DownloadPackage_shouldConsider()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void DownloadPackage_ShouldThrowTheException()
		{
			//Setup 
			SetupTarget();

			const string packageName = "NoFrameworkPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;

			var client = MockOf<IWebClient>();
			client.Setup(a => a.DownloadData(It.IsAny<string>())).Throws(new OutOfMemoryException());

			//Act
			ExceptionAssert.Throws<OutOfMemoryException>(
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				() => Target.DownloadPackage("net45", packageName, version, allowPreRelease).ToArray());
		}

		[TestMethod]
		public void DownloadPackage_ShouldDownloadDependenciesToo()
		{
			//Setup 
			SetupTarget();

			const string packageName1 = "NugetPackageWithDependencies.Test";
			const string version1 = "1.0.0";
			const bool allowPreRelease1 = true;
			var address1 = string.Format(NUGET_ORG, BuildNugetPath(packageName1, version1));
			var addressDll1 = string.Format(NUGET_ORG_FILE, packageName1, version1); 


			const string packageName2 = "NoFrameworkPackageFor.Test";
			const string version2 = "1.0.0";
			var address2 = string.Format(NUGET_ORG, BuildNugetPath(packageName2, version2));
			var addressDll2 = string.Format(NUGET_ORG_FILE, packageName2, version2);
			

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();


			var dllContent1 =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NugetPackageWithDependencies.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address1))
				.Returns(dllContent1);

			var dllContent2 =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NoFrameworkPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address2))
				.Returns(dllContent2);

			//Act
			var results = Target.DownloadPackage("net45", packageName1, version1, allowPreRelease1).ToArray();

			//Verify
			client.Verify(a => a.DownloadData(addressDll1));
			client.Verify(a => a.DownloadData(addressDll2));

			Assert.AreEqual(2, results.Length);

			var result = results.FirstOrDefault(a => a.Name == "NoFrameworkPackageFor.Test.dll");
			Assert.IsNotNull(result);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.NodeCsPackagesDirectory);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory);

				ll.LoadDll(result.Data);

				Assert.IsTrue(ll.DllLoaded);
				Assert.AreEqual("NoFrameworkPackageFor.Test", ll.Name.Name);
				Assert.AreEqual("40", ll.FrameworkVersion);
			}

			result = results.FirstOrDefault(a => a.Name == "NugetPackageWithDependencies.Test.dll");
			Assert.IsNotNull(result);
			using (var ll = new AssemblyVerifier())
			{
				ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
				ll.AddSearchPath(context.TempPath);
				ll.AddSearchPath(context.NodeCsPackagesDirectory);
				ll.AddSearchPath(context.CurrentDirectory.Data);
				ll.AddSearchPath(context.NodeCsExtraBinDirectory);

				ll.LoadDll(result.Data);

				Assert.IsTrue(ll.DllLoaded);
				Assert.AreEqual("NugetPackageWithDependencies.Test", ll.Name.Name);
				Assert.AreEqual("45", ll.FrameworkVersion);
			}
		}


		[TestMethod]
		public void DownloadPackage_ShouldHandleCircularReferences()
		{
			//Setup 
			SetupTarget();

			const string packageName = "NugetCircularReference.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));

			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version);
			

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();


			var dllContent1 =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NugetCircularReference.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
				.Returns(dllContent1);
			var count = 0;

			//Act
			client.Verify(a => a.DownloadData(addressDll));

			foreach (var result in Target.DownloadPackage("net45", packageName, version, allowPreRelease))
			{
				Assert.IsTrue(count <= 1);
				//Verify
				Assert.AreEqual("NugetCircularReference.Test.dll", result.Name);
				Assert.IsNotNull(result);
				using (var ll = new AssemblyVerifier())
				{
					ll.AddSearchPath(Path.GetDirectoryName(context.NodeCsExecutablePath));
					ll.AddSearchPath(context.TempPath);
					ll.AddSearchPath(context.NodeCsPackagesDirectory);
					ll.AddSearchPath(context.CurrentDirectory.Data);
					ll.AddSearchPath(context.NodeCsExtraBinDirectory);

					ll.LoadDll(result.Data);

					Assert.IsTrue(ll.DllLoaded);
					Assert.AreEqual("NugetCircularReference.Test", ll.Name.Name);
					Assert.AreEqual("45", ll.FrameworkVersion);
				}
				count++;
			}
		}


		[TestMethod]
		public void DownloadPackage_ShouldThrowIfNoSuitableFrameworkIsFound()
		{
			//Setup 
			SetupTarget();

			const string packageName = "NugetWithoutSuitableFramework.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var dllContent =
				File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\NugetWithoutSuitableFramework.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
				.Returns(dllContent);
			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version);
			

			//Act
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			client.Verify(a => a.DownloadData(addressDll));
			ExceptionAssert.Throws<DllNotFoundException>(() =>
				Target.DownloadPackage("net45", packageName, version, allowPreRelease).ToArray());
		}

		[TestMethod]
		public void DownloadPackage_ShouldStoreThePackageDataVerifyingItIsNotPresent()
		{
			//Setup 
			SetupTarget();

			const string packageName = "BasicNugetPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));

			var context = Object<INodeExecutionContext>();
			var client = MockOf<IWebClient>();
			var archive = MockOf<INugetArchiveList>();

			var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\BasicNugetPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
					.Returns(dllContent);
			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version);


			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
					.ToArray().FirstOrDefault();

			//Verify
			client.Verify(a => a.DownloadData(addressDll));
			Assert.IsNotNull(result);
			archive.Verify(a => a.Add(packageName, version, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<NugetPackageDependency>>()), Times.Once);
			archive.Verify(a => a.Check(packageName, version), Times.Once);
			client.Verify(a => a.DownloadData(It.IsAny<string>()), Times.Once);
		}

		[TestMethod]
		public void DownloadPackage_ShouldNotDownloadAnythingButLoadDataIfCheckFindsSomething()
		{
			//Setup 
			SetupTarget();
			var context = Object<INodeExecutionContext>();
			var path = Path.Combine(context.NodeCsPackagesDirectory, "BasicNugetPackageFor.Test.dll");
			File.WriteAllText(path, "thisisadll");

			const string packageName = "BasicNugetPackageFor.Test";
			const string version = "1.0.0";
			const bool allowPreRelease = true;
			var address = string.Format(NUGET_ORG, BuildNugetPath(packageName, version));
			var addressDll = string.Format(NUGET_ORG_FILE, packageName, version);

			var client = MockOf<IWebClient>();
			var archive = MockOf<INugetArchiveList>();
			archive.Setup(a => a.Check(packageName, version)).Returns(true);
			archive.Setup(a => a.Get(packageName, version))
				.Returns(new NugetPackage(packageName, version, new[] { "BasicNugetPackageFor.Test.dll" },new NugetPackageDependency[]{}));

			var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\BasicNugetPackageFor.Test.1.0.0.nupkg"));
			client.Setup(a => a.DownloadData(address))
					.Returns(dllContent);


			//Act
			var result = Target.DownloadPackage("net45", packageName, version, allowPreRelease)
					.FirstOrDefault();

			//Verify
			client.Verify(a => a.DownloadData(addressDll));
			archive.Verify(a => a.Check(packageName, version), Times.Once);
			archive.Verify(a => a.Get(packageName, version), Times.Once);
			archive.Verify(a => a.Add(packageName, version, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<NugetPackageDependency>>()), Times.Never);
			client.Verify(a => a.DownloadData(It.IsAny<string>()), Times.Never);
			Assert.IsNotNull(result);
		}
	}
}
