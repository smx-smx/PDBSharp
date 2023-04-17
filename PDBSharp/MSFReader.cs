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

	/// <summary>
	/// MultiStreamFile implementation
	/// </summary>
	public unsafe abstract class MSFReader
	{
		private byte[]? streamTable;

        public IHeader Header { get; }
		public readonly PDBType FileType;

		public Memory<byte> Memory;

		public static PDBType DetectPdbType(Span<byte> memory) {
			int maxSize = Math.Max(PDBFile.SMALL_MAGIC.Length, PDBFile.BIG_MAGIC.Length);
			
			byte[] magic = memory.Slice(0, maxSize).ToArray();
			string msfMagic = Encoding.ASCII.GetString(magic);
			if (msfMagic.StartsWith(PDBFile.BIG_MAGIC)) {
				return PDBType.Big;
			} else if (msfMagic.StartsWith(PDBFile.SMALL_MAGIC)) {
				return PDBType.Small;
			} else if (msfMagic.StartsWith(PDBFile.OLD_MAGIC)) {
				return PDBType.Old;
			} else {
				throw new InvalidDataException("No valid MSF header found");
			}
		}

		private unsafe IHeader ReadHeader(Span<byte> span) {
			switch (FileType) {
				case PDBType.Big:
					return span.Read<DSHeader>(0);
				case PDBType.Small:
					return span.Read<JGHeader>(0);
				case PDBType.Old:
					throw new NotSupportedException("PDB 1.0 is not a MSF");
				default:
					throw new InvalidDataException("No valid MSF header found");
			}
		}

		private MSFReader(Span<byte> span, PDBType type) {
			this.FileType = type;
			this.Header = ReadHeader(span);
		}

		public MSFReader(Memory<byte> mem, PDBType type) : this(mem.Span, type){
			this.Memory = mem;
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

		/// <summary>
		/// Reads a given page
		/// </summary>
		/// <param name="pageNumber">page number to read</param>
		/// <returns></returns>
		public virtual byte[] ReadPage(uint pageNumber) {
			long offset = pageNumber * Header.PageSize;

			byte[] data = Memory.Slice((int)offset, (int)Header.PageSize).ToArray();
			return data;
		}

		public abstract IEnumerable<byte[]> GetPages(long offset, uint numPages);

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
	}
}
