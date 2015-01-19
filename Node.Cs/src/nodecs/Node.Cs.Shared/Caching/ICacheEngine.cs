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


using System.Collections;
using System.Collections.Generic;
using CoroutinesLib.Shared;
using System;

namespace NodeCs.Shared.Caching
{
	public interface ICacheEngine
	{
		/// <summary>
		/// Add group definition.
		/// Eventually add an item to it
		/// </summary>
		/// <param name="groupDefinition">The cache group definition</param>
		/// <param name="cacheItem">An eventual value</param>
		void AddGroup(CacheGroupDefinition groupDefinition, CacheDefinition cacheItem = null);

		/// <summary>
		/// Invalidate group
		/// </summary>
		/// <param name="groupId"></param>
		void InvalidateGroup(string groupId);

		/// <summary>
		/// Invalidate group
		/// </summary>
		/// <param name="groupId"></param>
		void RemoveGroup(string groupId);

		/// <summary>
		/// Override the default timeout for a group
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="groupId"></param>
		void SetGroupTimeout(TimeSpan timeout, string groupId = null);

		/// <summary>
		/// Add an item
		/// </summary>
		/// <param name="cacheItem"></param>
		/// <param name="groupId"></param>
		void AddItem(CacheDefinition cacheItem, string groupId = null);

		/// <summary>
		/// Add an item
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		/// <param name="groupId"></param>
		void AddItem(string id, object value, string groupId = null);

		/// <summary>
		/// Add an item, refresh if rolling
		/// </summary>
		/// <param name="cacheItem"></param>
		/// <param name="groupId"></param>
		void AddOrUpdateItem(CacheDefinition cacheItem,  string groupId = null);

		/// <summary>
		/// Add an item, refresh if rolling
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		/// <param name="groupId"></param>
		void AddOrUpdateItem(string id, object value, string groupId = null);

		/// <summary>
		/// Invalidate a cache entry
		/// </summary>
		/// <param name="id"></param>
		/// <param name="groupId"></param>
		void InvalidateItem(string id, string groupId = null);

		/// <summary>
		/// Add an item, and get it. Refresh if rolling.
		/// Should be used when a function is needed to get the value
		/// </summary>
		/// <param name="cacheItem"></param>
		/// <param name="groupId"></param>
		/// <param name="set"></param>
		/// <returns></returns>
		ICoroutineResult AddAndGet(CacheDefinition cacheItem, Action<object> set, string groupId = null);


		/// <summary>
		/// Add an item, and get it. Refresh if rolling.
		/// Should be used when a function is needed to get the value
		/// </summary>
		/// <param name="cacheItem"></param>
		/// <param name="groupId"></param>
		/// <param name="set"></param>
		/// <returns></returns>
		ICoroutineResult AddOrUpdateAndGet(CacheDefinition cacheItem, Action<object> set, string groupId = null);

		/// <summary>
		/// Retrieve an item
		/// </summary>
		/// <param name="cacheId"></param>
		/// <param name="groupId"></param>
		/// <param name="set"></param>
		/// <returns></returns>
		ICoroutineResult Get(string cacheId, Action<object> set, string groupId = null);

		/// <summary>
		/// Elaborate data
		/// </summary>
		/// <returns></returns>
		IEnumerable<ICoroutineResult> Execute();

	}
}
