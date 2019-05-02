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
	public enum ModuleSignature  : UInt32
	{
		C6 = 0,
		C7 = 1,
		C11 = 2,
		C13 = 4
	}

	public class ModuleReader : ReaderBase, IModule
	{
		public ModuleInfoInstance Module { get; }

		public IEnumerable<ISymbol> Symbols {
			get {
				return ReadSymbols().Cached();
			}
		}

		public ModuleReader(ModuleInfoInstance modInfo, Stream stream) : base(stream) {
			this.Module = modInfo;

			ModuleSignature signature = ReadEnum<ModuleSignature>();
			if(signature != ModuleSignature.C13){
				throw new NotImplementedException($"CodeView {signature} not supported yet");
			}
		}

		private IEnumerable<ISymbol> ReadSymbols() {
			int symbolsSize = (int)Module.Header.SymbolsSize - sizeof(ModuleSignature); //exclude signature
			byte[] symbolsData = ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(new MemoryStream(symbolsData));
			return rdr.ReadSymbols();
		}
	}
}
