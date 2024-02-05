#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.LeafResolver;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp.Symbols.Structures
{
	namespace THREADSYM32 {
		public record Data(
			ILeafResolver? Type,
			uint Offset,
			ushort Segment,
			string Name
		) : ISymbolData;

		public class Serializer : SymbolSerializerBase, ISymbolSerializer
		{
			private readonly SpanStream stream;

			public Data? Data;

			public Serializer(IServiceContainer sc, SpanStream stream) : base(sc, stream) {
				this.stream = stream;
			}

			public ISymbolData? GetData() {
				return Data;
			}

			public void Read() {
				var r = CreateReader();
				var type = r.ReadIndexedType32Lazy();
				var offset = r.ReadUInt32();
				var segment = r.ReadUInt16();
				var name = r.ReadSymbolString();
				Data = new Data(type, offset, segment, name);
			}

			public void Write() {
			}
		}
	}
}
