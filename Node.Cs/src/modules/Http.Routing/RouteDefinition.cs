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


using Http.Shared.Routing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Http.Routing
{
	public class RouteDefinition
	{
		public string RouteString { get; private set; }
		public RouteDefinition(string route, Dictionary<string, object> objectToDictionary,bool isStatic=false,bool isBlock=false)
		{
			IsStatic = isStatic;
			IsBlock = isBlock;
			RouteString = route.ToLowerInvariant();
			Parameters = new Dictionary<string, ParameterDefintion>(StringComparer.OrdinalIgnoreCase);
			foreach (var item in objectToDictionary)
			{
				if (item.Value.GetType() == typeof(RoutingParameter))
				{
					Parameters.Add(item.Key, new ParameterDefintion { Name = item.Key, Optional = true });
				}
				else
				{
					Parameters.Add(item.Key, new ParameterDefintion { Name = item.Key, Value = item.Value });
				}
			}
			var splitted = route.Split('/');
			var resultUrl = new List<UrlItemDescriptor>();
			foreach (var block in splitted)
			{
				if (block.StartsWith("{") && block.EndsWith("}"))
				{
					resultUrl.Add(new UrlItemDescriptor
					{
						IsParameter = true,
						Name = block.TrimStart('{').TrimEnd('}'),
						LowerRoute = block.TrimStart('{').TrimEnd('}').ToLowerInvariant()
					});
				}
				else
				{
					resultUrl.Add(new UrlItemDescriptor
					{
						Name = block.TrimStart('{').TrimEnd('}'),
						LowerRoute = block.TrimStart('{').TrimEnd('}').ToLowerInvariant()
					});
				}
			}
			IsStatic = false;
			IsBlock = false;
			Url = new ReadOnlyCollection<UrlItemDescriptor>(resultUrl);
		}

		public bool IsBlock { get; private set; }
		public bool IsStatic { get; private set; }

		public RouteDefinition(string route, bool isStatic, bool isBlock)
		{
			RouteString = route.ToLowerInvariant();
			var splitted = route.Split('/');
			var resultUrl = new List<UrlItemDescriptor>();
			foreach (var block in splitted)
			{
				resultUrl.Add(new UrlItemDescriptor
					{
						Name = block,
						LowerRoute = block.ToLowerInvariant(),
						IsParameter = false
					});
			}
			Url = new ReadOnlyCollection<UrlItemDescriptor>(resultUrl);
			IsStatic = isStatic;
			IsBlock = isBlock;
		}

		public ReadOnlyCollection<UrlItemDescriptor> Url { get; private set; }
		public Dictionary<string, ParameterDefintion> Parameters { get; private set; }
	}


}
