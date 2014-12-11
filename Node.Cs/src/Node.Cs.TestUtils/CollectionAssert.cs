// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
