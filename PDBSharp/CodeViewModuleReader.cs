#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public enum CodeViewSignature : UInt32
	{
		C6 = 0,
		C7 = 1,
		C11 = 2,
		C13 = 4
	}

	public class CodeViewModuleReader : SpanStream, IModule
	{
		private readonly IServiceContainer ctx;
		private readonly ModuleInfo mod;

		public event OnSymbolDataDelegate OnSymbolData;

		private readonly IEnumerable<Symbol> symbols;
		public IEnumerable<Symbol> Symbols { get => symbols; }

		public CodeViewModuleReader(IServiceContainer ctx, ModuleInfo mod, SpanStream stream) : base(stream) {
			this.ctx = ctx;
			this.mod = mod;

			CodeViewSignature signature = ReadEnum<CodeViewSignature>();
			switch (signature) {
				case CodeViewSignature.C11:
				case CodeViewSignature.C13:
					break;
				default:
					throw new NotImplementedException($"CodeView {signature} not supported yet");
			}

			symbols = new CachedEnumerable<Symbol>(ReadSymbols());
			ReadLines();
			ReadC13Lines();
		}

		private void ReadLines() {
			//ReadBytes((int)mod.LinesSize);
			if(mod.LinesSize <= 0) {
				return;
			}

			SpanStream slice = this.SliceHere((int)mod.LinesSize);
			C11Lines lines = new C11Lines(slice);
		}

		private void ReadC13Lines() {
			SpanStream slice = this.SliceHere((int)mod.C13LinesSize);
			C13Lines lines = new C13Lines(slice);
		}

		private IEnumerable<Symbol> ReadSymbols() {
			int symbolsSize = (int)mod.SymbolsSize - sizeof(CodeViewSignature); //exclude signature
			
			byte[] symbolsData = ReadBytes(symbolsSize);
			var rdr = new SymbolsReader(ctx, this, symbolsData);
			if (OnSymbolData != null) {
				rdr.OnSymbolData += OnSymbolData;
			}

			return rdr.ReadSymbols();
		}
	}
}
