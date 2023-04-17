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

namespace Smx.PDBSharp.Symbols.S_DEFRANGE_REGISTER_REL
{
	public class Data : ISymbolData {
		public UInt16 BaseRegister;
		public bool SpilledUdtMember;
		public UInt16 ParentVariableOffset;
		public UInt32 BaseRegisterOffset;
		public CV_LVAR_ADDR_RANGE Range;
		public CV_LVAR_ADDR_GAP[] Gaps;

		public Data(ushort baseRegister, bool spilledUdtMember, ushort parentVariableOffset, uint baseRegisterOffset, CV_LVAR_ADDR_RANGE range, CV_LVAR_ADDR_GAP[] gaps) {
			BaseRegister = baseRegister;
			SpilledUdtMember = spilledUdtMember;
			ParentVariableOffset = parentVariableOffset;
			BaseRegisterOffset = baseRegisterOffset;
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
			var BaseRegister = r.ReadUInt16();

			UInt16 flags = r.ReadUInt16();
			var SpilledUdtMember = (flags & 1) == 1;
			var ParentVariableOffset = (UInt16)((flags >> 4) & 0xFFF);

			var BaseRegisterOffset = r.ReadUInt32();
			var Range = new CV_LVAR_ADDR_RANGE(stream);
			var Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);

			Data = new Data(
				baseRegister: BaseRegister,
				spilledUdtMember: SpilledUdtMember,
				parentVariableOffset: ParentVariableOffset,
				baseRegisterOffset: BaseRegisterOffset,
				range: Range,
				gaps: Gaps
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_DEFRANGE_REGISTER_REL);
			w.WriteUInt16(data.BaseRegister);

			UInt16 flags = (ushort)(
				((data.ParentVariableOffset << 4) & 0xFFF) |
				(Convert.ToByte(data.SpilledUdtMember) & 1)
			);
			w.WriteUInt16(flags);
			w.WriteUInt32(data.BaseRegisterOffset);

			data.Range.Write(w);
			foreach (CV_LVAR_ADDR_GAP gap in data.Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
