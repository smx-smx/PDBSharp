#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class CoffGroupSym
	{
		public UInt32 Size { get; set; }
		public UInt32 Characteristics { get; set; }
		public UInt32 SymbolOffset { get; set; }
		public UInt16 SymbolSegment { get; set; }
		public string Name { get; set; }
	}

	public class S_COFFGROUP : ISymbol
	{
		public readonly UInt32 Size;
		public readonly UInt32 Characteristics;
		public readonly UInt32 SymbolOffset;
		public readonly UInt16 SymbolSegment;
		public readonly string Name;

		public S_COFFGROUP(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);
			Size = r.ReadUInt32();
			Characteristics = r.ReadUInt32();
			SymbolOffset = r.ReadUInt32();
			SymbolSegment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public S_COFFGROUP(CoffGroupSym data) {
			Size = data.Size;
			Characteristics = data.Characteristics;
			SymbolOffset = data.SymbolOffset;
			SymbolSegment = data.SymbolSegment;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_COFFGROUP);
			w.WriteUInt32(Size);
			w.WriteUInt32(Characteristics);
			w.WriteUInt32(SymbolOffset);
			w.WriteUInt16(SymbolSegment);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
