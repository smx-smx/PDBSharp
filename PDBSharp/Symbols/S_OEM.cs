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
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_OEM
{
	public class Data : ISymbolData {
		public Guid Id { get; set; }
		public ILeafResolver? Type { get; set; }
		public byte[] UserData { get; set; }

		public Data(Guid id, ILeafResolver? type, byte[] userData) {
			Id = id;
			Type = type;
			UserData = userData;
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

			var Id = new Guid(r.ReadBytes(16));
			var Type = r.ReadIndexedType32Lazy();
			var UserData = r.ReadRemaining();
			Data = new Data(
				id: Id,
				type: Type,
				userData: UserData
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_OEM);
			w.WriteBytes(data.Id.ToByteArray());
			w.WriteIndexedType(data.Type);
			w.WriteBytes(data.UserData);

			w.WriteHeader();
		}
	}
}
