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
using System.Web;

namespace Http.Shared.Controllers
{
	public class ConversionService : IConversionService
	{
		private readonly Dictionary<string, ISerializer> _mimeConverters;


		public ConversionService()
		{
			_mimeConverters = new Dictionary<string, ISerializer>(StringComparer.OrdinalIgnoreCase);
		}

		public void AddConverter(ISerializer converter, string firstMime, params string[] mimes)
		{
			AddConverter(firstMime, converter);
			foreach (var mime in mimes)
			{
				AddConverter(mime, converter);
			}
		}

		private void AddConverter(string mime, ISerializer converter)
		{
			if (_mimeConverters.ContainsKey(mime))
			{
				_mimeConverters.Remove(mime);
			}
			_mimeConverters.Add(mime, converter);
		}

		public bool HasConverter(string mime)
		{
			if (string.IsNullOrWhiteSpace(mime)) return false;
			return _mimeConverters.ContainsKey(mime);
		}

		public T Convert<T>(string mime, HttpRequestBase request)
		{
			if (!HasConverter(mime)) return default(T);
			return (T)Convert(typeof(T), mime, request);
		}

		public object Convert(Type t, string mime, HttpRequestBase request)
		{
			if (!HasConverter(mime)) return null;
			return _mimeConverters[mime].Deserialize(t, request);
		}

		public byte[] Convert<T>(string mime, T src)
		{
			if (!HasConverter(mime)) return new byte[] { };
			return Convert(typeof(T), mime, src);
		}

		public byte[] Convert(Type t, string mime, object src)
		{
			if (!HasConverter(mime)) return new byte[] { };
			return _mimeConverters[mime].Serialize(t, src);
		}
	}
}
