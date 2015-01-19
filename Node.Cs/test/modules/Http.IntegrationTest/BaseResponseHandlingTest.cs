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
using System.Diagnostics;
using System.IO;
using System.Text;
using CoroutinesLib.TestHelpers;
using Http.Contexts;
using Http.Shared.Contexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.TestHelpers;

namespace Http.IntegrationTest
{
	public class BaseResponseHandlingTest
	{
		protected static string InferWebRootDir(string rootDir)
		{
			//Search all root directories
			while (!Directory.Exists(Path.Combine(rootDir, "web")))
			{
				rootDir = Path.GetDirectoryName(rootDir);
			}
			rootDir = Path.Combine(rootDir, "web");
			return rootDir;
		}

		protected static SimpleHttpContext CreateRequest(string uri)
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

		protected void VerifyContext(SimpleHttpContext context)
		{
			var outputStream = (MockStream)context.Response.OutputStream;
			Console.WriteLine(outputStream.WrittenBytes + " " + context.Request.Url + " " + outputStream.Sw.ElapsedMilliseconds);
			Console.WriteLine(" " + outputStream.Start + " " + outputStream.End);
			Assert.AreEqual(1, outputStream.ClosesCall);
			Assert.IsTrue(outputStream.WrittenBytes > 0);
			var os = (MemoryStream)context.Response.OutputStream;
			os.Seek(0, SeekOrigin.Begin);
			var bytes = os.ToArray();
			var strings = Encoding.UTF8.GetString(bytes);
			Assert.IsTrue(strings.Length > 0);
			Assert.IsTrue(strings.IndexOf("Exception", StringComparison.Ordinal) < 0, strings);
		}

		protected IHttpContext PrepareRequest(string uri)
		{
			var context = CreateRequest(uri);
			var outputStream = (MockStream)context.Response.OutputStream;
			outputStream.Initialize();
			return context;
		}

		protected byte[] RunRequest(string uri, HttpModule http, RunnerForTest runner, int timeoutMs = 250)
		{
			var context = PrepareRequest(uri);
			http.ExecuteRequest(context);
			var outputStream = (MockStream)context.Response.OutputStream;

			var sw = new Stopwatch();
			sw.Start();
			while (outputStream.ClosesCall != 1 && sw.ElapsedMilliseconds < timeoutMs)
			{
				runner.RunCycleFor(timeoutMs);
			}
			Console.WriteLine(outputStream.WrittenBytes + " " + uri);
			Assert.AreEqual(1, outputStream.ClosesCall);
			Assert.IsTrue(outputStream.WrittenBytes > 0);
			var os = (MemoryStream)context.Response.OutputStream;
			os.Seek(0, SeekOrigin.Begin);
			var bytes = os.ToArray();
			var strings = Encoding.UTF8.GetString(bytes);
			Assert.IsTrue(strings.Length > 0);
			Assert.IsTrue(strings.IndexOf("Exception", StringComparison.Ordinal) < 0, strings);
			return bytes;
		}
	}
}
