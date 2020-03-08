#region License
/*
 * Copyright (C) 2020 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	class MSFReaderJG : MSFReader
	{
		public MSFReaderJG(Memory<byte> mem) : base(mem, PDBType.Small) {
		}

		public MSFReaderJG(MemoryMappedSpan memSpan) : base(memSpan, PDBType.Small) {
		}

		/// <summary>
		/// Reads a series of pages
		/// </summary>
		/// <param name="offset">offset to read at</param>
		/// <param name="numPages">number of pages to read</param>
		/// <returns></returns>
		public override IEnumerable<byte[]> GetPages(long offset, uint numPages) {
			int dataLength = (int)(numPages * sizeof(ushort));
			return Span
				.Slice((int)offset, dataLength)
				.Cast<ushort>()
				.ToArray()
				.Select(pageNum => ReadPage((uint)pageNum));
		}

		public override IEnumerable<byte[]> GetPages_StreamTable() {
			var numPages = GetNumPages(Header.DirectorySize);
			
			long offset = Marshal.SizeOf(Header);
			return GetPages(offset, numPages);
		}
	}
}
