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
	public class LF_FIELDLIST : LeafBase
	{
		private readonly ILazy<IEnumerable<LeafContainerBase>> lazyFields;
		public IEnumerable<LeafContainerBase> Fields => lazyFields.Value;

		private IEnumerable<LeafContainerBase> ReadFields() {
			TypeDataReader r = CreateReader();
			while (stream.Position + sizeof(UInt16) < stream.Length) {
				// We have to read the type directly to increase Stream.Position
				LeafContainerBase leaf = r.ReadTypeDirect(hasSize: false);
				if (leaf == null)
					yield break;
				yield return leaf;
			}
		}

		public override void Read() {
			//no-op as we read lazily
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_FIELDLIST, false);

			foreach (LeafContainerBase lf in Fields) {
				lf.Write();
			}

			w.WriteHeader();
		}

		public LF_FIELDLIST(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
			lazyFields = LazyFactory.CreateLazy(ReadFields);
		}

		/*
		public override string ToString() {
			return $"LF_FIELDLIST[Fields='{string.Join(", ", Fields.Select(f => f.Data.ToString()))}']";
		}
		*/
	}
}
