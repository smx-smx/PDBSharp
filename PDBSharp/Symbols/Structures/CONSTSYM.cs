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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ConstSymData : ISymbolData {
		public ILeafResolver? Type { get; set; }
		public ILeafResolver? Value { get; set; }
		public string Name { get; set; }

		public ConstSymData(ILeafResolver? type, ILeafResolver? value, string name) {
			Type = type;
			Value = value;
			Name = name;
		}
	}


	public abstract class ConstSymSerializerBase : SymbolSerializerBase, ISymbolSerializer
	{
		public ConstSymData? Data { get; set; }

		public ConstSymSerializerBase(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public void Read() {
			var r = CreateReader();

			var Type = r.ReadIndexedType32Lazy();
			var Value = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadSymbolString();
			Data = new ConstSymData(
				type: Type,
				value: Value,
				name: Name
			);
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;

		public void Write(SymbolType symbolType) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(symbolType);

			w.WriteIndexedType(data.Type);
			w.WriteVaryingType(data.Value);
			w.WriteSymbolString(data.Name);
			w.WriteHeader();
		}
	}
}
