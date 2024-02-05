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

namespace Smx.PDBSharp.Symbols.S_EXPORT
{
	public class Data : ISymbolData {
		public UInt16 Ordinal { get; set; }
		public ExportSymFlags Flags { get; set; }
		public string Name { get; set; }

		public Data(ushort ordinal, ExportSymFlags flags, string name) {
			Ordinal = ordinal;
			Flags = flags;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();
			var Ordinal = r.ReadUInt16();
			var Flags = r.ReadFlagsEnum<ExportSymFlags>();
			var Name = r.ReadSymbolString();
			Data = new Data(
				ordinal: Ordinal,
				flags: Flags,
				name: Name
			);
		}		

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_EXPORT);
			w.WriteUInt16(data.Ordinal);
			w.Write<ExportSymFlags>(data.Flags);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
