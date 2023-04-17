#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

namespace Smx.PDBSharp.Symbols.S_DEFRANGE_SUBFIELD_REGISTER
{
	public class Data : ISymbolData
	{
		public UInt16 Register { get; set; }
		public RangeAttributes Attributes { get; set; }
		public UInt32 ParentVariableOffset { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }

		public Data(ushort register, RangeAttributes attributes, uint parentVariableOffset, CV_LVAR_ADDR_RANGE range, CV_LVAR_ADDR_GAP[] gaps) {
			Register = register;
			Attributes = attributes;
			ParentVariableOffset = parentVariableOffset;
			Range = range;
			Gaps = gaps;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 
		}

		public void Read() {
			var r = CreateReader();
			var Register = r.ReadUInt16();
			var Attributes = r.ReadFlagsEnum<RangeAttributes>();
			var ParentVariableOffset = r.ReadUInt32() & 0xFFF; //CV_OFFSET_PARENT_LENGTH_LIMIT
			var Range = new CV_LVAR_ADDR_RANGE(stream);
			var Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
			Data = new Data(
				register: Register,
				attributes: Attributes,
				parentVariableOffset: ParentVariableOffset,
				range: Range,
				gaps: Gaps
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_DEFRANGE_SUBFIELD_REGISTER);
			w.WriteUInt16(data.Register);
			w.Write<RangeAttributes>(data.Attributes);
			w.WriteUInt32(data.ParentVariableOffset & 0xFFF);
			data.Range.Write(w);
			foreach (CV_LVAR_ADDR_GAP gap in data.Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
