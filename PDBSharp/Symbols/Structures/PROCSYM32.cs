#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ProcSym32
	{
		public UInt32 Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Next { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 DebugStartOffset { get; set; }
		public UInt32 DebugEndOffset { get; set; }
		public LeafContainerBase Type { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public CV_PROCFLAGS Flags { get; set; }
		public string Name { get; set; }
	}

	public abstract class ProcSym32Base
	{
		private readonly UInt32 ParentOffset;
		public readonly UInt32 End;
		private readonly UInt32 NextOffset;
		public readonly UInt32 Length;
		public readonly UInt32 DebugStartOffset;
		public readonly UInt32 DebugEndOffset;
		public readonly ILeafContainer Type;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly CV_PROCFLAGS Flags;
		public readonly string Name;

		public readonly Symbol ParentSymbol;
		public readonly Symbol NextSymbol;

		public ProcSym32Base(IServiceContainer ctx, IModule mod, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);

			ParentOffset = r.ReadUInt32();
			ParentSymbol = r.ReadSymbol(mod, ParentOffset);

			End = r.ReadUInt32();
			NextOffset = r.ReadUInt32();
			NextSymbol = r.ReadSymbol(mod, NextOffset);

			Length = r.ReadUInt32();
			DebugStartOffset = r.ReadUInt32();
			DebugEndOffset = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			Name = r.ReadSymbolString();
		}

		public ProcSym32Base(ProcSym32 data) {
			ParentOffset = data.Parent;
			End = data.End;
			NextOffset = data.Next;
			Length = data.Length;
			DebugStartOffset = data.DebugStartOffset;
			DebugEndOffset = data.DebugEndOffset;
			Type = data.Type;
			Offset = data.Offset;
			Segment = data.Segment;
			Flags = data.Flags;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream, SymbolType symbolType) {
			var w = new SymbolDataWriter(pdb, stream, symbolType);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(NextOffset);
			w.WriteUInt32(Length);
			w.WriteUInt32(DebugStartOffset);
			w.WriteUInt32(DebugEndOffset);
			w.WriteIndexedType(Type);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteEnum<CV_PROCFLAGS>(Flags);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
