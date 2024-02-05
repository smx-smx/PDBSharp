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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.FUNCTIONLIST
{
	public class Data : ISymbolData {
		public UInt32 NumberOfFunctions { get; set; }
		public ILeafResolver?[] Functions { get; set; }

		public Data(UInt32 numberOfFunctions, ILeafResolver?[] functions) {
			NumberOfFunctions = numberOfFunctions;
			Functions = functions;
		}

	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) { 			
		}

		public void Read() {
			var r = CreateReader();

			var NumberOfFunctions = r.ReadUInt32();
			var Functions = Enumerable
				.Range(1, (int)NumberOfFunctions)
				.Select(_ => r.ReadIndexedType32Lazy())
				.ToArray();

			Data = new Data(
				numberOfFunctions: NumberOfFunctions,
				functions: Functions
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_CALLEES);
			w.WriteUInt32(data.NumberOfFunctions);
			foreach (ILeafResolver? fn in data.Functions) {
				w.WriteIndexedType(fn);
			}

			w.WriteHeader();
		}
	}
}
