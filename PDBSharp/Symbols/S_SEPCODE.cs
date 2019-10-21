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
	public class S_SEPCODE : SymbolBase
	{
		private UInt32 ParentSymOffset;
		public Symbol Parent;
		public UInt32 End;
		public UInt32 Size;
		public CV_SEPCODEFLAGS Flags;
		public UInt32 Offset;
		public UInt32 ParentOffset;
		public UInt16 Section;
		public UInt16 ParentSection;

		public S_SEPCODE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();

			ParentSymOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(Module, ParentSymOffset);

			End = r.ReadUInt32();

			Size = r.ReadUInt32();
			Flags = r.ReadFlagsEnum<CV_SEPCODEFLAGS>();

			Offset = r.ReadUInt32();
			ParentOffset = r.ReadUInt32();

			Section = r.ReadUInt16();
			ParentSection = r.ReadUInt16();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_SEPCODE);
			w.WriteUInt32(ParentSymOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(Size);
			w.Write<CV_SEPCODEFLAGS>(Flags);
			w.WriteUInt32(Offset);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt16(Section);
			w.WriteUInt16(ParentSection);

			w.WriteHeader();
		}
	}
}
