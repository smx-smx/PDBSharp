#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_COFFGROUP
{
	public class Data : ISymbolData {
		public UInt32 Size { get; set; }
		public UInt32 Characteristics { get; set; }
		public UInt32 SymbolOffset { get; set; }
		public UInt16 SymbolSegment { get; set; }
		public string Name { get; set; }

		public Data(uint size, uint characteristics, uint symbolOffset, ushort symbolSegment, string name) {
			Size = size;
			Characteristics = characteristics;
			SymbolOffset = symbolOffset;
			SymbolSegment = symbolSegment;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var Size = r.ReadUInt32();
			var Characteristics = r.ReadUInt32();
			var SymbolOffset = r.ReadUInt32();
			var SymbolSegment = r.ReadUInt16();
			var Name = r.ReadSymbolString();
			Data = new Data(
				size: Size,
				characteristics: Characteristics,
				symbolOffset: SymbolOffset,
				symbolSegment: SymbolSegment,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_COFFGROUP);
			w.WriteUInt32(data.Size);
			w.WriteUInt32(data.Characteristics);
			w.WriteUInt32(data.SymbolOffset);
			w.WriteUInt16(data.SymbolSegment);
			w.WriteSymbolString(data.Name);
			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
