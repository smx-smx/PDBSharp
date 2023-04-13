#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VBCLASS : LeafBase
	{
		public FieldAttributes Attributes { get; set; }
		public ILeafContainer VirtualBaseClassType { get; set; }
		public ILeafContainer VirtualBasePointerType { get; set; }

		public ILeafContainer OffsetFromAddress { get; set; }
		public ILeafContainer OffsetFromTable { get; set; }

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = new FieldAttributes(r.ReadUInt16());
			VirtualBaseClassType = r.ReadIndexedType32Lazy();
			VirtualBasePointerType = r.ReadIndexedType32Lazy();


			//virtual base pointer offset from address point
			OffsetFromAddress = r.ReadVaryingType(out uint dynSize1);
			//virtual base offset from vbtable
			OffsetFromTable = r.ReadVaryingType(out uint dynSize2);
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_IVBCLASS);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(VirtualBaseClassType);
			w.WriteIndexedType(VirtualBasePointerType);
			w.WriteVaryingType(OffsetFromAddress);
			w.WriteVaryingType(OffsetFromTable);
			w.WriteHeader();
		}

		public LF_VBCLASS(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		
	}
}
