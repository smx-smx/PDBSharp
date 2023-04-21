#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_NESTTYPE
{
	public class Data : ILeafData {
		public ILeafResolver? NestedTypeDef { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? nestedTypeDef, string name) {
			NestedTypeDef = nestedTypeDef;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data {  get; set; }

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public ILeafData? GetData() => Data;

		public long Read() {
			TypeDataReader r = CreateReader();

			r.ReadUInt16(); //padding
			var NestedTypeDef = r.ReadIndexedType32Lazy();
			var Name = r.ReadCString();
			Data = new Data(
				nestedTypeDef: NestedTypeDef,
				name: Name
			);
			
			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_NESTTYPE);
			w.WriteUInt16(0x00);
			w.WriteIndexedType(data.NestedTypeDef);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}
	}
}
