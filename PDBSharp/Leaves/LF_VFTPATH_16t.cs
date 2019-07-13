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

namespace Smx.PDBSharp.Leaves
{
	public class LF_VFTPATH_16t : ILeaf
	{
		public readonly UInt16 NumElements;
		public readonly ILeafContainer Bases;

		public LF_VFTPATH_16t(Context pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			NumElements = r.ReadUInt16();
			Bases = r.ReadIndexedType16Lazy();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_VFTPATH_16t);
			w.WriteUInt16(NumElements);
			w.WriteIndexedType16(Bases);
			w.WriteLeafHeader();
		}
	}
}
