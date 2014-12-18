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


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Node.Cs
{
	[TestClass]
	public class StringPathExtensionTest
	{
		[TestMethod]
		public void ToPath_ShouldConvertSlashToDirSeparator()
		{
			const string src = "C://test/dir.htm";
			var res = src.ToPath();
			Assert.IsTrue(Path.IsPathRooted(res));
		}


		[TestMethod]
		public void ToPath_ShouldConvertBacklashToDirSeparator()
		{
			const string src = "C:\\\\test\\dir.htm";
			var res = src.ToPath();
			Assert.IsTrue(Path.IsPathRooted(res));
		}

		[TestMethod]
		public void ToUrl_ShouldConvertDoubleSlashesToSingle()
		{
			const string src = "http://test.com//other";
			const string exp = "http://test.com/other";
			var res = src.ToUrl();
			Assert.AreEqual(exp,res);
		}


		[TestMethod]
		public void ToUrl_ShouldConvertBacklashToDirSeparator()
		{
			const string src = "\\test.com/other";
			const string exp = "/test.com/other";
			var res = src.ToUrl();
			Assert.AreEqual(exp, res);
		}

		[TestMethod]
		public void ToUrl_ShouldConvertDoubleSlashesToSingleIgnoringProtocol()
		{
			const string src = "http://test.com//other";
			const string exp = "http://test.com/other";
			var res = src.ToUrl();
			Assert.AreEqual(exp, res);
		}

		[TestMethod]
		public void ToUrl_ShouldConvertDoubleSlashesToSingleIgnoringProtocol_WithMixedSlashes()
		{
			const string src = "http:/\\test.com/\\other";
			const string exp = "http://test.com/other";
			var res = src.ToUrl();
			Assert.AreEqual(exp, res);
		}
	}
}