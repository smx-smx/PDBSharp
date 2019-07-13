#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	public class S_GMANPROC : ManProcSymBase, ISymbol
	{
		public S_GMANPROC(Context ctx, Stream stream) : base(ctx, stream) {
		}

		public S_GMANPROC(ManProcSym data) : base(data) {
		}

		public void Write(PDBFile pdb, Stream stream) {
			base.Write(pdb, stream, SymbolType.S_GMANPROC);
		}
	}
}
