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
using System.Collections.Generic;

namespace NodeCs.Shared.Attributes.Validation
{
	public class RangeAttribute : ValidationAttribute, IValidationAttribute
	{
		private readonly object _min;
		private readonly object _max;

		public object Min
		{
			get { return _min; }
		}
		public object Max
		{
			get { return _max; }
		}

		public RangeAttribute(object min, object max = null)
		{
			_min = min;
			_max = max;
		}

		public bool IsValid(object value, Type type)
		{
			try
			{
				if (value == null) return false;

				var partial = false;
				var typeName = _min.GetType().Name;
				switch (typeName)
				{
					case ("Decimal"):
						partial = Convert.ToDecimal(value) >= Convert.ToDecimal(_min);
						break;
					case ("Int64"):
					case ("Int32"):
					case ("Int16"):
						partial = Convert.ToInt64(value) >= Convert.ToInt64(_min);
						break;
					case ("UInt64"):
					case ("UInt32"):
					case ("UInt16"):
					case ("Boolean"):
						partial = Convert.ToUInt64(value) >= Convert.ToUInt64(_min);
						break;
					case ("Double"):
						partial = Convert.ToDouble(value) >= Convert.ToDouble( _min);
						break;
					case ("Single"):
						partial = Convert.ToSingle(value) >= Convert.ToSingle( _min);
						break;
					case ("DateTime"):
						partial = Convert.ToDateTime(value).ToUniversalTime() >= Convert.ToDateTime(_min).ToUniversalTime();
						break;
				}

				if (!partial) return false;
				if (_max == null) return true;

				typeName = _max.GetType().Name;
				switch (typeName)
				{
					case ("Decimal"):
						partial = Convert.ToDecimal(value) <= Convert.ToDecimal(_max);
						break;
					case ("Int64"):
					case ("Int32"):
					case ("Int16"):
						partial = Convert.ToInt64(value) <=Convert.ToInt64( _max);
						break;
					case ("UInt64"):
					case ("UInt32"):
					case ("UInt16"):
					case ("Boolean"):
						partial = Convert.ToUInt64(value) <= Convert.ToUInt64(_max);
						break;
					case ("Double"):
						partial = Convert.ToDouble(value) <=Convert.ToDouble( _max);
						break;
					case ("Single"):
						partial = Convert.ToSingle(value) <= Convert.ToSingle(_max);
						break;
					case ("DateTime"):
						partial = Convert.ToDateTime(value).ToUniversalTime() <= Convert.ToDateTime(_max).ToUniversalTime();
						break;
				}

				return partial;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}