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
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_FIELDLIST : ILeaf
	{
		private readonly ILazy<IEnumerable<LeafContainerBase>> lazyFields;
		public IEnumerable<LeafContainerBase> Fields => lazyFields.Value;

		private IEnumerable<LeafContainerBase> ReadFields(IServiceContainer pdb, SpanReader stream) {
			while (stream.Position + sizeof(UInt16) < stream.Length) {
				// We have to read the type directly to increase Stream.Position
				LeafContainerBase leaf = new TypeDataReader(pdb, stream).ReadTypeDirect(hasSize: false);
				if (leaf == null)
					yield break;
				yield return leaf;
			}
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_FIELDLIST);

			foreach (LeafContainerBase lf in Fields) {
				lf.Write(pdb, stream);
			}

			w.WriteLeafHeader(hasSize: false);
		}

		public LF_FIELDLIST(IServiceContainer pdb, SpanReader stream) {
			lazyFields = LazyFactory.CreateLazy(() => {
				return ReadFields(pdb, stream);
			});
		}

		/*
		public override string ToString() {
			return $"LF_FIELDLIST[Fields='{string.Join(", ", Fields.Select(f => f.Data.ToString()))}']";
		}
		*/
	}
}
