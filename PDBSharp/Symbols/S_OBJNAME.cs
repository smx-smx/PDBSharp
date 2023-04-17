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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_OBJNAME
{

	public class Data : ISymbolData {
		public UInt32 Signature { get; set; }
		public string Name { get; set; }

		public Data(uint signature, string name) {
			Signature = signature;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer {
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var Signature = r.ReadUInt32();
			var Name = r.ReadSymbolString();
			
			Data = new Data(
				signature: Signature,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_OBJNAME);
			w.WriteUInt32(data.Signature);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
