#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using C5;
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public struct TIOffset
	{
		public UInt32 TypeIndex;
		public UInt32 Offset;
	}

	public class HashDataReader : ReaderBase
	{
		public readonly TreeDictionary<UInt32, UInt32> TypeIndexToOffset = new TreeDictionary<uint, uint>();
		public readonly UInt32[] RecordHashValues;

		public HashDataReader(TPIReader tpi, Stream stream) : base(stream) {
			TPIHash hash = tpi.Header.Hash;
			uint NumTiPairs = (uint)(hash.TypeOffsets.Size / Marshal.SizeOf<TPISlice>());

			PerformAt(hash.TypeOffsets.Offset, () => {
				for (int i = 1; i < NumTiPairs; i++) {
					TIOffset tiOff = ReadStruct<TIOffset>();
					TypeIndexToOffset.Add(tiOff.TypeIndex, tiOff.Offset);
				}
			});

			uint NumHashValues = (uint)(hash.HashValues.Size / sizeof(UInt32));
			RecordHashValues = PerformAt(hash.HashValues.Offset, () => {
				return Enumerable.Range(1, (int)NumHashValues)
					.Select(_ => ReadUInt32())
					.ToArray();
			});
		}
	}
}
