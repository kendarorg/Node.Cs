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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Routing;
using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Enums;
using GenericHelpers;
using Http.Contexts;
using Http.Renderer.Razor.Helpers;
using Http.Renderer.Razor.Utils;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.Optimizations;
using Http.Shared.Routing;
using HttpMvc.Controllers;
using NodeCs.Shared;

namespace Http.Renderer.Razor.Integration
{
	public class RazorTemplateGenerator : IRazorTemplateGenerator
	{
		private readonly ConcurrentDictionary<string, RazorTemplateEntry> _templateItems =
			new ConcurrentDictionary<string, RazorTemplateEntry>();



		public void RegisterTemplate(string templateString, string templateName)
		{
			Type templateType = ModelTypeFromTemplate(ref templateString);
			RegisterTemplate(templateName, templateString, templateType);
		}

		private Type ModelTypeFromTemplate(ref string templateString)
		{
			var splittedTemplate = templateString
				.Split(new[] { '\r', '\f', '\n' }).ToList();

			var modelIndex = -1;
			for (int index = 0; index < splittedTemplate.Count; index++)
			{
				var item = splittedTemplate[index];
				var trimmed = item.Trim();
				if (trimmed.StartsWith("@model", StringComparison.InvariantCultureIgnoreCase))
				{
					modelIndex = index;
					break;
				}
			}
			if (modelIndex == -1) return null;
			var modelString = splittedTemplate[modelIndex];
			splittedTemplate.RemoveAt(modelIndex);
			templateString = string.Join("\r\n", splittedTemplate);
			modelString = modelString.Substring("@model ".Length);
			return AssembliesManager.LoadType(modelString);
		}

		private void RegisterTemplate(string templateName, string templateString, Type modelType)
		{
			if (templateName == null)
				throw new ArgumentNullException("templateName");
			if (templateString == null)
				throw new ArgumentNullException("templateString");

			_templateItems[templateName] = new RazorTemplateEntry()
			{
				ModelType = modelType ?? typeof(object),
				TemplateString = templateString,
				TemplateName = "Rzr" + Guid.NewGuid().ToString("N"),
				IsNoModel = modelType == null
			};
		}

		public void CompileTemplates()
		{
			Compiler.Compile(_templateItems.Values);
		}

		private static readonly object _locker = new object();
		private static MethodInfo _createMethod;

		public IEnumerable<ICoroutineResult> GenerateOutput(object model, string templateName, IHttpContext context,
			ModelStateDictionary modelStateDictionary, object viewBagPassed)
		{
			var viewBag = viewBagPassed as dynamic;
			if (templateName == null)
				throw new ArgumentNullException("templateName");

			RazorTemplateEntry entry = null;
			try
			{
				entry = _templateItems[templateName];
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentOutOfRangeException("No template has been registered under this model or name.");
			}
			var templateItem = _templateItems[templateName];
			if (templateItem.TemplateType == null)
			{
				lock (_locker)
				{
					templateItem.TemplateType =
						AssembliesManager.LoadType("Http.Renderer.Razor.Integration." + entry.TemplateName + "Template");
				}
			}
			var type = templateItem.TemplateType;
			var template = (RazorTemplateBase)Activator.CreateInstance(type);
			InjectData(template, type, context, modelStateDictionary, viewBag);

			template.ObjectModel = model;
			template.Execute();
			var layout = template.Layout;


			if (!string.IsNullOrWhiteSpace(layout))
			{
				var sb = new StringBuilder();
				foreach (var item in template.Buffer)
				{
					sb.Append(item.Value);
				}
				viewBag.ChildItem = sb.ToString();
				foreach (var item in RenderLayout(layout, context, viewBag))
				{
					yield return item;
				}
			}
			else
			{
				foreach (var item in template.Buffer)
				{
					var bytes = Encoding.UTF8.GetBytes(item.Value.ToString());
					yield return CoroutineResult.YieldReturn(bytes);
				}
			}
		}

