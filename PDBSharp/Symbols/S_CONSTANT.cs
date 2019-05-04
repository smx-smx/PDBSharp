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
	[SymbolReader(SymbolType.S_CONSTANT)]
	public class S_CONSTANT : ConstSym
	{
		public S_CONSTANT(PDBFile pdb, Stream stream) : base(pdb, stream) {
		}
	}
}
