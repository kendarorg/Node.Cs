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


using CoroutinesLib.TestHelpers;
using HtmlAgilityPack;
using Http.Contexts;
using Http.PathProvider.StaticContent;
using Http.Routing;
using Http.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Caching;
using Node.Cs.TestHelpers;
using NodeCs.Shared.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Http.IntegrationTest
{
	[TestClass]
	public class HttpResponseHandlingTest : BaseResponseHandlingTest
	{
		[TestMethod]
		public void ItShouldBePossibleToExecuteArequest()
		{
			var assemblyLocation = Assembly.GetExecutingAssembly().Location;
			var runner = new RunnerForTest();
			NodeMainInitializer.InitializeFakeMain(assemblyLocation, runner);
			var rootDir = InferWebRootDir(assemblyLocation);
			var pathProvider = new StaticContentPathProvider(rootDir);
			var http = new HttpModule();
			http.SetParameter(HttpParameters.HttpPort, 8881);
			http.SetParameter(HttpParameters.HttpVirtualDir, "nodecs");
			http.SetParameter(HttpParameters.HttpHost, "localhost");


			var routingService = new RoutingService();
			http.RegisterRouting(routingService);

			http.Initialize();

			http.RegisterPathProvider(pathProvider);

			const string uri = "http://localhost:8881/nodecs";

			var context = CreateRequest(uri);
			var outputStream = (MockStream)context.Response.OutputStream;
			outputStream.Initialize();

			//request.
			http.ExecuteRequest(context);
			runner.RunCycleFor(200);
			var os = (MemoryStream)context.Response.OutputStream;
			os.Seek(0, SeekOrigin.Begin);
			var bytes = os.ToArray();
			var result = Encoding.UTF8.GetString(bytes);

			Assert.IsTrue(outputStream.WrittenBytes > 0);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length > 0);
			Assert.IsTrue(result.IndexOf("Exception", StringComparison.Ordinal) < 0, result);
			Assert.AreEqual(1, outputStream.ClosesCall);
		}


		[TestMethod]
		public void ItShouldBePossibleToExecuteArequestWithCache()
		{
			var assemblyLocation = Assembly.GetExecutingAssembly().Location;
			var runner = new RunnerForTest();
			NodeMainInitializer.InitializeFakeMain(assemblyLocation, runner);
			var cachingModule = new CachingModule();
			cachingModule.Initialize();
			var rootDir = InferWebRootDir(assemblyLocation);
			var pathProvider = new StaticContentPathProvider(rootDir);
			pathProvider.SetCachingEngine(cachingModule.GetParameter<ICacheEngine>(HttpParameters.CacheInstance));
			var http = new HttpModule();
			http.SetParameter(HttpParameters.HttpPort, 8881);
			http.SetParameter(HttpParameters.HttpVirtualDir, "nodecs");
			http.SetParameter(HttpParameters.HttpHost, "localhost");


			var routingService = new RoutingService();
			http.RegisterRouting(routingService);

			http.Initialize();

			http.RegisterPathProvider(pathProvider);

			const string uri = "http://localhost:8881/nodecs";

			var context = CreateRequest(uri);
			var outputStream = (MockStream)context.Response.OutputStream;
			outputStream.Initialize();

			//request.
			http.ExecuteRequest(context);
			runner.RunCycleFor(200);
			var os = (MemoryStream)context.Response.OutputStream;
			os.Seek(0, SeekOrigin.Begin);
			var bytes = os.ToArray();
			var result = Encoding.UTF8.GetString(bytes);

			Assert.IsNotNull(result);
			Assert.IsTrue(outputStream.WrittenBytes > 0);
			Assert.IsTrue(result.Length > 0);
			Assert.IsTrue(result.IndexOf("Exception", StringComparison.Ordinal) < 0, result);
			Assert.AreEqual(1, outputStream.ClosesCall);
		}

		[TestMethod]
		public void ItShouldBePossibleToSimulateMulitpleRequests()
		{
			ItShouldBePossibleToSimulateRequest();
			ItShouldBePossibleToSimulateRequest();
			ItShouldBePossibleToSimulateRequest();
			ItShouldBePossibleToSimulateRequest();
		}

		[TestMethod]
		public void ItShouldBePossibleToSimulateRequest()
		{
			var assemblyLocation = Assembly.GetExecutingAssembly().Location;
			var runner = new RunnerForTest();
			NodeMainInitializer.InitializeFakeMain(assemblyLocation, runner);
			var rootDir = InferWebRootDir(assemblyLocation);
			var pathProvider = new StaticContentPathProvider(rootDir);
			var http = new HttpModule();
			http.SetParameter(HttpParameters.HttpPort, 8881);
			http.SetParameter(HttpParameters.HttpVirtualDir, "nodecs");
			http.SetParameter(HttpParameters.HttpHost, "localhost");


			var routingService = new RoutingService();
			http.RegisterRouting(routingService);

			http.Initialize();

			http.RegisterPathProvider(pathProvider);

			const string uri = "http://localhost:8881/nodecs";


			var bytes = RunRequest(uri + "/numbers.htm", http, runner);
			var result = Encoding.UTF8.GetString(bytes);
			var html = new HtmlDocument();
			html.LoadHtml(result);

			var nodes = html.DocumentNode.SelectNodes("//img/@src");
			var contexts = new List<SimpleHttpContext>();
			if (nodes != null)
			{
				foreach (HtmlNode node in nodes)
				{
					var src = node.Attributes["src"].Value;
					if (!src.StartsWith("http")) src = uri + "/" + src.Trim('/');
					contexts.Add((SimpleHttpContext)PrepareRequest(src));
					http.ExecuteRequest(contexts.Last());
				}
			}
			runner.RunCycleFor(500);
			for (int index = 0; index < contexts.Count; index++)
			{
				var ctx = contexts[index];
				VerifyContext(ctx);
			}
		}



	}
}
