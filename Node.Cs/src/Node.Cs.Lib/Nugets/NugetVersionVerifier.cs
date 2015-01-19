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
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Node.Cs.Nugets
{
	public class NugetVersionVerifier : INugetVersionVerifier
	{
		private static readonly Dictionary<string, string> _preQuery = new Dictionary<string, string>
		{
			{"#V1","(Id eq '#ID' and Version ge '#V1')"},
			{"(,#V2]","(Id eq '#ID' and Version le '#V2')"},
			{"(,#V2)","(Id eq '#ID' and Version lt '#V2')"},
			{"(#V1,)","(Id eq '#ID' and Version gt '#V1')"},
			{"[#V1,)","(Id eq '#ID' and Version ge '#V1')"},
			{"[#V1]","(Id eq '#ID' and Version eq '#V1')"},
			{"(#V1,#V2)","(Id eq '#ID' and Version gt '#V1' and Version lt '#V2')"},
			{"[#V1,#V2]","(Id eq '#ID' and Version ge '#V1' and Version le '#V2')"},
			{"(#V1,#V2]","(Id eq '#ID' and Version gt '#V1' and Version le '#V2')"},
			{"[#V1,#V2)","(Id eq '#ID' and Version ge '#V1' and Version lt '#V2')"},
			{"","(Id eq '#ID')"}
		};

		private static readonly Dictionary<string, Func<string, string, string, bool>> _isMatchingFuncs = new Dictionary<string, Func<string, string, string, bool>>
		{
			{"#V1", (toTest, l, r) => CompareInt(toTest,l)==0 },
			{"(,#V2]",(toTest, l, r) => CompareInt(toTest,r)<=0},
			{"(,#V2)",(toTest, l, r) => CompareInt(toTest,r)<0},
			{"(#V1,)",(toTest, l, r) => CompareInt(toTest,l)>0},
			{"[#V1,)",(toTest, l, r) => CompareInt(toTest,l)>=0},
			{"[#V1]",(toTest, l, r) => CompareInt(toTest,l)==0},
			{"(#V1,#V2)",(toTest, l, r) => CompareInt(toTest,l)>0 && CompareInt(toTest,r)<0},
			{"[#V1,#V2]", (toTest, l, r) => CompareInt(toTest,l)>=0 && CompareInt(toTest,r)<=0},
			{"(#V1,#V2]",(toTest, l, r) => CompareInt(toTest,l)>0 && CompareInt(toTest,r)<=0},
			{"[#V1,#V2)",(toTest, l, r) => CompareInt(toTest,l)>=0 && CompareInt(toTest,r)<0},
			{"",(toTest, l, r) => true}
		};


		public string BuildODataQuery(string id, string version)
		{
			id = id.Trim().Replace(" ", "");
			if (string.IsNullOrEmpty(version)) return DoReplace(string.Empty, id, null, null);
			version = version.Trim().Replace(" ", "");
			var parts = version.Split(',');
			if (parts.Length == 1)
			{
				if (version.StartsWith("[") || version.StartsWith("(") || version.EndsWith(")") || version.EndsWith("]"))
				{
					if (!(version.StartsWith("[") && version.EndsWith("]")))
					{
						throw new Exception("Invalid characters in nuget version string.");
					}
					return DoReplace("[#V1]", id, version.Trim('[', ']'), null);
				}
				return DoReplace("#V1", id, version.Trim('[', ']'), null);
			}
			if (parts.Length != 2)
			{
				throw new Exception("Invalid version format.");
			}
			if (!((version.StartsWith("[") || version.StartsWith("(")) || !(version.EndsWith(")") || version.EndsWith("]"))))
			{
				throw new Exception("Invalid termination characters in nuget version string.");
			}
			var resultQuery = "";
			var firstLimit = version[0] + "";
			var lastLimit = version[version.Length - 1] + "";
			var firstVersion = parts[0].Trim('[', '(');
			var secondVersion = parts[1].Trim(')', ']');
			if (firstLimit == "[" || firstLimit == "(") resultQuery += firstLimit;
			if (firstVersion.Length > 0) resultQuery += "#V1";
			resultQuery += ",";
			if (secondVersion.Length > 0) resultQuery += "#V2";
			if (lastLimit == "]" || lastLimit == ")") resultQuery += lastLimit;

			return DoReplace(resultQuery, id, firstVersion, secondVersion);
		}

		private static string DoReplace(string query, string id, string v1, string v2)
		{
			var result = _preQuery[query];
			if (!string.IsNullOrWhiteSpace(v1))
			{
				result = result.Replace("#V1", v1);
			}
			if (!string.IsNullOrWhiteSpace(v2))
			{
				result = result.Replace("#V2", v2);
			}
			return result.Replace("#ID", id);
		}

		public int Compare(string versionA, string versionB)
		{
			return CompareInt(versionA, versionB);
		}
		public static int CompareInt(string versionA, string versionB)
		{
			var l = ParseVersion(versionA);
			var r = ParseVersion(versionB);

			for (int i = 0; i < l.Version.Count && i < r.Version.Count; i++)
			{
				var lv = l.Version[i];
				var rv = r.Version[i];
				if (lv > rv) return 1;
				if (lv < rv) return -1;
			}
			if (l.Version.Count > r.Version.Count) return 1;
			if (l.Version.Count < r.Version.Count) return -1;


			if (l.Pre.Count == 0 && r.Pre.Count > 0) return 1;
			if (l.Pre.Count > 0 && r.Pre.Count == 0) return -1;

			for (int i = 0; i < l.Pre.Count && i < r.Pre.Count; i++)
			{
				var lv = l.Pre[i];
				var rv = r.Pre[i];
				int lvValue;
				int rvValue;
				if (int.TryParse(lv, out lvValue) && int.TryParse(rv, out rvValue))
				{
					if (lvValue > rvValue) return 1;
					if (lvValue < rvValue) return -1;
				}
				else
				{
					var cmp = string.Compare(lv, rv, StringComparison.InvariantCultureIgnoreCase);
					if (cmp != 0) return cmp;
				}
			}
			if (l.Pre.Count < r.Pre.Count) return -1;
			if (l.Pre.Count > r.Pre.Count) return 1;
			return 0;
		}

		public bool IsVersionMatching(string foundedVersion, string version)
		{

			if (string.IsNullOrEmpty(version)) return true;
			version = version.Trim().Replace(" ", "");
			var parts = version.Split(',');
			if (parts.Length == 1)
			{
				if (version.StartsWith("[") || version.StartsWith("(") || version.EndsWith(")") || version.EndsWith("]"))
				{
					if (!(version.StartsWith("[") && version.EndsWith("]")))
					{
						throw new Exception("Invalid characters in nuget version string.");
					}
					return _isMatchingFuncs["[#V1]"](foundedVersion, version.Trim('[', ']'), null);
				}
				return _isMatchingFuncs["#V1"](foundedVersion, version.Trim('[', ']'), null);
			}
			if (parts.Length != 2)
			{
				throw new Exception("Invalid version format.");
			}
			if (!((version.StartsWith("[") || version.StartsWith("(")) || !(version.EndsWith(")") || version.EndsWith("]"))))
			{
				throw new Exception("Invalid termination characters in nuget version string.");
			}
			var resultQuery = "";
			var firstLimit = version[0] + "";
			var lastLimit = version[version.Length - 1] + "";
			var firstVersion = parts[0].Trim('[', '(');
			var secondVersion = parts[1].Trim(')', ']');
			if (firstLimit == "[" || firstLimit == "(") resultQuery += firstLimit;
			if (firstVersion.Length > 0) resultQuery += "#V1";
			resultQuery += ",";
			if (secondVersion.Length > 0) resultQuery += "#V2";
			if (lastLimit == "]" || lastLimit == ")") resultQuery += lastLimit;
			return _isMatchingFuncs[resultQuery](foundedVersion, firstVersion, secondVersion);
		}

		internal class VersionNumber
		{
			public List<int> Version = new List<int>();
			public List<string> Pre = new List<string>();
		}

		private static VersionNumber ParseVersion(string v)
		{
			var result = new VersionNumber();
			var exploded = v.Split('-');

			var version = exploded[0].Split('.');
			foreach (var item in version)
			{
				result.Version.Add(int.Parse(item));
			}
			if (exploded.Length > 1)
			{
				result.Pre.AddRange(string.Join("-", exploded, 1, exploded.Length - 1).Split('.', '-'));
			}

			return result;
		}
	}
}
