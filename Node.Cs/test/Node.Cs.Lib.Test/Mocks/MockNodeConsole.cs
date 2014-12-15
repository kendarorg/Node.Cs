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


using Node.Cs.Consoles;
using System.Collections.Generic;

namespace Node.Cs.Mocks
{
    public class MockNodeConsole : INodeConsole
    {
        public List<string> Data { get; private set; }
        public int ErrorCode { get; private set; }
        public string LineContent { get; set; }

        public MockNodeConsole()
        {
            Data = new List<string> { string.Empty };
        }

        public void Write(string formatString, params object[] formatParameters)
        {
	        var value = string.Format(formatString, formatParameters);
					if (Data.Count == 0)
					{
						Data.Add(string.Empty);
					}
					if (Data[Data.Count - 1].EndsWith("\r\n"))
					{
						Data.Add(value);
					}
					else
					{
						Data[Data.Count - 1] += value;
					}
        }

        public void WriteLine(string formatString, params object[] formatParameters)
        {
						Data.Add(string.Format(formatString, formatParameters)+"\r\n");
        }

        public string ReadLine()
        {
            return LineContent;
        }
    }
}
