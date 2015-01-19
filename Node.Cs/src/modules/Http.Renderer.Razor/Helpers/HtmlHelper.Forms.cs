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
using System.Linq;
using GenericHelpers;
using Http.Renderer.Razor.Utils;
using NodeCs.Shared.Attributes;

namespace Http.Renderer.Razor.Helpers
{
	public partial class HtmlHelper<T>
	{
		public Form BeginForm()
		{
			const string verb = "POST";
			const string encType = "application/x-www-form-urlencoded";
			//return BeginForm(action, controller, verb, encType);

			var path = _routingHandler.ResolveFromParams(_context.RouteParams);

			var nodeCsForm = new Form(ViewContext, new Dictionary<string, object>
			{
				{"action",path},
				{"method",verb},
				{"enctype",encType}
			});

			return nodeCsForm;
		}

		public Form BeginForm(string action)
		{
			var controller = (string)_context.RouteParams["controller"];
			const string verb = "POST";
			const string encType = "application/x-www-form-urlencoded";
			return BeginForm(action, controller, verb, encType);
		}

		public Form BeginForm(string action, string controller, string verb, string encType)
		{
			if (controller == null)
			{
				controller = (string)_context.RouteParams["controller"];
			}
			if (action == null)
			{
				action = (string)_context.RouteParams["action"];
			}

			var path = _routingHandler.ResolveFromParams(
					new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
					{
						{"controller",controller},
						{"action",action}
					}
				);

			var nodeCsForm = new Form(ViewContext, new Dictionary<string, object>
			{
				{"action",path},
				{"method",verb},
				{"enctype",encType}
			});

			return nodeCsForm;
		}

		public RawString EditorForModel()
		{
			var result = string.Empty;
			foreach (var property in _classWrapper.Properties)
			{
				var pw = _wrapperDescriptor.GetProperty(property);
				if (!ReflectionUtils.IsSystemType(pw.PropertyType))
				{
					continue;
				}

#pragma warning disable 184
				var scaffold = (ScaffoldColumnAttribute)pw.Attributes.FirstOrDefault(a => (a is ScaffoldColumnAttribute));
				var doScaffold = scaffold == null || scaffold.Scaffold;
#pragma warning restore 184
				if (doScaffold)
				{
					result += EditorFor(pw).ToString();
				}
			}
			return new RawString(result);
		}
	}
}
