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

namespace Smx.PDBSharp.Leaves
{
	public class LF_VFUNCTAB : ILeaf
	{
		public readonly ILeafContainer PointerType;

		public LF_VFUNCTAB(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			r.ReadUInt16(); //padding
			PointerType = r.ReadIndexedTypeLazy();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_VFUNCTAB);
			w.WriteUInt16(0x00);
			w.WriteIndexedType(PointerType);
			w.WriteLeafHeader();
		}
	}
}
