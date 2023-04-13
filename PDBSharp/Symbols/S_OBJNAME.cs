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

	public class S_OBJNAME : SymbolBase
	{
		public UInt32 Signature { get; set; }
		public string Name { get; set; }

		public S_OBJNAME(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();

			Signature = r.ReadUInt32();
			Name = r.ReadSymbolString();
		}

		public void Write() {
			var w = CreateWriter(SymbolType.S_OBJNAME);
			w.WriteUInt32(Signature);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
