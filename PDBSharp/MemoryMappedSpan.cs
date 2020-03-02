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
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Smx.PDBSharp
{
	public unsafe class MemoryMappedSpan : IDisposable
	{
		public readonly long Length;

		private readonly MemoryMappedViewAccessor acc;
		private readonly byte* dptr = null;

		public MemoryMappedSpan(MemoryMappedFile mf, long length) {
			this.Length = length;
			this.acc = mf.CreateViewAccessor(0, length, MemoryMappedFileAccess.Read);
			this.acc.SafeMemoryMappedViewHandle.AcquirePointer(ref dptr);
		}

		public Span<byte> GetSpan() {
			return new Span<byte>((void*)dptr, (int)Length);
		}

		public void Dispose() {
			acc.Dispose();
		}
	}
}
