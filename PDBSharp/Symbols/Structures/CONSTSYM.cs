#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ConstSym : SymbolDataReader
	{
		public readonly UInt32 TypeIndex;
		public readonly UInt16 Value;
		public readonly string Name;

		public ConstSym(PDBFile pdb, Stream stream) : base(stream) {
			TypeIndex = ReadUInt32();

			//$TODO: Numeric Leaf
			Value = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}
