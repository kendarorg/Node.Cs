
using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Node.Cs.Test
{
	[DebuggerStepThrough]
	[DebuggerNonUserCode]
	public static class SetsAssert
	{
		public static int Contains<T,TK>(ICollection collection, T contained, Func<TK,T> getValue, int position = -1)
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
	}
}
