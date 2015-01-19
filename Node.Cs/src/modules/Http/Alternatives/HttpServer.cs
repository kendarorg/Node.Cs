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


using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Http.Alternatives
{
	public abstract class HttpServer
	{
		private readonly IPAddress _address;
		protected int _port;
		TcpListener _listener;
		bool _is_active = true;

		protected HttpServer(IPAddress address, int port)
		{
			_address = address;
			_port = port;
		}

		public void Listen()
		{
			_listener = new TcpListener(_address, _port);
			_listener.Start();
			while (_is_active)
			{
				_listener.AcceptTcpClientAsync()
					.ContinueWith(a =>
					{
						var processor = new HttpProcessor(a.Result, this);
						processor.Process();
					});
			}
		}

		public abstract void HandleGetRequest(HttpProcessor p);
		public abstract void HandlePostRequest(HttpProcessor p, StreamReader inputData);
	}
}
