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
	public class S_OEM : SymbolBase
	{
		public Guid Id { get; set; }
		public ILeafContainer Type { get; set; }
		public byte[] UserData { get; set; }

		public S_OEM(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			Id = new Guid(r.ReadBytes(16));
			Type = r.ReadIndexedType32Lazy();
			UserData = r.ReadRemaining();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_OEM);
			w.WriteBytes(Id.ToByteArray());
			w.WriteIndexedType(Type);
			w.WriteBytes(UserData);

			w.WriteHeader();
		}
	}
}
