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


using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoroutinesLib;
using CoroutinesLib.Shared;
using CoroutinesLib.TestHelpers;
using GenericHelpers;
using Http;
using Http.Contexts;
using Http.Renderer.Razor.Integration;
using Http.Routing;
using Http.Shared.Controllers;
using Http.Shared.PathProviders;
using Http.Shared.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.TestHelpers;
using NodeCs.Shared;

namespace HttpRendererRazorTest
{
	namespace Http.Test
	{
		[TestClass]
		public class RazorTemplateGeneratorTest
		{
			private string GetResult(IEnumerable<ICoroutineResult> res)
			{
				var ms = new MemoryStream();
				foreach (var item in res)
				{
					var bytes = item.Result as byte[];
					if (bytes != null)
					{
						ms.Write(bytes,0,bytes.Length);
					}
				}
				return Encoding.UTF8.GetString(ms.ToArray());
			}
			[TestMethod]
			public void ItShouldBePossibleToCreateSimpleTemplateWithNoModel()
			{
				ServiceLocator.Locator.Register<IRoutingHandler>(new RoutingService());
				var sourceText = ResourceContentLoader.LoadText("simpleTemplate.cshtml");
				var generator = new RazorTemplateGenerator();
				generator.RegisterTemplate(sourceText, "simpleTemplate");
				generator.CompileTemplates();
				var result = GetResult(generator.GenerateOutputString(null, "simpleTemplate", null, new ModelStateDictionary(), new ExpandoObject()));

				var year = DateTime.UtcNow.Year;
				Assert.IsTrue(result.Contains("Hello World"));
				Assert.IsTrue(result.Contains(year.ToString()));
			}

			[TestMethod]
			public void ItShouldBePossibleToCreateSimpleTemplateWithStringModel()
			{
				ServiceLocator.Locator.Register<IRoutingHandler>(new RoutingService());
				var sourceText = ResourceContentLoader.LoadText("simpleTemplateString.cshtml");
				var generator = new RazorTemplateGenerator();
				generator.RegisterTemplate(sourceText, "simpleTemplateString");
				generator.CompileTemplates();
				var model = "This is the model";
				var result = GetResult(generator.GenerateOutputString(model, "simpleTemplateString", null, new ModelStateDictionary(), new ExpandoObject()));

				var year = DateTime.UtcNow.Year;
				Assert.IsTrue(result.Contains("Hello World"));
				Assert.IsTrue(result.Contains(year.ToString()));
				Assert.IsTrue(result.Contains(model));
			}

			[TestMethod]
			public void ItShouldBePossibleToCreateSimpleTemplateWithGenericModel()
			{
				ServiceLocator.Locator.Register<IRoutingHandler>(new RoutingService());
				var sourceText = ResourceContentLoader.LoadText("simpleTemplateGeneric.cshtml");
				var generator = new RazorTemplateGenerator();
				generator.RegisterTemplate(sourceText, "simpleTemplateGeneric");
				generator.CompileTemplates();
				var model = new List<string>
				{
					"First",
					"Second"
				};
				var result = GetResult(generator.GenerateOutputString(model, "simpleTemplateGeneric", null, new ModelStateDictionary(), new ExpandoObject()));

				var year = DateTime.UtcNow.Year;
				Assert.IsTrue(result.Contains("Hello World"));
				Assert.IsTrue(result.Contains(year.ToString()));
				Assert.IsTrue(result.Contains(model[0]));
				Assert.IsTrue(result.Contains(model[1]));
			}


			[TestMethod]
			public void ItShouldBePossibleToCreateTemplateWithSection()
			{
				ServiceLocator.Locator.Register<IRoutingHandler>(new RoutingService());
				var sourceText = ResourceContentLoader.LoadText("section.cshtml");
				var generator = new RazorTemplateGenerator();
				generator.RegisterTemplate(sourceText, "simpleTemplateGeneric");
				generator.CompileTemplates();

				var result = GetResult(generator.GenerateOutputString(null, "simpleTemplateGeneric", null, new ModelStateDictionary(), new ExpandoObject()));

				var year = DateTime.UtcNow.Year;
				Assert.IsTrue(result.Contains("Hello World"));
				Assert.IsTrue(result.Contains("C:\\test"));
			}


			[TestMethod]
			public void ItShouldBePossibleToCreateTemplateWithRenderPage()
			{
				RunnerFactory.Initialize();
				var runner = RunnerFactory.Create();

				ServiceLocator.Locator.Register<IRoutingHandler>(new RoutingService());
				var httpModule = ServiceLocator.Locator.Resolve<HttpModule>();
				var resPathProvider = new ResourcesPathProvider(Assembly.GetExecutingAssembly());
				resPathProvider.RegisterPath("renderPage.cshtml", "renderPage.cshtml");
				resPathProvider.RegisterPath("renderPageSub.cshtml", "renderPageSub.cshtml");

				httpModule.RegisterPathProvider(resPathProvider);

				//var assemblyLocation = Assembly.GetExecutingAssembly().Location;
				//NodeMainInitializer.InitializeFakeMain(assemblyLocation, runner);

				runner.Start();
				var generator = new RazorTemplateGenerator();
				var sourceText = ResourceContentLoader.LoadText("renderPage.cshtml");
				generator.RegisterTemplate(sourceText, "renderPage");
				sourceText = ResourceContentLoader.LoadText("renderPageSub.cshtml");
				generator.RegisterTemplate(sourceText, "renderPageSub");

				generator.CompileTemplates();

				string result = string.Empty;
				Task.Run(() =>
				{
					var ctx = CreateRequest("http://127.0.0.1/renderPage.cshtml");
					result = GetResult(generator.GenerateOutputString(null, "renderPage", ctx, new ModelStateDictionary(), new ExpandoObject()));
				});
				Thread.Sleep(1000);

				runner.Stop();
				var year = DateTime.UtcNow.Year;
				Assert.IsTrue(result.Contains("Mainpage"));
				Assert.IsTrue(result.Contains("Subpage"));
			}

			private static SimpleHttpContext CreateRequest(string uri)
			{
				var context = new SimpleHttpContext();
				var request = new SimpleHttpRequest();
				request.SetUrl(new Uri(uri));
				var response = new SimpleHttpResponse();
				var outputStream = new MockStream();
				response.SetOutputStream(outputStream);
				context.SetRequest(request);
				context.SetResponse(response);
				return context;
			}
		}
	}
}
