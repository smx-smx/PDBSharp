#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class CV_LVAR_ADDR_GAP : ReaderBase
	{
		public const int SIZE = 4;

		public readonly UInt16 GapStartOffset;
		public readonly UInt16 Length;

		public CV_LVAR_ADDR_GAP(Stream stream) : base(stream) {
			GapStartOffset = ReadUInt16();
			Length = ReadUInt16();
		}

		public static CV_LVAR_ADDR_GAP[] ReadGaps(Stream stream) {
			// interpret remaining data as gaps
			int numGaps = (int)(stream.Length - stream.Position) / CV_LVAR_ADDR_GAP.SIZE;
			return Enumerable
				.Range(1, numGaps)
				.Select(_ => new CV_LVAR_ADDR_GAP(stream))
				.ToArray();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(GapStartOffset);
			w.WriteUInt16(Length);
		}
	}
}
