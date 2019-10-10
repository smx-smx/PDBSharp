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
	class LF_CHAR : ILeaf
	{
		public readonly byte Value;

		public LF_CHAR(IServiceContainer pdb, ReaderSpan stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);
			Value = r.ReadByte();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_CHAR);
			w.WriteByte(Value);

			w.WriteLeafHeader();
		}
	}
}
