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

namespace Smx.PDBSharp.Symbols.S_DEFRANGE_REGISTER
{
	public class Data : ISymbolData {
		public UInt16 Register { get; set; }
		public RangeAttributes Attributes { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public Structures.CV_LVAR_ADDR_GAP.Data[] Gaps { get; set; }

		public Data(ushort register, RangeAttributes attributes, CV_LVAR_ADDR_RANGE range, Structures.CV_LVAR_ADDR_GAP.Data[] gaps) {
			Register = register;
			Attributes = attributes;
			Range = range;
			Gaps = gaps;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = new SymbolData.Reader(ctx, stream);
			var Register = r.ReadUInt16();
			var Attributes = r.ReadFlagsEnum<RangeAttributes>();
			var Range = new CV_LVAR_ADDR_RANGE(stream);
			var Gaps = Structures.CV_LVAR_ADDR_GAP.Serializer.ReadGaps(r);

			Data = new Data(
				register: Register,
				attributes: Attributes,
				range: Range,
				gaps: Gaps
			);
		}

		public void Write() {
			/*
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_DEFRANGE_REGISTER);
			w.WriteUInt16(data.Register);
			w.Write<RangeAttributes>(data.Attributes);
			data.Range.Write(w);
			foreach (Structures.CV_LVAR_ADDR_GAP.Data gap in data.Gaps) {
				gap.Write(w);
			}

			w.WriteHeader();
			*/
		}

		public ISymbolData? GetData() => Data;
	}
}
