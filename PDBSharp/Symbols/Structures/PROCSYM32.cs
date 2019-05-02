#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ProcSym32: SymbolDataReader
	{
		public readonly UInt32 Parent;
		public readonly UInt32 End;
		public readonly UInt32 Next;
		public readonly UInt32 Length;
		public readonly UInt32 DebugStartOffset;
		public readonly UInt32 DebugEndOffset;
		public readonly UInt32 TypeIndex;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly CV_PROCFLAGS Flags;
		public readonly string Name;

		public ProcSym32(Stream stream) : base(stream) {
			Parent = ReadUInt32();
			End = ReadUInt32();
			Next = ReadUInt32();
			Length = ReadUInt32();
			DebugStartOffset = ReadUInt32();
			DebugEndOffset = ReadUInt32();
			TypeIndex = ReadUInt32();
			Offset = ReadUInt32();
			Segment = ReadUInt16();
			Flags = ReadEnum<CV_PROCFLAGS>();
			Name = ReadSymbolString();
		}
	}
}
