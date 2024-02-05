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

namespace Smx.PDBSharp.Symbols.S_REGISTER
{
	public class Data : ISymbolData {
		public ILeafResolver? Type { get; set; }
		public UInt16 Register { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? type, ushort register, string name) {
			Type = type;
			Register = register;
			Name = name;
		}
	}


	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var Type = r.ReadIndexedType32Lazy();
			var Register = r.ReadUInt16();
			var Name = r.ReadSymbolString();
			Data = new Data(
				type: Type,
				register: Register,
				name: Name
			);
		}		

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_REGISTER);
			w.WriteIndexedType(data.Type);
			w.WriteUInt16(data.Register);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
