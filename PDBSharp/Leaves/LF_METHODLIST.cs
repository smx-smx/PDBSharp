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
	public class LF_METHODLIST : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer ProcedureTypeRecord;

		public readonly UInt32 VBaseOffset;

		public LF_METHODLIST(Context pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			ProcedureTypeRecord = r.ReadIndexedTypeLazy();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = r.ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}

		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_METHODLIST);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(ProcedureTypeRecord);
			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(VBaseOffset);
					break;
			}
			w.WriteLeafHeader();
		}

		public override string ToString() {
			return $"LF_METHODLIST[Attributes='{Attributes}', ProcedureTypeRecord='{ProcedureTypeRecord}', VBaseOffset='{VBaseOffset}']";
		}
	}
}
