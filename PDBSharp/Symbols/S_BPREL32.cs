#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_BPREL32)]
	public class S_BPREL32 : SymbolDataReader
	{
		public readonly UInt32 Offset;
		public readonly ILeaf Type;
		public readonly string Name;
	
		public S_BPREL32(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Offset = ReadUInt32();
			Type = ReadIndexedTypeLazy();
			Name = ReadSymbolString();
		}
	}
}
