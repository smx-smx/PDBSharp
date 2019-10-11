#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class ExportSym
	{
		public UInt16 Ordinal { get; set; }
		public ExportSymFlags Flags { get; set; }
		public string Name { get; set; }
	}

	public class S_EXPORT : ISymbol
	{
		public readonly UInt16 Ordinal;
		public readonly ExportSymFlags Flags;
		public readonly string Name;

		public S_EXPORT(IServiceContainer ctx, IModule mod, SpanStream stream) {
			var r = new SymbolDataReader(ctx, stream);
			Ordinal = r.ReadUInt16();
			Flags = r.ReadFlagsEnum<ExportSymFlags>();
			Name = r.ReadSymbolString();
		}

		public S_EXPORT(ExportSym data) {
			Ordinal = data.Ordinal;
			Flags = data.Flags;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_EXPORT);
			w.WriteUInt16(Ordinal);
			w.WriteEnum<ExportSymFlags>(Flags);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
