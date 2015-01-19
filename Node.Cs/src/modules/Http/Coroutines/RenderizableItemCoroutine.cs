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


using CoroutinesLib.Shared;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.PathProviders;
using Http.Shared.Renderers;
using NodeCs.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Http.Coroutines
{
	public class RenderizableItemCoroutine : CoroutineBase
	{
		private readonly IRenderer _renderer;
		private readonly string _relativePath;
		private readonly IPathProvider _pathProvider;
		private readonly IHttpContext _context;
		private readonly Func<Exception, IHttpContext, bool> _specialHandler;
		private readonly object _model;
		private readonly ModelStateDictionary _modelStateDictionary;
		private readonly object _viewBag;

		public RenderizableItemCoroutine(IRenderer renderer, string relativePath, IPathProvider pathProvider, 
			IHttpContext context, Func<Exception, IHttpContext, bool> specialHandler, object model, 
			ModelStateDictionary modelStateDictionary, object viewBag)
		{
			InstanceName = "RenderItem(" + renderer.GetType().Namespace + "." + renderer.GetType().Name + "," + relativePath + ")";
			_renderer = renderer;
			_relativePath = relativePath;
			_pathProvider = pathProvider;
			_context = context;
			_specialHandler = specialHandler;
			_model = model;
			_modelStateDictionary = modelStateDictionary;
			_viewBag = viewBag;
		}

		public override void Initialize()
		{

		}
		private bool _iInitializedSession = false;

		public override IEnumerable<ICoroutineResult> OnCycle()
		{
			var completed = false;
			var result = new byte[0];
			var lastModification = DateTime.MaxValue;
			yield return CoroutineResult.RunAndGetResult(_pathProvider.GetStream(_relativePath, _context),
				string.Format("RenderItem::IPathProvider::GetStream('{0}',context)",
						_relativePath))
				.OnComplete<StreamResult>((r) =>
				{
					completed = true;
					result = r.Result;
					lastModification = r.LastModification;
				})
				.WithTimeout(TimeSpan.FromSeconds(60))
				.AndWait();

			if (!completed)
			{
				throw new HttpException(500, string.Format("Error loading '{0}'.", _relativePath));
			}

			Exception thrownException = null;
			var target = new MemoryStream(result);

			if (_renderer.IsSessionCapable)
			{
				if (_context.Session == null)
				{
					var sessionManager = ServiceLocator.Locator.Resolve<ISessionManager>();
					if (sessionManager != null)
					{
						_iInitializedSession = true;
						sessionManager.InitializeSession(_context);
					}
				}
			}

			yield return CoroutineResult.Run(_renderer.Render(_relativePath, lastModification, target,
					_context, _model, _modelStateDictionary, _viewBag),
					string.Format("RenderItem::Render '{0}'", _relativePath))
					.WithTimeout(TimeSpan.FromMinutes(1))
					.OnError((e) =>
					{
						thrownException = e;
						return true;
					})
					.AndWait();
			if (thrownException != null)
			{
				throw new HttpException(500,
					thrownException.Message + ": " + thrownException.GetType().Namespace + thrownException.GetType().Name, thrownException);
			}
			if (_iInitializedSession)
			{
				if (_context.Session != null)
				{
					var sessionManager = ServiceLocator.Locator.Resolve<ISessionManager>();
					sessionManager.SaveSession(_context);
				}
			}
			TerminateElaboration();

		}

		public override void OnEndOfCycle()
		{

		}

		public override bool OnError(Exception exception)
		{
			if (_specialHandler != null) _specialHandler(exception, _context);
			if (_iInitializedSession)
			{
				if (_context.Session != null)
				{
					var sessionManager = ServiceLocator.Locator.Resolve<ISessionManager>();
					sessionManager.SaveSession(_context);
				}
			}

			return true;
		}
	}
}
