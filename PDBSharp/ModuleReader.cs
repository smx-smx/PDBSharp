#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class ModuleReader : ReaderBase
	{
		private const int SIGNATURE = 4;
		private readonly ModuleInfoInstance modInfo;

		private IEnumerable<ISymbol> symbols;

		public IEnumerable<ISymbol> Symbols {
			get {
				if (symbols == null)
					symbols = GetSymbols();
				return symbols;
			}
		}

		public ModuleReader(ModuleInfoInstance modInfo, Stream stream) : base(stream) {
			this.modInfo = modInfo;

			uint signature = Reader.ReadUInt32();
			if (signature != SIGNATURE) {
				throw new InvalidDataException();
			}

			GetSymbols();
		}

		private IEnumerable<ISymbol> GetSymbols() {
			Stream.Position = sizeof(int); //after signature
			int symbolsSize = (int)modInfo.Header.SymbolsSize - sizeof(int); //exclude signature

			byte[] symbolsData = Reader.ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(new MemoryStream(symbolsData));
			return rdr.Symbols;
		}
	}
}
