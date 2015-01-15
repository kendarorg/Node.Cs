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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Node.Cs.Test
{
	public static class PathResolver
	{
		public static string GetAssemblyPath(Assembly asm)
		{
			var asmUri = new UriBuilder(asm.CodeBase);
			return Path.GetDirectoryName(asmUri.Path);
		}
		public static string GetSolutionRoot()
		{
			var caller = Assembly.GetCallingAssembly();
			var baseDir = GetAssemblyPath(caller);
			return FindFirstOfType(baseDir, "*.sln");
		}

		public static string FindFirstOfType(string path, string pattern)
		{
			path = path.ToPath();
			if (Directory.Exists(path))
			{
				var filesMatching = Directory.GetFiles(path, pattern);
				if (filesMatching.Any())
				{
					return path;
				}
			}
			var parent = Path.GetDirectoryName(path);
			if (parent == null) return null;
			return FindFirstOfType(parent, pattern);
		}

		public static string FindByPath(string path,Assembly asm=null)
		{
			path = path.ToPath();
			if (File.Exists(path))
			{
				return path;
			}
			
			if (!Path.IsPathRooted(path))
			{
				var dir = Path.GetDirectoryName(path);
				var file = Path.GetFileName(path);
				var asmDir = GetAssemblyPath(asm??Assembly.GetCallingAssembly());
				// ReSharper disable AssignNullToNotNullAttribute
				if (!string.IsNullOrEmpty(dir))
				{
					path = Path.Combine(asmDir, dir, file);
				}
				else
				{
					path = Path.Combine(asmDir, file);
				}
				// ReSharper restore AssignNullToNotNullAttribute
			}
			if (File.Exists(path))
			{
				return path;
			}

			var fdir = Path.GetDirectoryName(path);
			var ffile = Path.GetFileName(path);


			var resultDir = FindFirstOfType(fdir, ffile);

			if (resultDir == null)
			{
				fdir = GetAssemblyPath(asm ?? Assembly.GetCallingAssembly());
				resultDir = FindFirstOfType(fdir, ffile);
			}

			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(resultDir, ffile);
		}
	}
}
