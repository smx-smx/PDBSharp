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
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_MANSLOT
{
	public class Data : ISymbolData {
		public UInt32 SlotIndex { get; set; }
		public ILeafResolver? Type { get; set; }
		public CV_LVAR_ATTR Attributes { get; set; }
		public string Name { get; set; }

		public Data(uint slotIndex, ILeafResolver? type, CV_LVAR_ATTR attributes, string name) {
			SlotIndex = slotIndex;
			Type = type;
			Attributes = attributes;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();

			var SlotIndex = r.ReadUInt32();
			var Type = r.ReadIndexedType32Lazy();
			var Attributes = new CV_LVAR_ATTR(stream);
			var Name = r.ReadSymbolString();

			Data = new Data(
				slotIndex: SlotIndex,
				type: Type,
				attributes: Attributes,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_MANSLOT);
			w.WriteUInt32(data.SlotIndex);
			w.WriteIndexedType(data.Type);
			data.Attributes.Write(w);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
