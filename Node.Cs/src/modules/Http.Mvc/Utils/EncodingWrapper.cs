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


using System.Text;

namespace HttpMvc.Utils
{
	public class EncodingWrapper : Encoding
	{
		private readonly Encoding _encoding;

		public EncodingWrapper(Encoding encoding)
		{
			_encoding = encoding;
		}

		public override byte[] GetPreamble()
		{
			return new byte[0];
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return _encoding.GetByteCount(chars, index, count);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return _encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return _encoding.GetCharCount(bytes, index, count);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return _encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
		}

		public override int GetMaxByteCount(int charCount)
		{
			return _encoding.GetMaxByteCount(charCount);
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return _encoding.GetMaxCharCount(byteCount);
		}

		public override string BodyName
		{
			get { return _encoding.BodyName; }
		}

		public override object Clone()
		{
			return new EncodingWrapper((Encoding)_encoding.Clone());
		}

		public override int CodePage
		{
			get { return _encoding.CodePage; }
		}

		public override string EncodingName
		{
			get { return _encoding.EncodingName; }
		}

		public override Decoder GetDecoder()
		{
			return _encoding.GetDecoder();
		}

		public override Encoder GetEncoder()
		{
			return _encoding.GetEncoder();
		}

		public override string HeaderName
		{
			get { return _encoding.HeaderName; }
		}

		public override bool IsAlwaysNormalized(NormalizationForm form)
		{
			return _encoding.IsAlwaysNormalized(form);
		}

		public override bool IsBrowserDisplay
		{
			get { return _encoding.IsBrowserDisplay; }
		}

		public override bool IsBrowserSave
		{
			get { return _encoding.IsBrowserSave; }
		}

		public override bool IsMailNewsDisplay
		{
			get { return _encoding.IsMailNewsDisplay; }
		}

		public override bool IsMailNewsSave
		{
			get { return _encoding.IsMailNewsSave; }
		}

		public override bool IsSingleByte
		{
			get { return _encoding.IsSingleByte; }
		}

		public override string WebName
		{
			get { return _encoding.WebName; }
		}

		public override int WindowsCodePage
		{
			get { return _encoding.WindowsCodePage; }
		}
	}
}