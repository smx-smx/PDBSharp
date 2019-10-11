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
	public class BlockSym32
	{
		public UInt32 Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }

	}

	public class S_BLOCK32 : ISymbol
	{
		private readonly UInt32 ParentOffset;
		public readonly Symbol Parent;
		public readonly UInt32 End;
		public readonly UInt32 Length;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly string Name;

		public S_BLOCK32(IServiceContainer ctx, IModule mod, SpanReader stream) {
			SymbolDataReader r = new SymbolDataReader(ctx, stream);
			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(mod, ParentOffset);

			End = r.ReadUInt32();
			Length = r.ReadUInt32();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public S_BLOCK32(BlockSym32 data) {
			ParentOffset = data.Parent;
			End = data.End;
			Length = data.Length;
			Offset = data.Offset;
			Segment = data.Segment;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = new SymbolDataWriter(pdb, stream, SymbolType.S_BLOCK32);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(Length);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}