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


using System.Collections.Generic;

namespace Http.Contexts.ContentUtils
{
	public static class ByteMatch
	{
		public static int IndexOf(byte[] self, byte candidate, int startAt = 0, int length = -1)
		{
			if (startAt >= self.Length)
			{
				return -1;
			}
			if (length == -1) length = self.Length;
			else length = startAt + length;

			for (int i = startAt; i < length; i++)
			{
				if (candidate != self[i])
				{
					continue;
				}

				return i;
			}
			return -1;
		}

		public static int IndexOf(byte[] self, byte[] candidate, int startAt = 0, int length = -1)
		{
			if (IsEmptyLocate(self, candidate))
			{
				return -1;
			}
			if (startAt >= self.Length)
			{
				return -1;
			}
			if (length == -1) length = self.Length;
			else length = startAt + length;

			for (int i = startAt; i < length; i++)
			{
				if (!IsMatch(self, i, candidate))
				{
					continue;
				}

				return i;
			}
			return -1;
		}

		public static IEnumerable<int> Matches(byte[] self, byte[] candidate, int startAt = 0, int length = -1)
		{
			if (IsEmptyLocate(self, candidate))
			{
				yield break;
			}

			if (startAt >= self.Length)
			{
				yield break;
			}
			if (length == -1) length = self.Length;
			else length = startAt + length;

			for (int i = startAt; i < length; i++)
			{
				if (!IsMatch(self, i, candidate))
				{
					continue;
				}

				yield return i;
			}
		}

		private static bool IsMatch(byte[] array, int position, byte[] candidate)
		{
			if (candidate.Length > (array.Length - position))
			{
				return false;
			}

			for (int i = 0; i < candidate.Length; i++)
			{
				if (array[position + i] != candidate[i])
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsEmptyLocate(byte[] array, byte[] candidate)
		{
			return array == null
				|| candidate == null
				|| array.Length == 0
				|| candidate.Length == 0
				|| candidate.Length > array.Length;
		}
	}
}
