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
using System.Collections.Generic;
using System.IO;
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
			var tar = (TargetFrameworkAttribute)_dll
					.GetCustomAttributes(typeof(TargetFrameworkAttribute)).First();
		}

		public string GetName()
		{
			return _dll.GetName().FullName;
		}

		public string[] GetTypes()
		{
			var types =_dll.GetTypes();
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
				_name = tar.FrameworkName.Substring(las + 1).Replace(".","");
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