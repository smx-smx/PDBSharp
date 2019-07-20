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
using System.Linq;
using System.Text;

namespace Smx.PDBSharp
{
	public enum UdtNameTableVersion : UInt32
	{
		LongHash = 1,
		LongHashV2 = 2
	}

	public class UdtNameTableReader : ReaderBase
	{
		const UInt32 MAGIC = 0xEFFEEFFE;

		private readonly UInt32 Magic;
		private readonly UdtNameTableVersion Version;

		private readonly byte[] data;
		private readonly uint[] NameIndices;
		private readonly uint NumberOfNameIndices;

		private readonly ReaderBase rdr;

		public string GetString(uint nameIndex) {
			if (nameIndex == 0)
				return null;

			return rdr.PerformAt(nameIndex, () => {
				return rdr.ReadCString();
			});
		}

		public uint GetIndex(string str) {
			return NameIndices
				.Where(ni => GetString(ni) == str)
				.First();
		}

		public UdtNameTableReader(Stream stream) : base(stream) {
			Magic = ReadUInt32();
			if(Magic != MAGIC) {
				throw new InvalidDataException();
			}

			Version = ReadEnum<UdtNameTableVersion>();

			data = Deserializers.ReadBuffer(this);
			rdr = new ReaderBase(new MemoryStream(data));

			NameIndices = Deserializers.ReadArray<uint>(this);
			NumberOfNameIndices = ReadUInt32();
		}
	}
}
