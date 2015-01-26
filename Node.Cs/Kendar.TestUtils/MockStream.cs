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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kendar.TestUtils
{
	public class MockStream : MemoryStream
	{
		public void Initialize()
		{
			Sw = new Stopwatch();
			ClosesCall = 0;
			WrittenBytes = 0;
			Seek(0, SeekOrigin.Begin);
			SetLength(0);
		}

		public Stopwatch Sw { get; private set; }
		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }

		public int ClosesCall { get; private set; }
		public override void Close()
		{
			End = DateTime.Now;
			Sw.Stop();
			ClosesCall++;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (WrittenBytes == 0)
			{
				Start = DateTime.Now;
				Sw.Start();
			}
			AddBytes(count);
			base.Write(buffer, offset, count);
		}

		private void AddBytes(int count)
		{
			WrittenBytes += count;
		}

		public int WrittenBytes { get; private set; }

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (WrittenBytes == 0)
			{
				Start = DateTime.Now;
				Sw.Start();
			}
			AddBytes(count);
			return base.WriteAsync(buffer, offset, count, cancellationToken);
		}
	}
}
