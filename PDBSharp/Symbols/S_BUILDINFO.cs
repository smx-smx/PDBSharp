#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_BUILDINFO : SymbolBase
	{
		public ILeafContainer ItemID { get; set; }

		public S_BUILDINFO(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			ItemID = r.ReadIndexedType32Lazy();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_BUILDINFO);
			w.WriteIndexedType(ItemID);

			w.WriteHeader();
		}

		public override string ToString() {
			return $"S_BUILDINFO[ItemID='{ItemID}']";
		}
	}
}
