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
	public class S_BPREL32 : SymbolBase
	{
		public UInt32 Offset { get; set; }
		public ILeafContainer Type { get; set; }
		public string Name { get; set; }

		public S_BPREL32(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			SymbolDataReader r = CreateReader();
			Offset = r.ReadUInt32();
			Type = r.ReadIndexedTypeLazy();
			Name = r.ReadSymbolString(); 
		}		

		public override void Write() {
			SymbolDataWriter w = CreateWriter(SymbolType.S_BPREL32);
			w.WriteUInt32(Offset);
			w.WriteIndexedType(Type);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
