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
using System.Linq;

namespace Smx.PDBSharp
{
	public class BitSet
	{
		const int BITS_PER_WORD = 32;

		private readonly uint[] values;

		public BitSet(IEnumerable<UInt32> values) {
			this.values = values.ToArray();
		}

		public int Size => values.Length * BITS_PER_WORD;

		public int GetBit(int idx) => (int)(idx & (BITS_PER_WORD - 1));
		public uint GetBitmask(int idx) => (uint)(1 << GetBit(idx));
		public int GetIndex(int idx) => idx / BITS_PER_WORD;
		public uint GetWord(int idx) => values[GetIndex(idx)];

		public bool Contains(int i) {
			return i < Size && ((GetWord(i) & GetBitmask(i)) != 0);
		}
	}
}
