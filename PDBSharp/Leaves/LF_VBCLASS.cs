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

namespace Smx.PDBSharp.Leaves.LF_VBCLASS
{
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? VirtualBaseClassType { get; set; }
		public ILeafResolver? VirtualBasePointerType { get; set; }

		public ILeafResolver? OffsetFromAddress { get; set; }
		public ILeafResolver? OffsetFromTable { get; set; }

		public Data(FieldAttributes attributes, ILeafResolver? virtualBaseClassType, ILeafResolver? virtualBasePointerType, ILeafResolver? offsetFromAddress, ILeafResolver? offsetFromTable) {
			Attributes = attributes;
			VirtualBaseClassType = virtualBaseClassType;
			VirtualBasePointerType = virtualBasePointerType;
			OffsetFromAddress = offsetFromAddress;
			OffsetFromTable = offsetFromTable;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		


		public void Read() {
			TypeDataReader r = CreateReader();

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var VirtualBaseClassType = r.ReadIndexedType32Lazy();
			var VirtualBasePointerType = r.ReadIndexedType32Lazy();


			//virtual base pointer offset from address point
			var OffsetFromAddress = r.ReadVaryingType(out uint dynSize1);
			//virtual base offset from vbtable
			var OffsetFromTable = r.ReadVaryingType(out uint dynSize2);

			Data = new Data(
				attributes: Attributes,
				virtualBaseClassType: VirtualBaseClassType,
				virtualBasePointerType: VirtualBasePointerType,
				offsetFromAddress: OffsetFromAddress,
				offsetFromTable: OffsetFromTable
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_IVBCLASS);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.VirtualBaseClassType);
			w.WriteIndexedType(data.VirtualBasePointerType);
			w.WriteVaryingType(data.OffsetFromAddress);
			w.WriteVaryingType(data.OffsetFromTable);
			w.WriteHeader();
		}

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		
	}
}
