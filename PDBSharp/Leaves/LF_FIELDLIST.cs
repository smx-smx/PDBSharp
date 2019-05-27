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
using System.Linq;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_FIELDLIST)]
	public class LF_FIELDLIST : TypeDataReader
	{
		public readonly IEnumerable<ILeaf> Fields;

		private IEnumerable<ILeaf> ReadFields(Stream dataStream) {
			while(dataStream.Position + sizeof(UInt16) < dataStream.Length) {
				ILeaf leaf = new TypeDataReader(this.PDB, dataStream).ReadType(hasSize: false);
				if (leaf == null)
					yield break;
				yield return leaf;
			}
		}

		public LF_FIELDLIST(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Fields = ReadFields(stream).Cached();
		}
	}
}
