#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class DataSym32
	{
		public LeafContainerBase Type { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }
	}
	public abstract class DataSym32Base
	{
		public readonly ILeafContainer Type;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly string Name;

		public DataSym32Base(IServiceContainer ctx, SpanStream stream) {
			var r = new SymbolDataReader(ctx, stream);

			Type = r.ReadIndexedTypeLazy();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public DataSym32Base(DataSym32 data) {
			Type = data.Type;
			Offset = data.Offset;
			Segment = data.Segment;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream, SymbolType symbolType) {
			var w = new SymbolDataWriter(pdb, stream, symbolType);
			w.WriteIndexedType(Type);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
