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
	public class ManProcSym
	{
		/// <summary>
		/// Parent Symbol
		/// </summary>
		public UInt32 Parent { get; set; }
		/// <summary>
		/// End of block
		/// </summary>
		public UInt32 End { get; set; }
		/// <summary>
		/// Next Symbol
		/// </summary>
		public UInt32 Next { get; set; }
		public UInt32 ProcLength { get; set; }
		public UInt32 DebugStartOffset { get; set; }
		public UInt32 DebugEndOffset { get; set; }
		public UInt32 ComToken { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public CV_PROCFLAGS Flags { get; set; }
		public UInt16 ReturnRegister { get; set; }
		public string Name { get; set; }
	}

	public abstract class ManProcSymBase
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

		public ManProcSymBase(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);

			Parent = r.ReadUInt32();
			End = r.ReadUInt32();
			Next = r.ReadUInt32();
			ProcLength = r.ReadUInt32();
			DebugStartOffset = r.ReadUInt32();
			DebugEndOffset = r.ReadUInt32();
			ComToken = r.ReadUInt32();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			ReturnRegister = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public ManProcSymBase(ManProcSym data) {
			Parent = data.Parent;
			End = data.End;
			Next = data.Next;
			ProcLength = data.ProcLength;
			DebugStartOffset = data.DebugStartOffset;
			DebugEndOffset = data.DebugEndOffset;
			ComToken = data.ComToken;
			Offset = data.Offset;
			Segment = data.Segment;
			Flags = data.Flags;
			ReturnRegister = data.ReturnRegister;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream, SymbolType symbolType) {
			var w = new SymbolDataWriter(pdb, stream, symbolType);
			w.WriteUInt32(Parent);
			w.WriteUInt32(End);
			w.WriteUInt32(Next);
			w.WriteUInt32(ProcLength);
			w.WriteUInt32(DebugStartOffset);
			w.WriteUInt32(DebugEndOffset);
			w.WriteUInt32(ComToken);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteEnum<CV_PROCFLAGS>(Flags);
			w.WriteUInt16(ReturnRegister);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
