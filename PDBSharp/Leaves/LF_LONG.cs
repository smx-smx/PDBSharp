#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Leaves
{
	public class LF_LONG : ILeaf
	{
		public readonly int Value;

		public LF_LONG(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);
			Value = r.ReadInt32();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_LONG);
			w.WriteInt32(Value);
			w.WriteLeafHeader();
		}
	}
}
