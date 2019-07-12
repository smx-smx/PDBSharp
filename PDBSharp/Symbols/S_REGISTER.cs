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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	public class RegSym
	{
		public LeafBase Type { get; set; }
		public UInt16 Register { get; set; }
		public string Name { get; set; }
	}

	public class S_REGISTER : ISymbol
	{
		public readonly ILeafContainer Type;
		public readonly UInt16 Register;
		public readonly string Name;

		public S_REGISTER(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);

			Type = r.ReadIndexedTypeLazy();
			Register = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public S_REGISTER(RegSym data) {
			Type = data.Type;
			Register = data.Register;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_REGISTER);
			w.WriteIndexedType(Type);
			w.WriteUInt16(Register);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
