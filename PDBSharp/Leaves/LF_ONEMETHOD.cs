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
	public class LF_ONEMETHOD : ILeaf
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeafContainer ProcedureType;
		public readonly UInt32 VBaseOffset;

		public readonly string Name;

		public LF_ONEMETHOD(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = new FieldAttributes(r.ReadUInt16());
			ProcedureType = r.ReadIndexedTypeLazy();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = r.ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}

			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_ONEMETHOD);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(ProcedureType);
			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(VBaseOffset);
					break;
			}
			w.WriteCString(Name);
			w.WriteLeafHeader();
		}
	}
}
