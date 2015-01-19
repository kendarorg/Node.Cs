// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System.Diagnostics;
using Http.Shared.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeCs.Shared.Attributes;

namespace NodeCs.Test.Validation
{
	/// <summary>
	/// Summary description for AttributestTest
	/// </summary>
	[TestClass]
	public class AttributestTest
	{

		[TestMethod]
		public void ActionNameAttribute()
		{
			var an = new ActionName("test");
			Assert.AreEqual(an.Name, "test");
		}

		[TestMethod]
		public void BindAttribute()
		{
			var an = new BindAttribute
			{
				Exclude = "test"
			};
			Assert.AreEqual(an.Exclude, "test");
		}

		[TestMethod]
		public void DataType()
		{
			var an = new DataTypeAttribute(Shared.Attributes.DataType.DateTime);
			Assert.IsTrue(an.IsValid(new Stopwatch(), typeof(Stopwatch)));
			Assert.AreEqual(an.DataType, Shared.Attributes.DataType.DateTime);
		}

		[TestMethod]
		public void DisplayAttributeTest()
		{
			var an = new DisplayAttribute {Name = "test"};
			Assert.AreEqual(an.Name, "test");
		}

		[TestMethod]
		public void DisplayNameAttributeTest()
		{
			var an = new DisplayNameAttribute("test");
			Assert.AreEqual(an.Name, "test");
		}



		[TestMethod]
		public void HttpMethodsAttributes()
		{
			var de = new HttpDeleteAttribute("test");
			Assert.AreEqual(de.Action, "test");
			Assert.AreEqual(de.Verb, "DELETE");

			var g = new HttpGetAttribute("test");
			Assert.AreEqual(g.Action, "test");
			Assert.AreEqual(g.Verb, "GET");

			var pu = new HttpPutAttribute("test");
			Assert.AreEqual(pu.Action, "test");
			Assert.AreEqual(pu.Verb, "PUT");

			var p = new HttpPostAttribute("test");
			Assert.AreEqual(p.Action, "test");
			Assert.AreEqual(p.Verb, "POST");

			
			var r = new HttpRequestTypeAttribute("webDav","test");
			Assert.AreEqual(r.Action, "test");
			Assert.AreEqual(r.Verb, "WEBDAV");
		}

		[TestMethod]
		public void Scaffold()
		{
			var an = new ScaffoldColumnAttribute(true);
			Assert.IsTrue(an.Scaffold);

			an = new ScaffoldColumnAttribute(false);
			Assert.IsFalse(an.Scaffold);
		}
	}
}
