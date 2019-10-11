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
	public class LF_UQUADWORD : ILeaf
	{
		public readonly ulong Value;

		public LF_UQUADWORD(IServiceContainer pdb, SpanStream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Value = r.ReadUInt64();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_UQUADWORD);
			w.WriteUInt64(Value);
			w.WriteLeafHeader();
		}
	}
}
