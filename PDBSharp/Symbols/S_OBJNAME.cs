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

namespace Smx.PDBSharp.Symbols
{
	public class ObjNameSym
	{
		public UInt32 Signature { get; set; }
		public string Name { get; set; }
	}

	public class S_OBJNAME : ISymbol
	{
		public readonly UInt32 Signature;
		public readonly string Name;

		public S_OBJNAME(IServiceContainer ctx, IModule mod, SpanStream stream) {
			var r = new SymbolDataReader(ctx, stream);

			Signature = r.ReadUInt32();
			Name = r.ReadSymbolString();
		}

		public S_OBJNAME(ObjNameSym data) {
			Signature = data.Signature;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_OBJNAME);
			w.WriteUInt32(Signature);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
