#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public struct DSHeader
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
		public byte[] Magic;
		public UInt32 PageSize;
		public UInt32 FpmPageNumber;
		public UInt32 NumPages;
		public UInt32 DirectorySize;
		public UInt32 PageMap; //should be 0 in the header
	}

	public class MSFReader : ReaderBase
	{
		private readonly PDBType type;

		private DSHeader hdr;

		private byte[] streamTableList;
		private byte[] streamTable;

		public MSFReader(Stream msf, PDBType type) : base(msf) {
			this.type = type;
			this.hdr = ReadStruct<DSHeader>();
		}

		public uint GetPageLocation(uint pageNumber) {
			return hdr.PageSize * pageNumber;
		}

		public uint GetNumPages(uint numBytes) {
			return (numBytes + hdr.PageSize - 1) / hdr.PageSize;
		}

		public IEnumerable<byte[]> GetPages(uint numPages) {
			for(int i = 0; i < numPages; i++) {
				var pageNum = ReadUInt32();
				yield return ReadPage(pageNum);
			}
		}

		public IEnumerable<byte[]> GetPages(long offset, uint numPages) {
			return PerformAt(offset, () => GetPages(numPages));
		}

		private IEnumerable<byte[]> GetPages_StreamTableList() {
			var numStreamTablePages = GetNumPages(hdr.DirectorySize);
			var numListPages = GetNumPages(numStreamTablePages);

			long offset = Marshal.SizeOf<DSHeader>();
			return GetPages(offset, numListPages);
		}

		private IEnumerable<byte[]> GetPages_StreamTable() {
			var numStreamTablePages = GetNumPages(hdr.DirectorySize);

			var rdr = new BinaryReader(new MemoryStream(streamTableList));
			for(int i = 0; i < numStreamTablePages; i++) {
				uint pageNum = rdr.ReadUInt32();
				yield return ReadPage(pageNum);
			}
		}

		private byte[] StreamTableList() {
			if(streamTableList != null)
				return streamTableList;

			streamTableList = GetPages_StreamTableList().SelectMany(x => x).ToArray();
			return streamTableList;
		}

		public byte[] StreamTable() {
			if(streamTableList == null) {
				StreamTableList();
			}

			if(streamTable != null)
				return streamTable;

			streamTable = GetPages_StreamTable().SelectMany(x => x).ToArray();
			return streamTable;
		}

		public byte[] ReadPage(uint pageNumber) {
			return PerformAt<byte[]>(pageNumber * hdr.PageSize, () => {
				Trace.WriteLine($"Reading Page {pageNumber} @ {pageNumber * hdr.PageSize:X8}");
				return ReadBytes((int)hdr.PageSize);
			});
		}
	}
}
