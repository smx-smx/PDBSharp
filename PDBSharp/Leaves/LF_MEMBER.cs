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
	public class LF_MEMBER : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer FieldType;

		public readonly ILeafContainer Offset;

		public readonly string Name;

		public LF_MEMBER(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			FieldType = r.ReadIndexedTypeLazy();

			Offset = r.ReadVaryingType(out uint dataSize);

			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_MEMBER);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(FieldType);
			w.WriteVaryingType(Offset);
			w.WriteCString(Name);
			w.WriteLeafHeader();
		}
	}
}
