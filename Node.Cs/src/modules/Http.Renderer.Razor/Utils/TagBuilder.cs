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


using System.Collections.Generic;
using System.Text;

namespace Http.Renderer.Razor.Utils
{
	public static class TagBuilder
	{
		public static string StartTag(string tagname, Dictionary<string, object> attributes = null, bool selfClosed = false)
		{
			var strbu = new StringBuilder();
			strbu.Append("<")
				.Append(tagname)
				.Append(" ");

			if (attributes != null)
			{
				foreach (var kvp in attributes)
				{
					strbu.Append(" ")
						.Append(kvp.Key);
					if (kvp.Value != null)
					{
						strbu.Append("=\"")
							.Append(kvp.Value.ToString().Replace("\"", "\\\""))
							.Append("\"");
					}
				}
			}
			if (selfClosed)
			{
				strbu.Append("/>");
				return strbu.ToString();
			}

			strbu.Append(">");
			return strbu.ToString();
		}

		public static string EndTag(string tagname)
		{
			return "</" + tagname + ">";
		}

		public static string TagWithValue(string tagname, string value, Dictionary<string, object> attributes)
		{
			return StartTag(tagname, attributes) + value + EndTag(tagname);
		}
	}
}
