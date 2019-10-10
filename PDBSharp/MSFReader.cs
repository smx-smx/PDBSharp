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
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	public unsafe struct DSHeader
	{
		public const int MAGIC_SIZE = 32;

		public fixed byte Magic[MAGIC_SIZE];
		public UInt32 PageSize;
		public UInt32 FpmPageNumber;
		public UInt32 NumPages;
		public UInt32 DirectorySize;
		public UInt32 PageMap; //should be 0 in the header
	}

	public unsafe class MSFReader : IDisposable
	{
		private readonly PDBType type;
		private byte[] streamTableList;
		private byte[] streamTable;

        public DSHeader Header { get; }

        private readonly long Length;

		private readonly MemoryMappedSpan mf;

		private unsafe DSHeader ReadHeader() {
			var span = this.mf.GetSpan();
			int size = sizeof(DSHeader);
			return span.Read<DSHeader>(0);
		}

		public MSFReader(MemoryMappedFile mfile, long length, PDBType type) {
			this.mf = new MemoryMappedSpan(mfile, length);
			this.type = type;
			this.Header = ReadHeader();
		}

		public uint GetPageLocation(uint pageNumber) {
			return Header.PageSize * pageNumber;
		}

		public uint GetNumPages(uint numBytes) {
			return (numBytes + Header.PageSize - 1) / Header.PageSize;
		}

		private long GetDataSize(uint numPages) {
			return numPages * Header.PageSize;
		}

		public IEnumerable<byte[]> GetPages(long offset, uint numPages) {
			int dataLength = (int)(numPages * sizeof(uint));	
			uint[] pageNums = mf.GetSpan()
				.Slice((int)offset, dataLength)
				.Cast<uint>()
				.ToArray();

			foreach (uint pageNum in pageNums) {
				yield return ReadPage(pageNum);
			}		
		}

		private IEnumerable<byte[]> GetPages_StreamTableList() {
			// number of pages to represent the list of streams
			var numStreamTablePages = GetNumPages(Header.DirectorySize);
			// number of pages to represent the list of pages
			var numListPages = GetNumPages(numStreamTablePages);

			long offset = Marshal.SizeOf<DSHeader>();
			return GetPages(offset, numListPages);
		}

		private IEnumerable<byte[]> GetPages_StreamTable() {
			var numStreamTablePages = GetNumPages(Header.DirectorySize);

			var rdr = new BinaryReader(new MemoryStream(streamTableList));
			for (int i = 0; i < numStreamTablePages; i++) {
				uint pageNum = rdr.ReadUInt32();
				yield return ReadPage(pageNum);
			}
		}

		private byte[] StreamTableList() {
			if (streamTableList != null)
				return streamTableList;

			streamTableList = GetPages_StreamTableList()
				.SelectMany(x => x)
				.ToArray();

			return streamTableList;
		}

		public byte[] StreamTable() {
			if (streamTableList == null) {
				StreamTableList();
			}

			if (streamTable != null)
				return streamTable;

			streamTable = GetPages_StreamTable().SelectMany(x => x).ToArray();
			return streamTable;
		}

		public byte[] ReadPage(uint pageNumber) {
			long offset = pageNumber * Header.PageSize;

			byte[] data = mf.GetSpan().Slice((int)offset, (int)Header.PageSize).ToArray();
			return data;
		}

		public void Dispose() {
			mf.Dispose();
		}
	}
}
