#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

	public class LF_MODIFIER : ILeaf
	{
		public readonly CVModifier Flags;
		public readonly uint ModifiedType;

		public LF_MODIFIER(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			ModifiedType = r.ReadUInt32();
			Flags = r.ReadFlagsEnum<CVModifier>();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_MODIFIER);
			w.WriteUInt32(ModifiedType);
			w.WriteEnum<CVModifier>(Flags);
			w.WriteLeafHeader();
		}
	}
}
