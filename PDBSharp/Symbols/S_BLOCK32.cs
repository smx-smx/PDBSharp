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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public readonly UInt32 Parent;
		public readonly UInt32 End;
		public readonly UInt32 Length;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly string Name;

		public S_BLOCK32(PDBFile pdb, Stream stream) {
			SymbolDataReader r = new SymbolDataReader(pdb, stream);
			Parent = r.ReadUInt32();
			End = r.ReadUInt32();
			Length = r.ReadUInt32();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public S_BLOCK32(BlockSym32 data){
			Parent = data.Parent;
			End = data.End;
			Length = data.Length;
			Offset = data.Offset;
			Segment = data.Segment;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = new SymbolDataWriter(pdb, stream, SymbolType.S_BLOCK32);
			w.WriteUInt32(Parent);
			w.WriteUInt32(End);
			w.WriteUInt32(Length);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}