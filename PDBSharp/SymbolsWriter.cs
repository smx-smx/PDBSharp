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

namespace Smx.PDBSharp
{
	public class SymbolsWriter : WriterBase
	{
		public SymbolsWriter(Stream stream) : base(stream) {
		}

		public void WriteSymbols(IEnumerable<Symbol> symbols) {
		}
	}
}
