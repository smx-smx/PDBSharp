#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE : ISymbol
	{
		public S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE(PDBFile pdb, Stream stream) {

		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = new SymbolDataWriter(pdb, stream, SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE);
			w.WriteSymbolHeader();
		}
	}
}
