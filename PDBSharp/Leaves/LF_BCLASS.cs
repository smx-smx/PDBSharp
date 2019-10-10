#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_BCLASS : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer BaseClassType;

		public readonly ILeafContainer Offset;

		public LF_BCLASS(IServiceContainer pdb, ReaderSpan stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			BaseClassType = r.ReadIndexedTypeLazy();

			Offset = r.ReadVaryingType(out uint dataSize);
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_BCLASS);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(BaseClassType);
			w.WriteIndexedType(Offset);
			w.WriteLeafHeader();
		}
	}
}
