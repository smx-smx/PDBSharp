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
	public class S_WITH32 : ISymbol
	{
		private readonly UInt32 ParentOffset;
		public readonly Symbol Parent;
		private readonly UInt32 EndOffset;
		public readonly UInt32 Length;
		public readonly UInt32 SegmentOffset;
		public readonly UInt16 Segment;
		public readonly string Expression;

		public S_WITH32(IServiceContainer ctx, IModule mod, SpanStream stream) {
			var r = new SymbolDataReader(ctx, stream);
			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(mod, ParentOffset);
			EndOffset = r.ReadUInt32();
			Length = r.ReadUInt32();
			SegmentOffset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Expression = r.ReadSymbolString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = new SymbolDataWriter(pdb, stream, SymbolType.S_WITH32);
			w.WriteSymbolHeader();
			throw new NotImplementedException();
		}
	}
}
