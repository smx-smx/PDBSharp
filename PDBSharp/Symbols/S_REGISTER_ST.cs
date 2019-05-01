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
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_REGISTER_ST)]
	public class S_REGISTER_ST : ReaderBase, ISymbol
	{
		public SymbolHeader Header { get; }
		public readonly RegSymInstance Data;

		public S_REGISTER_ST(Stream stream) : base(stream) {
			var rdr = new RegSymReader(stream);
			Header = rdr.Header;
			Data = rdr.Data;
		}
	}
}
