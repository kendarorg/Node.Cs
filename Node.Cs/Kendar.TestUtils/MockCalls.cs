using System;
using System.Collections.Generic;
using System.Linq;

namespace Kendar.TestUtils
{
	public class MockCalls
	{
		public class AnyItem
		{
			public AnyItem(Type itemType)
			{
				ItemType = itemType;
			}
			public Type ItemType { get; private set; }
		}
		public List<Tuple<string, object[]>> Run = new List<Tuple<string, object[]>>();
		protected void AddTuple(string dotest, params object[] pars)
		{
			Run.Add(new Tuple<string, object[]>(dotest, pars));
		}

		public AnyItem IsAny<T>()
		{
			return new AnyItem(typeof(T));
		}

		public bool Verify(string name, params object[] pars)
		{
			return VerifyMany(name, 1, pars);
		}
		public bool VerifyNone(string name, params object[] pars)
		{
			return VerifyMany(name, 0, pars);
		}

		public bool VerifyMany(string name, int howMany, params object[] pars)
		{
			var corresponding = Run.Where(a => a.Item1 == name);
			var founded = 0;
			foreach (var item in corresponding)
			{
				var call = item;
				if (call.Item2.Length != pars.Length)
				{
					continue;
				}
				for (int i = 0; i < call.Item2.Length; i++)
				{
					var result = call.Item2[i];
					var expect = pars[i];
					var type = expect as AnyItem;
					if (type != null)
					{
						if (result != null && result.GetType() != type.ItemType) return false;
					}
					else
					{
						if (expect != result) return false;
					}
				}
				founded++;
			}
			return founded == howMany;
		}
	}
}
