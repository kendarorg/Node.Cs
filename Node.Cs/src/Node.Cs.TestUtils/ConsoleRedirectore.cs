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
