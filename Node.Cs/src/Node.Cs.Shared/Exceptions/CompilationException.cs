using System;

namespace Node.Cs.Exceptions
{
	public class CompilationException : Exception
	{
		public string CompiledSource { get; private set; }

		public CompilationException(string message,string source = "")
			: base(string.Format(message))
		{
			CompiledSource = source;
		}
	}
}