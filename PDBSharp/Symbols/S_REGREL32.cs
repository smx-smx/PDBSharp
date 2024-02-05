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

namespace Smx.PDBSharp.Symbols.S_REGREL32
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public ILeafResolver? Type { get; set; }
		public UInt16 RegisterIndex { get; set; }
		public string Name { get; set; }

		public Data(uint offset, ILeafResolver? type, ushort registerIndex, string name) {
			Offset = offset;
			Type = type;
			RegisterIndex = registerIndex;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var Offset = r.ReadUInt32();
			var Type = r.ReadIndexedType32Lazy();
			var RegisterIndex = r.ReadUInt16();
			var Name = r.ReadSymbolString();
			Data = new Data(
				offset: Offset,
				type: Type,
				registerIndex: RegisterIndex,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_REGREL32);
			w.WriteUInt32(data.Offset);
			w.WriteIndexedType(data.Type);
			w.WriteUInt16(data.RegisterIndex);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
