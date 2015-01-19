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


using System.IO;
using System.Web;

namespace Http.Contexts.GenericUtils
{
	public class PostedFile : HttpPostedFileBase
	{
		private readonly string _fileName;
		private readonly Stream _stream;
		private readonly string _contentType;
		private readonly int _length;

		public PostedFile(string fileName, MemoryStream stream, string contentType)
		{
			_fileName = fileName;
			_stream = stream;
			_stream.Seek(0, SeekOrigin.Begin);
			_contentType = contentType;
			_length = (int)_stream.Length;
		}

		public PostedFile(HttpPostedFile httpPostedFile)
		{
			_fileName = httpPostedFile.FileName;
			_stream = httpPostedFile.InputStream;
			_contentType = httpPostedFile.ContentType;
			_length = httpPostedFile.ContentLength;
		}

		public override string FileName
		{
			get { return _fileName; }
		}

		public override Stream InputStream
		{
			get { return _stream; }
		}

		public override string ContentType
		{
			get { return _contentType; }
		}

		public override int ContentLength
		{
			get { return _length; }
		}

		public override void SaveAs(string filename)
		{
			var ms = _stream as MemoryStream;
			if (ms != null)
			{
				ms.Seek(0, SeekOrigin.Begin);
				File.WriteAllBytes(filename, ms.ToArray());
				return;
			}
			using (var streamReader = new MemoryStream())
			{
				InputStream.CopyTo(streamReader);
				File.WriteAllBytes(filename, streamReader.ToArray());
			}
		}
	}

}
