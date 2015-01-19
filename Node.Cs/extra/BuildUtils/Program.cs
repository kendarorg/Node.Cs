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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using GenericHelpers;
using System.Reflection;

namespace BuildUtils
{
	class Program
	{
		private static string _nugetExe;

		static void Main(string[] args)
		{
			var helpMessage = ResourceContentLoader.LoadText("Help.txt", Assembly.GetExecutingAssembly());
			var clp = new CommandLineParser(args, helpMessage);
			if (clp.Has("makenuspec"))
			{
				MakeNuspec(clp["makenuspec"]);
			}
			if (clp.Has("nuget"))
			{
				_nugetExe = clp["nuget"];
			}
			if (clp.Has("pack", "nuget", "nuspec", "sourceDir", "destDir", "dlls"))
			{
				BuildNuget(clp["nuspec"], clp["sourceDir"], clp["destDir"], clp["dlls"]);
			}
		}

		private static void BuildNuget(string nuspecFile, string sourceDir, string destDir, string dlls)
		{
			
		}

		private static void MakeNuspec(string csprojFile)
		{
			var csprojName = Path.GetFileNameWithoutExtension(csprojFile);
			var nuspecPath = Path.Combine(Path.GetDirectoryName(csprojFile), csprojName + ".nuspec");
			var document = XDocument.Load(csprojFile);
			var result = document.Descendants("PropertyGroup").FirstOrDefault(n => n.Descendants("PostBuildEvent").Any());
			if (result == null)
			{
				result = new XElement("PropertyGroup");
				result.Add(new XElement("PostBuildEvent"));
				document.Add(result);
			}
			var rewriteCsproj = true;
			var pbe = result.Descendants("PostBuildEvent").First();
			var value = pbe.Value;
			if (string.IsNullOrWhiteSpace(value))
			{
				pbe.Value = ResourceContentLoader.LoadText("nuspecCsproj.txt", Assembly.GetExecutingAssembly());
			}
			else if (value.ToLowerInvariant().IndexOf("nuget pack", 0, value.Length, StringComparison.InvariantCultureIgnoreCase) < 0)
			{
				pbe.Value += "\r\n" + ResourceContentLoader.LoadText("nuspecCsproj.txt", Assembly.GetExecutingAssembly());
			}
			else
			{
				rewriteCsproj = false;
			}

			if (rewriteCsproj)
			{
				using (var xtw = new XmlTextWriter(csprojFile, Encoding.UTF8))
				{
					xtw.Formatting = Formatting.Indented; // optional, if you want it to look nice
					document.WriteTo(xtw);
				}
			}
			if (!File.Exists(nuspecPath))
			{
				var template = ResourceContentLoader.LoadText("nuspecTemplate.xml", Assembly.GetExecutingAssembly());
				template = template.Replace("$(ProjectName)", csprojName);
				File.WriteAllText(csprojFile, template, Encoding.UTF8);
			}
		}
	}
}
