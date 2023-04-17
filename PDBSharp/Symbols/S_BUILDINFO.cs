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
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_BUILDINFO
{
	public class Data : ISymbolData {
		public ILeafResolver? ItemID { get; set; }
		
		public Data(ILeafResolver? itemId) {
			ItemID = itemId;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public void Read() {
			var r = CreateReader();
			var ItemID = r.ReadIndexedType32Lazy();
			Data = new Data(
				itemId: ItemID
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_BUILDINFO);
			w.WriteIndexedType(data.ItemID);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"S_BUILDINFO[ItemID='{data?.ItemID}']";
		}
	}
}
