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
	public class LF_ARRAY : ILeaf
	{
		public readonly ILeafContainer ElementType;
		public readonly ILeafContainer IndexingType;

		public readonly ILeafContainer Size;

		public readonly string Name;

		public LF_ARRAY(Context pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			ElementType = r.ReadIndexedTypeLazy();
			IndexingType = r.ReadIndexedTypeLazy();

			Size = r.ReadVaryingType(out uint dataSize);

			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, Leaves.LeafType.LF_ARRAY);
			w.WriteIndexedType(ElementType);
			w.WriteIndexedType(IndexingType);
			w.WriteVaryingType(Size);
			w.WriteCString(Name);
			w.WriteLeafHeader();
		}
	}
}
