#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{	public abstract class ManProcSymBase : SymbolBase
	{
		/// <summary>
		/// Parent Symbol
		/// </summary>
		public Symbol Parent;
		private UInt32 ParentOffset;
		/// <summary>
		/// End of block
		/// </summary>
		public UInt32 End;
		/// <summary>
		/// Next Symbol
		/// </summary>
		public Symbol Next;
		private UInt32 NextOffset;
		public UInt32 ProcLength;
		public UInt32 DebugStartOffset;
		public UInt32 DebugEndOffset;
		public UInt32 ComToken;
		public UInt32 Offset;
		public UInt16 Segment;
		public CV_PROCFLAGS Flags;
		public UInt16 ReturnRegister;
		public string Name;

		public ManProcSymBase(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(Module, ParentOffset);
			End = r.ReadUInt32();
			NextOffset = r.ReadUInt32();
			Next = r.ReadSymbol(Module, NextOffset);
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

		public void Write(SymbolType symbolType) {
			var w = CreateWriter(symbolType);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(NextOffset);
			w.WriteUInt32(ProcLength);
			w.WriteUInt32(DebugStartOffset);
			w.WriteUInt32(DebugEndOffset);
			w.WriteUInt32(ComToken);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.Write<CV_PROCFLAGS>(Flags);
			w.WriteUInt16(ReturnRegister);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
