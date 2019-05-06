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
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ManProcSym : SymbolDataReader
	{
		/// <summary>
		/// Parent Symbol
		/// </summary>
		public readonly UInt32 Parent;
		/// <summary>
		/// End of block
		/// </summary>
		public readonly UInt32 End;
		/// <summary>
		/// Next Symbol
		/// </summary>
		public readonly UInt32 Next;
		public readonly UInt32 ProcLength;
		public readonly UInt32 DebugStartOffset;
		public readonly UInt32 DebugEndOffset;
		public readonly UInt32 ComToken;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly CV_PROCFLAGS Flags;
		public readonly UInt16 ReturnRegister;
		public readonly string Name;

		public ManProcSym(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Parent = ReadUInt32();
			End = ReadUInt32();
			Next = ReadUInt32();
			ProcLength = ReadUInt32();
			DebugStartOffset = ReadUInt32();
			DebugEndOffset = ReadUInt32();
			ComToken = ReadUInt32();
			Offset = ReadUInt32();
			Segment = ReadUInt16();
			Flags = ReadFlagsEnum<CV_PROCFLAGS>();
			ReturnRegister = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}
