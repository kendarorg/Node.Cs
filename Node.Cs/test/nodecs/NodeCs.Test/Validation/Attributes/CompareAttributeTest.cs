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


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeCs.Shared.Attributes.Validation;

namespace NodeCs.Test.Validation.Attributes
{
	/// <summary>
	/// Summary description for CompareAttributesTest
	/// </summary>
	[TestClass]
	public class CompareAttributeTest
	{
		[TestMethod]
		public void ItShouldBePossibleToSetTheWithField()
		{
			var sc = new CompareAttribute("FieldA");
			Assert.AreEqual("FieldA", sc.WithField);
		}

		[TestMethod]
		public void CompareValidStrings()
		{
			var sc = new CompareAttribute("FieldA");
			Assert.IsFalse(sc.IsValid("value", "Value"));
			Assert.IsTrue(sc.IsValid("value", "value"));
		}

		[TestMethod]
		public void CompareValueTypes()
		{
			var sc = new CompareAttribute("FieldA");
			Assert.IsFalse(sc.IsValid(12, 44));
			Assert.IsTrue(sc.IsValid(23, 23));
		}

		[TestMethod]
		public void CompareStructs()
		{
			var d1 = new DateTime(1000, 1, 1);
			var d2 = new DateTime(1000, 1, 1);
			var d3 = new DateTime(1001, 1, 1);
			var sc = new CompareAttribute("FieldA");
			Assert.IsFalse(sc.IsValid(d1, d3));
			Assert.IsTrue(sc.IsValid(d1, d2));
		}
	}
}
