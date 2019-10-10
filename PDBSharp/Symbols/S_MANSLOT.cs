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
	public class AttrSlotSym
	{
		public UInt32 SlotIndex { get; set; }
		public LeafContainerBase Type { get; set; }
		public CV_LVAR_ATTR Attributes { get; set; }
		public string Name { get; set; }
	}

	public class S_MANSLOT : ISymbol
	{
		public readonly UInt32 SlotIndex;
		public readonly ILeafContainer Type;
		public readonly CV_LVAR_ATTR Attributes;
		public readonly string Name;

		public S_MANSLOT(IServiceContainer ctx, IModule mod, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);

			SlotIndex = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			Attributes = new CV_LVAR_ATTR(stream);
			Name = r.ReadSymbolString();
		}

		public S_MANSLOT(AttrSlotSym data) {
			SlotIndex = data.SlotIndex;
			Type = data.Type;
			Attributes = data.Attributes;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_MANSLOT);
			w.WriteUInt32(SlotIndex);
			w.WriteIndexedType(Type);
			Attributes.Write(w);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
