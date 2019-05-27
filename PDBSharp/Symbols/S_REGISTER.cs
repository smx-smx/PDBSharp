#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_REGISTER)]
	public class S_REGISTER : SymbolDataReader
	{
		public readonly ILeaf Type;
		public readonly UInt16 Register;
		public readonly string Name;

		public S_REGISTER(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Type = ReadIndexedTypeLazy();
			Register = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}
