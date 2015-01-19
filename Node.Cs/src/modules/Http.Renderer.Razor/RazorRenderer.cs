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
using System.Diagnostics;
using System.Threading.Tasks;
using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Enums;
using CoroutinesLib.Shared.Logging;
using GenericHelpers;
using Http.Renderer.Razor.Integration;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.PathProviders;
using Http.Shared.Renderers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NodeCs.Shared.Caching;

namespace Http.Renderer.Razor
{
	public class RazorRenderer : IRenderer, ILoggable
	{
		private const string RAZOR_CACHE_ID = "RazorRenderer";
		public RazorRenderer()
		{
			_renderer = new RazorTemplateGenerator();
		}

		private ICacheEngine _cacheEngine;
		private readonly RazorTemplateGenerator _renderer;

		public bool CanHandle(string extension)
		{
			return extension.ToLowerInvariant() == "cshtml";
		}


		public IEnumerable<ICoroutineResult> Render(string itemPath, DateTime lastModification, MemoryStream source, IHttpContext context, 
			object model, ModelStateDictionary modelStateDictionary, object viewBag)
		{

			if (_cacheEngine != null)
			{
				StreamResult streamResult = null;
				yield return _cacheEngine.Get(
					itemPath, (a) =>
					{
						streamResult = (StreamResult)a;
					}, RAZOR_CACHE_ID);

				if (streamResult == null || streamResult.LastModification < lastModification)
				{
					yield return _cacheEngine.AddOrUpdateAndGet(new CacheDefinition
					{
						Id = itemPath,
						LoadData = () => RunAsTask(itemPath, source, lastModification, model),
						ExpireAfter = TimeSpan.FromSeconds(60)
					}, (a) =>
					{
						streamResult = (StreamResult)a;
					}, RAZOR_CACHE_ID);
				}
			}
			else
			{
				yield return CoroutineResult.RunTask(Task.Run(() => LoadTransformedBytes(itemPath, source, model)))
					.WithTimeout(TimeSpan.FromSeconds(60))
					.AndWait();
			}
			context.Response.ContentType = MimeHelper.HTML_MIME;
			
			var newSoure = new MemoryStream();
			foreach (ICoroutineResult result in _renderer.GenerateOutputString(model, itemPath, context, modelStateDictionary, viewBag))
			{
				if (result.ResultType == ResultType.YieldReturn || result.ResultType == ResultType.Return)
				{
					var bytes = (byte[]) result.Result;
					newSoure.Write(bytes, 0, bytes.Length);
				}
				else
				{
					yield return result;
				}
			}
			newSoure.Seek(0, SeekOrigin.Begin);
			var target = context.Response.OutputStream;
			yield return CoroutineResult.RunTask(newSoure.CopyToAsync(target),
				string.Format("RazorRenderer::CopyStream '{0}'", context.Request.Url))
				.AndWait();
		}

		public bool IsSessionCapable
		{
			get { return true; }
		}

		private IEnumerable<ICoroutineResult> RunAsTask(string itemPath, MemoryStream source, DateTime lastModification, object model)
		{
			byte[] bytes = null;
			var task = Task.Run(() => LoadTransformedBytes(itemPath, source, model));
			while (!task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
			{
				yield return CoroutineResult.Wait;
			}
			yield return CoroutineResult.Return(new StreamResult(lastModification, bytes));
		}

		private void LoadTransformedBytes(string itemPath, MemoryStream source, object model)
		{
			try
			{
				var text = PathUtils.RemoveBom(Encoding.UTF8.GetString(source.ToArray()));
				_renderer.RegisterTemplate(text, itemPath);
				_renderer.CompileTemplates();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error loading {0}", itemPath);
				throw;
			}
		}

		public void SetCachingEngine(ICacheEngine cacheEngine)
		{
			_cacheEngine = cacheEngine;
			_cacheEngine.AddGroup(new CacheGroupDefinition
			{
				ExpireAfter = TimeSpan.FromDays(1),
				Id = RAZOR_CACHE_ID,
				RollingExpiration = true
			});
		}

		public ILogger Log { get; set; }
	}
}
