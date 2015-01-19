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
using System.Text.RegularExpressions;
using CoroutinesLib.Shared;
using System.Collections.Generic;
using Http.Shared.Contexts;

namespace Http.Shared.PathProviders
{
	public class StreamResult
	{
		public StreamResult(DateTime lastModification, byte[] result)
		{
			LastModification = lastModification;
			Result = result;
		}
		public DateTime LastModification { get; private set; }
		public byte[] Result { get; private set; }
	}

	public interface IPathProvider
	{
		/// <summary>
		/// Find all files inside the given dir
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		IEnumerable<string> FindFiles(string dir);

		/// <summary>
		/// Find all dirs inside the given dir
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		IEnumerable<string> FindDirs(string dir);

		/// <summary>
		/// Find if a path exists
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="isDir"></param>
		/// <returns></returns>
		bool Exists(string relativePath,out bool isDir);

		/// <summary>
		/// Must return a StreamResult
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		IEnumerable<ICoroutineResult> GetStream(string relativePath, IHttpContext context);

		/// <summary>
		/// To true if the directory content must be shown
		/// </summary>
		/// <param name="showDirectoryContent"></param>
		void ShowDirectoryContent(bool showDirectoryContent);
	}
}