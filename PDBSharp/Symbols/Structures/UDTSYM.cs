#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class UdtSymData
	{
		public LeafBase Type { get; set; }
		public string Name { get; set; }
	}

	public class UdtSym
	{
		public readonly ILeafContainer Type;
		public readonly string Name;

		public UdtSym(Context ctx, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);
			Type = r.ReadIndexedTypeLazy();
			Name = r.ReadSymbolString();
		}

		public UdtSym(UdtSymData data) {
			Type = data.Type;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream, SymbolType symbolType) {
			var w = new SymbolDataWriter(pdb, stream, symbolType);
			w.WriteIndexedType(Type);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
