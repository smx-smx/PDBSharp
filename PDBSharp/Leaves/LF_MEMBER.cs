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
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_MEMBER
{
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? FieldType { get; set; }

		public ILeafResolver? Offset { get; set; }

		public string Name { get; set; }

		public Data(FieldAttributes attributes, ILeafResolver? fieldType, ILeafResolver? offset, string name) {
			Attributes = attributes;
			FieldType = fieldType;
			Offset = offset;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var FieldType = r.ReadIndexedType32Lazy();
			var Offset = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadCString();
			Data = new Data(
				attributes: Attributes,
				fieldType: FieldType,
				offset: Offset,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_MEMBER);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.FieldType);
			w.WriteVaryingType(data.Offset);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_MEMBER[Attributes='{data?.Attributes}'" +
				$", FieldType='{data?.FieldType}'" +
				$", Offset='{data?.Offset}'" +
				$", Name='{data?.Name}']";
		}
	}
}
