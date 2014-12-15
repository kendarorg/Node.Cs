using System;

namespace Node.Cs.Exceptions
{
	public class NugetDownloadException : Exception
	{
        public NugetDownloadException(string format, params object[] pars)
			:base(string.Format(format,pars))
		{
			
		}
	}
}
