#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public unsafe struct JGHeader : IHeader
	{
		public const int MAGIC_SIZE = 44;

		public fixed byte Magic[MAGIC_SIZE];
		public UInt32 PageSize;
		public UInt16 FpmPageNumber;
		public UInt16 NumPages;
		public UInt32 DirectorySize;
		public UInt32 PageMap;


		string IHeader.Magic {
			get => GetMagic();
			set => SetMagic(value);
		}

		/// <summary>
		/// Returns the current MSF magic as string
		/// </summary>
		/// <returns></returns>
		private string GetMagic() {
			fixed (byte* ptr = Magic) {
				if (*(ushort*)&ptr[PDBFile.JG_OFFSET] == 0x474A) { //JG (Little Endian)
					return Encoding.ASCII.GetString(ptr, PDBFile.SMALL_MAGIC.Length);
				}

				throw new InvalidDataException("Invalid magic");
			}
		}

		/// <summary>
		/// Sets the current MSF magic
		/// </summary>
		/// <param name="magic"></param>
		private void SetMagic(string magic) {
			byte[] data = Encoding.ASCII.GetBytes(magic);
			fixed (byte* ptr = Magic) {
				new Span<byte>((void*)ptr, MAGIC_SIZE).WriteBytes(0, data);
			}
		}

		private void EnsureShort(uint value) {
			if(value > ushort.MaxValue) {
				throw new ArgumentException($"{value} does not fit in an u16");
			}
		}

		uint IHeader.PageSize {
			get => PageSize;
			set => PageSize = value;
		}
		uint IHeader.FpmPageNumber {
			get => FpmPageNumber;
			set => FpmPageNumber = (ushort)value;
		}
		uint IHeader.NumPages {
			get => NumPages;
			set => NumPages = (ushort)value;
		}
		uint IHeader.StreamTableSize {
			get => DirectorySize;
			set => DirectorySize = value;
		}
	}
}
