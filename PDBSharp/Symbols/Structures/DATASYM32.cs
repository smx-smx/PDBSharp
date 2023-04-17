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
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.DataSym32
{
	public class Data : ISymbolData {
		public ILeafResolver? Type { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? type, uint offset, ushort segment, string name) {
			Type = type;
			Offset = offset;
			Segment = segment;
			Name = name;
		}
	}

	public abstract class SerializerBase : SymbolSerializerBase
	{
		public Data? Data { get; set; }
		
		public ISymbolData? GetData() => Data;

		public SerializerBase(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public void Read() {
			var r = CreateReader();

			var Type = r.ReadIndexedType32Lazy();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var Name = r.ReadSymbolString();
			Data = new Data(
				type: Type,
				offset: Offset,
				segment: Segment,
				name: Name
			);
		}

		public void Write(SymbolType symbolType) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(symbolType);
			w.WriteIndexedType(data.Type);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.Segment);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
