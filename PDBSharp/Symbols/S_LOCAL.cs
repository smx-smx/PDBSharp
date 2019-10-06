#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class LocalSym
	{
		public LeafContainerBase Type { get; set; }
		public CV_LVARFLAGS Flags { get; set; }
		public string Name { get; set; }
	}

	public class S_LOCAL : ISymbol
	{
		public readonly ILeafContainer Type;
		public readonly CV_LVARFLAGS Flags;
		public readonly string Name;

		public S_LOCAL(IServiceContainer ctx, IModule mod, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);
			Type = r.ReadIndexedTypeLazy();
			Flags = r.ReadFlagsEnum<CV_LVARFLAGS>();
			Name = r.ReadSymbolString();
		}

		public S_LOCAL(LocalSym data) {
			Type = data.Type;
			Flags = data.Flags;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_LOCAL);
			w.WriteIndexedType(Type);
			w.WriteEnum<CV_LVARFLAGS>(Flags);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
