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
using System.Text;
using System.Web;
using Http.Shared;
using Newtonsoft.Json;

namespace HttpMvc.Conversions
{
	public class FormNodeCsSerializer : ISerializer
	{
		public object Deserialize(Type t, HttpRequestBase request, Encoding encoding = null)
		{
			var jsonFromRequest = MakeJsonFromRequestParams(request);
			return JsonConvert.DeserializeObject(jsonFromRequest, t);
		}

		public byte[] Serialize(Type t, object src, Encoding encoding = null)
		{
			throw new NotSupportedException();
		}

		private string MakeJsonFromRequestParams(HttpRequestBase request)
		{
			var jf = new JsonForm();
			foreach (var key in request.Params.AllKeys)
			{
				jf.Add(key, request.Params[key]);
			}
			return jf.ToJson();
		}


	}

	internal class JsonForm
	{
		private readonly List<string> _items = new List<string>();
		public string ToJson()
		{
			return "{" + string.Join(",", _items) + "}";
		}


		public void Add(string key, string value)
		{
			var firstDot = key.IndexOf('.');
			string next = null;
			string currentField = null;
			if (firstDot > 0)
			{
				currentField = key.Substring(0, firstDot);
				next = key.Substring(firstDot + 1);
				throw new NotImplementedException("Dot notation in forms not yet supported " + key);
			}
			else if (firstDot < 0)
			{
				currentField = key;
			}
			else
			{
				throw new NotImplementedException("Notation in forms not yet supported " + key);
			}
			_items.Add("\"" + key + "\":\"" + value.Replace("\"", "\\\"") + "\"");
		}
	}
}
