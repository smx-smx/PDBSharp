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
	public class MSFWriter : WriterBase
	{
		private readonly PDBType type;

		private DSHeader hdr = new DSHeader();

		private uint lastPage = 0;

		public readonly StreamTableWriter StreamTable;

		public uint PageSize {
			get => hdr.PageSize;
			set => hdr.PageSize = value;
		}

		private unsafe void WriteMagic(string magicStr) {
			byte[] magic = Encoding.ASCII.GetBytes(magicStr);
			fixed(byte *ptr = hdr.Magic) {
				for(int i=0; i<magic.Length; i++) {
					ptr[i] = magic[i];
				}
			}
		}

		public MSFWriter(Stream stream) : base(stream) {
			WriteMagic(PDBFile.BIG_MAGIC);
			StreamTable = new StreamTableWriter(this, this.Stream);
		}

		public void Commit() {
			uint directoryPages = GetNumPages(StreamTable.GetCurrentSize());
			uint directoryPageListCount = GetNumPages(directoryPages);
			uint directorySize = directoryPageListCount * PageSize;

			hdr.DirectorySize = directorySize;
			WriteHeader();

			{
				byte[] directoryData = new byte[directorySize];
				uint maxDirectoryPages = directorySize / sizeof(UInt32);

				WriterBase directoryWriter = new WriterBase(new MemoryStream(directoryData));
				for (int i = 0; i < directoryPageListCount; i++) {
					uint pageNum = AllocPageNumber();
					directoryWriter.WriteUInt32(pageNum);
				}

				for (uint i = directoryPageListCount; i < maxDirectoryPages; i++) {
					directoryWriter.WriteInt32(-1);
				}

				WriteBytes(directoryData);
			}

			StreamTable.Commit();
		}

		private void WriteHeader() {
			byte[] hdrPage = AllocPage(out uint hdrPageNumber);
			WriterBase wr = new WriterBase(new MemoryStream(hdrPage));
			wr.WriteStruct<DSHeader>(hdr);

			WriteBytes(hdrPage);
		}

		public uint AllocPageNumber() {
			return lastPage++;
		}

		public byte[] AllocPage(out uint pageNum) {
			pageNum = AllocPageNumber();
			return new byte[hdr.PageSize];
		}

		public void WritePages(Dictionary<uint, byte[]> pages) {
			foreach (var pageNum in pages.Keys) {
				WriteUInt32(pageNum);
			}

			foreach (var page in pages) {
				WritePage(page.Key, page.Value);
			}
		}

		public void WritePage(uint pageNumber, byte[] pageData) {
			Debug.Assert(pageData.Length == hdr.PageSize);

			PerformAt(pageNumber * hdr.PageSize, () => {
				WriteBytes(pageData);
			});
		}

		//public void WriteStreamTable()

		public uint GetNumPages(uint numBytes) {
			return (numBytes + hdr.PageSize - 1) / hdr.PageSize;
		}
	}
}
