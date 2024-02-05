#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using System.Runtime.InteropServices;

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

		private readonly IEnumerable<ISymbolResolver> symbols;
		public IEnumerable<ISymbolResolver> Symbols { get => symbols; }

		public readonly C11Lines? C11Lines;
		public readonly C13Lines? C13Lines;

		public event OnSymbolDataDelegate? OnSymbolData;

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

			symbols = new CachedEnumerable<ISymbolResolver>(ReadSymbols());
			Position = mod.SymbolsSize; // includes CodeViewSignature

			C11Lines = ReadLines();
			C13Lines = ReadC13Lines();
		}

		private C11Lines? ReadLines() {
			//ReadBytes((int)mod.LinesSize);
			if(mod.LinesSize <= 0) {
				return null;
			}

			SpanStream slice = this.SliceHere((int)mod.LinesSize);
			return new C11Lines(slice);
		}

		private C13Lines? ReadC13Lines() {
			if(mod.C13LinesSize <= 0) {
				return null;
			}

			SpanStream slice = this.SliceHere((int)mod.C13LinesSize);
			return new C13Lines(slice);
		}

		private IEnumerable<ISymbolResolver> ReadSymbols() {
			var symbolsStart = sizeof(CodeViewSignature);
			var symbolsSize = (int)mod.SymbolsSize - sizeof(CodeViewSignature); //exclude signature

			var symbolsData = PerformAt(symbolsStart, () => {
				return ReadBytes(symbolsSize);
			});
			
			// need to pass in the current stream, since CodeView offsets are relative to the stream
			var rdr = new SymbolsReader(ctx, new SpanStream(symbolsData), this);
			if (OnSymbolData != null) {
				rdr.OnSymbolData += OnSymbolData;
			}

			return rdr.ReadSymbols();
		}
	}
}
