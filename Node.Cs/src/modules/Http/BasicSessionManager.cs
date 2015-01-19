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
using NodeCs.Shared.Caching;
using System;
using System.Collections.Generic;

namespace Http
{
	public class BasicSessionManager : ISessionManager
	{
		private ICacheEngine _cacheEngine;

		public void SaveSession(IHttpContext context)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICoroutineResult> InitializeSession(IHttpContext context)
		{
			if (_cacheEngine == null) yield break;
			if (context.Session != null) yield break;

			var newSession = new SimpleHttpSessionState();
			var cookie = context.Request.Cookies[SessionConstants.SessionId];
			if (cookie == null)
			{
				var nodecsSid = Guid.NewGuid().ToString();
				cookie = new System.Web.HttpCookie(SessionConstants.SessionId, nodecsSid);
				context.Response.SetCookie(cookie);
			}
			newSession.SetSessionID(cookie.Value);
			var realSession = newSession;
			yield return _cacheEngine.AddAndGet(new CacheDefinition
			{
				Value = newSession,
			}, (a) => realSession = (SimpleHttpSessionState)a, "basicSessionManager");
			context.SetSession(realSession);
		}

		public void SetCachingEngine(ICacheEngine cacheEngine)
		{
			_cacheEngine = cacheEngine;
			_cacheEngine.AddGroup(new CacheGroupDefinition
			{
				ExpireAfter = TimeSpan.FromMinutes(20),
				Id = "basicSessionManager",
				RollingExpiration = true
			});
		}
	}
}
