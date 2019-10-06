#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using C5;
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

	public class TPIHashReader : ReaderBase
	{
		public readonly TreeDictionary<UInt32, UInt32> TypeIndexToOffset = new TreeDictionary<uint, uint>();
		public Dictionary<UInt32, UInt32> HashValueToTypeIndex = new Dictionary<uint, uint>();
		public Dictionary<uint, uint> NameIndexToTypeIndex { get; private set; }


		public readonly UInt32[] RecordHashValues;

		public TPIHashReader(IServiceContainer ctx, Stream stream) : base(stream) {
			TPIReader tpi = ctx.GetService<TPIReader>();
			TPIHash hash = tpi.Header.Hash;

			switch (hash.HashKeySize) {
				case sizeof(UInt16):
				case sizeof(UInt32):
					break;
				default:
					throw new InvalidDataException();
			}

			PerformAt(hash.TypeOffsets.Offset, () => {
				uint NumTiPairs = (uint)(hash.TypeOffsets.Size / Marshal.SizeOf<TIOffset>());
				for (int i = 1; i < NumTiPairs; i++) {
					TIOffset tiOff = ReadStruct<TIOffset>();
					TypeIndexToOffset.Add(tiOff.TypeIndex, tiOff.Offset);
				}
			});

			uint NumHashValues = hash.HashValues.Size / sizeof(UInt32);
			RecordHashValues = PerformAt(hash.HashValues.Offset, () => {
				return Enumerable.Range(1, (int)NumHashValues)
					.Select(_ => ReadUInt32())
					.ToArray();
			});

			PerformAt(hash.HashHeadList.Offset, () => {
				NameIndexToTypeIndex = Deserializers.ReadMap<UInt32, UInt32>(this);
			});
		}
	}
}
