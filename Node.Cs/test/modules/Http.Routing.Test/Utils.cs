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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using Moq;

namespace Http.Routing.Test
{
	class Utils
	{

		public static HttpContextBase MockContext(string url, Dictionary<string, string> pars = null, byte[] content = null, string httpMethod = "GET")
		{
			if (pars == null) pars = new Dictionary<string, string>();
			if (content == null) content = new byte[] { };

			var nvc = new NameValueCollection();
			
			foreach (var kvp in pars)
			{
				nvc.Add(kvp.Key, kvp.Value);
			}

			var requestStream = new MemoryStream(content);
			requestStream.Seek(0, SeekOrigin.Begin);
			var responseStream = new MemoryStream();
			var server = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);
			var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
			response.Setup(r => r.OutputStream).Returns(responseStream);

			var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
			request.Setup(r => r.UserHostAddress).Returns("127.0.0.1");
			request.Setup(r => r.Url).Returns(new Uri(url));
			request.Setup(r => r.ContentEncoding).Returns(Encoding.UTF8);
			request.Setup(r => r.Params).Returns(nvc);
			request.Setup(r => r.InputStream).Returns(requestStream);
			request.Setup(r => r.HttpMethod).Returns(httpMethod);

			var session = new Mock<HttpSessionStateBase>();
			session.Setup(s => s.SessionID).Returns(Guid.NewGuid().ToString());

			var context = new Mock<HttpContextBase>();
			context.SetupGet(c => c.Request).Returns(request.Object);
			context.SetupGet(c => c.Response).Returns(response.Object);
			context.SetupGet(c => c.Server).Returns(server.Object);
			context.SetupGet(c => c.Session).Returns(session.Object);
			return context.Object;
		}
	}
}
