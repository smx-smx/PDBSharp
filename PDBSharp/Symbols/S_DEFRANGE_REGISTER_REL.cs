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
	public class DefrangeSymRegisterRel
	{
		public UInt16 BaseRegister { get; set; }
		public bool SpilledUdtMember { get; set; }
		public UInt16 ParentVariableOffset { get; set; }
		public UInt32 BaseRegisterOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }
	}

	public class S_DEFRANGE_REGISTER_REL : ISymbol
	{
		public readonly UInt16 BaseRegister;
		public readonly bool SpilledUdtMember;
		public readonly UInt16 ParentVariableOffset;
		public readonly UInt32 BaseRegisterOffset;
		public readonly CV_LVAR_ADDR_RANGE Range;
		public readonly CV_LVAR_ADDR_GAP[] Gaps;


		public S_DEFRANGE_REGISTER_REL(IServiceContainer ctx, IModule mod, SpanReader stream) {
			var r = new SymbolDataReader(ctx, stream);
			BaseRegister = r.ReadUInt16();

			UInt16 flags = r.ReadUInt16();
			SpilledUdtMember = (flags & 1) == 1;
			ParentVariableOffset = (UInt16)((flags >> 4) & 0xFFF);

			BaseRegisterOffset = r.ReadUInt32();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public S_DEFRANGE_REGISTER_REL(DefrangeSymRegisterRel data) {
			BaseRegister = data.BaseRegister;
			SpilledUdtMember = data.SpilledUdtMember;
			ParentVariableOffset = data.ParentVariableOffset;
			BaseRegisterOffset = data.BaseRegisterOffset;
			Range = data.Range;
			Gaps = data.Gaps;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_DEFRANGE_REGISTER_REL);
			w.WriteUInt16(BaseRegister);

			UInt16 flags = (ushort)(
				((ParentVariableOffset << 4) & 0xFFF) |
				(Convert.ToByte(SpilledUdtMember) & 1)
			);
			w.WriteUInt16(flags);
			w.WriteUInt32(BaseRegisterOffset);

			Range.Write(w);
			foreach (CV_LVAR_ADDR_GAP gap in Gaps) {
				gap.Write(w);
			}

			w.WriteSymbolHeader();
		}
	}
}
