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
	[SymbolReader(SymbolType.S_COFFGROUP)]
	public class S_COFFGROUP : SymbolDataReader
	{
		public readonly UInt32 Size;
		public readonly UInt32 Characteristics;
		public readonly UInt32 SymbolOffset;
		public readonly UInt16 SymbolSegment;
		public readonly string Name;

		public S_COFFGROUP(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Size = ReadUInt32();
			Characteristics = ReadUInt32();
			SymbolOffset = ReadUInt32();
			SymbolSegment = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}
