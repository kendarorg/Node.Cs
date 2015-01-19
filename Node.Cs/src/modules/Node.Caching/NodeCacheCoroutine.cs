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


using System.Linq;
using System.Threading;
using ConcurrencyHelpers.Containers;
using CoroutinesLib.Shared;
using NodeCs.Shared.Caching;
using System.Collections.Generic;

namespace Node.Caching
{


	public class NodeCacheCoroutine : CoroutineBase
	{
		
		private readonly ICacheEngine _nodeCache;

		public NodeCacheCoroutine(ICacheEngine nodeCache)
		{
			_nodeCache = nodeCache;
			InstanceName = "NodeCacheCoroutine";

		}

		public override void Initialize()
		{

		}

		public override IEnumerable<ICoroutineResult> OnCycle()
		{
			if (_actionRequested.Data != null)
			{
				HandleActionRequested(_actionRequested.Data);
			}
			foreach (var item in _nodeCache.Execute())
			{
				yield return CoroutineResult.Wait;
			}
		}


		public override void OnEndOfCycle()
		{

		}

		public void Stop()
		{
			TerminateElaboration();
		}

		private readonly LockFreeItem<string> _actionRequested = new LockFreeItem<string>(null);
		private readonly LockFreeItem<object> _actionRequestedResult = new LockFreeItem<object>(null);



		private void HandleActionRequested(string data)
		{
			if (data == null) return;
			if (data == "ListGroups")
			{
				_actionRequestedResult.Data = new Dictionary<string,CacheGroupDefinition>(((NodeCache)_nodeCache).Groups);
				
			}else if (data.StartsWith("ListItems:"))
			{
				var groupId = data.Split('~').Last();
				_actionRequestedResult.Data = new Dictionary<string, CacheDefinition>(((NodeCache)_nodeCache).GetItems(groupId));
			}
			_actionRequested.Data = null;
		}

		public IEnumerable<CacheGroupDefinition> ListGroups()
		{
			_actionRequested.Data = "ListGroups";

			while (_actionRequested.Data == "ListGroups")
			{
				Thread.Sleep(250);
			}
			return _actionRequestedResult.Data as IEnumerable<CacheGroupDefinition>;
		}


		public IEnumerable<CacheDefinition> ListGroupItems(string groupId)
		{
			_actionRequested.Data = "ListItems~" + groupId;

			while (_actionRequested.Data == "ListItems~" + groupId)
			{
				Thread.Sleep(250);
			}
			return _actionRequestedResult.Data as IEnumerable<CacheDefinition>;
		}
	}
}