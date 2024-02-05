#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Thunks
{
	namespace VCALL {
		public class Data : IThunk {
			public ushort VTableOffset;
		}
		public class Serializer : ISerializer<Data> {
			public Serializer(IServiceContainer sc, SymbolHeader header, SpanStreamEx stream) {
				this.sc = sc;
				this.header = header;
				this.stream = new SpanStreamEx(stream);
				reader = new SymbolData.Reader(sc, stream);
			}

			public Data Data = new Data();
			private readonly IServiceContainer sc;
			private readonly SymbolHeader header;
			private SpanStreamEx stream;
			private readonly SymbolData.Reader reader;


			public Data Read() {
				reader.Initialize(header);

				var VTableOffset = stream.ReadUInt16();
				
				Data = new Data {
					VTableOffset = VTableOffset
				};
				return Data;
			}


			public void Write(Data data) {
				/*
				w.WriteUInt16(Data.VTableOffset);
				*/
			}
		}
	}
}
