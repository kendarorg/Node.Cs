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
using Http.Contexts;
using Http.Shared.Routing;
using HttpMvc.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.TestHelpers;

namespace Http.Routing.Test
{
	public class MockController : ControllerBase
	{

	}

	// ReSharper disable PossibleNullReferenceException
	[TestClass]
	public class RoutingServiceTest
	{
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
		[TestMethod]
		public void ItShouldBePossibleToHandleDirectUrlParameters()
		{
			IRoutingHandler rs = new RoutingService();
			rs.LoadControllers(new[] { typeof(MockController) });
			rs.MapRoute("","~/{controller}/{method}/{id}", new { Controller = "Mock", Action = "Index", Id = RoutingParameter.Optional });
			var ctx = CreateRequest("http://127.0.0.1/Mock/Index");

			var result = rs.Resolve(ctx.Request.Url.LocalPath, ctx);
			Assert.IsNotNull(result);

			Assert.AreEqual(3, result.Parameters.Count);
			Assert.IsTrue(result.Parameters.ContainsKey("controller"));
			Assert.AreEqual("Mock", result.Parameters["controller"]);
			Assert.IsTrue(result.Parameters.ContainsKey("action"));
			Assert.AreEqual("Index", result.Parameters["action"]);
		}


		[TestMethod]
		public void ItShouldBePossibleToHandleUrlWithParameterSetValueOnly()
		{
			IRoutingHandler rs = new RoutingService();
			rs.LoadControllers(new[] { typeof(MockController) });
			rs.MapRoute("", "~/{controller}/Fuffa/{id}", new { Controller = "Mock", Action = "Index", Id = RoutingParameter.Optional });
			var ctx = CreateRequest("http://127.0.0.1/Mock/Fuffa");

			var result = rs.Resolve(ctx.Request.Url.LocalPath, ctx);
			Assert.IsNotNull(result);

			Assert.AreEqual(2, result.Parameters.Count);
			Assert.IsTrue(result.Parameters.ContainsKey("controller"));
			Assert.AreEqual("Mock", result.Parameters["controller"]);
			Assert.IsTrue(result.Parameters.ContainsKey("action"));
			Assert.AreEqual("Index", result.Parameters["action"]);
		}

		[TestMethod]
		public void ItShouldBePossibleToDistinguishTwoDifferentRequest()
		{
			IRoutingHandler rs = new RoutingService();
			rs.LoadControllers(new[] { typeof(MockController) });
			rs.MapRoute("", "~/{controller}/Index/{id}", new { Controller = "Mock", Action = "MethodIndex", Id = RoutingParameter.Optional });
			rs.MapRoute("", "~/{controller}/Test/{id}", new { Controller = "Mock", Action = "MethodTest", Id = RoutingParameter.Optional });

			var ctx = CreateRequest("http://127.0.0.1/Mock/Index");
			var result = rs.Resolve(ctx.Request.Url.LocalPath, ctx);
			Assert.IsNotNull(result);

			Assert.AreEqual(2, result.Parameters.Count);
			Assert.IsTrue(result.Parameters.ContainsKey("controller"));
			Assert.AreEqual("Mock", result.Parameters["controller"]);
			Assert.IsTrue(result.Parameters.ContainsKey("action"));
			Assert.AreEqual("MethodIndex", result.Parameters["action"]);

			ctx = CreateRequest("http://127.0.0.1/Mock/Test");
			result = rs.Resolve(ctx.Request.Url.LocalPath, ctx);
			Assert.IsNotNull(result);

			Assert.AreEqual(2, result.Parameters.Count);
			Assert.IsTrue(result.Parameters.ContainsKey("controller"));
			Assert.AreEqual("Mock", result.Parameters["controller"]);
			Assert.IsTrue(result.Parameters.ContainsKey("action"));
			Assert.AreEqual("MethodTest", result.Parameters["action"]);
		}


		[TestMethod]
		public void ItShouldBePossibleToSetOptionalParameters()
		{
			IRoutingHandler rs = new RoutingService();
			rs.LoadControllers(new []{typeof(MockController)});
			rs.MapRoute("", "~/{controller}/Fuffa/{id}", new { Controller = "Mock", Action = "Index", Id = RoutingParameter.Optional });
			var ctx = CreateRequest("http://127.0.0.1/Mock/Fuffa/200");

			var result = rs.Resolve(ctx.Request.Url.LocalPath, ctx);
			Assert.IsNotNull(result);

			Assert.AreEqual(3, result.Parameters.Count);
			Assert.IsTrue(result.Parameters.ContainsKey("controller"));
			Assert.AreEqual("Mock", result.Parameters["controller"]);
			Assert.IsTrue(result.Parameters.ContainsKey("action"));
			Assert.AreEqual("Index", result.Parameters["action"]);
			Assert.IsTrue(result.Parameters.ContainsKey("id"));
			Assert.AreEqual("200", result.Parameters["id"]);
		}

		[TestMethod]
		[Ignore]
		public void ItShouldBePossibleToHandleStarParametersPath()
		{

		}

		[TestMethod]
		[Ignore]
		public void ItShouldBePossibleToOverrideRoutes()
		{

		}

		[TestMethod]
		[Ignore]
		public void ItShouldBePossibleToIgnoreRoutes()
		{

		}
	}
	// ReSharper restore PossibleNullReferenceException
}
