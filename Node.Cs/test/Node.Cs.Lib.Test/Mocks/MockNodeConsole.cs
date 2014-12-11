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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Data[Data.Count - 1] += string.Format(formatString, formatParameters);
        }

        public void WriteLine(string formatString, params object[] formatParameters)
        {
            Data[Data.Count - 1] += string.Format(formatString, formatParameters);
            Data.Add(string.Empty);
        }

        public string ReadLine()
        {
            return LineContent;
        }

        public void Exit(int errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
