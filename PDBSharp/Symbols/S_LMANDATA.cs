#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_LMANDATA : DataSym32Base, ISymbol
	{
		public S_LMANDATA(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, stream) {
		}

		public S_LMANDATA(DataSym32 data) : base(data) {
		}

		public void Write(PDBFile pdb, Stream stream) {
			base.Write(pdb, stream, SymbolType.S_LMANDATA);
		}
	}
}
