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
	public class S_REGISTER : SymbolBase
	{
		public ILeafContainer Type { get; set; }
		public UInt16 Register { get; set; }
		public string Name { get; set; }

		public S_REGISTER(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();

			Type = r.ReadIndexedTypeLazy();
			Register = r.ReadUInt16();
			Name = r.ReadSymbolString();
		}		

		public override void Write() {
			var w = CreateWriter(SymbolType.S_REGISTER);
			w.WriteIndexedType(Type);
			w.WriteUInt16(Register);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
