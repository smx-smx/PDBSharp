#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_CALLSITEINFO
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public ILeafResolver? Type { get; set; }
		public Data(uint offset, ushort sectionIndex, ILeafResolver? type) {
			Offset = offset;
			SectionIndex = sectionIndex;
			Type = type;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get;set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) { 			
		}
		public void Read() {
			var r = CreateReader();
			var Offset = r.ReadUInt32();
			var SectionIndex = r.ReadUInt16();
			r.ReadUInt16(); //padding
			var Type = r.ReadIndexedType32Lazy();
			Data = new Data(
				offset: Offset,
				sectionIndex: SectionIndex,
				type: Type
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_CALLSITEINFO);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.SectionIndex);
			w.WriteUInt16(0x00); //padding
			w.WriteIndexedType(data.Type);

			w.WriteHeader();
		}
	}
}
