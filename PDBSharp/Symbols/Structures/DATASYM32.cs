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
	public abstract class DataSym32Base : SymbolBase
	{
		public ILeafContainer Type { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }

		public DataSym32Base(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			Type = r.ReadIndexedTypeLazy();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Name = r.ReadSymbolString(); 
		}

		public void Write(SymbolType symbolType) {
			var w = CreateWriter(symbolType);
			w.WriteIndexedType(Type);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
