#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class SymbolsReader : ReaderBase
	{
		public SymbolsReader(Stream stream) : base(stream) {
		}

		private IEnumerable<ISymbol> symbols;

		public IEnumerable<ISymbol> Symbols {
			get {
				if (symbols == null)
					symbols = GetSymbols();
				return symbols;
			}
		}

		private IEnumerable<ISymbol> GetSymbols() {
			var remaining = Stream.Length;

			while(remaining > 0) {
				// number of bytes that follow
				UInt16 length = Reader.ReadUInt16();

				// the data view includes the length field we just read
				int dataSize = length + sizeof(UInt16);
				byte[] symDataBuf = new byte[dataSize];

				BinaryWriter wr = new BinaryWriter(new MemoryStream(symDataBuf));
				wr.Write(length);
				wr.Write(Reader.ReadBytes((int)length));

				SymbolDataReader rdr = new SymbolDataReader(new MemoryStream(symDataBuf));
				yield return rdr.Symbol;

				remaining -= dataSize;
			}
		}
	}
}
