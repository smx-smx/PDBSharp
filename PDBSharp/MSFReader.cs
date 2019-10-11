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
using System.Text;

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

		public string GetMagic() {
			fixed(byte *ptr = Magic) {
				if(*(ushort*)&ptr[PDBFile.DS_OFFSET] == 0x5344) { //DS (Little Endian)
					return Encoding.ASCII.GetString(ptr, PDBFile.BIG_MAGIC.Length);
				}
				if(*(ushort*)&ptr[PDBFile.JG_OFFSET] == 0x474A) { //JG (Little Endian)
					return Encoding.ASCII.GetString(ptr, PDBFile.SMALL_MAGIC.Length);
				}

				throw new InvalidDataException("Invalid magic");
			}
		}

		public void SetMagic(string magic) {
			byte[] data = Encoding.ASCII.GetBytes(magic);
			fixed (byte* ptr = Magic) {
				new Span<byte>((void*)ptr, MAGIC_SIZE).WriteBytes(0, data);
			}
		}
	}

	public unsafe class MSFReader : IDisposable
	{
		private byte[] streamTableList;
		private byte[] streamTable;

        public DSHeader Header { get; }

        private readonly long Length;

		// used for memory based sources
		private Memory<byte> memory;
		
		// used for file based sources
		private readonly MemoryMappedSpan mf;

		public Span<byte> Span {
			get {
				if (mf != null) {
					return mf.GetSpan();
				}
				return memory.Span;
			}
		}

		private unsafe DSHeader ReadHeader() {
			int size = sizeof(DSHeader);
			return Span.Read<DSHeader>(0);
		}

		public MSFReader(Memory<byte> mem) {
			this.memory = mem;
			this.Header = ReadHeader();
		}

		public MSFReader(MemoryMappedFile mfile, long length) {
			this.mf = new MemoryMappedSpan(mfile, length);
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
			uint[] pageNums = Span
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

			byte[] data = Span.Slice((int)offset, (int)Header.PageSize).ToArray();
			return data;
		}

		public void Dispose() {
			mf?.Dispose();
		}
	}
}
