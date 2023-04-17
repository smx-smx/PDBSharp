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

namespace Smx.PDBSharp.Leaves.LF_BCLASS
{
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? BaseClassType { get; set; }

		public ILeafResolver? Offset { get; set; }

		public Data(FieldAttributes attributes, ILeafResolver? baseClassType, ILeafResolver? offset) {
			Attributes = attributes;
			BaseClassType = baseClassType;
			Offset = offset;
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
			var BaseClassType = r.ReadIndexedType32Lazy();
			var Offset = r.ReadVaryingType(out uint dataSize);
			Data = new Data(
				attributes: Attributes,
				baseClassType: BaseClassType,
				offset: Offset
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_BCLASS);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.BaseClassType);
			w.WriteIndexedType(data.Offset);
			w.WriteHeader();
		}
	}
}
