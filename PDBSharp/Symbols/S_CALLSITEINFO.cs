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
	public class S_CALLSITEINFO : SymbolBase
	{
		public UInt32 Offset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public ILeafContainer Type { get; set; }

		public S_CALLSITEINFO(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 			
		}
		public override void Read() {
			var r = CreateReader();
			Offset = r.ReadUInt32();
			SectionIndex = r.ReadUInt16();
			r.ReadUInt16(); //padding
			Type = r.ReadIndexedTypeLazy();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_CALLSITEINFO);
			w.WriteUInt32(Offset);
			w.WriteUInt16(SectionIndex);
			w.WriteUInt16(0x00); //padding
			w.WriteIndexedType(Type);

			w.WriteHeader();
		}
	}
}
