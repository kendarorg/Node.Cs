using System.IO;

// ReSharper disable once CheckNamespace
namespace System
{
	public static class StringPathExtension
	{
		public static string ToPath(this string src)
		{
			return src
				.Replace('/', Path.DirectorySeparatorChar)
				.Replace('\\', Path.DirectorySeparatorChar);
		}
		
		public static string ToUrl(this string src)
		{
			return src
				.Replace('\\', '/')
				.Replace("//","/");
		}
	}
}
