#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	public class S_INLINESITE : ISymbol
	{
		private readonly UInt32 InlinerParentOffset;
		public readonly Symbol Inliner;
		public readonly UInt32 End;
		public readonly ILeafContainer Inlinee;
		public readonly byte[] BinaryAnnotations;

		public S_INLINESITE(Context ctx, IModule mod, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);
			InlinerParentOffset = r.ReadUInt32();
			Inliner = r.ReadSymbol(mod, InlinerParentOffset);
			End = r.ReadUInt32();
			Inlinee = r.ReadIndexedTypeLazy();
			BinaryAnnotations = r.ReadRemaining();
		}

		public void Write(PDBFile pdb, Stream stream) {
			throw new NotImplementedException();
		}
	}
}
