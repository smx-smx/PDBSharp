#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_THUNK32 : SymbolBase
	{
		private UInt32 ParentOffset;
		public Symbol Parent;
		public UInt32 End;
		private UInt32 NextOffset;
		public Symbol Next;
		public UInt32 Offset;
		public UInt16 Segment;
		public UInt16 ThunkLength;
		public ThunkType ThunkType;
		public string Name;

		public IThunk Thunk;

		public S_THUNK32(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(Module, ParentOffset);

			End = r.ReadUInt32();

			NextOffset = r.ReadUInt32();
			Next = r.ReadSymbol(Module, NextOffset);

			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			ThunkLength = r.ReadUInt16();
			ThunkType = r.ReadEnum<ThunkType>();
			Name = r.ReadSymbolString();
			Thunk = r.ReadThunk(ThunkType);
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_THUNK32);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(NextOffset);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteUInt16(ThunkLength);
			w.Write<ThunkType>(ThunkType);
			w.WriteSymbolString(Name);
			Thunk.Write(w);

			w.WriteHeader();
		}
	}
}
