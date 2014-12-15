using System;

namespace Node.Cs.Exceptions
{
	public class MissingFunctionException : Exception
	{
		public MissingFunctionException(string format, params object[] pars)
			: base(string.Format(format, pars))
		{

		}
	}
}