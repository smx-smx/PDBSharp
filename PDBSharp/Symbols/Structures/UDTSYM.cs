#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.Structures.UdtSym
{
	public class Data : ISymbolData {
		public ILeafResolver? Type { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? type, string name) {
			Type = type;
			Name = name;
		}

		public override string ToString() {
			return $"UdtSym(Type='{Type}', Name='{Name}')";
		}
	}

	public class UdtSym : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() { return Data; }

		public UdtSym(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();
			var Type = r.ReadIndexedType32Lazy();
			var Name = r.ReadSymbolString();
			Data = new Data(
				type: Type,
				name: Name
			);
		}

		public void Write(SymbolType symbolType) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(symbolType);
			w.WriteIndexedType(data.Type);
			w.WriteSymbolString(data.Name);
			w.WriteHeader();
		}
	}
}
