// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using Node.Cs.Lib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Node.Cs.Lib.Conversions
{
	public class JsonNodeCsSerializer : ISerializer
	{
		public object Deserialize(Type t, HttpRequestBase request, Encoding encoding = null)
		{
			var reader = new StreamReader(request.InputStream);
			var stringContent = reader.ReadToEnd();
			var jsSerializer = new JavaScriptSerializer();
			return jsSerializer.Deserialize(stringContent,t);
		}

		public byte[] Serialize(Type t, object src, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.UTF8;
			var jsSerializer = new JavaScriptSerializer();
			var serialized = jsSerializer.Serialize(src);
			return encoding.GetBytes(serialized);
		}
	}
}