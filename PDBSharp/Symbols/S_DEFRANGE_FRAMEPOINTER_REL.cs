#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_DEFRANGE_FRAMEPOINTER_REL
{
	public class Data : ISymbolData {
		public UInt32 FramePointerOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }

		public Data(uint framePointerOffset, CV_LVAR_ADDR_RANGE range, CV_LVAR_ADDR_GAP[] gaps) {
			FramePointerOffset = framePointerOffset;
			Range = range;
			Gaps = gaps;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var FramePointerOffset = r.ReadUInt32();
			var Range = new CV_LVAR_ADDR_RANGE(stream);
			var Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
			Data = new Data(
				framePointerOffset: FramePointerOffset,
				range: Range,
				gaps: Gaps
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_DEFRANGE_FRAMEPOINTER_REL);
			w.WriteUInt32(data.FramePointerOffset);
			data.Range.Write(w);

			foreach (CV_LVAR_ADDR_GAP gap in data.Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
