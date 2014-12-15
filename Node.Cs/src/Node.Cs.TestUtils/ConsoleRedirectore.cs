using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Node.Cs.Test
{
	public class ConsoleRedirector : TextWriter
	{
		public List<string> Data { get; private set; }

		public ConsoleRedirector()
		{
			Data = new List<string>();
		}

		public override void Write(char value)
		{
			if (Data.Count == 0)
			{
				Data.Add(string.Empty);
			}
			if (Data[Data.Count - 1].EndsWith("\r\n"))
			{
				Data.Add(value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				Data[Data.Count - 1] += value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public override void Write(string value)
		{
			if (Data.Count == 0)
			{
				Data.Add(string.Empty);
			}
			if (Data[Data.Count - 1].EndsWith("\r\n"))
			{
				Data.Add(value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				Data[Data.Count - 1] += value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public override Encoding Encoding
		{
			get { return Encoding.ASCII; }
		}
	}
}
