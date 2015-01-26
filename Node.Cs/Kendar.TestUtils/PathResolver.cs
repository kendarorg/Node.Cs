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
using System.Reflection;

namespace Kendar.TestUtils
{
	public static class PathResolver
	{
		public static string ToPath( string src)
		{
			return src
				.Replace('/', Path.DirectorySeparatorChar)
				.Replace('\\', Path.DirectorySeparatorChar);
		}

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
			path = ToPath(path);
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

		public static string FindByPath(string path, Assembly asm = null)
		{
			path = ToPath(path);
			if (File.Exists(path))
			{
				return path;
			}

			if (!Path.IsPathRooted(path))
			{
				var dir = Path.GetDirectoryName(path);
				var file = Path.GetFileName(path);
				var asmDir = GetAssemblyPath(asm ?? Assembly.GetCallingAssembly());
				// ReSharper disable AssignNullToNotNullAttribute
				// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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
