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
	[SymbolReader(SymbolType.S_GPROC32)]
	public class S_GPROC32 : ReaderBase, ISymbol
	{
		public SymbolHeader Header { get; }
		public readonly ProcSym32Instance Data;

		public S_GPROC32(Stream stream) : base(stream) {
			var rdr = new ProcSym32Reader(stream);
			Header = rdr.Header;
			Data = rdr.Data;
		}
	}
}
