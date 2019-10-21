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
	public class S_DEFRANGE_FRAMEPOINTER_REL : SymbolBase
	{
		public UInt32 FramePointerOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }

		public S_DEFRANGE_FRAMEPOINTER_REL(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			FramePointerOffset = r.ReadUInt32();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_DEFRANGE_FRAMEPOINTER_REL);
			w.WriteUInt32(FramePointerOffset);
			Range.Write(w);

			foreach (CV_LVAR_ADDR_GAP gap in Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
		}
	}
}
