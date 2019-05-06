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
	[SymbolReader(SymbolType.S_SECTION)]
	public class S_SECTION : SymbolDataReader
	{
		public readonly UInt16 SectionNumber;
		/// <summary>
		/// Alignment of this section (power of 2)
		/// </summary>
		public readonly byte Alignment;
		public readonly UInt32 Rva;
		public readonly UInt32 Length;
		public readonly UInt32 Characteristics;
		public readonly string Name;

		public S_SECTION(PDBFile pdb, Stream stream) : base(pdb, stream) {
			SectionNumber = ReadUInt16();
			Alignment = ReadByte();
			ReadByte(); //reserved
			Rva = ReadUInt32();
			Length = ReadUInt32();
			Characteristics = ReadUInt32();
			Name = ReadSymbolString();
		}
	}
}
