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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public class MSFWriter
	{
		private readonly PDBType type;

		private IHeader hdr = new DSHeader();

		private uint currentPage = 0;

		public readonly StreamTableWriter StreamTable;

		private SpanStream msf;

		public Memory<byte> Memory => msf.Memory;

		public uint PageSize {
			get => hdr.PageSize;
			set => hdr.PageSize = value;
		}

		public MSFWriter() {
			hdr.Magic = PDBFile.BIG_MAGIC;
			StreamTable = new StreamTableWriter(this);
		}

		private void SeekPage(uint pageNum) {
			msf.Position = pageNum * hdr.PageSize;
		}

		public unsafe long GetDataSize() {
			uint directoryPages = GetNumPages(StreamTable.GetCurrentSize());
			uint directoryPageListCount = GetNumPages(directoryPages);
			uint directorySize = directoryPageListCount * PageSize;
			uint maxDirectoryPages = directorySize / sizeof(UInt32);

			long dataSize = 0;
			dataSize += sizeof(DSHeader);
			dataSize += directorySize;
			dataSize += sizeof(uint) * maxDirectoryPages;

			dataSize += StreamTable.GetDataSize();
			return dataSize;
		}

		public void Commit() {
			msf = new SpanStream(new byte[(int)GetDataSize()]);

			uint directoryPages = GetNumPages(StreamTable.GetCurrentSize());
			uint directoryPageListCount = GetNumPages(directoryPages);
			uint directorySize = directoryPageListCount * PageSize;
			uint maxDirectoryPages = directorySize / sizeof(UInt32);

			hdr.DirectorySize = directorySize;
			WriteHeader();
			SeekPage(1);

			for (int i = 0; i < directoryPageListCount; i++) {
				msf.WriteUInt32(AllocPageNumber());
			}

			for (uint i = directoryPageListCount; i < maxDirectoryPages; i++) {
				msf.WriteInt32(-1);
			}

			StreamTable.Commit();
		}

		private unsafe void WritePage<T>(uint pageNum, T data) where T : unmanaged {
			if(sizeof(T) > hdr.PageSize) {
				throw new ArgumentOutOfRangeException(typeof(T).Name + $" is bigger than PageSize {hdr.PageSize}");
			}
			long offset = hdr.PageSize * pageNum;
			msf.WriteAt(offset, data);
		}

		private void WriteHeader() {
			WritePage(AllocPageNumber(), (DSHeader)hdr);
		}

		public uint AllocPageNumber() {
			return currentPage++;
		}

		public byte[] AllocPage(out uint pageNum) {
			pageNum = AllocPageNumber();
			return new byte[hdr.PageSize];
		}

		public void WritePages(Dictionary<uint, byte[]> pages) {
			foreach (var pageNum in pages.Keys) {
				msf.WriteUInt32(pageNum);
			}

			foreach (var page in pages) {
				WritePage(page.Key, page.Value);
			}
		}

		public void WritePage(uint pageNumber, byte[] pageData) {
			Debug.Assert(pageData.Length == hdr.PageSize);

			SeekPage(pageNumber);
			msf.WriteBytes(pageData);
		}

		//public void WriteStreamTable()

		public uint GetNumPages(uint numBytes) {
			return (numBytes + hdr.PageSize - 1) / hdr.PageSize;
		}
	}
}
