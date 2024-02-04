#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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

	namespace NameTable
	{
		public class Data {
			public SpanStream? Strings;
			public NameTableVersion Version;

			public uint NumberOfElements;
			public uint[] Indices = new uint[0];

			public HashFunc? Hasher { get; internal set; }

			public uint HashName(string name) {
				Debug.Assert(Hasher != null);
				byte[] data = Encoding.ASCII.GetBytes(name);
				return Hasher(data, unchecked((uint)-1));
			}

			public string GetString(uint index) {
				Debug.Assert(Strings != null);
				Strings.Position = index;
				return Strings.ReadCString();
			}
		}

		public class Serializer(SpanStream stream) {
			private const uint MAGIC = 0xeffeeffe;

			public Data Data = new Data();

			public Data Read() {
				var magic = stream.ReadUInt32();
				if (magic != MAGIC) {
					throw new InvalidDataException($"Invalid verHdr magic 0x{magic:X}");
				}

				HashFunc? hashFunc;

				var Version = stream.ReadEnum<NameTableVersion>();
				switch (Version) {
					case NameTableVersion.Hash:
						hashFunc = HasherV1.HashData;
						break;
					case NameTableVersion.HashV2:
						hashFunc = HasherV2.HashData;
						break;
					default:
						throw new InvalidDataException();
				}

				var buf = Deserializers.ReadBuffer(stream);
				var stringsData = new SpanStream(buf);

				var Indices = Deserializers.ReadArray<uint>(stream);
				var NumberOfElements = stream.ReadUInt32();

				Data = new Data {
					Version = Version,
					Indices = Indices,
					NumberOfElements = NumberOfElements,
					Strings = stringsData,
					Hasher = hashFunc,
				};

				return Data;
			}
		}

	}
}
