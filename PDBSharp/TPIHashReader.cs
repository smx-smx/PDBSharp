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

	public class TPIHashReader : SpanStream
	{
		public readonly TreeDictionary<UInt32, UInt32> TypeIndexToOffset = new TreeDictionary<uint, uint>();
		public Dictionary<UInt32, UInt32> HashValueToTypeIndex = new Dictionary<uint, uint>();
		public Dictionary<uint, uint>? NameIndexToTypeIndex { get; private set; }


		public readonly uint[]? RecordHashValues;

		public TPIHashReader(IServiceContainer ctx, byte[] hashData) : base(hashData) {
			TPIReader tpi = ctx.GetService<TPIReader>();
			TPIHash hash = tpi.Header.Hash;

			switch (hash.HashKeySize) {
				case sizeof(UInt16):
				case sizeof(UInt32):
					break;
				default:
					throw new InvalidDataException();
			}

			if(hash.TypeOffsets.Size > 0) {
				Position = hash.TypeOffsets.Offset;
				uint NumTiPairs = (uint)(hash.TypeOffsets.Size / Marshal.SizeOf<TIOffset>());
				for (int i = 1; i < NumTiPairs; i++) {
					TIOffset tiOff = Read<TIOffset>();
					TypeIndexToOffset.Add(tiOff.TypeIndex, tiOff.Offset);
				}
			}

			if(hash.HashValues.Size > 0) {
				Position = hash.HashValues.Offset;
				uint NumHashValues = hash.HashValues.Size / sizeof(UInt32);
				RecordHashValues = PerformAt(hash.HashValues.Offset, () => {
					return Enumerable.Range(1, (int)NumHashValues)
						.Select(_ => ReadUInt32())
						.ToArray();
				});
			}

			if (hash.HashHeadList.Size > 0) {
				Position = hash.HashHeadList.Offset;
				NameIndexToTypeIndex = Deserializers.ReadMap<UInt32, UInt32>(this);
			}
		}
	}
}
