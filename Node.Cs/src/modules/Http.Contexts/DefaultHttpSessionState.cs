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


using System.Web.SessionState;
using System;
using System.Collections;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using Http.Shared.Contexts;

namespace Http.Contexts
{
	public class DefaultHttpSessionState : HttpSessionStateBase, IHttpSession
	{
		public DefaultHttpSessionState() { }
		public object SourceObject { get { return _httpSessionState; } }
		private readonly HttpSessionState _httpSessionState;

		public DefaultHttpSessionState(HttpSessionState httpSessionState)
		{
			_httpSessionState = httpSessionState;
			InitializeUnsettable();
		}

		public override void Abandon()
		{
			_isChanged = true;
			_httpSessionState.Abandon();
		}

		public override void Add(String name, Object value)
		{
			_isChanged = true;
			_httpSessionState.Add(name, value);
		}

		public override void Clear()
		{
			_isChanged = true;
			_httpSessionState.Clear();
		}

		public override void Remove(String name)
		{
			_isChanged = true;
			_httpSessionState.Remove(name);
		}

		public override void RemoveAll()
		{
			_isChanged = true;
			_httpSessionState.RemoveAll();
		}

		public override void RemoveAt(Int32 index)
		{
			_isChanged = true;
			_httpSessionState.RemoveAt(index);
		}

		public override void CopyTo(Array array, Int32 index)
		{
			if (array != null)
			{
				_httpSessionState.CopyTo(array, index);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return _httpSessionState.GetEnumerator();
		}

		public override Int32 CodePage
		{
			set
			{
				_httpSessionState.CodePage = value;
			}
			get
			{
				return _httpSessionState.CodePage;
			}
		}

		public override HttpSessionStateBase Contents { get { return null; } }

		public void SetContents(HttpSessionStateBase val)
		{

		}


		public override HttpCookieMode CookieMode { get { return _httpSessionState.CookieMode; } }

		public void SetCookieMode(HttpCookieMode val)
		{
		}


		public override Boolean IsCookieless { get { return _httpSessionState.IsCookieless; } }

		public void SetIsCookieless(Boolean val)
		{
		}


		public override Boolean IsNewSession { get { return _httpSessionState.IsNewSession; } }

		public void SetIsNewSession(Boolean val)
		{
		}


		public override Boolean IsReadOnly { get { return _httpSessionState.IsReadOnly; } }

		public void SetIsReadOnly(Boolean val)
		{
		}


		public override NameObjectCollectionBase.KeysCollection Keys { get { return _httpSessionState.Keys; } }

		public void SetKeys(NameObjectCollectionBase.KeysCollection val)
		{
		}

		public override Int32 LCID
		{
			set
			{
				_httpSessionState.LCID = value;
			}
			get
			{
				return _httpSessionState.LCID;
			}
		}


		public override SessionStateMode Mode { get { return _httpSessionState.Mode; } }

		public void SetMode(SessionStateMode val)
		{
		}


		public override String SessionID { get { return _httpSessionState.SessionID; } }

		public void SetSessionID(String val)
		{
		}


		public override HttpStaticObjectsCollectionBase StaticObjects { get { return null; } }

		public void SetStaticObjects(HttpStaticObjectsCollectionBase val)
		{
		}

		public override Int32 Timeout
		{
			set
			{
				_isChanged = true;
				_httpSessionState.Timeout = value;
			}
			get
			{
				return _httpSessionState.Timeout;
			}
		}

		public override object this[int index]
		{
			get { return _httpSessionState[index]; }
			set
			{
				_isChanged = true;
				_httpSessionState[index] = value;
			}
		}

		public override object this[string name]
		{
			get { return _httpSessionState[name]; }
			set
			{
				_isChanged = true;
				_httpSessionState[name] = value;
			}
		}


		public override Int32 Count { get { return _httpSessionState.Count; } }

		public void SetCount(Int32 val)
		{
		}


		public override Boolean IsSynchronized { get { return _httpSessionState.IsSynchronized; } }

		public void SetIsSynchronized(Boolean val)
		{
		}


		public override Object SyncRoot { get { return _httpSessionState.SyncRoot; } }

		public void SetSyncRoot(Object val)
		{
		}
		public void InitializeUnsettable()
		{
		}

		private bool _isChanged;

		public bool IsChanged
		{
			get { return _isChanged; }
		}

		public void Initialize(System.Collections.Generic.Dictionary<string, object> initVals)
		{
			foreach (var kvp in initVals)
			{
				_httpSessionState.Add(kvp.Key, kvp.Value);
			}
		}


		public System.Collections.Generic.Dictionary<string, object> ItemDictionary
		{
			get
			{
				var result = new Dictionary<String, object>();
				foreach (string item in _httpSessionState.Keys)
				{
					var val = _httpSessionState[item];
					result.Add(item, val);
				}

				return result;
			}
		}

		public void SetIsChanged(bool val)
		{
			
		}
	}
}