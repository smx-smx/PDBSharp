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

namespace Smx.PDBSharp.Symbols
{
	public class S_BLOCK32 : SymbolBase
	{
		public UInt32 ParentOffset { get; set; }
		public Symbol Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }

		public S_BLOCK32(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			SymbolDataReader r = CreateReader();
			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(Module, ParentOffset);

			End = r.ReadUInt32();
			Length = r.ReadUInt32();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public override void Write() {
			SymbolDataWriter w = CreateWriter(SymbolType.S_BLOCK32);
			w.WriteUInt32(ParentOffset);
			w.WriteUInt32(End);
			w.WriteUInt32(Length);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}