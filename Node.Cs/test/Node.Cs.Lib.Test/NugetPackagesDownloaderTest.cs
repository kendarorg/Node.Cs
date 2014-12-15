using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Consoles;
using Node.Cs.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public void DownloadPackage_ShouldExtractTheCorrectDlls()
        {
            //Setup 
            SetupTarget();

            const string packageName = "GenericHelpers";
            const string version = "1.0.5";
            const bool allowPreRelease = true;
            var address = string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", packageName, version);

            var context = Object<INodeExecutionContext>();
            var client = MockOf<IWebClient>();
            var dllContent = File.ReadAllBytes(Path.Combine(context.CurrentDirectory.Data, "Nuget\\GenericHelpers.1.0.5.nupkg"));
            client.Setup(a => a.DownloadData(address))
                .Returns(dllContent);

            //Act
            var result = Target.DownloadPackage("net45",packageName, version, allowPreRelease)
                .ToArray();

            //Verify
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("GenericHelpers.dll",result[0].Name);

            var asm = Assembly.ReflectionOnlyLoad(result[0].Data);
            Assert.IsNotNull(asm);
            Assert.AreEqual("GenericHelpers", asm.GetName().Name);
        }


        [TestMethod]
        public void DownloadPackage_ShouldFallbackToNoFramework()
        {
            Assert.Inconclusive("DownloadPackage_ShouldFallbackToNoFramework");
        }
    }
}
