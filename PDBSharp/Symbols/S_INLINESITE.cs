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

namespace Smx.PDBSharp.Symbols
{
	public class S_INLINESITE : SymbolBase
	{
		private UInt32 InlinerParentOffset;
		public Symbol Inliner;
		public UInt32 End;
		public ILeafContainer Inlinee;
		public byte[] BinaryAnnotations;

		public S_INLINESITE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			InlinerParentOffset = r.ReadUInt32();
			Inliner = r.ReadSymbol(Module, InlinerParentOffset);
			End = r.ReadUInt32();
			Inlinee = r.ReadIndexedType32Lazy();
			BinaryAnnotations = r.ReadRemaining();
		}
	}
}
