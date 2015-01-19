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
	/// Summary description for RangeAttributesTest
	/// </summary>
	[TestClass]
	public class RangeAttributeTest
	{
		[TestMethod]
		public void ItShouldBePossibleToSetBothMinMax()
		{
			var sc = new RangeAttribute(10, 20);
			Assert.AreEqual(10, sc.Min);
			Assert.AreEqual(20, sc.Max);
		}


		[TestMethod]
		public void ItShouldBePossibleToSetMinOnly()
		{
			var sc = new RangeAttribute(10);
			Assert.AreEqual(10, sc.Min);
			Assert.AreEqual(null, sc.Max);
		}

		public void DoComapare<T>(object min, object max, object lower, object mid, object greater)
		{
			var type = typeof(T);
			var sc = new RangeAttribute((T)min);
			Assert.IsFalse(sc.IsValid((T)lower, type), type.ToString());
			Assert.IsTrue(sc.IsValid((T)mid, type), type.ToString());

			sc = new RangeAttribute((T)min, (T)max);
			Assert.IsFalse(sc.IsValid((T)lower, type), type.ToString());
			Assert.IsTrue(sc.IsValid((T)mid, type), type.ToString());
			Assert.IsFalse(sc.IsValid((T)greater, type), type.ToString());
		}

		[TestMethod]
		public void CompareInt32()
		{
			DoComapare<Int32>(10, 40, 1, 30, 100);
		}

		[TestMethod]
		public void CompareDecimal()
		{
			DoComapare<Decimal>((Decimal)10, (Decimal)40, (Decimal)1, (Decimal)30, (Decimal)100);
		}

		[TestMethod]
		public void CompareInt64()
		{
			DoComapare<Int64>((Int64)10, (Int64)40, (Int64)1, (Int64)30, (Int64)100);
		}

		[TestMethod]
		public void CompareDouble()
		{
			DoComapare<Double>(10.0, 40.0, 1.0, 30.0, 100.0);
		}

		[TestMethod]
		public void CompareSingle()
		{
			DoComapare<Single>((Single)10.0, (Single)40.0, (Single)1.0, (Single)30.0, (Single)100.0);
		}
	}
}
