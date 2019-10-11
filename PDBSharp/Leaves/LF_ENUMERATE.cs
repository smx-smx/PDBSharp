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

	public class LF_ENUMERATE : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer Length;
		public readonly string FieldName;

		public LF_ENUMERATE(IServiceContainer pdb, SpanStream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			Length = r.ReadVaryingType(out uint ILeafSize);
			FieldName = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_ENUMERATE);
			w.WriteUInt16((ushort)Attributes);
			w.WriteVaryingType(Length);
			w.WriteCString(FieldName);
			w.WriteLeafHeader();
		}
	}
}
