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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Node.Cs.Test
{
	[DebuggerStepThrough]
	[DebuggerNonUserCode]
	public static class SetsAssert
	{
		public static int Contains<T, TK>(ICollection collection, T contained, Func<TK, T> getValue, int position = -1)
		{
			var enumerator = collection.GetEnumerator();
			var currentPosition = 0;
			while (enumerator.MoveNext())
			{
				var current = getValue((TK)enumerator.Current);
				if (current.Equals(contained) || current.ToString() == contained.ToString())
				{
					if (position >= 0)
					{
						if (position != currentPosition)
						{
							Assert.Fail("Item '{0}' found at position '{1}' instead of '{2}'.", contained, currentPosition, position);
						}
					}
					return currentPosition;
				}
				currentPosition++;
			}
			Assert.Fail("No item '{0}' found.", contained);
			return -1;
		}

		public static int Contains<T>(ICollection collection, T contained, Func<T, T, bool> verifyFunction, int position = -1)
		{
			var enumerator = collection.GetEnumerator();
			var currentPosition = 0;
			while (enumerator.MoveNext())
			{
				var current = (T)enumerator.Current;
				if (verifyFunction(current, contained))
				{
					if (position >= 0)
					{
						if (position != currentPosition)
						{
							Assert.Fail("Item '{0}' found at position '{1}' instead of '{2}'.", contained, currentPosition, position);
						}
					}
					return currentPosition;
				}
				currentPosition++;
			}
			Assert.Fail("No item '{0}' found.", contained);
			return -1;
		}

		public static int Contains<T>(ICollection collection, T contained, int position = -1)
		{
			return Contains(collection, contained, (l, r) => l.Equals(r) || l.ToString() == r.ToString());
		}

		public static T ContainsKey<T>(IDictionary collection, T contained)
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var current in collection.Keys)
			{
				if (current.Equals(contained) || current.ToString() == contained.ToString())
				{
					return (T)current;
				}
			}
			Assert.Fail("No key '{0}' found.", contained);
			return default(T);
		}



		public static T ContainsValue<T>(IDictionary collection, T contained)
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var current in collection.Values)
			{
				if (current.Equals(contained) || current.ToString() == contained.ToString())
				{
					return (T)current;
				}
			}
			Assert.Fail("No value '{0}' found.", contained);
			return default(T);
		}

		public static void AreEqual<TK>(IEnumerable<TK> expected, IEnumerable<TK> result)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			var e = expected.ToArray();
			var r = result.ToArray();
			if (e.Length != r.Length)
			{
				Assert.Fail("Expected length is '{0}' whilst result length is '{1}'.", e.Length, r.Length);
			}
			for (int i = 0; i < e.Length; i++)
			{
				var ed = e[i];
				var rd = r[i];
				if (ed.Equals(rd) || ed.ToString() == rd.ToString())
				{
					continue;
				}
				Assert.Fail("Item at index '{0}' differ. Expected is '{1}' whilst result is '{2}'.", i, ed, rd);
			}
		}
	}
}
