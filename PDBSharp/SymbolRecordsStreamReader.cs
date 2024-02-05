#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Threading;

namespace Smx.PDBSharp
{
	namespace SymbolRecordsStreamReader
	{
		public class Data {
			public byte[] Records = new byte[0];
		}
		public class Serializer {
			public Data Data = new Data();
			private readonly IServiceContainer sc;
			private readonly SpanStream stream;

			public Serializer(IServiceContainer sc, SpanStream stream) {
				this.sc = sc;
				this.stream = stream;
			}

			public IEnumerable<ISymbolResolver> Read() {
				var rdr = new SymbolsReader(this.sc, stream);
				return rdr.ReadSymbols();
			}
		}
	}
}
