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
using System.Text;

namespace Smx.PDBSharp
{
	namespace PublicSymbolsStream {
		public struct PSGIHDR {
			public int cbSymHash;
			public int cbAddrMap;
			public uint nThunks;
			public int  cbSizeOfThunk;
			public int isectThunkTable;
			public int offThunkTable;
			public uint nSects;

		}

		public class Data {
			public PSGIHDR Header;
			public byte[] SymbolHashMap = new byte[0];
			public byte[] AddressMap = new byte[0];
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var hdr = stream.ReadStruct<PSGIHDR>();

				var symbolHashMap = stream.ReadBytes(hdr.cbSymHash);
				var addressMap = stream.ReadBytes(hdr.cbAddrMap);
				Data = new Data {
					SymbolHashMap = symbolHashMap,
					AddressMap = addressMap,
				};
				return Data;
			}
		}
	}
}