		static RazorTemplateGenerator()
		{
			Type type = Type.GetType("CoroutinesLib.RunnerFactory,CoroutinesLib");
			if (!(type != (Type)null))
				return;
			_createMethod = type.GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public IEnumerable<ICoroutineResult> RenderLayout(string name, IHttpContext mainContext, dynamic viewBag)
		{
			var http = ServiceLocator.Locator.Resolve<HttpModule>();

			var context = new WrappedHttpContext(mainContext);

			var newUrl = name.TrimStart('~');

			((IHttpRequest)context.Request).SetUrl(new Uri(newUrl, UriKind.Relative));
			((IHttpRequest)context.Request).SetQueryString(mainContext.Request.QueryString);
			var internalCoroutine = http.SetupInternalRequestCoroutine(context, null, viewBag);


			yield return CoroutineResult.RunCoroutine(internalCoroutine)
				.WithTimeout(TimeSpan.FromMinutes(1))
				.AndWait();
			/*Exception problem = null;
			Action<Exception> onError = (Action<Exception>)(ex => problem = ex);
			((ICoroutinesManager)_createMethod.Invoke((object)null, new object[0])).StartCoroutine(internalCoroutine, onError);

			ManualResetEventSlim waitSlim = new ManualResetEventSlim(false);
			while (4L > (long)internalCoroutine.Status)
				waitSlim.Wait(10);
			while (!RunningStatusExtension.Is(internalCoroutine.Status, RunningStatus.NotRunning))
				waitSlim.Wait(10);
			if (problem != null)
				throw new Exception("Error running subtask", problem);*/
			//task.Wait();
			var stream = context.Response.OutputStream as MemoryStream;

			// ReSharper disable once PossibleNullReferenceException
			//r result = Encoding.UTF8.GetString(stream.ToArray());
			stream.Seek(0, SeekOrigin.Begin);
			var bytes = stream.ToArray();
			yield return CoroutineResult.Return(bytes);
			//rn new BufferItem { Value = result };
		}

		private void InjectData(RazorTemplateBase template, Type type, IHttpContext context,
			ModelStateDictionary modelStateDictionary, object viewBag)
		{
			string layout = null;
			var routeHandler = ServiceLocator.Locator.Resolve<IRoutingHandler>();
			var bundlesHandler = ServiceLocator.Locator.Resolve<IResourceBundles>(true);
			var properties = type.GetProperties();
			var model = properties.FirstOrDefault(p => p.Name == "Model");

			var ga = type.GetGenericArguments();
			if (ga.Length == 0)
			{
				ga = new[] { model != null ? model.PropertyType : typeof(object) };
			}
			foreach (var property in type.GetProperties())
			{
				if (!property.CanWrite) continue;
				switch (property.Name)
				{
					case ("Styles"):
						if (bundlesHandler != null)
						{
							property.SetValue(template, bundlesHandler.GetStyles());
						}
						break;
					case ("Scripts"):
						if (bundlesHandler != null)
						{
							property.SetValue(template, bundlesHandler.GetScripts());
						}
						break;
					case ("Html"):
						var urlHelper = typeof(HtmlHelper<>).MakeGenericType(ga);
						//public HtmlHelper(IHttpContext context, ViewContext viewContext,
						//	object nodeCsTemplateBase, string localPath, dynamic viewBag,IRoutingHandler routingHandler)
						property.SetValue(template, Activator.CreateInstance(urlHelper,
							new object[]
							{
								context,new ViewContext(context,template,modelStateDictionary),
								template,context==null?string.Empty:context.Request.Url.ToString(),new ExpandoObject(),routeHandler
							}
							));
						break;
					case ("Url"):
						property.SetValue(template, new UrlHelper(context, routeHandler));
						break;
					case ("Context"):
						property.SetValue(template, context);
						break;
					case ("ViewBag"):
						property.SetValue(template, viewBag ?? new ExpandoObject());
						break;
					default:
						var value = ServiceLocator.Locator.Resolve(property.PropertyType, true);
						if (value != null)
						{
							property.SetValue(template, value);
						}
						break;
				}
			}
		}

		public IEnumerable<ICoroutineResult> GenerateOutputString(object model, string templateName, IHttpContext context, ModelStateDictionary modelStateDictionary, object viewBag)
		{
			foreach (var item in GenerateOutput(model, templateName, context, modelStateDictionary, viewBag))
			{
				yield return item;
			}
		}

	}
}
