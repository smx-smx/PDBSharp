#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public unsafe struct JGHeaderOld {
		public const int MAGIC_SIZE = 44;

		public fixed byte _magic[MAGIC_SIZE];
		public PDBInternalVersion InternalVersion;
		public UInt32 Timestamp;
		/// <summary>
		/// number of times written
		/// </summary>
		public UInt32 Age;
		public UInt16 MinTi;
		public UInt16 MaxTi;
		public UInt32 GpRecSize;

		public int SIZE => sizeof(JGHeaderOld);

		/// <summary>
		/// Returns the current MSF magic as string
		/// </summary>
		/// <returns></returns>
		private string GetMagic() {
			fixed (byte* ptr = _magic) {
				if (*(ushort*)&ptr[PDBFile.JG_OFFSET] == 0x474A) { //JG (Little Endian)
					return Encoding.ASCII.GetString(ptr, PDBFile.OLD_MAGIC.Length);
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
			fixed (byte* ptr = _magic) {
				new Span<byte>((void*)ptr, MAGIC_SIZE).WriteBytes(0, data);
			}
		}

		string Magic {
			get => GetMagic();
			set => SetMagic(value);
		}
	}
}
