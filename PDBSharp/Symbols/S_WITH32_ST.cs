#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
	[SymbolReader(SymbolType.S_WITH32_ST)]
	class S_WITH32_ST : S_WITH32
	{
		public S_WITH32_ST(PDBFile pdb, Stream stream) : base(pdb, stream) {
		}
	}
}
