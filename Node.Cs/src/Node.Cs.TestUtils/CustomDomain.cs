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
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace Node.Cs.Test
{
	[Serializable]
	public class CustomDomain : MarshalByRefObject
	{


		private Assembly _dll;
		private string _name;


		public void LoadDll(string path)
		{
			_dll = Assembly.LoadFrom(path);
		}

		public string GetName()
		{
			return _dll.GetName().FullName;
		}

		public string[] GetTypes()
		{
			var types = _dll.GetTypes();
			var result = new string[types.Length];
			for (int index = 0; index < types.Length; index++)
			{
				var type = types[index];
				result[index] = type.Namespace + "." + type.Name;
			}
			return result;
		}

		public void LoadDll(byte[] bytes)
		{
			try
			{
				_dll = Assembly.Load(bytes);
				var tar = (TargetFrameworkAttribute)_dll
					.GetCustomAttributes(typeof(TargetFrameworkAttribute)).First();
				var las = tar.FrameworkName.LastIndexOf("v", StringComparison.Ordinal);
				_name = tar.FrameworkName.Substring(las + 1).Replace(".", "");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public bool IsDllLoaded()
		{
			return _dll != null;
		}

		internal string GetFrameworkVersion()
		{
			return _name;
		}
	}
}