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

namespace Smx.PDBSharp.Symbols.S_BPREL32
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public ILeafResolver? Type { get; set; }
		public string Name { get; set; }

		public Data(uint offset, ILeafResolver? type, string name) {
			Offset = offset;
			Type = type;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			SymbolDataReader r = CreateReader();
			var Offset = r.ReadUInt32();
			var Type = r.ReadIndexedType32Lazy();
			var Name = r.ReadSymbolString();

			Data = new Data(
				offset: Offset,
				type: Type,
				name: Name
			);
		}		

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			SymbolDataWriter w = CreateWriter(SymbolType.S_BPREL32);
			w.WriteUInt32(data.Offset);
			w.WriteIndexedType(data.Type);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
