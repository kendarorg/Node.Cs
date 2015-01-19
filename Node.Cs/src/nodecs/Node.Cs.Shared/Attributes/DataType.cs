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


namespace NodeCs.Shared.Attributes
{
	// Summary:
	//     Represents an enumeration of the data types associated with data fields and
	//     parameters.
	public enum DataType
	{
		// Summary:
		//     Represents a custom data type.
		Custom = 0,
		//
		// Summary:
		//     Represents an instant in time, expressed as a date and time of day.
		DateTime = 1,
		//
		// Summary:
		//     Represents a date value.
		Date = 2,
		//
		// Summary:
		//     Represents a time value.
		Time = 3,
		//
		// Summary:
		//     Represents a continuous time during which an object exists.
		Duration = 4,
		//
		// Summary:
		//     Represents a phone number value.
		PhoneNumber = 5,
		//
		// Summary:
		//     Represents a currency value.
		Currency = 6,
		//
		// Summary:
		//     Represents text that is displayed.
		Text = 7,
		//
		// Summary:
		//     Represents an HTML file.
		Html = 8,
		//
		// Summary:
		//     Represents multi-line text.
		MultilineText = 9,
		//
		// Summary:
		//     Represents an e-mail address.
		EmailAddress = 10,
		//
		// Summary:
		//     Represent a password value.
		Password = 11,
		//
		// Summary:
		//     Represents a URL value.
		Url = 12,
		//
		// Summary:
		//     Represents a URL to an image.
		ImageUrl = 13,
		//
		// Summary:
		//     Represents a credit card number.
		CreditCard = 14,
		//
		// Summary:
		//     Represents a postal code.
		PostalCode = 15,
		//
		// Summary:
		//     Represents file upload data type.
		Upload = 16,
	}
}