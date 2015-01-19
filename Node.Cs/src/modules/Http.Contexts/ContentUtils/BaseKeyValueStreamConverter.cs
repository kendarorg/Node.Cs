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
using System.IO;
using System.Text;

namespace Http.Contexts.ContentUtils
{
	public abstract class BaseKeyValueStreamConverter : IRequestStreamConverter
	{
		private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		protected byte[] _content = { };
		protected Encoding _encoding;
		protected string _contentType;

		public byte[] Content { get { return _content; } }

		public string this[string key]
		{
			get
			{
				if (_parameters.ContainsKey(key)) return _parameters[key];
				return null;
			}
			set
			{
				_parameters.Add(key, value);
			}
		}
		public IEnumerable<string> Keys { get { return _parameters.Keys; } }

		protected abstract void InitializeInternal();

		public void Initialize(Stream body, Encoding encoding, String contentType)
		{
			var inputStream = new MemoryStream();
			using (inputStream) // here we have data
			{
				body.CopyTo(inputStream);
			}
			_contentType = contentType;
			_encoding = encoding;
			_content = inputStream.ToArray();
			InitializeInternal();
		}
	}
}
