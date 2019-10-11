#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class SepCodeSym
	{
		public UInt32 Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Size { get; set; }
		public CV_SEPCODEFLAGS Flags { get; set; }
		public UInt32 Offset { get; set; }
		public UInt32 ParentOffset { get; set; }
		public UInt16 Section { get; set; }
		public UInt16 ParentSection { get; set; }
	}

	public class S_SEPCODE : ISymbol
	{
		private readonly UInt32 ParentSymOffset;
		public readonly Symbol Parent;
		public readonly UInt32 End;
		public readonly UInt32 Size;
		public readonly CV_SEPCODEFLAGS Flags;
		public readonly UInt32 Offset;
		public readonly UInt32 ParentOffset;
		public readonly UInt16 Section;
		public readonly UInt16 ParentSection;

		public S_SEPCODE(IServiceContainer ctx, IModule mod, SpanStream stream) {
			var r = new SymbolDataReader(ctx, stream);

			ParentSymOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(mod, ParentSymOffset);

			End = r.ReadUInt32();

			Size = r.ReadUInt32();
			Flags = r.ReadFlagsEnum<CV_SEPCODEFLAGS>();

			Offset = r.ReadUInt32();
			ParentOffset = r.ReadUInt32();

			Section = r.ReadUInt16();
			ParentSection = r.ReadUInt16();
		}

		public S_SEPCODE(SepCodeSym data) {
			ParentSymOffset = data.Parent;
			End = data.End;
			Size = data.Size;
			Flags = data.Flags;
			Offset = data.Offset;
			ParentOffset = data.ParentOffset;
			Section = data.Section;
			ParentSection = data.ParentSection;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_SEPCODE);
			w.WriteUInt32(ParentSymOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(Size);
			w.WriteEnum<CV_SEPCODEFLAGS>(Flags);
			w.WriteUInt32(Offset);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt16(Section);
			w.WriteUInt16(ParentSection);

			w.WriteSymbolHeader();
		}
	}
}
