#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_FIELDLIST
{
	public class Data : ILeafData {
		public IList<ILeafResolver?> Fields;

		public Data(IList<ILeafResolver?> fields) {
			Fields = fields;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		private static IEnumerable<ILeafResolver?> ReadFields(TypeDataReader r) {
			while (r.Position + sizeof(UInt16) < r.Length) {
				// we need to read the type Directly since we don't know how long it is
				ILeafResolver? leaf = r.ReadTypeDirect(hasSize: false);
				if (leaf == null)
					yield break;
				
				yield return leaf;
			}
		}

		public void Read() {
			var r = CreateReader();
			var fields = ReadFields(r).ToList();
			Data = new Data(
				fields: fields
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_FIELDLIST, false);

			foreach (var lf in data.Fields
				         .Where(lf => lf != null)
				         .Cast<LeafContext>()
			) {
				lf.CreateSerializer(w).Write();
			}

			w.WriteHeader();
		}

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		/*
		public override string ToString() {
			return $"LF_FIELDLIST[Fields='{string.Join(", ", Fields.Select(f => f.Data.ToString()))}']";
		}
		*/
	}
}
