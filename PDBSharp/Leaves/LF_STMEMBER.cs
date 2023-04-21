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

namespace Smx.PDBSharp.Leaves.LF_STMEMBER
{
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? TypeRecord { get; set; }
		public string Name { get; set; }

		public Data(FieldAttributes attributes, ILeafResolver? typeRecord, string name) {
			Attributes = attributes;
			TypeRecord = typeRecord;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public long Read() {
			TypeDataReader r = CreateReader();

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var TypeRecord = r.ReadIndexedType32Lazy();
			var Name = r.ReadCString();
			Data = new Data(
				attributes: Attributes,
				typeRecord: TypeRecord,
				name: Name
			);
			
			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_STMEMBER);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.TypeRecord);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}
	}
}
