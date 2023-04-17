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
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_INLINESITE
{
	public class Data : ISymbolData {
		public UInt32 InlinerParentOffset { get; set; }
		public ISymbolResolver? Inliner { get; set; }
		public UInt32 End { get; set; }
		public ILeafResolver? Inlinee { get; set; }
		public byte[] BinaryAnnotations { get; set; }

		public Data(uint inlinerParentOffset, ISymbolResolver? inliner, uint end, ILeafResolver? inlinee, byte[] binaryAnnotations) {
			InlinerParentOffset = inlinerParentOffset;
			Inliner = inliner;
			End = end;
			Inlinee = inlinee;
			BinaryAnnotations = binaryAnnotations;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public void Read() {
			var r = CreateReader();
			var InlinerParentOffset = r.ReadUInt32();
			var Inliner = r.ReadSymbol(Module, InlinerParentOffset);
			var End = r.ReadUInt32();
			var Inlinee = r.ReadIndexedType32Lazy();
			var BinaryAnnotations = r.ReadRemaining();

			Data = new Data(
				inlinerParentOffset: InlinerParentOffset,
				inliner: Inliner,
				end: End,
				inlinee: Inlinee,
				binaryAnnotations: BinaryAnnotations
			);
		}
	}
}
