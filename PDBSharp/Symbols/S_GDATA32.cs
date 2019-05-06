#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_GDATA32)]
	public class S_GDATA32 : DataSym32
	{
		public S_GDATA32(PDBFile pdb, Stream stream) : base(pdb, stream) {
		}
	}
}
