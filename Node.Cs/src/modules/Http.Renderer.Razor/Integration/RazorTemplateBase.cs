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


using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Threading;
using System.Web.WebPages;
using System.Web.WebPages.Instrumentation;
using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Enums;
using Http.Contexts;
using Http.Renderer.Razor.Helpers;
using Http.Renderer.Razor.Utils;
using Http.Shared.Contexts;
using Http.Shared.Optimizations;
using HttpMvc.Controllers;
using NodeCs.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Http.Renderer.Razor.Integration
{
	public abstract class RazorTemplateBase : IRazorTemplate
	{
		public IHttpContext Context { get; set; }
		public List<BufferItem> Buffer { get; private set; }
		public string Layout { get; set; }

		public dynamic ViewBag
		{
			get { return _viewBag; }
			set { _viewBag = value; }
		}

		public object ObjectModel { get; set; }
		public StileHelper Styles { get; set; }
		public ScriptHelper Scripts { get; set; }

		public virtual string ResolveUrl(string path)
		{
			// TODO: Actually resolve the url
			if (path.StartsWith("~"))
			{
				path = path.Substring(1);
			}
			return path;
		}

		/// <summary>
		/// Writes an attribute to the result.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		public virtual void WriteAttribute(string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
		{
			bool first = true;
			bool wroteSomething = false;
			if (values.Length == 0)
			{
				// Explicitly empty attribute, so write the prefix and suffix
				WriteLiteral( prefix);
				WriteLiteral(suffix);
			}
			else
			{
				for (int i = 0; i < values.Length; i++)
				{
					AttributeValue attrVal = values[i];
					PositionTagged<object> val = attrVal.Value;
					bool? boolVal = null;
					if (val.Value is bool)
					{
						boolVal = (bool)val.Value;
					}
					if (val.Value != null && (boolVal == null || boolVal.Value))
					{
						string valStr = val.Value as string;
						if (valStr == null)
						{
							valStr = val.Value.ToString();
						}
						if (boolVal != null)
						{
							Debug.Assert(boolVal.Value);
							valStr = name;
						}
						if (first)
						{
							WriteLiteral( prefix);
							first = false;
						}
						else
						{
							WriteLiteral( attrVal.Prefix);
						}
						if (attrVal.Literal)
						{
							WriteLiteral( valStr);
						}
						else
						{
							WriteLiteral( valStr); // Write value
						}
						wroteSomething = true;
					}
				}
				if (wroteSomething)
				{
					WriteLiteral( suffix);
				}
			}
		}

		private void WritePositionTaggedLiteral(TextWriter writer, PositionTagged<string> value)
		{
			WriteLiteralTo(writer, value.Value);
		}
		/// <summary>
		/// Writes an attribute to the specified <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="name">The name of the attribute to be written.</param>
		public virtual void WriteAttributeTo(TextWriter writer, string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
		{
			bool first = true;
			bool wroteSomething = false;
			if (values.Length == 0)
			{
				// Explicitly empty attribute, so write the prefix and suffix
				WritePositionTaggedLiteral(writer, prefix);
				WritePositionTaggedLiteral(writer, suffix);
			}
			else
			{
				for (int i = 0; i < values.Length; i++)
				{
					AttributeValue attrVal = values[i];
					PositionTagged<object> val = attrVal.Value;
					bool? boolVal = null;
					if (val.Value is bool)
					{
						boolVal = (bool)val.Value;
					}
					if (val.Value != null && (boolVal == null || boolVal.Value))
					{
						string valStr = val.Value as string;
						if (valStr == null)
						{
							valStr = val.Value.ToString();
						}
						if (boolVal != null)
						{
							Debug.Assert(boolVal.Value);
							valStr = name;
						}
						if (first)
						{
							WritePositionTaggedLiteral(writer, prefix);
							first = false;
						}
						else
						{
							WritePositionTaggedLiteral(writer, attrVal.Prefix);
						}
						if (attrVal.Literal)
						{
							WriteLiteralTo(writer, valStr);
						}
						else
						{
							WriteTo(writer, valStr); // Write value
						}
						wroteSomething = true;
					}
				}
				if (wroteSomething)
				{
					WritePositionTaggedLiteral(writer, suffix);
				}
			}
		}

		protected Dictionary<string, Action> _sections = new Dictionary<string, Action>(StringComparer.InvariantCultureIgnoreCase);
		private dynamic _viewBag;
		private static MethodInfo _createMethod;

		public RawString RenderBody()
		{
			return new RawString(ViewBag.ChildItem as string);
		}

		static RazorTemplateBase()
    {
      Type type = Type.GetType("CoroutinesLib.RunnerFactory,CoroutinesLib");
      if (!(type != (Type) null))
        return;
     _createMethod = type.GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

		public RawString RenderPage(string name, object model = null, bool skipLayout = false)
		{
			var http = ServiceLocator.Locator.Resolve<HttpModule>();

			var context = new WrappedHttpContext(Context);

			var newUrl = name.TrimStart('~');

			((IHttpRequest)context.Request).SetUrl(new Uri(newUrl, UriKind.Relative));
			((IHttpRequest)context.Request).SetQueryString(Context.Request.QueryString);
			var internalCoroutine = http.SetupInternalRequestCoroutine(context, model, ViewBag);
			Exception problem = null;
			Action<Exception> onError = (Action<Exception>)(ex => problem = ex);
			((ICoroutinesManager)_createMethod.Invoke((object)null, new object[0])).StartCoroutine(internalCoroutine, onError);

			ManualResetEventSlim waitSlim = new ManualResetEventSlim(false);
			while (4L > (long)internalCoroutine.Status)
				waitSlim.Wait(10);
			while (!RunningStatusExtension.Is(internalCoroutine.Status, RunningStatus.NotRunning))
				waitSlim.Wait(10);
			if (problem != null)
				throw new Exception("Error running subtask", problem);
			/*var task = CoroutineResult.WaitForCoroutine(internalCoroutine);
			//task.Wait();*/
			var stream = context.Response.OutputStream as MemoryStream;

			// ReSharper disable once PossibleNullReferenceException
			var result = Encoding.UTF8.GetString(stream.ToArray());
			return new RawString(result);
		}

		public RawString RenderSection(string sectionName, bool required = false)
		{
			if (IsSectionDefined(sectionName))
			{
				_sections[sectionName]();
			}
			return new RawString(string.Empty);
		}

		public bool IsSectionDefined(string sectionName)
		{
			return _sections.ContainsKey(sectionName);
		}

		public void DefineSection(string name, Action action)
		{
			_sections[name] = action;
		}

		public void WriteLiteralTo(TextWriter writer, object value)
		{
			writer.Write(value);
		}

		public void WriteTo(TextWriter writer, object value)
		{
			writer.Write(value);
		}

		protected RazorTemplateBase()
		{
			Buffer = new List<BufferItem>();
		}

		public abstract void Execute();

		public virtual void Write()
		{
			
		}
		public virtual void Write(object value)
		{
			WriteLiteral(value);
		}

		public virtual void WriteLiteral(object value)
		{
			Buffer.Add(new BufferItem { Value = value });
		}

	}
	public abstract class RazorTemplateBase<T> : RazorTemplateBase
	{
		public HtmlHelper<T> Html { get; set; }
		public UrlHelper Url { get; set; }
		public T Model
		{
			get { return (T)ObjectModel; }
			set { ObjectModel = (T)value; }
		}

		// ReSharper disable InconsistentNaming
		public T model
		{
			get { return Model; }
			set { Model = value; }
		}
		// ReSharper restore InconsistentNaming
	}
}