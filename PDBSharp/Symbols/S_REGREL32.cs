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
	public class RegRel32
	{
		public UInt32 Offset { get; set; }
		public LeafContainerBase Type { get; set; }
		public UInt16 RegisterIndex { get; set; }
		public string Name { get; set; }
	}

	public class S_REGREL32 : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly ILeafContainer Type;
		public readonly UInt16 RegisterIndex;
		public readonly string Name;

		public S_REGREL32(IServiceContainer ctx, IModule mod, SpanReader stream) {
			var r = new SymbolDataReader(ctx, stream);

			Offset = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			RegisterIndex = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public S_REGREL32(RegRel32 data) {
			Offset = data.Offset;
			Type = data.Type;
			RegisterIndex = data.RegisterIndex;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_REGREL32);
			w.WriteUInt32(Offset);
			w.WriteIndexedType(Type);
			w.WriteUInt16(RegisterIndex);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
