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
	public class S_COFFGROUP : SymbolBase
	{
		public UInt32 Size { get; set; }
		public UInt32 Characteristics { get; set; }
		public UInt32 SymbolOffset { get; set; }
		public UInt16 SymbolSegment { get; set; }
		public string Name { get; set; }

		public S_COFFGROUP(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();
			Size = r.ReadUInt32();
			Characteristics = r.ReadUInt32();
			SymbolOffset = r.ReadUInt32();
			SymbolSegment = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_COFFGROUP);
			w.WriteUInt32(Size);
			w.WriteUInt32(Characteristics);
			w.WriteUInt32(SymbolOffset);
			w.WriteUInt16(SymbolSegment);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
