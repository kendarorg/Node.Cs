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


using System.Collections.Generic;
using CoroutinesLib.Shared.Logging;
using Http.Shared;
using Http.Shared.Contexts;
using System;
using NodeCs.Shared;

namespace Http
{
	public class FilterHandler : IFilterHandler, ILoggable
	{
		private readonly List<IFilter> _globalFilters = new List<IFilter>();
		public FilterHandler()
		{
			Log = ServiceLocator.Locator.Resolve<ILogger>();
		}
		public void AddFilter(IFilter instance)
		{
			_globalFilters.Add(instance);
		}

		public void AddFilter(Type type)
		{
			throw new NotImplementedException();
		}

		public void RemoveFilter(IFilter instance)
		{
			_globalFilters.Remove(instance);
		}

		public void RemoveFilter(Type type)
		{
			throw new NotImplementedException();
		}

		public bool OnPreExecute(IHttpContext context)
		{
			try
			{
				for (int index = 0; index < _globalFilters.Count; index++)
				{
					var filter = _globalFilters[index];
					if (!filter.OnPreExecute(context))
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return false;
			}

			return true;
		}

		public void OnPostExecute(IHttpContext context)
		{
			try
			{
				for (int index = 0; index < _globalFilters.Count; index++)
				{
					var filter = _globalFilters[index];
					filter.OnPostExecute(context);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		public ILogger Log { get; set; }
	}
}
