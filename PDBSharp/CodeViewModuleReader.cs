#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
		private readonly Context ctx;
		private readonly ModuleInfo mod;

		private readonly Lazy<IEnumerable<Symbol>> lazySymbols;

		public event OnSymbolDataDelegate OnSymbolData;

		public IEnumerable<Symbol> Symbols => lazySymbols.Value;

		public CodeViewModuleReader(Context ctx, ModuleInfo mod, Stream stream) : base(stream) {
			this.ctx = ctx;
			this.mod = mod;

			CodeViewSignature signature = ReadEnum<CodeViewSignature>();
			if(signature != CodeViewSignature.C13){
				throw new NotImplementedException($"CodeView {signature} not supported yet");
			}

			lazySymbols = new Lazy<IEnumerable<Symbol>>(ReadSymbols);
		}

		private IEnumerable<Symbol> ReadSymbols() {
			int symbolsSize = (int)mod.SymbolsSize - sizeof(CodeViewSignature); //exclude signature
			byte[] symbolsData = ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(ctx, this, new MemoryStream(symbolsData));
			if(OnSymbolData != null) {
				rdr.OnSymbolData += OnSymbolData;
			}

			return rdr.ReadSymbols();
		}
	}
}
