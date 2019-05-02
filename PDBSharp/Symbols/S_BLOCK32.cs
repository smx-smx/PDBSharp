#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
	[SymbolReader(SymbolType.S_BLOCK32)]
	public class S_BLOCK32 : SymbolDataReader
	{
		public readonly UInt32 Parent;
		public readonly UInt32 End;
		public readonly UInt32 Length;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly string Name;

		public S_BLOCK32(Stream stream) : base(stream) {
			Parent = ReadUInt32();
			End = ReadUInt32();
			Length = ReadUInt32();
			Offset = ReadUInt32();
			Segment = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}