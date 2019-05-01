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

		private IEnumerable<ISymbol> symbols;

		public IEnumerable<ISymbol> Symbols {
			get {
				if (symbols == null)
					symbols = GetSymbols();
				return symbols;
			}
		}

		public ModuleReader(ModuleInfoInstance modInfo, Stream stream) : base(stream) {
			this.Module = modInfo;

			ModuleSignature signature = (ModuleSignature)Reader.ReadUInt32();

			if(!Enum.IsDefined(typeof(ModuleSignature), signature)) {
				throw new InvalidDataException();
			}

			if(signature != ModuleSignature.C13){
				throw new NotImplementedException($"CodeView {signature} not supported yet");
			}

			GetSymbols();
		}

		private IEnumerable<ISymbol> GetSymbols() {
			Stream.Position = sizeof(int); //after signature
			int symbolsSize = (int)Module.Header.SymbolsSize - sizeof(int); //exclude signature

			byte[] symbolsData = Reader.ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(new MemoryStream(symbolsData));
			return rdr.Symbols;
		}
	}
}
