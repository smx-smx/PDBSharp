#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Smx.PDBSharp
{
	public enum NameTableVersion : UInt32
	{
		Hash = 1,
		HashV2 = 2
	}

	public delegate uint HashFunc(byte[] data, uint modulo);

	public class NameTableReader
	{
		private const UInt32 MAGIC = 0xeffeeffe;

		public readonly NameTableVersion Version;

		private readonly SpanStream rdr;

		public readonly uint NumberOfElements;
		public readonly uint[] Indices;

		private HashFunc? hasher;

		public uint HashName(string name) {
			Debug.Assert(hasher != null);
			byte[] data = Encoding.ASCII.GetBytes(name);
			return hasher(data, unchecked((uint)-1));
		}

		public string GetString(uint index) {
			rdr.Position = index;
			return rdr.ReadCString();
		}

		public NameTableReader(SpanStream r) {
			UInt32 magic = r.ReadUInt32();
			if (magic != MAGIC) {
				throw new InvalidDataException($"Invalid verHdr magic 0x{magic:X}");
			}

			Version = r.ReadEnum<NameTableVersion>();
			switch (Version) {
				case NameTableVersion.Hash:
					hasher = HasherV1.HashData;
					break;
				case NameTableVersion.HashV2:
					hasher = HasherV2.HashData;
					break;
				default:
					throw new InvalidDataException();
					break;
			}

			byte[] buf = Deserializers.ReadBuffer(r);
			rdr = new SpanStream(buf);

			Indices = Deserializers.ReadArray<UInt32>(r);
			NumberOfElements = r.ReadUInt32();
		}
	}
}
