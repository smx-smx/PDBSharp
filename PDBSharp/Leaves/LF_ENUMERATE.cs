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
using System.Text;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_ENUMERATE
{

	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? Value { get; set; }
		public string FieldName { get; set; }
		public Data(FieldAttributes attributes, ILeafResolver? value, string fieldName) {
			Attributes = attributes;
			Value = value;
			FieldName = fieldName;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var Value = r.ReadVaryingType(out uint ILeafSize);
			var FieldName = r.ReadCString();
			
			Data = new Data(
				attributes: Attributes,
				value: Value,
				fieldName: FieldName
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_ENUMERATE);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteVaryingType(data.Value);
			w.WriteCString(data.FieldName);
			w.WriteHeader();
		}
	}
}
