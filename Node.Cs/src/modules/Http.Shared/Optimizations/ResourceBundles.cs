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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Http.Shared.PathProviders;

namespace Http.Shared.Optimizations
{
	public class ResourceBundles : IResourceBundles
	{
		private readonly List<IPathProvider> _pathProviders;
		private readonly string _virtualDir;
		private readonly Dictionary<string, IBundle> _script = new Dictionary<string, IBundle>(StringComparer.InvariantCultureIgnoreCase);
		private readonly Dictionary<string, IBundle> _style = new Dictionary<string, IBundle>(StringComparer.InvariantCultureIgnoreCase);

		public ResourceBundles(string virtualDir, List<IPathProvider> pathProviders)
		{
			_pathProviders = pathProviders;
			_virtualDir = virtualDir.Trim('/');
		}

		public void Add(IBundle include)
		{
			var fisAdd = new List<string>();
			for (int index = 0; index < include.FisicalAddresses.Count; index++)
			{
				var item = include.FisicalAddresses[index].Trim('/');
				if (item.StartsWith("~"))
				{
					item = item.Trim('~', '/');
					item =  "/" + item;
				}
				if (item.Contains("{") || item.Contains("*"))
				{
					fisAdd.AddRange(FindRealPaths(item));
				}
				else
				{
					fisAdd.Add("/" + _virtualDir +item);
				}

			}
			var script = include as ScriptBundle;
			if (script != null)
			{
				_script[script.LogicalAddress] = new ScriptBundle(script.LogicalAddress, fisAdd);
			}
			else
			{
				var style = include as StyleBundle;
				if (style != null)
				{
					_style[style.LogicalAddress] = new StyleBundle(style.LogicalAddress, fisAdd);
				}
			}
		}

		private IEnumerable<string> FindRealPaths(string item)
		{
			var splittedItems = item.Split(new string[]{"/"},StringSplitOptions.RemoveEmptyEntries);
			var baseDir = string.Empty;
			foreach (var splittedItem in splittedItems)
			{
				if (splittedItem.IndexOf("*", StringComparison.OrdinalIgnoreCase) > 0 ||
					splittedItem.IndexOf("{version}", StringComparison.OrdinalIgnoreCase) > 0)
				{
					var re = splittedItem.Replace("*", "(.)*").Replace("{version}", "(.)*");
					var regex = new Regex(re);
					foreach (var pp in _pathProviders)
					{
						var ppr = pp;
						foreach (var sf in ppr.FindFiles(baseDir))
						{
							var splittedSf = sf.Split('/');
							var lastf = splittedSf.Last();
							if (regex.IsMatch(lastf))
							{
								yield return "/" + _virtualDir + baseDir+ "/"+lastf;
							}
						}
					}
				}
				else
				{
					baseDir = "/" + _virtualDir + baseDir + "/" + splittedItem;
				}
			}
		}

		public StileHelper GetStyles()
		{
			return new StileHelper(_style);
		}

		public ScriptHelper GetScripts()
		{
			return new ScriptHelper(_script);
		}
	}

	public abstract class BundleHelper
	{
		private readonly Dictionary<string, IBundle> _data;

		protected BundleHelper(Dictionary<string, IBundle> data)
		{
			_data = data;
		}

		public string Render(string index)
		{
			if (_data.ContainsKey(index))
			{
				return BuildData(_data[index]);
			}
			return string.Empty;
		}

		private string BuildData(IBundle bundle)
		{
			var sb = new StringBuilder();
			foreach (var item in bundle.FisicalAddresses)
			{
				AddItem(sb, item);
			}
			return sb.ToString();
		}

		protected abstract void AddItem(StringBuilder sb, string item);
	}

	public class StileHelper : BundleHelper
	{
		public StileHelper(Dictionary<string, IBundle> data)
			: base(data)
		{
		}

		protected override void AddItem(StringBuilder sb, string item)
		{
			sb.Append(string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"></link>", item));
		}
	}

	public class ScriptHelper : BundleHelper
	{
		public ScriptHelper(Dictionary<string, IBundle> data)
			: base(data)
		{
		}

		protected override void AddItem(StringBuilder sb, string item)
		{
			sb.Append(string.Format("<script type=\"javascript\" src=\"{0}\"></script>", item));
		}
	}
}