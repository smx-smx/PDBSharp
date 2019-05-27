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
	public enum CodeViewSignature  : UInt32
	{
		C6 = 0,
		C7 = 1,
		C11 = 2,
		C13 = 4
	}

	public class CodeViewModuleReader : ReaderBase, IModule
	{
		public ModuleInfo Module { get; }

		public IEnumerable<Symbol> Symbols {
			get {
				return ReadSymbols().Cached();
			}
		}

		private readonly PDBFile pdb;

		public CodeViewModuleReader(PDBFile pdb, ModuleInfo modInfo, Stream stream) : base(stream) {
			this.pdb = pdb;
			this.Module = modInfo;

			CodeViewSignature signature = ReadEnum<CodeViewSignature>();
			if(signature != CodeViewSignature.C13){
				throw new NotImplementedException($"CodeView {signature} not supported yet");
			}
		}

		private IEnumerable<Symbol> ReadSymbols() {
			int symbolsSize = (int)Module.SymbolsSize - sizeof(CodeViewSignature); //exclude signature
			byte[] symbolsData = ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(pdb, new MemoryStream(symbolsData));
			return rdr.ReadSymbols();
		}
	}
}
