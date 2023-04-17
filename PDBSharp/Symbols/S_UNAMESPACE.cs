#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

using System;
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_UNAMESPACE
{
	public class Data : ISymbolData
	{
		public string NamespaceName { get; set; }

		public Data(string namespaceName) {
			NamespaceName = namespaceName;
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
			var NamespaceName = r.ReadSymbolString();
			Data = new Data(
				namespaceName: NamespaceName
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_UNAMESPACE);
			w.WriteSymbolString(data.NamespaceName);
			w.WriteHeader();
		}
	}
}
