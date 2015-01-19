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


using ClassWrapper;
using GenericHelpers;
using Http.Renderer.Razor.Utils;
using Http.Shared.Contexts;
using Http.Shared.Routing;
using NodeCs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Http.Renderer.Razor.Helpers
{
	public partial class HtmlHelper<T>
	{
		private readonly string _localPath;
		private readonly IRoutingHandler _routingHandler;
		public ViewContext ViewContext { get; set; }
		public dynamic ViewBag { get; set; }
		private readonly IHttpContext _context;
		private T _model;
		private ClassWrapper.ClassWrapper _classWrapper;
		private ClassWrapperDescriptor _wrapperDescriptor;

		public T Model
		{
			get { return _model; }
			set
			{
				_model = value;
				if (_model != null)
				{
					_classWrapper = ValidationService.GetWrapper(_model);
					_wrapperDescriptor = ValidationService.GetWrapperDescriptor(_model);
				}
			}
		}

		public HtmlHelper(IHttpContext context, ViewContext viewContext,
			object nodeCsTemplateBase, string localPath, dynamic viewBag,
			IRoutingHandler routingHandler)
		{
			Lambda = new LambdaHelper();
			_localPath = localPath;
			_routingHandler = routingHandler;
			ViewContext = viewContext;
			ViewBag = viewBag;
			_context = context;
		}

		public LambdaHelper Lambda { get; set; }

		public RawString ActionLink(string title, string action, string controller)
		{
			return ActionLink(title, action, controller, null);
		}

		public RawString ActionLink(string title, string action)
		{
			var controller = (string)_context.RouteParams["controller"];
			return ActionLink(title, action, controller, null);
		}


		public RawString ActionLink(string title, string action, string controller, dynamic routeValues, object htmlAttributesOb)
		{
			var result = ReflectionUtils.ObjectToDictionary(routeValues);
			var htmlAttributes = ReflectionUtils.ObjectToDictionary(htmlAttributesOb);
			result.Add("action", action);
			result.Add("controller", controller);
			var link = _routingHandler.ResolveFromParams(result).ToString();
			if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
			htmlAttributes.Add("href", "/" + _context.RootDir.Trim('/') + "/" + link.TrimStart('/'));
			return new RawString(TagBuilder.TagWithValue("a", title, htmlAttributes));
		}

		public RawString ActionLink(string title, string action, string controller, dynamic routeValues)
		{
			return ActionLink(title, action, controller, routeValues, null);
		}

		public RawString ActionLink(string title, string action, dynamic routeValues)
		{
			var controller = (string)_context.RouteParams["controller"];
			return ActionLink(title, action, controller, routeValues, null);
		}


		public RawString ActionLink(
			string title,
			string action,
			string controller,
			string protocol,
			string hostName,
			string fragment,
			Object routeValues,
			Dictionary<string, object> dict)
		{
			return ActionLink(title, action, controller, routeValues, dict);
		}

		public RawString Partial(string path, object model = null)
		{
			path = path.Trim('~').Replace("\\", "/").Replace("//", "/");
			var splittedPath = path.Split('/').Reverse().ToArray();
			var splittedLocal = path.Split('/').Reverse().ToArray();
			for (int i = 0; i < splittedPath.Count(); i++)
			{
				splittedLocal[i] = splittedPath[i];
			}
			var straight = string.Join("/", splittedLocal.Reverse().ToArray());

			dynamic resulting = null;
			if (model == null)
			{
				//resulting = RazorEngine.Razor.Resolve(straight);
			}
			else
			{
				//resulting = RazorEngine.Razor.Resolve(straight, model);
			}
			return new RawString("Partial NOT IMPLEMENTED");
			/*var dwb = new DynamicViewBag();
			dwb.AddValue("NodeCsContext", _context);
			dwb.AddValue("NodeCsLocalPath", _localPath);
			dwb.AddValue("ModelState", ViewContext.ModelState);

			var context = new ExecuteContext(dwb);
			var template = resulting as ITemplate;
			var resultContent = template.Run(context);
			return new RawString(resultContent);
			 */
		}

		public void RenderAction(string action, string controller)
		{
			var context = (IHttpContext)_context;
			var guid = Guid.NewGuid().ToString();
			/*context.Data.Add("@" + guid + "@", new RenderActionData
			{
				Action = action,
				Controller = controller,
				Params = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "controller", controller }, { "action", action } }
			});
			ViewContext.WriteLiteral("@" + guid + "@");*/
			throw new NotImplementedException();
		}

	}

	public class RenderActionData
	{
		public string Action;
		public Dictionary<string, object> Params = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		public string Controller;
	}
}
