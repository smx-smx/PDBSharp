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
	public abstract class ProcSym32Base : SymbolBase
	{
		private UInt32 ParentOffset;
		public UInt32 End;
		private UInt32 NextOffset;
		public UInt32 Length;
		public UInt32 DebugStartOffset;
		public UInt32 DebugEndOffset;
		public ILeafContainer Type;
		public UInt32 Offset;
		public UInt16 Segment;
		public CV_PROCFLAGS Flags;
		public string Name;

		public Symbol ParentSymbol;
		public Symbol NextSymbol;

		public ProcSym32Base(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();

			ParentOffset = r.ReadUInt32();
			ParentSymbol = r.ReadSymbol(Module, ParentOffset);

			End = r.ReadUInt32();
			NextOffset = r.ReadUInt32();
			NextSymbol = r.ReadSymbol(Module, NextOffset);

			Length = r.ReadUInt32();
			DebugStartOffset = r.ReadUInt32();
			DebugEndOffset = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			Name = r.ReadSymbolString();
		}

		public void Write(SymbolType symbolType) {
			var w = CreateWriter(symbolType);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(NextOffset);
			w.WriteUInt32(Length);
			w.WriteUInt32(DebugStartOffset);
			w.WriteUInt32(DebugEndOffset);
			w.WriteIndexedType(Type);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.Write<CV_PROCFLAGS>(Flags);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
