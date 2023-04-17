#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Linq;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class CV_LVAR_ADDR_GAP
	{
		public const int SIZE = 4;

		public readonly UInt16 GapStartOffset;
		public readonly UInt16 Length;

		public CV_LVAR_ADDR_GAP(SymbolDataReader r) {
			GapStartOffset = r.ReadUInt16();
			Length = r.ReadUInt16();
		}

		public static CV_LVAR_ADDR_GAP[] ReadGaps(SymbolDataReader r) {
			// interpret remaining data as gaps
			int numGaps = r.Remaining / CV_LVAR_ADDR_GAP.SIZE;
			return Enumerable
				.Range(1, numGaps)
				.Select(_ => new CV_LVAR_ADDR_GAP(r))
				.ToArray();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(GapStartOffset);
			w.WriteUInt16(Length);
		}
	}
}
