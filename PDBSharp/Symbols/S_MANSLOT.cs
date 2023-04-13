#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_MANSLOT : SymbolBase
	{
		public UInt32 SlotIndex { get; set; }
		public ILeafContainer Type { get; set; }
		public CV_LVAR_ATTR Attributes { get; set; }
		public string Name { get; set; }

		public S_MANSLOT(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			SlotIndex = r.ReadUInt32();
			Type = r.ReadIndexedType32Lazy();
			Attributes = new CV_LVAR_ATTR(stream);
			Name = r.ReadSymbolString();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_MANSLOT);
			w.WriteUInt32(SlotIndex);
			w.WriteIndexedType(Type);
			Attributes.Write(w);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
