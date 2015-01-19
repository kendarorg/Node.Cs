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
using Http.Contexts;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.PathProviders;
using System;
using System.Collections.Generic;
using System.IO;
using NodeCs.Shared;

namespace Http.Coroutines
{
	public class StaticItemCoroutine : CoroutineBase
	{
		private readonly string _relativePath;
		private readonly IPathProvider _pathProvider;
		private readonly IHttpContext _context;
		private readonly Func<Exception, IHttpContext, bool> _specialHandler;

		public StaticItemCoroutine(string relativePath, IPathProvider pathProvider, IHttpContext context,
			Func<Exception, IHttpContext, bool> specialHandler)
		{
			InstanceName = Guid.NewGuid().ToString().Replace("-","")+" WriteFromPathProvider(" + Path.GetFileName(relativePath) + ")";
			_relativePath = relativePath;
			_pathProvider = pathProvider;
			_context = context;
			_specialHandler = specialHandler;
		}



		public override void Initialize()
		{

		}

		public override IEnumerable<ICoroutineResult> OnCycle()
		{
			var completed = false;
			byte[] result = null;
			var lastModification = DateTime.MaxValue;

			yield return CoroutineResult.RunAndGetResult(_pathProvider.GetStream(_relativePath, _context),
				string.Format("{0}::GetStream('{1}',context)",
						InstanceName,Path.GetFileName(_relativePath)))
				.OnComplete<StreamResult>((r) =>
				{
					completed = true;
					result = r.Result;
					lastModification = r.LastModification;
				})
				.WithTimeout(TimeSpan.FromSeconds(60))
				.AndWait();

			if (!completed || result == null)
			{
				throw new HttpException(500, string.Format("Error loading '{0}'.", _relativePath));
			}

			var target = _context.Response.OutputStream;

			var source = new MemoryStream(result);
			source.Seek(0, SeekOrigin.Begin);
			//Wait that the copy is completed
            /*
			try
			{
				source.CopyToAsync(target)
					.ContinueWith((c) => _con text.Resp  onse.Close());
			}
			catch (Exception)
			{
				
			}*/
            yield return CoroutineResult.RunTask(source.CopyToAsync(target)).AndWait();

			TerminateElaboration();
            

		}

		public override bool OnError(Exception exception)
		{
			if (_specialHandler != null) _specialHandler(exception,_context);

			return true;
		}

		public override void OnEndOfCycle()
		{
		}
	}
}
