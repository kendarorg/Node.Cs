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
using ConcurrencyHelpers.Utils;
using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Logging;
using GenericHelpers;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.PathProviders;
using Http.Shared.Renderers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NodeCs.Shared.Caching;

namespace Http.Renderer.Markdown
{
	public class MarkdownRenderer : IRenderer, ILoggable
	{
		private const string MARKDOWN_CACHE_ID = "MarkdownRenderer";
		public MarkdownRenderer()
		{
			_renderer = new MarkdownSharp.Markdown();
			_locker = new ConcurrentInt64();
		}

		private ICacheEngine _cacheEngine;
		private readonly MarkdownSharp.Markdown _renderer;
		private readonly ConcurrentInt64 _locker;

		public bool CanHandle(string extension)
		{
			return extension.ToLowerInvariant() == "md";
		}


		public IEnumerable<ICoroutineResult> Render(string itemPath, DateTime lastModification, MemoryStream source, IHttpContext context, object model, ModelStateDictionary modelStateDictionary, object viewBag)
		{
			var bytes = new byte[] { };
			if (_cacheEngine != null)
			{
				StreamResult streamResult = null;
				yield return _cacheEngine.Get(
					itemPath, (a) =>
					{
						streamResult = (StreamResult)a;
					}, MARKDOWN_CACHE_ID);

				if (streamResult == null || streamResult.LastModification < lastModification)
				{
					yield return _cacheEngine.AddOrUpdateAndGet(new CacheDefinition
					{
						Id = itemPath,
						LoadData = () => LoadTransformedBytes(itemPath, source, lastModification),
						ExpireAfter = TimeSpan.FromSeconds(60)
					}, (a) =>
					{
						streamResult = (StreamResult)a;
					}, MARKDOWN_CACHE_ID);
				}

				bytes = streamResult.Result;
			}
			else
			{
				yield return CoroutineResult.RunAndGetResult(LoadTransformedBytes(itemPath, source, lastModification))
					.OnComplete((a) =>
					{
						// ReSharper disable once SuspiciousTypeConversion.Global
						//bytes = ((StreamResult)a).Result;
						bytes = ((StreamResult)a.Result).Result;
					})
					.WithTimeout(TimeSpan.FromSeconds(60))
					.AndWait();
			}
			context.Response.ContentType = MimeHelper.HTML_MIME;
			var newSoure = new MemoryStream(bytes);
			var target = context.Response.OutputStream;
			yield return CoroutineResult.RunTask(newSoure.CopyToAsync(target),
				string.Format("MarkdownRenderer::CopyStream '{0}'", context.Request.Url))
				.AndWait();
		}

		public bool IsSessionCapable {
			get { return false; }
		}

		private IEnumerable<ICoroutineResult> LoadTransformedBytes(string itemPath, MemoryStream source, DateTime lastModification)
		{
			string result;
			var text = PathUtils.RemoveBom(Encoding.UTF8.GetString(source.ToArray()));
			while (_locker.Value != 0)
			{
				yield return CoroutineResult.Wait;
			}
			_locker.Increment();
			try
			{
				result = _renderer.Transform(text);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error loading {0}", itemPath);
				throw;
			}
			finally
			{
				_locker.Value = 0;
			}

			var bytes = Encoding.UTF8.GetBytes(result);
			yield return CoroutineResult.Return(new StreamResult(lastModification, bytes));
		}

		public void SetCachingEngine(ICacheEngine cacheEngine)
		{
			_cacheEngine = cacheEngine;
			_cacheEngine.AddGroup(new CacheGroupDefinition
			{
				ExpireAfter = TimeSpan.FromDays(1),
				Id = MARKDOWN_CACHE_ID,
				RollingExpiration = true
			});
		}

		public ILogger Log { get; set; }
	}
}
