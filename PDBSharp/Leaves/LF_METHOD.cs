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
	public class LF_METHOD : ILeaf
	{
		public readonly UInt16 NumberOfOccurrences;
		public readonly ILeafContainer MethodListRecord;

		public readonly string Name;

		public LF_METHOD(Context pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			NumberOfOccurrences = r.ReadUInt16();
			MethodListRecord = r.ReadIndexedTypeLazy();
			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_METHOD);
			w.WriteUInt16(NumberOfOccurrences);
			w.WriteIndexedType(MethodListRecord);
			w.WriteCString(Name);
			w.WriteLeafHeader();
		}
	}
}
