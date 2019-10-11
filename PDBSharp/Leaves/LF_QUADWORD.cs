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
	public class LF_QUADWORD : ILeaf
	{
		public readonly long Value;

		public LF_QUADWORD(IServiceContainer pdb, SpanReader stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);
			Value = r.ReadInt64();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_QUADWORD);
			w.WriteInt64(Value);
			w.WriteLeafHeader();
		}
	}
}
