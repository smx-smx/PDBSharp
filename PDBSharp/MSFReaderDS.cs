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
	public class MSFReaderDS : MSFReader
	{
		public MSFReaderDS(Memory<byte> mem) : base(mem) {
		}

		public MSFReaderDS(MemoryMappedFile mfile, long length) : base(mfile, length) {
		}

		/// <summary>
		/// Reads the page list, which can be split across multiple pages
		/// </summary>
		/// <returns></returns>
		private IEnumerable<byte[]> GetPages_StreamTableList() {
			// number of pages to represent the list of streams
			var numStreamTablePages = GetNumPages(Header.DirectorySize);

			long offset = Marshal.SizeOf(Header);

			// number of pages to represent the list of pages
			var numListPages = GetNumPages(numStreamTablePages);
			return GetPages(offset, numListPages);
		}

		/// <summary>
		/// Reads the pages that make up the Stream Table
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<byte[]> GetPages_StreamTable() {
			byte[] streamTableList = GetPages_StreamTableList()
				.SelectMany(x => x)
				.ToArray();

			var rdr = new BinaryReader(new MemoryStream(streamTableList));

			var numStreamTablePages = GetNumPages(Header.DirectorySize);
			for (int i = 0; i < numStreamTablePages; i++) {
				uint pageNum = ReadPageNumber(rdr);
				yield return ReadPage(pageNum);
			}
		}


		/// <summary>
		/// Reads a given page
		/// </summary>
		/// <param name="pageNumber">page number to read</param>
		/// <returns></returns>
		public override byte[] ReadPage(uint pageNumber) {
			long offset = pageNumber * Header.PageSize;

			byte[] data = Span.Slice((int)offset, (int)Header.PageSize).ToArray();
			return data;
		}

		/// <summary>
		/// Reads a series of pages
		/// </summary>
		/// <param name="offset">offset to read at</param>
		/// <param name="numPages">number of pages to read</param>
		/// <returns></returns>
		public override IEnumerable<byte[]> GetPages(long offset, uint numPages) {
			int dataLength = (int)(numPages * sizeof(uint));
			return Span
				.Slice((int)offset, dataLength)
				.Cast<uint>()
				.ToArray()
				.Select(ReadPage);
		}

		public override uint ReadPageNumber(BinaryReader rdr) {
			return rdr.ReadUInt32();
		}
	}
}
