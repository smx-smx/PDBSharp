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
	public class LF_VBCLASS : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer VirtualBaseClassType;
		public readonly ILeafContainer VirtualBasePointerType;

		public readonly ILeafContainer OffsetFromAddress;
		public readonly ILeafContainer OffsetFromTable;

		public LF_VBCLASS(IServiceContainer pdb, SpanReader stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			VirtualBaseClassType = r.ReadIndexedTypeLazy();
			VirtualBasePointerType = r.ReadIndexedTypeLazy();


			//virtual base pointer offset from address point
			OffsetFromAddress = r.ReadVaryingType(out uint dynSize1);
			//virtual base offset from vbtable
			OffsetFromTable = r.ReadVaryingType(out uint dynSize2);
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_IVBCLASS);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(VirtualBaseClassType);
			w.WriteIndexedType(VirtualBasePointerType);
			w.WriteVaryingType(OffsetFromAddress);
			w.WriteVaryingType(OffsetFromTable);
			w.WriteLeafHeader();
		}
	}
}
