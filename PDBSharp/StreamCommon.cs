#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public abstract class StreamCommon
	{
		protected readonly Stream Stream;

		public Stream BaseStream => Stream;

		public StreamCommon(Stream stream) {
			this.Stream = stream;
		}

		public void PerformAt(long offset, Action action) {
			long curPos = Stream.Position;
			Stream.Position = offset;
			action.Invoke();
			Stream.Position = curPos;
		}

		public T PerformAt<T>(long offset, Func<T> action) {
			long curPos = Stream.Position;
			Stream.Position = offset;
			T result = action.Invoke();
			Stream.Position = curPos;
			return result;
		}

		public int AlignStream(uint alignment) {
			long position = (Stream.Position + alignment - 1) & ~(alignment - 1);
			long skipped = position - Stream.Position;
			Stream.Position = position;
			return (int)skipped;
		}
	}
}
