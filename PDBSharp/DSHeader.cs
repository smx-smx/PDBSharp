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
	public unsafe struct DSHeader : IHeader
	{
		public const int MAGIC_SIZE = 32;

		private fixed byte Magic[MAGIC_SIZE];
		private UInt32 PageSize;
		private UInt32 FpmPageNumber;
		private UInt32 NumPages;
		private UInt32 DirectorySize;
		private UInt32 PageMap; //should be 0 in the header

		string IHeader.Magic {
			get => GetMagic();
			set => SetMagic(value);
		}
		uint IHeader.PageSize {
			get => PageSize;
			set => PageSize = value;
		}
		uint IHeader.FpmPageNumber {
			get => FpmPageNumber;
			set => FpmPageNumber = value;
		}
		uint IHeader.NumPages {
			get => NumPages;
			set => NumPages = value;
		}
		uint IHeader.DirectorySize {
			get => DirectorySize;
			set => DirectorySize = value;
		}

		/// <summary>
		/// Returns the current MSF magic as string
		/// </summary>
		/// <returns></returns>
		private string GetMagic() {
			fixed(byte *ptr = Magic) {
				if(*(ushort*)&ptr[PDBFile.DS_OFFSET] == 0x5344) { //DS (Little Endian)
					return Encoding.ASCII.GetString(ptr, PDBFile.BIG_MAGIC.Length);
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
	}
}
