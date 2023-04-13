#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_UNAMESPACE : SymbolBase
	{
		public string NamespaceName { get; set; }

		public S_UNAMESPACE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){			
		}

		public override void Read() {
			var r = CreateReader();
			NamespaceName = r.ReadSymbolString();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_UNAMESPACE);
			w.WriteSymbolString(NamespaceName);

			w.WriteHeader();
		}
	}
}
