#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{

	[LeafReader(LeafType.LF_MODIFIER)]
	public class LF_MODIFIER : TypeDataReader
	{
		public readonly CVModifier Flags;
		public readonly uint ModifiedType;

		public LF_MODIFIER(PDBFile pdb, Stream stream) : base(pdb, stream) {
			ModifiedType = ReadUInt32();
			Flags = ReadFlagsEnum<CVModifier>();
		}
	}
}
