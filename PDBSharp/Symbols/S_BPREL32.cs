#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Symbols
{
	public class BpRelSym32
	{
		public UInt32 Offset { get; set; }
		public LeafBase Type { get; set; }
		public string Name { get; set; }
	}

	public class S_BPREL32 : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly ILeafContainer Type;
		public readonly string Name;
	
		public S_BPREL32(PDBFile pdb, Stream stream) {
			SymbolDataReader r = new SymbolDataReader(pdb, stream);
			Offset = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			Name = r.ReadSymbolString();
		}

		public S_BPREL32(BpRelSym32 data) {
			Offset = data.Offset;
			Type = data.Type;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = new SymbolDataWriter(pdb, stream, SymbolType.S_BPREL32);
			w.WriteUInt32(Offset);
			w.WriteIndexedType(Type);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
