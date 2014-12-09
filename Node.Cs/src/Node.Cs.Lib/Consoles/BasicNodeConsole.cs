using System;

namespace Node.Cs.Consoles
{
	public class BasicNodeConsole:INodeConsole
	{
		public void Write(string formatString, params object[] formatParameters)
		{
			Console.Write(formatString,formatParameters);
		}

		public void WriteLine(string formatString, params object[] formatParameters)
		{
			Console.WriteLine(formatString, formatParameters);
		}

		public string ReadLine()
		{
			return Console.ReadLine();
		}
	}
}
