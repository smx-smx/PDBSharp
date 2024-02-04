#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using C5;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	public struct TIOffset
	{
		public UInt32 TypeIndex;
		public UInt32 Offset;
	}

	namespace TPIHash {
		public class Data : IPDBService {
			public TreeDictionary<uint, uint>? TypeIndexToOffset;
			public Dictionary<uint, uint>? NameIndexToTypeIndex;
			public uint[]? RecordHashValues;
		}
		public class Serializer {
			public Data Data = new Data();

			private TPI.Serializer tpi;
			private readonly SpanStream stream;

			public Serializer(TPI.Serializer tpi, SpanStream stream) {
				this.tpi = tpi;
				this.stream = stream;
			}

			public Data Read() {
				// read hash header info
				TPIHashData hash = tpi.Data.Header.Hash;

				TreeDictionary<uint, uint>? TypeIndexToOffset = null;
				Dictionary<uint, uint>? NameIndexToTypeIndex = null;
				uint[]? RecordHashValues = null;

				switch (hash.HashKeySize) {
					case sizeof(UInt16):
					case sizeof(UInt32):
						break;
					default:
						throw new InvalidDataException();
				}

				if (hash.TypeOffsets.Size > 0) {
					TypeIndexToOffset = new TreeDictionary<uint, uint>();

					stream.Position = hash.TypeOffsets.Offset;
					uint NumTiPairs = (uint)(hash.TypeOffsets.Size / Marshal.SizeOf<TIOffset>());
					for (int i = 1; i < NumTiPairs; i++) {
						TIOffset tiOff = stream.Read<TIOffset>();
						TypeIndexToOffset.Add(tiOff.TypeIndex, tiOff.Offset);
					}
				}

				if (hash.HashValues.Size > 0) {
					stream.Position = hash.HashValues.Offset;
					uint NumHashValues = hash.HashValues.Size / sizeof(UInt32);
					RecordHashValues = stream.PerformAt(hash.HashValues.Offset, () => {
						return Enumerable.Range(1, (int)NumHashValues)
							.Select(_ => stream.ReadUInt32())
							.ToArray();
					});
				}

				if (hash.HashHeadList.Size > 0) {
					stream.Position = hash.HashHeadList.Offset;
					NameIndexToTypeIndex = Deserializers.ReadMap<UInt32, UInt32>(stream);
				}

				Data = new Data {
					TypeIndexToOffset = TypeIndexToOffset,
					RecordHashValues = RecordHashValues,
					NameIndexToTypeIndex = NameIndexToTypeIndex
				};

				return Data;
			}
		}
	}
}
