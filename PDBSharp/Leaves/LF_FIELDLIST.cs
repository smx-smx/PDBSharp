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
using System.Linq;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class LF_FIELDLIST : ILeaf
	{
		private readonly Lazy<IEnumerable<LeafBase>> lazyFields;
		public IEnumerable<LeafBase> Fields => lazyFields.Value;

		private IEnumerable<LeafBase> ReadFields(PDBFile pdb, Stream stream) {
			while(stream.Position + sizeof(UInt16) < stream.Length) {
				// We have to read the type directly to increase Stream.Position
				LeafBase leaf = new TypeDataReader(pdb, stream).ReadTypeDirect(hasSize: false);
				if (leaf == null)
					yield break;
				yield return leaf;
			}
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_FIELDLIST);
			
			foreach(LeafBase lf in Fields) {
				lf.Write(pdb, stream);
			}

			w.WriteLeafHeader(hasSize: false);
		}

		public LF_FIELDLIST(PDBFile pdb, Stream stream) {
			lazyFields = new Lazy<IEnumerable<LeafBase>>(() => {
				return ReadFields(pdb, stream);
			});
		}
	}
}
