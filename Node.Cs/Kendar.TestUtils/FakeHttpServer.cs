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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;

namespace Kendar.TestUtils
{
	[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Used only for testing")]
	public class FakeHttpServer : IDisposable
	{
		public int Port { get; private set; }
		public string Prefix { get; private set; }
		private Thread _thread;
		private HttpListener _listener;

		public FakeHttpServer(int port = 12530, string prefix = "test")
		{
			Port = port;
			Prefix = prefix;
			Requests = new List<string>();
			Responses = new Dictionary<string, Action<HttpListenerContext, FakeHttpServer>>(StringComparer.OrdinalIgnoreCase);
		}

		public List<string> Requests { get; private set; }
		public Dictionary<string, Action<HttpListenerContext, FakeHttpServer>> Responses { get; private set; }

		public string Root
		{
			get { return "http://localhost:" + Port + "/" + Prefix.Trim('/') + "/"; }
		}

		public bool Start()
		{
			try
			{
				_listener = new HttpListener();
				_listener.Prefixes.Add(Root);
				_listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
				_listener.Start();

				_thread = new Thread(Listen);
				_thread.Start();
				Thread.Sleep(100);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_listener", Justification = "Used only for testing"),
		SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Used only for testing")]
		public void Dispose()
		{
			try
			{
				_listener.Abort();
				_thread.Abort();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private void Listen()
		{
			try
			{
				while (true)
				{
					var context = _listener.GetContext();
					var requestUrl = context.Request.Url.ToString();
					if (Responses.ContainsKey(requestUrl))
					{
						Responses[requestUrl](context, this);
					}
					else
					{
						context.Response.StatusCode = 404;
						context.Response.Close();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void Respond(HttpListenerContext httpListenerContext, byte[] dllContent)
		{
			httpListenerContext.Response.OutputStream.Write(dllContent, 0, dllContent.Length);
			httpListenerContext.Response.Close();
		}

		public string CreateAddress(string subPath = "")
		{
			return Root + subPath.Trim('/');
		}
	}
}
