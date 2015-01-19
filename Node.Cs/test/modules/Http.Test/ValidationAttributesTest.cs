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

namespace Http.Test
{
	[TestClass]
	public class ValidationAttributesTest
	{
		#region StringLengthAttribute
		[TestMethod]
		public void StringLengthAttribute_validating_zero_max_length()
		{
			var an = new StringLengthAttribute(0);
			Assert.AreEqual(0, an.MinimumLength);
			Assert.IsTrue(an.IsValid(null, typeof(string)));
			Assert.IsTrue(an.IsValid(string.Empty, typeof(string)));
			Assert.IsFalse(an.IsValid("valid", typeof(string)));
		}

		[TestMethod]
		public void StringLengthAttribute_valid_with_length_non_zero()
		{
			var an = new StringLengthAttribute(5);
			Assert.AreEqual(0, an.MinimumLength);
			Assert.IsTrue(an.IsValid(null, typeof(string)));
			Assert.IsTrue(an.IsValid(string.Empty, typeof(string)));
			Assert.IsTrue(an.IsValid("valid", typeof(string)));
			Assert.IsFalse(an.IsValid("invalid", typeof(string)));
		}


		[TestMethod]
		public void StringLengthAttribute_valid_with_min_length_not_zero()
		{

			var an = new StringLengthAttribute(5)
			{
				MinimumLength = 1
			};
			Assert.AreEqual(1, an.MinimumLength);
			Assert.IsFalse(an.IsValid(null, typeof(string)));
			Assert.IsFalse(an.IsValid(string.Empty, typeof(string)));
			Assert.IsTrue(an.IsValid("valid", typeof(string)));
			Assert.IsFalse(an.IsValid("invalid", typeof(string)));
		}
		#endregion

		#region RangeAttribute
		[TestMethod]
		public void RangeAttribute_validating_zero_min_length()
		{
			var an = new RangeAttribute(0);
			Assert.AreEqual(0, an.Min);
			Assert.IsTrue(an.IsValid(1, null));
			Assert.IsFalse(an.IsValid(string.Empty, typeof(string)));
			Assert.IsTrue(an.IsValid("0", typeof(string)));
			Assert.IsTrue(an.IsValid(Int64.MaxValue, typeof(Int64)));
		}

		[TestMethod]
		public void RangeAttribute_valid_with_length_non_zero()
		{
			var an = new RangeAttribute(5);
			Assert.AreEqual(5, an.Min);
			Assert.IsFalse(an.IsValid(null, typeof(string)));
			Assert.IsFalse(an.IsValid(string.Empty, typeof(string)));
			Assert.IsTrue(an.IsValid("44", typeof(string)));
			Assert.IsFalse(an.IsValid("invalid", typeof(string)));
		}


		[TestMethod]
		public void RangeAttribute_valid_with_min_length_not_zero()
		{

			var an = new RangeAttribute(1, 5);
			Assert.AreEqual(1, an.Min);
			Assert.AreEqual(5, an.Max);
			Assert.IsFalse(an.IsValid(null, typeof(string)));
			Assert.IsFalse(an.IsValid(string.Empty, typeof(string)));
			Assert.IsTrue(an.IsValid("4", typeof(string)));
			Assert.IsFalse(an.IsValid("28", typeof(string)));
		}
		#endregion
	}
}
