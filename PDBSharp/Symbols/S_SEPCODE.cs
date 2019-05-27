#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_SEPCODE)]
	public class S_SEPCODE : SymbolDataReader
	{
		public readonly UInt32 Parent;
		public readonly UInt32 End;
		public readonly UInt32 Size;
		public readonly CV_SEPCODEFLAGS Flags;
		public readonly UInt32 Offset;
		public readonly UInt32 ParentOffset;
		public readonly UInt16 Section;
		public readonly UInt16 ParentSection;

		public S_SEPCODE(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Parent = ReadUInt32();
			End = ReadUInt32();

			Size = ReadUInt32();
			Flags = ReadFlagsEnum<CV_SEPCODEFLAGS>();

			Offset = ReadUInt32();
			ParentOffset = ReadUInt32();

			Section = ReadUInt16();
			ParentSection = ReadUInt16();
		}
	}
}
