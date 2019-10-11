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
	public class CallSiteInfo
	{
		public UInt32 Offset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public LeafContainerBase Type { get; set; }
	}

	public class S_CALLSITEINFO : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly UInt16 SectionIndex;
		public readonly ILeafContainer Type;

		public S_CALLSITEINFO(IServiceContainer ctx, IModule mod, SpanReader stream) {
			var r = new SymbolDataReader(ctx, stream);
			Offset = r.ReadUInt32();
			SectionIndex = r.ReadUInt16();
			r.ReadUInt16(); //padding
			Type = r.ReadIndexedTypeLazy();
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_CALLSITEINFO);
			w.WriteUInt32(Offset);
			w.WriteUInt16(SectionIndex);
			w.WriteUInt16(0x00); //padding
			w.WriteIndexedType(Type);

			w.WriteSymbolHeader();
		}
	}
}
