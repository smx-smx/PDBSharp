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
	public class S_DEFRANGE_SUBFIELD_REGISTER : SymbolBase
	{
		public UInt16 Register { get; set; }
		public RangeAttributes Attributes { get; set; }
		public UInt32 ParentVariableOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }

		public S_DEFRANGE_SUBFIELD_REGISTER(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 
		}

		public override void Read() {
			var r = CreateReader();
			Register = r.ReadUInt16();
			Attributes = r.ReadFlagsEnum<RangeAttributes>();
			ParentVariableOffset = r.ReadUInt32() & 0xFFF; //CV_OFFSET_PARENT_LENGTH_LIMIT
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_DEFRANGE_SUBFIELD_REGISTER);
			w.WriteUInt16(Register);
			w.Write<RangeAttributes>(Attributes);
			w.WriteUInt32(ParentVariableOffset & 0xFFF);
			Range.Write(w);
			foreach (CV_LVAR_ADDR_GAP gap in Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
		}
	}
}
