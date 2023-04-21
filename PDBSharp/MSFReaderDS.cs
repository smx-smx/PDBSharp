#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using Smx.SharpIO;

namespace Smx.PDBSharp
{
	public class MSFReaderDS : MSFReader
	{
		public MSFReaderDS(Memory<byte> mem) : base(mem, PDBType.Big) { }

		/// <summary>
		/// Reads the page list, which can be split across multiple pages
		/// </summary>
		/// <returns></returns>
		private IEnumerable<byte[]> GetPages_StreamTablePagesList() {
			// number of pages to represent the list of streams
			var numStreamTablePages = GetNumPages(Header.StreamTableSize);

			long offset = Marshal.SizeOf(Header);

			// the page list might span multiple pages itself
			var numListPages = GetNumPages(numStreamTablePages * sizeof(uint));

			// read in the page list
			return GetPages(offset, numListPages);
		}

		/// <summary>
		/// Reads the pages that make up the Stream Table
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<byte[]> GetPages_StreamTable() {
			Memory<byte> streamTablePageListData = GetPages_StreamTablePagesList()
				.SelectMany(x => x)
				.ToArray();

			SpanStream stream = new SpanStream(streamTablePageListData);

			var numStreamTablePages = GetNumPages(Header.StreamTableSize);
			for (int i = 0; i < numStreamTablePages; i++) {
				uint pageNum = stream.ReadUInt32();
				yield return ReadPage(pageNum);
			}
		}


		/// <summary>
		/// Reads a series of pages
		/// </summary>
		/// <param name="offset">offset to read at</param>
		/// <param name="numPages">number of pages to read</param>
		/// <returns></returns>
		public override IEnumerable<byte[]> GetPages(long offset, uint numPages) {
			int dataLength = (int)(numPages * sizeof(uint));
			return Memory.Span
				.Slice((int)offset, dataLength)
				.Cast<uint>()
				.ToArray()
				.Select(ReadPage);
		}
	}
}
