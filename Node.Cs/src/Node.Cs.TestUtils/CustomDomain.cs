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
				_name = tar.FrameworkName.Substring(las + 1);
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