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
	public class NameTableReader
	{
		public readonly byte[] StringTableData;

		public readonly Dictionary<uint, uint> StringOffsetToNameIndex;
		public readonly Dictionary<uint, uint> NameIndexToStringOffset;

		private readonly ReaderBase rdr;

		public string GetString(uint nameIndex) {
			if (!NameIndexToStringOffset.ContainsKey(nameIndex)) {
				return null;
			}

			uint offset = NameIndexToStringOffset[nameIndex];
			rdr.BaseStream.Position = offset;
			return rdr.ReadCString();
		}

		public uint GetIndex(string str) {
			//$TODO: optimize/cache values
			return StringOffsetToNameIndex
				.Where(p => GetString(p.Value) == str)
				.Select(p => p.Value)
				.First();
		}

		public NameTableReader(ReaderBase r) {
			StringTableData = Deserializers.ReadBuffer(r);
			rdr = new ReaderBase(new MemoryStream(StringTableData));

			StringOffsetToNameIndex = Deserializers.ReadMap<uint, uint>(r);

			uint maxNameIndices = r.ReadUInt32();

			NameIndexToStringOffset = StringOffsetToNameIndex.ToDictionary(x => x.Value, x => x.Key);
		}
	}
}
