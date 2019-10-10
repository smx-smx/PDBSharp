#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class DefrangeFramePointerRel
	{
		public UInt32 FramePointerOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }
	}

	public class S_DEFRANGE_FRAMEPOINTER_REL : ISymbol
	{
		public readonly UInt32 FramePointerOffset;
		public CV_LVAR_ADDR_RANGE Range;
		public CV_LVAR_ADDR_GAP[] Gaps;

		public S_DEFRANGE_FRAMEPOINTER_REL(IServiceContainer ctx, IModule mod, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);
			FramePointerOffset = r.ReadUInt32();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public S_DEFRANGE_FRAMEPOINTER_REL(DefrangeFramePointerRel data) {
			FramePointerOffset = data.FramePointerOffset;
			Range = data.Range;
			Gaps = data.Gaps;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_DEFRANGE_FRAMEPOINTER_REL);
			w.WriteUInt32(FramePointerOffset);
			Range.Write(w);

			foreach (CV_LVAR_ADDR_GAP gap in Gaps) {
				gap.Write(w);
			}

			w.WriteSymbolHeader();
		}
	}
}
