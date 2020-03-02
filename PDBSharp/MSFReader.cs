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

	public unsafe abstract class MSFReader : IDisposable
	{
		public const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		public const string BIG_MAGIC = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private byte[] streamTable;

        public IHeader Header { get; }
		public readonly PDBType FileType;

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

		public static PDBType DetectPdbType(Span<byte> memory) {
			int maxSize = Math.Max(SMALL_MAGIC.Length, BIG_MAGIC.Length);
			
			byte[] magic = memory.Slice(0, maxSize).ToArray();
			string msfMagic = Encoding.ASCII.GetString(magic);
			if (msfMagic.StartsWith(BIG_MAGIC)) {
				return PDBType.Big;
			} else if (msfMagic.StartsWith(SMALL_MAGIC)) {
				return PDBType.Small;
			} else {
				throw new InvalidDataException("No valid MSF header found");
			}
		}

		private unsafe IHeader ReadHeader() {
			switch (FileType) {
				case PDBType.Big:
					return Span.Read<DSHeader>(0);
				case PDBType.Small:
					return Span.Read<JGHeader>(0);
				default:
					throw new InvalidDataException("No valid MSF header found");
			}
		}

		public MSFReader(Memory<byte> mem) {
			this.memory = mem;
			FileType = DetectPdbType(mem.Span);
			this.Header = ReadHeader();
		}

		public MSFReader(MemoryMappedFile mfile, long length) {
			this.mf = new MemoryMappedSpan(mfile, length);
			FileType = DetectPdbType(mf.GetSpan());
			this.Header = ReadHeader();
		}

		/// <summary>
		/// Computes the byte offset for the specified page
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public uint GetPageLocation(uint pageNumber) {
			return Header.PageSize * pageNumber;
		}

		/// <summary>
		/// Computes the number of pages needed to represent the specified number of bytes
		/// </summary>
		/// <param name="numBytes"></param>
		/// <returns></returns>
		public uint GetNumPages(uint numBytes) {
			return (numBytes + Header.PageSize - 1) / Header.PageSize;
		}

		private long GetDataSize(uint numPages) {
			return numPages * Header.PageSize;
		}

		public abstract byte[] ReadPage(uint pageNumber);
		public abstract IEnumerable<byte[]> GetPages(long offset, uint numPages);
		public abstract uint ReadPageNumber(BinaryReader rdr);

		public abstract IEnumerable<byte[]> GetPages_StreamTable();

		/// <summary>
		/// Returns the merged stream table data
		/// </summary>
		/// <returns></returns>
		public byte[] StreamTable() {
			if (streamTable != null) {
				return streamTable;
			}

			streamTable = GetPages_StreamTable()
				.SelectMany(x => x)
				.ToArray();

			return streamTable;
		}

		public void Dispose() {
			mf?.Dispose();
		}
	}
}
