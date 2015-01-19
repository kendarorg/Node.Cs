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


using CoroutinesLib.Shared.Logging;
using Http.Shared;
using NodeCs.Shared;
using NodeCs.Shared.Caching;

namespace Http.Renderer.Razor
{
	public class RazorRendererModule : NodeModuleBase
	{
		private RazorRenderer _renderer;
		private INodeModule _cachingModule;
		private RazorViewHandler _handler;

		public override void Initialize()
		{
			var httpModule = ServiceLocator.Locator.Resolve<HttpModule>(); ;
			_renderer = new RazorRenderer();
			_renderer.Log = ServiceLocator.Locator.Resolve<ILogger>();
			_cachingModule = GetParameter<INodeModule>(HttpParameters.CacheInstance);
			if (_cachingModule != null)
			{
				_renderer.SetCachingEngine(_cachingModule.GetParameter<ICacheEngine>(HttpParameters.CacheInstance));
			}
			ServiceLocator.Locator.Register<RazorRenderer>(_renderer);
			SetParameter(HttpParameters.RendererInstance, _renderer);
			httpModule.RegisterRenderer(_renderer);
			_handler = new RazorViewHandler();
			httpModule.RegisterResponseHandler(_handler);
			httpModule.RegisterDefaultFiles("default.cshtml");
			httpModule.RegisterDefaultFiles("index.cshtml");
		}

		protected override void Dispose(bool disposing)
		{
			var httpModule = ServiceLocator.Locator.Resolve<HttpModule>(); ;
			httpModule.UnregisterRenderer(_renderer);
			httpModule.UnregisterResponseHandler(_handler);
			httpModule.UnregisterDefaultFiles("default.cshtml");
			httpModule.UnregisterDefaultFiles("index.cshtml");
		}
	}
}
