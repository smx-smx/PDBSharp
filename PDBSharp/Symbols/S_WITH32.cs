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

namespace Smx.PDBSharp.Symbols
{
	public class S_WITH32 : SymbolBase
	{
		private UInt32 ParentOffset;
		public Symbol Parent;
		private UInt32 EndOffset;
		public UInt32 Length;
		public UInt32 SegmentOffset;
		public UInt16 Segment;
		public string Expression;

		public S_WITH32(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 
		}

		public override void Read() {
			var r = CreateReader();
			ParentOffset = r.ReadUInt32();
			Parent = r.ReadSymbol(Module, ParentOffset);
			EndOffset = r.ReadUInt32();
			Length = r.ReadUInt32();
			SegmentOffset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Expression = r.ReadSymbolString();
		}

		public override void Write() {
			SymbolDataWriter w = CreateWriter(SymbolType.S_WITH32);
			w.WriteHeader();
			throw new NotImplementedException();
		}
	}
}
