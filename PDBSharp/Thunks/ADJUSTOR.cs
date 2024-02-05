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
using System.Xml.Linq;

namespace Smx.PDBSharp.Thunks
{
	namespace ADJUSTOR {
		public class Data : IThunk {
			public UInt16 Delta;
			public string Name = string.Empty;
		}

		public class Serializer : ISerializer<Data>
		{
			private readonly IServiceContainer sc;
			private readonly SpanStreamEx stream;
			private readonly SymbolHeader header;
			public Data Data = new Data();
			private readonly SymbolData.Reader reader;

			public Serializer(IServiceContainer sc, SymbolHeader header, SpanStreamEx stream) {
				this.sc = sc;
				this.stream = stream;
				this.header = header;
				this.reader = new SymbolData.Reader(sc, stream);
			}

			public Data Read() {
				reader.Initialize(header);
				var Delta = stream.ReadUInt16();
				var Name = reader.ReadSymbolString();

				Data = new Data {
					Delta = Delta,
					Name = Name
				};
				return Data;
			}

			public void Write(Data data) {
				/*
				w.WriteUInt16(Data.Delta);
				w.WriteSymbolString(Data.Name);
				*/
			}
		}
	}
}
