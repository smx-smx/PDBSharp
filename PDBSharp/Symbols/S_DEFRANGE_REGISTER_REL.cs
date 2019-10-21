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
	public class S_DEFRANGE_REGISTER_REL : SymbolBase
	{
		public UInt16 BaseRegister;
		public bool SpilledUdtMember;
		public UInt16 ParentVariableOffset;
		public UInt32 BaseRegisterOffset;
		public CV_LVAR_ADDR_RANGE Range;
		public CV_LVAR_ADDR_GAP[] Gaps;


		public S_DEFRANGE_REGISTER_REL(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			BaseRegister = r.ReadUInt16();

			UInt16 flags = r.ReadUInt16();
			SpilledUdtMember = (flags & 1) == 1;
			ParentVariableOffset = (UInt16)((flags >> 4) & 0xFFF);

			BaseRegisterOffset = r.ReadUInt32();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_DEFRANGE_REGISTER_REL);
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

			w.WriteHeader();
		}
	}
}
